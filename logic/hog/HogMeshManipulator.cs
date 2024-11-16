using Godot;

public partial class HogMeshManipulator : Node2D
{
    public void SetDirection(float direction)
    {
        Rotation = direction;
    }
}