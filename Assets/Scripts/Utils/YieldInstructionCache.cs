using System;
using System.Collections.Generic;

using UnityEngine;

//TODO: 하나의 WaitForSeconds를 같이 사용할 때 예상하지 못하는 문제가 발생한다. WaitForSeconds 재사용 방법을 변경할 필요가 있다.
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
            _timeInterval.Add(seconds, wfs = new(seconds));

        return wfs;
    }

    public static WaitForSecondsRealtime WaitForSecondsRealtime(float seconds)
    {
        if (!_realTimeInterval.TryGetValue(seconds, out WaitForSecondsRealtime wfsr))
            _realTimeInterval.Add(seconds, wfsr = new(seconds));

        return wfsr;
    }
}
