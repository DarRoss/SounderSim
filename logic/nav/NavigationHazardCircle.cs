using Godot;

public partial class NavigationHazardCircle : NavigationHazard
{
	private CircleShape2D collisionCircle;

    public override void _Ready()
	{
		collisionCircle = GetNode<CollisionShape2D>("CollisionCircle").Shape as CircleShape2D;
	}

    protected override float GetRepelLinePos()
    {
		return collisionCircle.Radius * repelBoundary;
    }

    public override Vector2 GetRepelVector(Vector2 clientBirdseyePos)
    {
		Vector2 vecToClient = clientBirdseyePos - birdseyePosition;
		float clientDist = vecToClient.Length();
		// normalize the client position vector
		Vector2 output = vecToClient / clientDist;
		// check if client is in the gradient zone
		if(clientDist > repelLinePos)
		{
			output *= (collisionCircle.Radius - clientDist) / (collisionCircle.Radius - repelLinePos);
		}
		return output;
    }
}
