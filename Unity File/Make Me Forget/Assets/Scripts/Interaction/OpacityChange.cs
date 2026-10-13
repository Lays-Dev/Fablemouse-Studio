using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class OpacityChange : MonoBehaviour
{
    public enum CueType { Darken, HueChange, Outline }

    // pick which cue to show <3
    [Tooltip("Which effect shows when the player gets close")]
    public CueType cue = CueType.Darken;

    // where to measure distance from 
    [Tooltip("Where distance is measured from")]
    public Transform interactionPoint;
    public string playerTag = "Player";

    // how close before the cue shows
    [Header("Distance")]
    [Tooltip("The cue starts when the player is this close.")]
    public float startDistance = 5f;
    [Tooltip("The cue is at full strength when the player is this close.")]
    public float fullDistance = 1.5f;
    [Tooltip("How quickly the cue catches up. Higher = snappier.")]
    public float smoothing = 8f;

    // darken settings
    [Header("Darken")]
    [Tooltip("How bright the art gets up close")]
    [Range(0f, 2f)] public float nearBrightness = 0.7f;

    // hue color settings
    [Header("Hue Change")]
    [Tooltip("The color the art shifts toward when the player is close.")]
    public Color highlightColor = new Color(1f, 0.85f, 0.4f);
    [Tooltip("How much of the highlight color to mix in. 0 = none, 1 = full.")]
    [Range(0f, 1f)] public float hueStrength = 0.5f;

    // outline settings
    [Header("Outline")]
    public Color outlineColor = Color.white;
    [Tooltip("How thick the outline is, in world units.")]
    public float outlineWidth = 0.08f;
    [Tooltip("Drag the InteractOutline shader here (needed for builds). Found automatically in the editor.")]
    public Shader outlineShader;

    // for custom shaders (like Shader Graph) with their own color name
    [Header("Custom Shaders")]
    [Tooltip("Optional. The color setting's name in a custom shader ")]
    public string customColorProperty = "";

    // pulse stuff (untick to turn off)
    [Header("Pulse (optional)")]
    [Tooltip("Gently pulses the cue while the player is close.")]
    public bool pulseWhenClose = false;
    public float pulseSpeed = 3f;
    [Range(0f, 0.5f)] public float pulseAmount = 0.2f;

    // one material slot on a 3D renderer (for darken and hue)
    class Slot
    {
        public Renderer renderer;
        public int index;
        public int prop;
        public Color original;
        public bool hasEmission;
        public Color emission;
    }

    SpriteRenderer[] sprites;
    Color[] spriteColors;
    readonly List<Slot> slots = new List<Slot>();
    MaterialPropertyBlock block;

    Material outlineMaterial;
    readonly List<GameObject> shells = new List<GameObject>();
    bool outlineShowing;

    Transform player;
    float current;

    static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
    static readonly int Color_ = Shader.PropertyToID("_Color");
    static readonly int EmissionId = Shader.PropertyToID("_EmissionColor");
    static readonly int WidthId = Shader.PropertyToID("_OutlineWidth");
    static readonly int OutlineColorId = Shader.PropertyToID("_OutlineColor");

    void Awake()
    {
        if (!interactionPoint) interactionPoint = transform;
        SetUpColors();
        SetUpOutline();
        Apply(0f, 1f);
    }

    // remember the normal colors (for darken and hue)
    void SetUpColors()
    {
        block = new MaterialPropertyBlock();

        sprites = GetComponentsInChildren<SpriteRenderer>(true);
        spriteColors = new Color[sprites.Length];
        for (int i = 0; i < sprites.Length; i++) spriteColors[i] = sprites[i].color;

        foreach (var r in GetComponentsInChildren<Renderer>(true))
        {
            if (r is SpriteRenderer) continue;
            var mats = r.sharedMaterials;
            for (int i = 0; i < mats.Length; i++)
            {
                var m = mats[i];
                if (!m) continue;
                int prop = FindColorProperty(m);
                if (prop == -1) continue;

                // glowing (emission) materials need their glow changed too
                bool glow = m.HasProperty(EmissionId) && m.IsKeywordEnabled("_EMISSION");
                slots.Add(new Slot
                {
                    renderer = r, index = i, prop = prop, original = m.GetColor(prop),
                    hasEmission = glow, emission = glow ? m.GetColor(EmissionId) : Color.black
                });
            }
        }
    }

    // find the color setting in a material's shader so I don't tpuch Lindsey's work
    int FindColorProperty(Material m)
    {
        if (!string.IsNullOrEmpty(customColorProperty) && m.HasProperty(customColorProperty))
            return Shader.PropertyToID(customColorProperty);
        if (m.HasProperty(BaseColor)) return BaseColor;
        if (m.HasProperty(Color_)) return Color_;

        // custom shader use its first visible color setting
        var shader = m.shader;
        for (int i = 0; i < shader.GetPropertyCount(); i++)
        {
            if (shader.GetPropertyType(i) != ShaderPropertyType.Color) continue;
            if ((shader.GetPropertyFlags(i) & ShaderPropertyFlags.HideInInspector) != 0) continue;
            return Shader.PropertyToID(shader.GetPropertyName(i));
        }
        return -1;
    }

    // make a slightly bigger copy of each 3D mesh (for the outline part you wanted)
    void SetUpOutline()
    {
        if (!outlineShader) outlineShader = Shader.Find("Custom/InteractOutline");
        if (!outlineShader) return;
        outlineMaterial = new Material(outlineShader);

        foreach (var mf in GetComponentsInChildren<MeshFilter>(true))
        {
            var mr = mf.GetComponent<MeshRenderer>();
            if (!mr || !mf.sharedMesh) continue;

            var shell = new GameObject(mf.name + " (Outline)");
            shell.transform.SetParent(mf.transform, false);
            shell.AddComponent<MeshFilter>().sharedMesh = mf.sharedMesh;
            var r = shell.AddComponent<MeshRenderer>();
            var mats = new Material[mf.sharedMesh.subMeshCount];
            for (int i = 0; i < mats.Length; i++) mats[i] = outlineMaterial;
            r.sharedMaterials = mats;
            r.shadowCastingMode = ShadowCastingMode.Off;
            r.receiveShadows = false;
            shell.SetActive(false);
            shells.Add(shell);
        }
    }

    void Update()
    {
        // find the player
        if (!player)
        {
            var p = GameObject.FindGameObjectWithTag(playerTag);
            if (p) player = p.transform;
        }

        // work out how close the player is (0 = far, 1 = right there)
        float target = 0f;
        if (player)
        {
            float distance = Vector3.Distance(player.position, interactionPoint.position);
            target = Mathf.InverseLerp(startDistance, fullDistance, distance);
        }

        // smooth it out
        current = Mathf.Lerp(current, target, 1f - Mathf.Exp(-smoothing * Time.deltaTime));

        // pulse
        float pulse = 1f;
        if (pulseWhenClose && current > 0.001f)
            pulse += Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;

        Apply(current, pulse);
    }

    // show the chosen cue
    void Apply(float amount, float pulse)
    {
        float a = Mathf.Clamp01(amount * pulse);

        if (cue == CueType.Darken)
            ApplyColors(Mathf.Lerp(1f, nearBrightness, a), 0f);
        else if (cue == CueType.HueChange)
            ApplyColors(1f, Mathf.Clamp01(a * hueStrength));
        else
            ApplyColors(1f, 0f);

        ApplyOutline(cue == CueType.Outline ? amount * pulse : 0f);
    }

    // change brightness and mix in the highlight color (doesn't change the material files)
    void ApplyColors(float brightness, float hue)
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            if (!sprites[i]) continue;
            sprites[i].color = Change(spriteColors[i], brightness, hue);
        }

        foreach (var s in slots)
        {
            if (!s.renderer) continue;
            s.renderer.GetPropertyBlock(block, s.index);
            block.SetColor(s.prop, Change(s.original, brightness, hue));

            // dim or tint the glow the same way
            if (s.hasEmission)
            {
                float glowStrength = s.emission.maxColorComponent;
                var e = Color.Lerp(s.emission * brightness, highlightColor * glowStrength, hue);
                e.a = s.emission.a;
                block.SetColor(EmissionId, e);
            }
            s.renderer.SetPropertyBlock(block, s.index);
        }
    }

    Color Change(Color original, float brightness, float hue)
    {
        var c = Color.Lerp(original * brightness, highlightColor, hue);
        c.a = original.a;
        return c;
    }

    // show or hide the outline and set its size and color
    void ApplyOutline(float amount)
    {
        if (!outlineMaterial) return;

        bool show = amount > 0.001f;
        if (show != outlineShowing)
        {
            outlineShowing = show;
            foreach (var s in shells) if (s) s.SetActive(show);
        }

        outlineMaterial.SetFloat(WidthId, outlineWidth * amount);
        outlineMaterial.SetColor(OutlineColorId, outlineColor);
    }

    // turning the script off puts the art back to normal
    void OnDisable()
    {
        current = 0f;
        ApplyColors(1f, 0f);
        ApplyOutline(0f);
    }

    // clean up
    void OnDestroy()
    {
        foreach (var s in shells) if (s) Destroy(s);
        if (outlineMaterial) Destroy(outlineMaterial);
    }

    // draws the circles in the Scene view
    void OnDrawGizmosSelected()
    {
        var center = interactionPoint ? interactionPoint.position : transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, startDistance);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(center, fullDistance);
    }
}