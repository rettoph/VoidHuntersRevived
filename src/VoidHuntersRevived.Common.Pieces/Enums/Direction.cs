namespace VoidHuntersRevived.Common.Pieces.Enums
{
    [Flags]
    public enum Direction : byte
    {
        None = 0,
        Forward = 1,
        Right = 2,
        Backward = 4,
        Left = 8,
        TurnRight = 16,
        TurnLeft = 32
    }
}
