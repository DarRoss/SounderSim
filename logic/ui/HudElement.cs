using Godot;

public abstract partial class HudElement : Control
{
	protected CamController camera;

	protected Color drawColor;
	protected Vector2 GetPosOnHud(Vector2 birdseyePos)
	{
		return birdseyePos * camera.CamTransform + GetViewportRect().Size / 2;
	}

    public override void _Ready()
    {
		camera = GetNode<CamController>("../..");
    }

    public override void _Process(double delta)
	{
		QueueRedraw();
	}
}
