using Godot;

/**
 * Navigation agent for hog usage.
 */
public partial class HogNavigator : Node
{
    public const float DIST_FACTOR = 100;
    private Vector2 TargetPosition;
    private Vector2[] pathPts;

    /**
     * Determine a new target position based on birdseye position, direction and speed.
     */
    public void UpdateTargetPosition(Vector2 birdseyePos, float dir, float speed)
    {
        Vector2 directionVec = Vector2.Right.Rotated(dir).Normalized();
        // target position will be some distance in front of the hog
        TargetPosition = birdseyePos + (directionVec * speed * DIST_FACTOR);
    }

    /**
     * Compute a new position on the navmesh.
     */
    public Vector2 GetNextPosition(double delta, Vector2 birdseyePos, float speed)
    {
        Vector2 output = birdseyePos;
        if(Global.Instance.MapRid.IsValid)
        {
            pathPts = NavigationServer2D.MapGetPath(Global.Instance.MapRid, birdseyePos, TargetPosition, false);
            if(pathPts.Length > 0)
            {
                output = birdseyePos.MoveToward(pathPts[0], speed * (float)delta);
            }
        }
        return output;
    }
}