using Godot;

/**
 * Contains global variables.
 */
public partial class Global : Node
{
    public static Global Instance {get; private set;} = null;
    // RID is 0 by default
    public Rid MapRid{get; set;} = new();

    public override void _Ready()
    {
        Instance ??= this;
    }
}