using Godot;

public partial class HogMeshManipulator : Node2D
{
    public void SetDirection(Vector2 direction)
    {
        Rotation = Vector2.Right.AngleTo(direction);
    }
}