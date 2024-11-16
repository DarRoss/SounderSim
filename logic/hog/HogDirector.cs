using Godot;

public partial class HogDirector : Node
{
    // the owner hog
    private Hog hog;
    /**
     * The polar direction we want the hog to go.
     * 0 means facing the same direction as the +X axis.
     * PI/2 means facing the same direction as the +Y axis.
     * -PI/2 means facing the same direction as the -Y axis.
     */
    public float HogDesiredPolarDirection{get; private set;}

    public override void _Ready()
    {
        hog = GetNode<Hog>("..");
        // randomize direction on startup
        HogDesiredPolarDirection = (float)GD.RandRange(-Mathf.Pi, Mathf.Pi);
    }

    public void UpdateDesiredDirection()
    {
        S2hInfo sounderInfo = hog.SounderInfo;
        // vector pointing to average position of hogs in the sounder
        Vector2 avgPosVec = sounderInfo.AverageBirdseyePosition - hog.HogInfoPacket.BirdseyePosition;
        // polar direction of the above vector
        float avgPosDir = Vector2.Right.AngleTo(avgPosVec);

        if (hog.NeighborAveragePosition != Vector2.Inf)
        {
            // vector pointing away from average position of neighbours
            Vector2 negNeighborVec = hog.HogInfoPacket.BirdseyePosition - hog.NeighborAveragePosition;
            // polar direction of the above vector
            float negNeighborDir = Vector2.Right.AngleTo(negNeighborVec);

            HogDesiredPolarDirection
                = 0.8f * negNeighborDir
                + 0.1f * sounderInfo.AveragePolarDirection
                + 0.1f * avgPosDir;
        }
        else
        {
            HogDesiredPolarDirection
                = 0.6f * sounderInfo.AveragePolarDirection
                + 0.4f * avgPosDir;
        }
           
        HogDesiredPolarDirection = 
            (1 - sounderInfo.DesiredDirectionInfluence) * HogDesiredPolarDirection
            + sounderInfo.DesiredDirectionInfluence * sounderInfo.DesiredPolarDirection;
    }
}