using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public enum CarveTool { Shave = 1, Cut = 2 }

[RequireComponent(typeof(SpriteRenderer))]
public class PumpkinCarver : MonoBehaviour
{
    [Header("Look")]
    [Tooltip("Pumpkin face art. Must have Read/Write enabled.")]
    public Texture2D pumpkinTexture;
    public Color fleshColor = new Color(1.00f, 0.82f, 0.50f);
    public Color hollowColor = new Color(0.12f, 0.05f, 0.02f);
    public Color glowColor = new Color(1.00f, 0.78f, 0.25f);
    [Tooltip("Subtle grain in the flesh so it isn't a flat color. 0 = off.")]
    [Range(0f, 0.3f)] public float fleshGrain = 0.08f;

    [Header("Guide lines")]
    public bool showGuide = true;
    public Color cutGuideColor = new Color(1f, 1f, 1f, 0.7f);
    public Color shaveGuideColor = new Color(1f, 0.95f, 0.6f, 0.5f);

    [Header("Size")]
    [Tooltip("Carve grid resolution. 256 is plenty and cheap to score.")]
    public int resolution = 256;
    [Tooltip("Width/height of the pumpkin in world units.")]
    public float worldSize = 4f;

    [Header("Knife")]
    [Tooltip("Brush radius in grid pixels.")]
    public float brushRadius = 4f;
    [Tooltip("Width of the flesh-colored rim around cuts, in grid pixels.")]
    public float cutRim = 1.5f;

    public Camera cam;
    public bool InputEnabled { get; set; }
    public CarveTool Tool { get; set; } = CarveTool.Cut;
    public bool Lit { get; private set; }

    // 0 = skin, 1 = shaved, 2 = cut through
    public byte[] Depth { get; private set; }
    public byte[] Target { get; private set; }
    public byte[] Allowed { get; private set; }

    SpriteRenderer sr;
    Texture2D display;
    Color32[] skin, pixels;
    bool dirty;
    Vector2? lastPx;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (!cam) cam = Camera.main;
    }

    public void UseShave() => Tool = CarveTool.Shave;
    public void UseCut() => Tool = CarveTool.Cut;

    public void Begin(CarvingTemplate template)
    {
        int n = resolution * resolution;
        Depth = new byte[n];
        Target = template.BuildTarget(resolution, resolution);
        Allowed = CarvingScorer.Dilate(Target, resolution, resolution, template.tolerancePx);
        Lit = false;
        Tool = CarveTool.Cut;

        if (!display)
        {
            display = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };
            sr.sprite = Sprite.Create(display, new Rect(0, 0, resolution, resolution),
                new Vector2(0.5f, 0.5f), resolution / worldSize);
        }

        skin = new Color32[n];
        pixels = new Color32[n];
        for (int y = 0; y < resolution; y++)
        for (int x = 0; x < resolution; x++)
        {
            int i = y * resolution + x;
            Color c = pumpkinTexture
                ? pumpkinTexture.GetPixelBilinear((x + 0.5f) / resolution, (y + 0.5f) / resolution)
                : new Color(1f, 0.5f, 0.05f);
            if (showGuide)
            {
                int edge = EdgeLevel(x, y);
                if (edge == 2) c = Color.Lerp(c, cutGuideColor, cutGuideColor.a);
                else if (edge == 1) c = Color.Lerp(c, shaveGuideColor, shaveGuideColor.a);
            }
            skin[i] = c;
            pixels[i] = c;
        }
        dirty = true;
        lastPx = null;
        InputEnabled = true;
    }

    int EdgeLevel(int x, int y)
    {
        int w = resolution;
        int t = Target[y * w + x];
        if (t == 0) return 0;
        bool edge = x == 0 || y == 0 || x == w - 1 || y == w - 1
            || Target[y * w + x - 1] < t || Target[y * w + x + 1] < t
            || Target[(y - 1) * w + x] < t || Target[(y + 1) * w + x] < t;
        return edge ? t : 0;
    }

    public void SetLit(bool lit)
    {
        Lit = lit;
        for (int i = 0; i < Depth.Length; i++)
            if (Depth[i] == 2) pixels[i] = ColorFor(i);
        dirty = true;
    }

    Color32 ColorFor(int i)
    {
        switch (Depth[i])
        {
            case 0: return skin[i];
            case 1:
                uint h = (uint)i * 2654435761u; h ^= h >> 15;
                float n = (h & 255) / 255f;
                return fleshColor * (1f - fleshGrain + fleshGrain * 2f * n);
            default: return Lit ? glowColor : hollowColor;
        }
    }

    void Update()
    {
        if (Depth == null) return;

        if (InputEnabled && TryGetPointer(out Vector2 screen))
        {
            Vector2 px = ScreenToGrid(screen);
            if (lastPx.HasValue)
            {
                // Fill gaps between frames so fast strokes stay consistant
                float dist = Vector2.Distance(lastPx.Value, px);
                int steps = Mathf.Max(1, Mathf.CeilToInt(dist / (brushRadius * 0.5f)));
                for (int s = 1; s <= steps; s++)
                    Stamp(Vector2.Lerp(lastPx.Value, px, s / (float)steps));
            }
            else Stamp(px);
            lastPx = px;
        }
        else lastPx = null;

        if (dirty)
        {
            display.SetPixels32(pixels);
            display.Apply(false);
            dirty = false;
        }
    }

    Vector2 ScreenToGrid(Vector2 screen)
    {
        Vector3 world = cam.ScreenToWorldPoint(new Vector3(screen.x, screen.y,
            Mathf.Abs(cam.transform.position.z - transform.position.z)));
        Vector3 local = transform.InverseTransformPoint(world);
        return new Vector2(local.x / worldSize + 0.5f, local.y / worldSize + 0.5f) * resolution;
    }

    void Stamp(Vector2 c)
    {
        int r = Mathf.CeilToInt(brushRadius);
        float r2 = brushRadius * brushRadius;
        float inner = Mathf.Max(0f, brushRadius - cutRim);
        float inner2 = inner * inner;
        int cx = Mathf.RoundToInt(c.x), cy = Mathf.RoundToInt(c.y);

        for (int y = cy - r; y <= cy + r; y++)
        for (int x = cx - r; x <= cx + r; x++)
        {
            if (x < 0 || y < 0 || x >= resolution || y >= resolution) continue;
            float d2 = (x - c.x) * (x - c.x) + (y - c.y) * (y - c.y);
            if (d2 > r2) continue;

            byte level = (byte)Tool;
            // Outer ring of a cut only shaves leaving a flesh rim around holes
            if (Tool == CarveTool.Cut && d2 > inner2) level = 1;

            int i = y * resolution + x;
            if (Depth[i] >= level) continue;
            Depth[i] = level;
            pixels[i] = ColorFor(i);
            dirty = true;
        }
    }

    static bool TryGetPointer(out Vector2 pos)
    {
#if ENABLE_INPUT_SYSTEM
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        { pos = Touchscreen.current.primaryTouch.position.ReadValue(); return true; }
        if (Pen.current != null && Pen.current.tip.isPressed)
        { pos = Pen.current.position.ReadValue(); return true; }
        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        { pos = Mouse.current.position.ReadValue(); return true; }
#else
        if (Input.touchCount > 0) { pos = Input.GetTouch(0).position; return true; }
        if (Input.GetMouseButton(0)) { pos = Input.mousePosition; return true; }
#endif
        pos = default;
        return false;
    }
}