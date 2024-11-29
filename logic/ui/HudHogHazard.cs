using Godot;

public partial class HudHogHazard : HudElement
{
	private const int REPEL_VEC_SCALE = 20;
	private const float RADIUS = 4;
	private const float THICKNESS = 1;

    public override void _Ready()
    {
		base._Ready();
		drawColor = Colors.Orange;
    }

    public override void _Draw()
    {
		Hog currHog = Global.Instance.InspectionHog;
		if(currHog != null && !currHog.AverageHazardRepelVector.IsZeroApprox())
		{
			Vector2 hogPos = GetPosOnHud(currHog.BirdseyePosition);
			Vector2 repelVec = GetPosOnHud(currHog.BirdseyePosition + currHog.AverageHazardRepelVector * REPEL_VEC_SCALE);
			DrawCircle(repelVec, RADIUS, drawColor, false, THICKNESS);
			DrawLine(hogPos, repelVec, drawColor);
		}
    }
}
