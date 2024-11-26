using Godot;

public enum HazardRectDirection
{
	Up,
	Down,
	Left,
	Right,
	Horizontal,
	Vertical
}

public partial class NavigationHazardRect : NavigationHazard
{
	[Export]
	private HazardRectDirection directionType;

	private RectangleShape2D collisionRectangle;
	private Vector2 rectHalfSize;

	public override void _Ready()
	{
		collisionRectangle = GetNode<CollisionShape2D>("CollisionRectangle").Shape as RectangleShape2D;
		rectHalfSize = collisionRectangle.Size / 2;
		base._Ready();
	}

	protected override float GetRepelLinePos()
	{
		float output = Mathf.Inf;
		switch(directionType)
		{
			case HazardRectDirection.Horizontal:
				output = repelGradientDepth * rectHalfSize.X;
				break;
			case HazardRectDirection.Vertical:
				output = repelGradientDepth * rectHalfSize.Y;
				break;
			case HazardRectDirection.Up:
				output = rectHalfSize.Y * (2 * repelGradientDepth - 1);
				break;
			case HazardRectDirection.Down:
				output = rectHalfSize.Y * (1 - 2 * repelGradientDepth);
				break;
			case HazardRectDirection.Right:
				output = rectHalfSize.X * (2 * repelGradientDepth - 1);
				break;
			case HazardRectDirection.Left:
				output = rectHalfSize.X * (1 - 2 * repelGradientDepth);
				break;
		}
		return output;
	}

	public override Vector2 GetRepelVector(Vector2 clientBirdseyePos)
	{
		Vector2 repelVector = Vector2.Zero;
		// assume that rectangle is axis aligned
		Vector2 cliPosRelToHaz = clientBirdseyePos - birdseyePosition;
		// check if the client is within the rectangle bounds
		if( cliPosRelToHaz.X <=  rectHalfSize.X &&
			cliPosRelToHaz.X >= -rectHalfSize.X &&
			cliPosRelToHaz.Y <=  rectHalfSize.Y &&
			cliPosRelToHaz.Y >= -rectHalfSize.Y)
		{
			repelVector = GetRepelDirection(cliPosRelToHaz);
			float repelMagnitude = GetRepelMagnitude(cliPosRelToHaz);
			if(repelMagnitude < 1)
			{
				// repel vector is scaled according to the calculated magnitude
				repelVector *= repelMagnitude;
			}
		}
		// rotate repel vector according to the hazard's rotation
		return repelVector;
	}

	private Vector2 GetRepelDirection(Vector2 cliPosRelToHaz)
	{
		Vector2 repelVector = Vector2.Zero;
		switch(directionType)
		{
			case HazardRectDirection.Horizontal:
				repelVector = cliPosRelToHaz.X < 0 ? Vector2.Left : Vector2.Right;
				break;
			case HazardRectDirection.Vertical:
				repelVector = cliPosRelToHaz.Y < 0 ? Vector2.Down : Vector2.Up;
				break;
			case HazardRectDirection.Up:
				repelVector = Vector2.Up;
				break;
			case HazardRectDirection.Down:
				repelVector = Vector2.Down;
				break;
			case HazardRectDirection.Right:
				repelVector = Vector2.Right;
				break;
			case HazardRectDirection.Left:
				repelVector = Vector2.Left;
				break;
		}
		return repelVector;
	}

	private float GetRepelMagnitude(Vector2 cliPosRelToHaz)
	{
		float repelMagnitude = 1.0f;
		switch(directionType)
		{
			case HazardRectDirection.Horizontal:
				/**
				 * Gradient requirements, one must apply:
				 * - Client is on the left half of the rectangle and is to the left of the repel line
				 * - Client is on the right half of the rectangle and is to the right of the repel line
				 */
				if(cliPosRelToHaz.X < 0 && cliPosRelToHaz.X < -gradientLinePos
					|| cliPosRelToHaz.X >= 0 && cliPosRelToHaz.X > gradientLinePos)
				{
					// calculate gradient value
					repelMagnitude = (rectHalfSize.X - Mathf.Abs(cliPosRelToHaz.X)) 
						/ (rectHalfSize.X - Mathf.Abs(gradientLinePos));
				}
				break;
			case HazardRectDirection.Vertical:
				/**
				 * Gradient requirements, one must apply:
				 * - Client is on the lower half of the rectangle and is below the repel line
				 * - Client is on the upper half of the rectangle and is above the repel line
				 */
				if(cliPosRelToHaz.Y < 0 && cliPosRelToHaz.Y < -gradientLinePos
					|| cliPosRelToHaz.Y >= 0 && cliPosRelToHaz.Y > gradientLinePos)
				{
					// calculate gradient value
					repelMagnitude = (rectHalfSize.Y - Mathf.Abs(cliPosRelToHaz.Y)) 
						/ (rectHalfSize.Y - Mathf.Abs(gradientLinePos));
				}
				break;
			case HazardRectDirection.Up:
				// check if client is above the peak repel line
				if(cliPosRelToHaz.Y > gradientLinePos)
				{
					repelMagnitude = (rectHalfSize.Y - cliPosRelToHaz.Y) / (rectHalfSize.Y - gradientLinePos);
				}
				break;
			case HazardRectDirection.Down:
				// check if client is below the peak repel line
				if(cliPosRelToHaz.Y < gradientLinePos)
				{
					repelMagnitude = (rectHalfSize.Y + cliPosRelToHaz.Y) / (rectHalfSize.Y + gradientLinePos);
				}
				break;
			case HazardRectDirection.Right:
				// check if client is to the right of the peak repel line
				if(cliPosRelToHaz.X > gradientLinePos)
				{
					repelMagnitude = (rectHalfSize.X - cliPosRelToHaz.X) / (rectHalfSize.X - gradientLinePos);
				}
				break;
			case HazardRectDirection.Left:
				// check if client is to the left of the peak repel line
				if(cliPosRelToHaz.X < gradientLinePos)
				{
					repelMagnitude = (rectHalfSize.X + cliPosRelToHaz.X) / (rectHalfSize.X + gradientLinePos);
				}
				break;
		}
		return repelMagnitude;
	}
}