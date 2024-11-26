using Godot;

public abstract partial class NavigationHazard : Area2D
{
	/**
	 * Determines the end of the repellent gradient and start of the peak repellent wall. 0 to 1 inclusive.
	 * - A value of 1: peak repellent starts at the shape's leading edge. The entire shape is 100% (peak) repellent.
	 * - A value of 0: peak repellent starts at the shape's trailing edge / center of the shape. 
	 *   This means that repellent value increases as you get closer to the trailing edge / center.
	 */
	[Export]
	protected float repelGradientDepth = 1;
	/**
	 * The position of the line that divides the peak repellent zone and the repellent gradient.
	 * - May either be horizontal, vertical or radial depending on the hazard direction.
     * - (-r <= gradientLinePos <= r), where r is the hazard's halfwidth or radius.
	 * - Is always a non negative value.
	 */
	protected float gradientLinePos;
	protected Vector2 birdseyePosition;

	public override void _Ready()
	{
		gradientLinePos = GetRepelLinePos();
		birdseyePosition = Global.GlobalPosToBirdseye(GlobalPosition);
	}

    private void ProcessInputEvent(Node viewport, InputEvent ie, long shapeIdx)
    {
        if(ie is InputEventMouseButton iemb)
        {
            if(iemb.Pressed)
            {
                // hazard area clicked
            }
        }
    }

    protected abstract float GetRepelLinePos();
	public abstract Vector2 GetRepelVector(Vector2 clientBirdseyePos);
}