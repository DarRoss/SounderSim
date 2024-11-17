using Godot;

/**
 * Hog detection bubble that detects other hog detection bubbles.
 */
public partial class HogDetectionArea : Area2D
{
    public Hog Hog{get; private set;}
    public override void _Ready()
    {
        Hog = GetNode<Hog>("..");
    }

    /**
     * Determine average global position of all detection bubbles that are touching this bubble.
     */
    public Vector2 GetAverageNeighborPosition()
    {
        // the infinity vector represents a lack of neighbors
        Vector2 output = Vector2.Inf;
        if(HasOverlappingAreas())
        {
            Vector2 neighborPosSum = Vector2.Zero;
            int neighborCount = 0;
            Godot.Collections.Array<Area2D> areaArray = GetOverlappingAreas();
            foreach(Area2D area in areaArray)
            {
                // ensure that the Area2D is a hog neighbor detection bubble
                if (area is HogDetectionArea neighborArea)
                {
                    neighborPosSum += neighborArea.Hog.InfoPacket.BirdseyePosition;
                    ++neighborCount;
                }
            }
            if(neighborCount > 0)
            {
                // calculate average position
                output = neighborPosSum / neighborCount;
            }
        }
        return output;
    }
}