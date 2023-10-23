using System;

using UnityEngine;
using UnityEngine.UI;

public static class Utils
{
    public static void ChangeButtonColor(Button btn, Color color)
    {
        var colors = btn.colors;
        colors.normalColor = color;
        colors.highlightedColor = color;
        colors.pressedColor = color;
        colors.selectedColor = color;
        btn.colors = colors;
    }

    public static bool V3Equal(Vector3 a, Vector3 b)
    {
        return Vector3.SqrMagnitude(a - b) < 0.0001;
    }
}

[Serializable]
struct JsonDateTime
{
    public long _value;

    public static implicit operator DateTime(JsonDateTime jdt)
    {
        return DateTime.FromFileTime(jdt._value);
    }

    public static implicit operator JsonDateTime(DateTime dt)
    {
        JsonDateTime jdt = new()
        {
            _value = dt.ToFileTime()
        };

        return jdt;
    }
}
