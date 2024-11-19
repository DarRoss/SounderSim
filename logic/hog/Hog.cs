using Godot;

/**
 * Quadruped animal that travels in a group (sounder).
 */
public partial class Hog : Node2D
{
    // for use in moving towards desired speed and direction
    private const float SPEED_LERP = 5;
    private const float DIRECTION_LERP = 1;

    // The time between intermittent calculations. Measured in seconds.
    [Export]
    private float recalculationInterval = 0.1f;

    // nodes of interest
    private H2sCommunicator sounderCommunicator;
    private HogDetectionArea detectionArea;
    private HogNavigator navigator;
    private HogDirector director;
    private HogMeshManipulator meshManipulator;

    // other nodes
    private Timer intermittentTimer = new();

    /**
     * hog variables
     */
    // speed that the hog is currently moving at
    private float currSpeed;
    public H2sInfo InfoPacket{get; private set;} = new();
    public Vector2 NeighborAveragePosition{get; private set;}
    // points away from nearby hazards
    public Vector2 HazardAverageNegVector{get; private set;}

    /**
     * "passthrough" variables
     */
    public S2hInfo SounderInfo => sounderCommunicator.SounderInfo;

    /*
     * sounder to hog (S2H) fields
     */
    public Sounder.AnnounceSounderInfoEventHandler OnReceiveSounderInfo
        => sounderCommunicator.OnReceiveSounderInfo;

    /*
     * hog to sounder (H2S) fields
     */
    public event H2sCommunicator.AnnounceHogInfoEventHandler AnnounceHogInfo
    {
        add => sounderCommunicator.AnnounceHogInfo += value;
        remove => sounderCommunicator.AnnounceHogInfo -= value;
    }

    public override void _Ready()
    {
        // initialize nodes of interest
        sounderCommunicator = GetNode<H2sCommunicator>("H2sCommunicator");
        detectionArea = GetNode<HogDetectionArea>("HogDetectionArea");
        navigator = GetNode<HogNavigator>("HogNavigator");
        director = GetNode<HogDirector>("HogDirector");
        meshManipulator = GetNode<HogMeshManipulator>("HogMeshManipulator");

        // update packet information
        InfoPacket.HogName = Name;
        InfoPacket.BirdseyePosition = GlobalPosition;

        SetupIntermittentTimer();
    }

    private void SetupIntermittentTimer()
    {
        AddChild(intermittentTimer);
        // attach intermittent calculations to timer
        intermittentTimer.Timeout += PerformIntermittentCalculations;
        intermittentTimer.WaitTime = recalculationInterval;
        intermittentTimer.Start();
    }

    public override void _PhysicsProcess(double delta)
    {
        HazardAverageNegVector = detectionArea.GetNegHazardVector();
        AdjustCurrentVelocity(delta);
        Vector2 oldPos = InfoPacket.BirdseyePosition;
        InfoPacket.BirdseyePosition = navigator.GetNextPosition(delta, InfoPacket.BirdseyePosition, currSpeed);
        Vector2 currDirection = InfoPacket.BirdseyePosition - oldPos;
        meshManipulator.SetDirection(currDirection);
        GlobalPosition = InfoPacket.BirdseyePosition;
    }

    /**
     * Calculations that should only be called every once in a while.
     */
    private void PerformIntermittentCalculations()
    {
        navigator.UpdateTargetPosition(InfoPacket.BirdseyePosition, InfoPacket.PolarDirection, currSpeed);
        NeighborAveragePosition = detectionArea.GetAverageNeighborPosition();
        director.UpdateDesiredDirection();
        sounderCommunicator.AnnounceHogInfoToSounder(InfoPacket);
    }

    /**
     * Move the current direction and speed towards the desired direction and speed.
     */
    private void AdjustCurrentVelocity(double delta)
    {
        InfoPacket.PolarDirection = Mathf.LerpAngle(InfoPacket.PolarDirection, 
            director.HogDesiredPolarDirection, DIRECTION_LERP * (float)delta);
        currSpeed = Mathf.MoveToward(currSpeed, SounderInfo.DesiredSpeed, SPEED_LERP * (float)delta);
    }

    /**
     * This function runs when the hog is deceased.
     */
    private void Die()
    {
        if(InfoPacket.IsAlive)
        {
            InfoPacket.IsAlive = false;
            GD.Print("Hog '" + Name + "' has died");
            sounderCommunicator.AnnounceHogInfoToSounder(InfoPacket);
            QueueFree();
        }
    }
}