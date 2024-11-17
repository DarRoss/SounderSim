using Godot;

public partial class NavRegionGlobal : NavigationRegion2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Callable.From(MapSetup).CallDeferred();
	}

	private async void MapSetup()
	{
		// Wait for the first physics frame so the NavigationServer can sync.
		await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
		Global.Instance.MapRid = GetNavigationMap();
	}
}
