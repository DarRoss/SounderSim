using Godot;

public partial class S2hInfo : Node
{
    public S2hInfo()
    {
        DesiredDirectionInfluence = 0.0f;
    }

    // average birdseye position of all hogs in the sounder
    public Vector2 AverageBirdseyePosition{get; set;}
    // average polar coordinate direction of all hogs in the sounder. -pi uninclusive to +pi inclusive
    public float AveragePolarDirection{get; set;}
    // direction that the sounder wants all hogs to travel
    public float DesiredPolarDirection{get; set;}
    // how much the hog should consider the desired direction. 0 to 1 inclusive
    public float DesiredDirectionInfluence{get; set;}
    // speed that the sounder wants all hogs to travel at
    public float DesiredSpeed{get; set;}
}