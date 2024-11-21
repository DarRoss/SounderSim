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
	}

	protected override float GetRepelLinePos()
	{
		float output = Mathf.Inf;
		switch(directionType)
		{
			case HazardRectDirection.Horizontal:
				output = repelBoundary * rectHalfSize.X;
				break;
			case HazardRectDirection.Vertical:
				output = repelBoundary * rectHalfSize.Y;
				break;
			case HazardRectDirection.Up:
				output = rectHalfSize.Y * (2 * repelBoundary - 1);
				break;
			case HazardRectDirection.Down:
				output = rectHalfSize.Y * (1 - 2 * repelBoundary);
				break;
			case HazardRectDirection.Right:
				output = rectHalfSize.X * (2 * repelBoundary - 1);
				break;
			case HazardRectDirection.Left:
				output = rectHalfSize.X * (1 - 2 * repelBoundary);
				break;
		}
		return output;
	}

	public override Vector2 GetRepelVector(Vector2 clientBirdseyePos)
	{
		Vector2 repelVector = Vector2.Zero;
		Vector2 vecToClient = birdseyePosition - clientBirdseyePos;
		Vector2 clientRelToHaz = vecToClient * GetTransform().Inverse();
		// check if the client is within the rectangle bounds
		if( clientRelToHaz.X <=  rectHalfSize.X &&
			clientRelToHaz.X >= -rectHalfSize.X &&
			clientRelToHaz.Y <=  rectHalfSize.Y &&
			clientRelToHaz.Y >= -rectHalfSize.Y)
		{
			float repelMagnitude = 1.0f;
			switch(directionType)
			{
				case HazardRectDirection.Horizontal:
					repelVector = clientRelToHaz.X < 0 ? Vector2.Left : Vector2.Right;
					/**
					 * Gradient requirements, one must apply:
					 * - Client is on the left half of the rectangle and is to the left of the repel line
					 * - Client is on the right half of the rectangle and is to the right of the repel line
					 */
					if(clientRelToHaz.X < 0 && clientRelToHaz.X < -repelLinePos
						|| clientRelToHaz.X >= 0 && clientRelToHaz.X > repelLinePos)
					{
						// calculate gradient value
						repelMagnitude = (rectHalfSize.X - Mathf.Abs(clientRelToHaz.X)) 
							/ (rectHalfSize.X - Mathf.Abs(repelLinePos));
					}
					break;
				case HazardRectDirection.Vertical:
					repelVector = clientRelToHaz.Y < 0 ? Vector2.Down : Vector2.Up;
					/**
					 * Gradient requirements, one must apply:
					 * - Client is on the lower half of the rectangle and is below the repel line
					 * - Client is on the upper half of the rectangle and is above the repel line
					 */
					if(clientRelToHaz.Y < 0 && clientRelToHaz.Y < -repelLinePos
						|| clientRelToHaz.Y >= 0 && clientRelToHaz.Y > repelLinePos)
					{
						// calculate gradient value
						repelMagnitude = (rectHalfSize.Y - Mathf.Abs(clientRelToHaz.Y)) 
							/ (rectHalfSize.Y - Mathf.Abs(repelLinePos));
					}
					break;
				case HazardRectDirection.Up:
					// check if client is above the peak repel line
					if(clientRelToHaz.Y > repelLinePos)
					{
						repelMagnitude = (rectHalfSize.Y - clientRelToHaz.Y) / (rectHalfSize.Y - repelLinePos);
					}
					repelVector = Vector2.Up;
					break;
				case HazardRectDirection.Down:
					// check if client is below the peak repel line
					if(clientRelToHaz.Y < repelLinePos)
					{
						repelMagnitude = (clientRelToHaz.Y + rectHalfSize.Y) / (rectHalfSize.Y + repelLinePos);
					}
					repelVector = Vector2.Down;
					break;
				case HazardRectDirection.Right:
					// check if client is to the right of the peak repel line
					if(clientRelToHaz.X > repelLinePos)
					{
						repelMagnitude = (rectHalfSize.X - clientRelToHaz.X) / (rectHalfSize.X - repelLinePos);
					}
					repelVector = Vector2.Right;
					break;
				case HazardRectDirection.Left:
					// check if client is to the left of the peak repel line
					if(clientRelToHaz.X < repelLinePos)
					{
						repelMagnitude = (clientRelToHaz.X + rectHalfSize.X) / (rectHalfSize.X + repelLinePos);
					}
					repelVector = Vector2.Left;
					break;
			}
			repelVector *= repelMagnitude;
		}
		return repelVector * GetTransform();
	}
}