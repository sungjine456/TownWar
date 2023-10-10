using System;
using System.Collections.Generic;

using UnityEngine;

public static class YieldInstructionCache
{
    class FloatComparer : IEqualityComparer<float>
    {
        bool IEqualityComparer<float>.Equals(float x, float y)
        {
            float diff = Math.Abs(x - y);
            float tolerance = 0.0001f;

            return diff <= tolerance ||
                   diff <= Math.Max(Math.Abs(x), Math.Abs(y)) * tolerance;
        }
        int IEqualityComparer<float>.GetHashCode(float obj) { return obj.GetHashCode(); }
    }

    public static readonly WaitForEndOfFrame _waitForEndOfFrame = new();
    public static readonly WaitForFixedUpdate _waitForFixedUpdate = new();
    static readonly Dictionary<float, WaitForSeconds> _timeInterval = new(new FloatComparer());
    static readonly Dictionary<float, WaitForSecondsRealtime> _realTimeInterval = new(new FloatComparer());

    public static WaitForSeconds WaitForSeconds(float seconds)
    {
        if (!_timeInterval.TryGetValue(seconds, out WaitForSeconds wfs))
            _timeInterval.Add(seconds, wfs = new WaitForSeconds(seconds));

        return wfs;
    }

    public static WaitForSecondsRealtime WaitForSecondsRealtime(float seconds)
    {
        if (!_realTimeInterval.TryGetValue(seconds, out WaitForSecondsRealtime wfsr))
            _realTimeInterval.Add(seconds, wfsr = new WaitForSecondsRealtime(seconds));

        return wfsr;
    }
}
