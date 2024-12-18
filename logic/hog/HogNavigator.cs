using Godot;

/**
 * Navigation agent for hog usage.
 */
public partial class HogNavigator : Node
{
    private const float TARGET_DIST_FACTOR = 4;
    private const float DIST_SQUARED_MIN = 36;
    private Vector2 TargetPosition;
    public Vector2[] PathPoints{get; private set;}

    /**
     * Determine a new target position based on birdseye position, direction and speed.
     */
    public void UpdateTargetPosition(Vector2 birdseyePos, float polarDirection, float speed)
    {
        Vector2 directionVec = Vector2.Right.Rotated(polarDirection).Normalized();
        // target position will be some distance in front of the hog
        TargetPosition = birdseyePos + (directionVec * speed * TARGET_DIST_FACTOR);
    }

    /**
     * Compute a new position on the birdseye navmesh.
     */
    public Vector2 GetNextPosition(double delta, Vector2 birdseyePos, float speed)
    {
        Vector2 output = birdseyePos;
        if(Global.Instance.MapRid.IsValid)
        {
            bool found = false;
            PathPoints = NavigationServer2D.MapGetPath(Global.Instance.MapRid, birdseyePos, TargetPosition, true);
            for(int ptIndex = 0; ptIndex < PathPoints.Length && !found; ++ptIndex)
            {
                if(PathPoints[ptIndex].DistanceSquaredTo(birdseyePos) >= DIST_SQUARED_MIN)
                {
                    output = birdseyePos.MoveToward(PathPoints[ptIndex], speed * (float)delta);
                    found = true;
                }
            }
        }
        return output;
    }
}