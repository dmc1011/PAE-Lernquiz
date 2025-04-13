
using System.Collections.Generic;
using UnityEngine;



public static class Functions
{
    // Fisher Yates Shuffle Inplace: Taken from the unity forums.
    public static void Shuffle<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }

    public static Color Lerp3(Color a, Color b, Color c, float t)
    {
        if (t > 0.5f)
            return Color.Lerp(b, c, (t - 0.5f) * 2);
        else
            return Color.Lerp(a, b, t * 2);
    }

    public static string CleanInput(string input)
    {
        return input.Replace("\u200B", "").Trim();
    }

}
