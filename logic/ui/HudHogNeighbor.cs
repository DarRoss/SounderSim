using Godot;

public partial class HudHogNeighbor : HudElement
{
	private const int WIDTH = 15;
	private const int THICKNESS = 2;

	private Rect2 square = new(0, 0, WIDTH, WIDTH);
	private Vector2 halfWidthVec = new(WIDTH / 2, WIDTH / 2);

    public override void _Ready()
    {
		base._Ready();
		drawColor = Colors.Cyan;
    }

    public override void _Draw()
    {
		Hog currHog = Global.Instance.InspectionHog;
		if(currHog != null)
		{
			square.Position = GetPosOnHud(currHog.AverageNeighborPosition) - halfWidthVec;
			DrawRect(square, drawColor, false, THICKNESS);
		}
    }
}
