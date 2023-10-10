public static class PathingConstants 
{
    public static readonly StepDirection[] _directions = 
    {
        new StepDirection(-1, +0), // W
        new StepDirection(+1, +0), // E
        new StepDirection(+0, +1), // N 
        new StepDirection(+0, -1), // S
        new StepDirection(-1, -1), // NW
        new StepDirection(-1, +1), // SW
        new StepDirection(+1, -1), // NE
        new StepDirection(+1, +1)  // SE
    };
}
