using Godot;

public abstract partial class NavigationHazard : Area2D
{
	/**
	 * Determines the location of the hazard's peak repellent wall and the end of the gradient. 0 to 1 inclusive.
	 * - A value of 1: peak repellent starts at the shape's leading edge. The entire shape is 100% (peak) repellent.
	 * - A value of 0: peak repellent starts at the shape's trailing edge / center of the shape. 
	 *   This means that repellent value increases as you get closer to the trailing edge / center.
	 */
	[Export]
	protected float repelGradientEnd = 1;
	/**
	 * The position of the line that divides the peak repellent zone and the repellent gradient.
	 * - May either be horizontal, vertical or radial depending on the hazard direction.
     * - Is less than or equal to the hazard's halfwidth / radius.
	 * - Is always a non negative value.
	 */
	protected float repelLinePos;
	protected Vector2 birdseyePosition;

	public override void _Ready()
	{
		repelLinePos = GetRepelLinePos();
		birdseyePosition = GlobalPosition;
	}

    protected abstract float GetRepelLinePos();
	public abstract Vector2 GetRepelVector(Vector2 clientBirdseyePos);
}