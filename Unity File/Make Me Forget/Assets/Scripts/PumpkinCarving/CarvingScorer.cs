using UnityEngine;

public struct CarvingResult
{
    /// <summary>How much of each</summary>
    public float coverage;
    public float precision;
    public float accuracy;
    public int stars;
    public int Percent => Mathf.RoundToInt(accuracy * 100f);
}

/// <summary>
/// coverage and precision
/// </summary>
public static class CarvingScorer
{
    public static CarvingResult Score(byte[] depth, byte[] target, byte[] allowed, CarvingTemplate template)
    {
        int targetCount = 0, carvedCount = 0, carvedOk = 0;
        float credit = 0f;

        for (int i = 0; i < depth.Length; i++)
        {
            int t = target[i], d = depth[i];
            if (t > 0)
            {
                targetCount++;
                credit += d >= t ? 1f : d / (float)t;
            }
            if (d > 0)
            {
                carvedCount++;
                if (d <= allowed[i]) carvedOk++;
            }
        }

        var r = new CarvingResult
        {
            coverage = targetCount == 0 ? 0f : credit / targetCount,
            precision = carvedCount == 0 ? 0f : (float)carvedOk / carvedCount,
        };
        r.accuracy = r.coverage * r.precision;
        r.stars = template.StarsFor(r.accuracy);
        return r;
    }

    /// <summary> each depth region by radius px </summary>
    public static byte[] Dilate(byte[] src, int w, int h, int radius)
    {
        var dst = (byte[])src.Clone();
        if (radius <= 0) return dst;
        int r2 = radius * radius;
        for (int y = 0; y < h; y++)
        for (int x = 0; x < w; x++)
        {
            byte v = src[y * w + x];
            if (v == 0) continue;
            for (int dy = -radius; dy <= radius; dy++)
            for (int dx = -radius; dx <= radius; dx++)
            {
                if (dx * dx + dy * dy > r2) continue;
                int nx = x + dx, ny = y + dy;
                if (nx < 0 || ny < 0 || nx >= w || ny >= h) continue;
                int n = ny * w + nx;
                if (dst[n] < v) dst[n] = v;
            }
        }
        return dst;
    }
}