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
    private HogDetectionArea neighborDetectionArea;
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
    public H2sInfo HogInfoPacket{get; private set;} = new();
    public Vector2 NeighborAveragePosition{get; private set;}

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
        neighborDetectionArea = GetNode<HogDetectionArea>("HogDetectionArea");
        navigator = GetNode<HogNavigator>("HogNavigator");
        director = GetNode<HogDirector>("HogDirector");
        meshManipulator = GetNode<HogMeshManipulator>("HogMeshManipulator");

        // update packet information
        HogInfoPacket.HogName = Name;
        HogInfoPacket.BirdseyePosition = GlobalPosition;

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
        AdjustCurrentVelocity(delta);
        HogInfoPacket.BirdseyePosition = navigator.GetNextPosition(delta, HogInfoPacket.BirdseyePosition, currSpeed);
        meshManipulator.SetDirection(HogInfoPacket.PolarDirection);
        GlobalPosition = HogInfoPacket.BirdseyePosition;
    }

    /**
     * Calculations that should only be called every once in a while.
     */
    private void PerformIntermittentCalculations()
    {
        navigator.UpdateTargetPosition(HogInfoPacket.BirdseyePosition, HogInfoPacket.PolarDirection, currSpeed);
        NeighborAveragePosition = neighborDetectionArea.GetAverageNeighborPosition();
        director.UpdateDesiredDirection();
        sounderCommunicator.AnnounceHogInfoToSounder(HogInfoPacket);

        if(Name == "Hog1")
        {
//            GD.Print(navigator.TargetPosition);
//            if(navigator.pathPts.Length > 0)
//            {
//                GD.Print(navigator.pathPts[0]);
//            }
        }
    }

    /**
     * Move the current direction and speed towards the desired direction and speed.
     */
    private void AdjustCurrentVelocity(double delta)
    {
        HogInfoPacket.PolarDirection = Mathf.LerpAngle(HogInfoPacket.PolarDirection, 
            director.HogDesiredPolarDirection, DIRECTION_LERP * (float)delta);
        currSpeed = Mathf.MoveToward(currSpeed, SounderInfo.DesiredSpeed, SPEED_LERP * (float)delta);
    }

    /**
     * This function runs when the hog is deceased.
     */
    private void Die()
    {
        if(HogInfoPacket.IsAlive)
        {
            HogInfoPacket.IsAlive = false;
            GD.Print("Hog '" + Name + "' has died");
            sounderCommunicator.AnnounceHogInfoToSounder(HogInfoPacket);
            QueueFree();
        }
    }
}