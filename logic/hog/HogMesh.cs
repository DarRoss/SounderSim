using Godot;

public partial class HogMesh : MeshInstance2D
{
    public void SetDirection(float direction)
    {
        Rotation = direction;
    }
}