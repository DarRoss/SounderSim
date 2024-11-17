using Godot;

public enum HazardDirection
{
	Radial,
	Horizontal,
	Vertical,
	Up,
	Down,
	Left,
	Right
}

public partial class NavigationHazard : Area2D
{
	[Export]
	private HazardDirection dirType;
	[Export]
	/**
	 * Gradient boundary determines the hazard's "wall"
	 * A value of 0: the hazard wall starts at the outer edge.
	 * A value of 1: the hazard wall starts at the inner edge / center of the shape.
	 */
	private float gradientBoundary = 0.5f;
	private RectangleShape2D collisionRectangle;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		collisionRectangle = GetNode<CollisionShape2D>("CollisionShape").Shape as RectangleShape2D;
	}

	/**
	 * Closeness determines how close you are to the hazard's wall.
	 * A value of 0: as far from the wall as possible.
	 * A value of 1: touching / inside the wall.
	 */
	public float GetHazardCloseness(Vector2 detectorPos)
	{
		float output = 0;
		Vector2 toDetectorVec = Position - detectorPos;
		switch(dirType)
		{
			case HazardDirection.Radial:

				break;
			case HazardDirection.Horizontal:

				break;
			case HazardDirection.Vertical:

				break;
			case HazardDirection.Up:

				break;
			case HazardDirection.Down:

				break;
			case HazardDirection.Left:

				break;
			case HazardDirection.Right:

				break;
		}
		return output;
	}
}
