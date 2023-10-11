public static class PathingConstants 
{
    public static readonly StepDirection[] _directions = 
    {
        new(-1, +0), // W
        new(+1, +0), // E
        new(+0, +1), // N 
        new(+0, -1), // S
        new(-1, -1), // NW
        new(-1, +1), // SW
        new(+1, -1), // NE
        new(+1, +1)  // SE
    };
}
