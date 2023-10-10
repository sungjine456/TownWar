using System;

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
