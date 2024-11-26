using Godot;

public partial class CamController : Camera2D
{
	private float MOVE_SPEED = 500;
	private float ZOOM_SPEED = 1;

	private Vector2 moveDelta;
	private float zoomDelta;
	private float speedFactor = 1;

	public override void _Input(InputEvent ie)
    {
        if(ie.IsAction("MoveUp") || ie.IsAction("MoveDown"))
        {
			moveDelta.Y = Input.GetActionStrength("MoveUp") - Input.GetActionStrength("MoveDown");
        }
        if(ie.IsAction("MoveLeft") || ie.IsAction("MoveRight"))
        {
			moveDelta.X = Input.GetActionStrength("MoveRight") - Input.GetActionStrength("MoveLeft");
        }
        if(ie.IsAction("ZoomIn") || ie.IsAction("ZoomOut"))
        {
			zoomDelta = Input.GetActionStrength("ZoomIn") - Input.GetActionStrength("ZoomOut");
        }
        if(ie.IsAction("SpeedUp") || ie.IsAction("SpeedDown"))
        {
			// speedDelta is in [-1,1]
			float speedDelta = Input.GetActionStrength("SpeedUp") - Input.GetActionStrength("SpeedDown");
			// speedFactor is in [0.2, 2.2]
			speedFactor = speedDelta + 1.2f;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if(!moveDelta.IsZeroApprox())
		{
			// movement slows down when zoomed in
			GlobalPosition += 1 / Zoom.X * moveDelta * speedFactor * MOVE_SPEED * (float)delta;
		}

		if(!Mathf.IsZeroApprox(zoomDelta))
		{
			// zoom speeds up when already zoomed in
			float zoomVal = Zoom.X * (1 + zoomDelta * ZOOM_SPEED * (float)delta);
			Zoom = new Vector2(zoomVal, -zoomVal);
		}
    }
}
