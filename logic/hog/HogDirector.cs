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
        // vector pointing to average position of hogs in the sounder
        Vector2 avgPosVec = hog.OwnerSounder.AverageBirdseyePosition - hog.BirdseyePosition;
        // polar direction of the above vector
        float avgPosDir = Vector2.Right.AngleTo(avgPosVec);
        float hazardMagnitude = hog.AverageHazardRepelVector.Length();
        if(hazardMagnitude < 1)
        {
            if (hog.AverageNeighborPosition != Vector2.Inf)
            {
                // vector pointing away from average position of neighbours
                Vector2 negNeighborVec = hog.BirdseyePosition - hog.AverageNeighborPosition;
                // polar direction of the above vector
                float negNeighborDir = Vector2.Right.AngleTo(negNeighborVec);

                HogDesiredPolarDirection
                    = 0.7f * negNeighborDir
                    + 0.2f * hog.OwnerSounder.AveragePolarDirection
                    + 0.1f * avgPosDir;
            }
            else
            {
//                HogDesiredPolarDirection
//                    = 0.6f * hog.OwnerSounder.AveragePolarDirection
//                    + 0.4f * avgPosDir;
                HogDesiredPolarDirection = hog.OwnerSounder.AveragePolarDirection;
            }
            // check if hazard repellent demands at least some directional control
            if(hazardMagnitude > 0)
            {
                // apply hazard-overridden direction
                HogDesiredPolarDirection = 
                    (1 - hazardMagnitude) * HogDesiredPolarDirection
                    + hazardMagnitude * Vector2.Right.AngleTo(hog.AverageHazardRepelVector);
            }
        }
        else
        {
            // hazard repellent demands 100% control of hog direction
            HogDesiredPolarDirection = Vector2.Right.AngleTo(hog.AverageHazardRepelVector);
        }
    }
}