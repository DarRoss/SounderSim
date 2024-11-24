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
    [Signal]
    public delegate void AnnounceHogDeathEventHandler(string hogName);

    // nodes of interest
    private HogDetectionArea detectionArea;
    private HogNavigator navigator;
    private HogDirector director;
    private HogMeshManipulator meshManipulator;

    // other nodes
    private Timer intermittentTimer = new();

    // speed that the hog is currently moving at
    private float currSpeed;

    /**
     * public hog variables
     */
    // true if the hog is not dead
    public bool IsAlive{get; set;}
    // the position of the hog in the world
    public Vector2 BirdseyePosition{get; set;}
    // the polar coordinate direction that the hog is pointing. -pi uninclusive to +pi inclusive
    public float PolarDirection{get; set;} = (float)GD.RandRange(-Mathf.Pi, Mathf.Pi);
    public Vector2 AverageNeighborPosition{get; private set;}
    // points away from nearby hazards
    public Vector2 AverageHazardRepelVector{get; private set;}
    // the sounder that owns this hog
    public Sounder OwnerSounder{get; private set;}

    public override void _Ready()
    {
        // initialize nodes of interest
        detectionArea = GetNode<HogDetectionArea>("HogDetectionArea");
        navigator = GetNode<HogNavigator>("HogNavigator");
        director = GetNode<HogDirector>("HogDirector");
        meshManipulator = GetNode<HogMeshManipulator>("HogMeshManipulator");
        OwnerSounder = GetNode<Sounder>("../..");

        BirdseyePosition = GlobalPosition;

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
        Vector2 oldPos = BirdseyePosition;
        BirdseyePosition = navigator.GetNextPosition(delta, BirdseyePosition, currSpeed);
        Vector2 currDirection = BirdseyePosition - oldPos;
        meshManipulator.SetDirection(currDirection);
        GlobalPosition = BirdseyePosition;
    }

    /**
     * Calculations that should only be called every once in a while.
     */
    private void PerformIntermittentCalculations()
    {
        AverageHazardRepelVector = detectionArea.GetAverageHazardRepelVector();
        navigator.UpdateTargetPosition(BirdseyePosition, PolarDirection, currSpeed);
        AverageNeighborPosition = detectionArea.GetAverageNeighborPosition();
        director.UpdateDesiredDirection();
    }

    /**
     * Move the current direction and speed towards the desired direction and speed.
     */
    private void AdjustCurrentVelocity(double delta)
    {
        PolarDirection = Mathf.LerpAngle(PolarDirection, 
            director.HogDesiredPolarDirection, DIRECTION_LERP * (float)delta);
        currSpeed = Mathf.MoveToward(currSpeed, OwnerSounder.DesiredSpeed, SPEED_LERP * (float)delta);
    }

    /**
     * This function runs when the hog is deceased.
     */
    private void Die()
    {
        if(IsAlive)
        {
            IsAlive = false;
            GD.Print("Hog '" + Name + "' has died");
            EmitSignal(SignalName.AnnounceHogDeath, Name);
            QueueFree();
        }
    }
}