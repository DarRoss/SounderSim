using Godot;

public partial class SelectionSquare : Control
{
	private const int WIDTH = 20;
	private const int THICKNESS = 2;

	private CamController camera;

	private Color color = Colors.Green;
	private Vector2 squarePos = Vector2.Zero;
	private Rect2 square = new(0, 0, WIDTH, WIDTH);
	private Vector2 halfWidthVec = new(WIDTH / 2, WIDTH / 2);

    public override void _Ready()
    {
		camera = GetNode<CamController>("../..");
    }

    public override void _Process(double delta)
	{
		QueueRedraw();
	}

    public override void _Draw()
    {
		Hog currHog = Global.Instance.InspectionHog;
		if(currHog != null)
		{
			square.Position = Global.Instance.InspectionHog.BirdseyePosition * camera.CamTransform
				+ GetViewport().GetVisibleRect().Size / 2 - halfWidthVec;
			DrawRect(square, color, false, THICKNESS);
		}
    }
}
