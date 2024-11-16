using Godot;

/**
 * Navigation agent for hog usage.
 */
public partial class HogNavigator : Node
{
    private const float DIST_SQUARED_MIN = 9;
    public Vector2 TargetPosition;
    public Vector2[] pathPts;

    /**
     * Determine a new target position based on birdseye position, direction and speed.
     */
    public void UpdateTargetPosition(Vector2 birdseyePos, float polarDirection, float speed)
    {
        Vector2 directionVec = Vector2.Right.Rotated(polarDirection).Normalized();
        // target position will be some distance in front of the hog
        TargetPosition = birdseyePos + (directionVec * speed);
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
            pathPts = NavigationServer2D.MapGetPath(Global.Instance.MapRid, birdseyePos, TargetPosition, true);
            for(int ptIndex = 0; ptIndex < pathPts.Length && !found; ++ptIndex)
            {
                if(pathPts[ptIndex].DistanceSquaredTo(birdseyePos) >= DIST_SQUARED_MIN)
                {
                    output = birdseyePos.MoveToward(pathPts[ptIndex], speed * (float)delta);
                    found = true;
                }
            }
        }
        return output;
    }
}