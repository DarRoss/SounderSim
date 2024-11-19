using Godot;

public partial class HogDirector : Node
{
    // the owner hog
    private Hog hog;
    // the polar direction we want the hog to go
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
        Vector2 avgPosVec = sounderInfo.AverageBirdseyePosition - hog.InfoPacket.BirdseyePosition;
        // polar direction of the above vector
        float avgPosDir = Vector2.Right.AngleTo(avgPosVec);
        float hazardStrength = hog.HazardAverageNegVector.Length();
        if(hazardStrength < 1)
        {
            if (hog.NeighborAveragePosition != Vector2.Inf)
            {
                // vector pointing away from average position of neighbours
                Vector2 negNeighborVec = hog.InfoPacket.BirdseyePosition - hog.NeighborAveragePosition;
                // polar direction of the above vector
                float negNeighborDir = Vector2.Right.AngleTo(negNeighborVec);

                HogDesiredPolarDirection
                    = 0.7f * negNeighborDir
                    + 0.2f * sounderInfo.AveragePolarDirection
                    + 0.1f * avgPosDir;
            }
            else
            {
                HogDesiredPolarDirection
                    = 0.6f * sounderInfo.AveragePolarDirection
                    + 0.4f * avgPosDir;
            }
            // check if hazard repellent demands at least some directional control
            if(hazardStrength > 0)
            {
                // apply hazard-overridden direction
                HogDesiredPolarDirection = 
                    (1 - hazardStrength) * HogDesiredPolarDirection
                    + hazardStrength * Vector2.Right.AngleTo(hog.HazardAverageNegVector);
            }
        }
        else
        {
            // hazard repellent demands 100% control of hog direction
            HogDesiredPolarDirection = Vector2.Right.AngleTo(hog.HazardAverageNegVector);
        }
    }
}