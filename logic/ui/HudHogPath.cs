using Godot;

public partial class HudHogPath : HudElement
{
	private const float RADIUS = 2;

    public override void _Ready()
    {
		base._Ready();
		drawColor = Colors.Red;
    }

    public override void _Draw()
    {
		Hog currHog = Global.Instance.InspectionHog;
		if(currHog != null)
		{
			Vector2 pos;
			Vector2 prevPos = Vector2.Inf;
			Vector2[] PathPoints = currHog.PathPoints;
			foreach(Vector2 pt in PathPoints)
			{
				pos = pt * camera.CamTransform + GetViewportRect().Size / 2;
				DrawCircle(pos, RADIUS, drawColor);
				if(!prevPos.IsEqualApprox(Vector2.Inf))
				{
					DrawDashedLine(prevPos, pos, drawColor);
				}
				prevPos = pos;
			}
		}
    }
}
