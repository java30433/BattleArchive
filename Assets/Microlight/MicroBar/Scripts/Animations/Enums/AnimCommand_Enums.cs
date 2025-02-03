namespace Microlight.MicroBar
{
    // ****************************************************************************************************
    // Enums that help AnimCommand to be built
    // * Enums must be public because editor scripts are in another assembly
    // ****************************************************************************************************

    public enum AnimExecution
    {
        Sequence,
        Parallel,
        Wait,
    }

    public enum AnimEffect
    {
        Color,
        Fade,
        Fill,
        Move,
        Rotate,
        Scale,
        Punch,
        Shake,
        AnchorMove,
    }

    public enum ValueMode
    {
        Absolute,
        Additive,
        Multiplicative,
        StartingValue,
        DefaultValue,
    }

    public enum AnimAxis
    {
        Uniform,
        XY,
        X,
        Y,
    }

    public enum TransformProperties
    {
        Position,
        Rotation,
        Scale,
        AnchorPosition,
    }
}
