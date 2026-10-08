using UnityEngine;

/// <summary>
///   BLACK  = leave the skin alone
///   GRAY   = shave the skin off (shows pale flesh)
///   WHITE  = cut all the way through (hole that glows at the end hopefully)
/// </summary>
[CreateAssetMenu(menuName = "Minigames/Pumpkin Carving Template")]
public class CarvingTemplate : ScriptableObject
{
    public string displayName = "Classic Face";
    public Texture2D mask;

    [Header("Rules")]
    public float timeLimit = 60f;
    [Tooltip("Pixels just outside the line that aren't penalized. Higher = more forgiving.")]
    public int tolerancePx = 3;

    [Header("Star thresholds (accuracy 0-1)")]
    public float oneStar = 0.50f;
    public float twoStar = 0.70f;
    public float threeStar = 0.85f;

   
    /// 0 = skin, 1 = shave, 2 = cut through.
    ///
    public byte[] BuildTarget(int w, int h)
    {
        var target = new byte[w * h];
        for (int y = 0; y < h; y++)
        for (int x = 0; x < w; x++)
        {
            Color c = mask.GetPixelBilinear((x + 0.5f) / w, (y + 0.5f) / h);
            float g = c.grayscale * c.a;
            target[y * w + x] = g < 0.25f ? (byte)0 : g < 0.75f ? (byte)1 : (byte)2;
        }
        return target;
    }

    public int StarsFor(float accuracy)
    {
        if (accuracy >= threeStar) return 3;
        if (accuracy >= twoStar) return 2;
        if (accuracy >= oneStar) return 1;
        return 0;
    }
}