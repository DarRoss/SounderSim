using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class NavMeshChunks : Node2D
{
	private const float MAP_CELL_SIZE = 1;
	private const int CHUNK_SIZE = 256;
	private const float CELL_SIZE = 1;
	private const float AGENT_RADIUS = 10;
	private static readonly Dictionary<Vector2I, NavigationRegion2D> chunkIdToRegion = new();

	private Node2D parseRootNode;
	private Node2D chunkContainer;
	private Rid map;

    public override void _Ready()
    {
		parseRootNode = GetNode<Node2D>("ParseRootNode");
		chunkContainer = GetNode<Node2D>("ChunkContainer");

		map = GetWorld2D().NavigationMap;
		NavigationServer2D.MapSetCellSize(map, MAP_CELL_SIZE);
		// disable performance-costly edge connection margin feature
		NavigationServer2D.MapSetUseEdgeConnections(map, false);

		// parse collisions shapes within the parse root node
		NavigationMeshSourceGeometryData2D sourceGeometry = new();
        NavigationPolygon parseSettings = new()
        {
            ParsedGeometryType = NavigationPolygon.ParsedGeometryTypeEnum.StaticColliders
        };
		NavigationServer2D.ParseSourceGeometryData(parseSettings, sourceGeometry, parseRootNode);

		// add an outline to define the navigable surface
		Vector2[] traversableOutline = {
			new(4096, 4096),
			new(-4096, 4096),
			new(-4096, -4096),
			new(4096, -4096),
		};
		sourceGeometry.AddTraversableOutline(traversableOutline);

		CreateRegionChunks(chunkContainer, sourceGeometry, CHUNK_SIZE * CELL_SIZE, AGENT_RADIUS);	

		Callable.From(MapSetup).CallDeferred();
    }

	private async void MapSetup()
	{
		// Wait for the first physics frame so the NavigationServer can sync.
		await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
		Global.Instance.MapRid = map;
	}

	private static void CreateRegionChunks(Node chunksRootNode, 
		NavigationMeshSourceGeometryData2D pSourceGeometry, float pChunkSize, float pAgentRadius)
	{
		Rect2 inputGeometryBounds = CalculateSourceGeometryBounds(pSourceGeometry);
		Vector2 startChunk = (inputGeometryBounds.Position / pChunkSize).Floor();
		Vector2 endChunk = ((inputGeometryBounds.Position + inputGeometryBounds.Size) / pChunkSize).Floor();

		Vector2I chunkId;
		Rect2 chunkBoundingBox;
		Rect2 bakingBounds;
		NavigationPolygon chunkNavmesh;
		Vector2[] navmeshVertices;
		NavigationRegion2D chunkRegion;
		Vector2 vertex;

		for(int chunkY = (int)startChunk.Y; chunkY < endChunk.Y + 1; ++chunkY)
		{
			for (int chunkX = (int)startChunk.X; chunkX < endChunk.X + 1; chunkX++)
			{
				chunkId = new(chunkX, chunkY);

				chunkBoundingBox = new(
					new Vector2(chunkX, chunkY) * pChunkSize, 
					new Vector2(pChunkSize, pChunkSize));
				
				bakingBounds = chunkBoundingBox.Grow(pChunkSize);

				chunkNavmesh = new()
				{
					ParsedGeometryType = NavigationPolygon.ParsedGeometryTypeEnum.StaticColliders,
					BakingRect = bakingBounds,
					BorderSize = pChunkSize,
					AgentRadius = pAgentRadius
				};
				NavigationServer2D.BakeFromSourceGeometryData(chunkNavmesh, pSourceGeometry);

				chunkNavmesh.BakingRect = new();

				navmeshVertices = chunkNavmesh.GetVertices();
				for(int i = 0; i < navmeshVertices.Length; ++i)
				{
					vertex = navmeshVertices[i];
					navmeshVertices[i] = vertex.Snapped(MAP_CELL_SIZE * 0.1f);
				}
				chunkNavmesh.SetVertices(navmeshVertices);

                chunkRegion = new()
                {
                    NavigationPolygon = chunkNavmesh
                };

				chunksRootNode.AddChild(chunkRegion);
				chunkIdToRegion[chunkId] = chunkRegion;
            }
		}
	}

	private static Rect2 CalculateSourceGeometryBounds(NavigationMeshSourceGeometryData2D pSourceGeometry)
	{
		Rect2 bounds = new();
		bool firstVertex = true;

		foreach (Vector2[] traversableOutline in pSourceGeometry.GetTraversableOutlines())
		{
			foreach (Vector2 traversablePoint in traversableOutline)
			{
				if(firstVertex)
				{
					firstVertex = false;
					bounds.Position = traversablePoint;
				}
				else
				{
					bounds = bounds.Expand(traversablePoint);
				}
			}
		}

		foreach (Vector2[] obstructionOutline in pSourceGeometry.GetObstructionOutlines())
		{
			foreach (Vector2 obstructionPoint in obstructionOutline)
			{
				if(firstVertex)
				{
					firstVertex = false;
					bounds.Position = obstructionPoint;
				}
				else
				{
					bounds = bounds.Expand(obstructionPoint);
				}
			}
		}

		foreach (Godot.Collections.Dictionary projectedObstruction 
			in pSourceGeometry.GetProjectedObstructions().Select(v => (Godot.Collections.Dictionary)v))
		{
			float[] projectedObstructionVertices = (float[])projectedObstruction["vertices"];
			Vector2 vertex;
			for (int i = 0; i < projectedObstructionVertices.Length / 2; ++i)
			{
				vertex = new(projectedObstructionVertices[i * 2], projectedObstructionVertices[i * 2 + 1]);
				if(firstVertex)
				{
					firstVertex = false;
					bounds.Position = vertex;
				}
				else
				{
					bounds = bounds.Expand(vertex);
				}
			}
		}
		return bounds;
	}
}
