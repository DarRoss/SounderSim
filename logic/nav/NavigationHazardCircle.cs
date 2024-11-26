using Godot;

public partial class NavigationHazardCircle : NavigationHazard
{
	private CircleShape2D collisionCircle;

    public override void _Ready()
	{
		collisionCircle = GetNode<CollisionShape2D>("CollisionCircle").Shape as CircleShape2D;
		base._Ready();
	}

    protected override float GetRepelLinePos()
    {
		return collisionCircle.Radius * repelGradientDepth;
    }

    public override Vector2 GetRepelVector(Vector2 clientBirdseyePos)
    {
		Vector2 output = Vector2.Zero;
		Vector2 vecToClient = clientBirdseyePos - birdseyePosition;
		float clientDist = vecToClient.Length();
		// check if client is within the hazard circle
		if(clientDist <= collisionCircle.Radius)
		{
			// normalize the client position vector
			output = vecToClient / clientDist;
			// check if client is in the gradient zone
			if(clientDist > gradientLinePos)
			{
				// apply gradient value to repel vector
				output *= (collisionCircle.Radius - clientDist) / (collisionCircle.Radius - gradientLinePos);
			}
		}
		return output;
    }
}
