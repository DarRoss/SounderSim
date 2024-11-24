using System;
using Godot;

public partial class HogDirector : Node
{
    private const float SOUNDER_DIR_INFLUENCE = 0.6f;

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
        float hazardMagnitude = hog.AverageHazardRepelVector.Length();
        if (hog.AverageNeighborPosition != Vector2.Inf)
        {
            // vector pointing away from average position of neighbours
            Vector2 negNeighborVec = hog.BirdseyePosition - hog.AverageNeighborPosition;
            // polar direction of the above vector
            HogDesiredPolarDirection = Vector2.Right.AngleTo(negNeighborVec);
        }
        else if(hazardMagnitude < 1)
        {
            // vector pointing to average position of hogs in the sounder
            Vector2 avgPosVec = hog.OwnerSounder.AverageBirdseyePosition - hog.BirdseyePosition;
            // polar direction of the above vector
            float avgPosDir = Vector2.Right.AngleTo(avgPosVec);

            (float avgPosDirS, float avgPosDirC) = Mathf.SinCos(avgPosDir);
            (float sdrAvgDirS, float sdrAvgDirC) = Mathf.SinCos(hog.OwnerSounder.AveragePolarDirection);
            float dirSumS = SOUNDER_DIR_INFLUENCE * sdrAvgDirS + (1 - SOUNDER_DIR_INFLUENCE) * avgPosDirS;
            float dirSumC = SOUNDER_DIR_INFLUENCE * sdrAvgDirC + (1 - SOUNDER_DIR_INFLUENCE) * avgPosDirC;

            HogDesiredPolarDirection = Mathf.Atan2(dirSumS, dirSumC);

            // check if hazard repellent demands at least some directional control
            if(hazardMagnitude > 0)
            {
                float repelDirection = Vector2.Right.AngleTo(hog.AverageHazardRepelVector);

                (float currDirS, float currDirC) = Mathf.SinCos(HogDesiredPolarDirection);
                (float repelDirS, float repelDirC) = Mathf.SinCos(repelDirection);
                float repelSumS = (1 - hazardMagnitude) * currDirS + hazardMagnitude * repelDirS;
                float repelSumC = (1 - hazardMagnitude) * currDirC + hazardMagnitude * repelDirC;

                // apply hazard-overridden direction
                HogDesiredPolarDirection = Mathf.Atan2(repelSumS, repelSumC);
            }
        }
        else
        {
            // hazard repellent demands 100% control of hog direction
            HogDesiredPolarDirection = Vector2.Right.AngleTo(hog.AverageHazardRepelVector);
        }
    }
}