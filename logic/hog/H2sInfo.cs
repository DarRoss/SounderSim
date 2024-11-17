using Godot;

public partial class H2sInfo : Node
{
    public H2sInfo()
    {
        IsAlive = true;
    }

    // true if the hog is not dead
    public bool IsAlive{get; set;}
    // unique name of the hog
    public string HogName{get; set;}
    // the position of the hog in the world
    public Vector2 BirdseyePosition{get; set;}
    // the polar coordinate direction that the hog is pointing. -pi uninclusive to +pi inclusive
    public float PolarDirection{get; set;}
}