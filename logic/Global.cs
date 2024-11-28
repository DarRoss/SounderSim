using Godot;

/**
 * Contains global variables.
 */
public partial class Global : Node
{
    [Signal]
    public delegate void InspectionHogChangedEventHandler();

    public static Global Instance {get; private set;} = null;
    // RID is 0 by default
    public Rid MapRid{get; set;} = new();

    private Hog _inspectionHog;
    public Hog InspectionHog
    {
        get => _inspectionHog;
        set
        {
            _inspectionHog = value;
            EmitSignal(SignalName.InspectionHogChanged);
        }
    }

    public override void _Ready()
    {
        Instance ??= this;
    }

    public static Vector2 GlobalPosToBirdseye(Vector2 globalPos)
    {
        return globalPos;
    }

    public static Vector2 BirdseyePosToGlobal(Vector2 birdseyePos)
    {
        return birdseyePos;
    }
}