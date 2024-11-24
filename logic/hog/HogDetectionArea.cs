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
        InputEvent += ProcessInputEvent;
    }

    private void ProcessInputEvent(Node viewport, InputEvent ie, long shapeIdx)
    {
        if(ie is InputEventMouseButton iemb)
        {
            if(iemb.Pressed)
            {
                // hog detection area clicked
            }
        }
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
                    neighborPosSum += neighborArea.Hog.BirdseyePosition;
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

    public Vector2 GetAverageHazardRepelVector()
    {
        // the zero magnitude represents a lack of hazards
        Vector2 output = Vector2.Zero;
        if(HasOverlappingAreas())
        {
            Vector2 repelVectorSum = Vector2.Zero;
            int hazardCount = 0;
            Godot.Collections.Array<Area2D> areaArray = GetOverlappingAreas();
            foreach(Area2D area in areaArray)
            {
                // ensure that the Area2D is a navigational hazard
                if (area is NavigationHazard hazard)
                {
                    repelVectorSum += hazard.GetRepelVector(Hog.BirdseyePosition);
                    ++hazardCount;
                }
            }
            if(hazardCount > 0)
            {
                // calculate average repel vector
                output = repelVectorSum / hazardCount;
            }
        }
        return output;
    }
}