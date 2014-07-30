using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CatchingMiceLevelManager : LugusSingletonExisting<CatchingMiceLevelManagerDefault>
{

}

public class CatchingMiceLevelManagerDefault : MonoBehaviour
{
	// Accessors
	#region Accessors
	public Transform ObjectParent
	{
		get
		{
			return objectParent;
		}
	}
	public Transform CookieParent
	{
		get
		{
			return cookieParent;
		}
	}
	public CatchingMiceLevelDefinition CurrentLevel
	{
		get
		{
			return currentLevel;
		}
		set
		{
			currentLevel = value;
		}
	}
	public int Width
	{
		get
		{
			return width;
		}
	}
	public int Height
	{
		get
		{
			return height;
		}
	}
	public CatchingMiceTile[,] Tiles
	{
		get
		{
			return tiles;
		}
	}
	public List<CatchingMiceCharacterPlayer> Players
	{
		get
		{
			return playerList;
		}
	}
	public List<CatchingMiceWaypoint> Waypoints
	{
		get
		{
			return waypointList;
		}
	}
	public List<CatchingMiceWaveDefinition> Waves
	{
		get
		{
			return wavesList;
		}
	}
	public List<CatchingMiceHole> MiceHoles
	{
		get
		{
			return miceHoles;
		}
	}
	public List<CatchingMiceTile> TrapTiles
	{
		get
		{
			return trapTiles;
		}
	}
	public List<CatchingMiceTile> CheeseTiles
	{
		get
		{
			return cheeseTiles;
		}
	}
	public List<CatchingMiceTile> FakeCheeseTiles
	{
		get
		{
			return fakeCheeseTiles;
		}
	}
	public List<CatchingMiceCharacterMouse> Enemies
	{
		get
		{
			return enemies;
		}
	}
	public CatchingMiceCage Cage
	{
		get
		{
			return cage;
		}
		set
		{
			cage = value;
		}
	}
	#endregion

	// Events
	#region Events
	public delegate void TrapRemovedEventHandler(CatchingMiceTile trapTile);
	public event TrapRemovedEventHandler OnTrapRemoved;

	public delegate void CheeseRemovedEventHandler(CatchingMiceTile cheeseTile);
	public event CheeseRemovedEventHandler OnCheeseRemoved;

	public delegate void LevelBuiltEventHandler();
	public event LevelBuiltEventHandler OnLevelBuilt;
	#endregion

	// Inspector
	public float scale = 1;

	// Prefabs
	#region Prefabs
	public CatchingMiceFurniture[] furniturePrefabs = null;
	public CatchingMiceObstacle[] obstaclePrefabs = null;
	public CatchingMiceCheese[] cheesePrefabs = null;
	public CatchingMiceTrap[] trapPrefabs = null;
	public CatchingMiceHole[] holePrefabs = null;
	public CatchingMiceCharacterPlayer[] characterPrefabs = null;
	public CatchingMiceCharacterPatrol[] patrolPrefabs = null;
	public CatchingMiceCharacterMouse[] enemyPrefabs = null;
	public GameObject[] cookiePrefabs = null;
	public GameObject miceStepsPrefab = null;
	public Sprite floorTileSprite = null;
	public GameObject[] wallPieces = null;
	#endregion
	
	// Protected
	#region Protected
	protected Transform objectParent = null;
	protected Transform levelRoot = null;
	protected Transform levelParent = null;
	protected Transform navigationParent = null;
	protected Transform characterParent = null;
	protected Transform spawnParent = null;
	protected Transform enemyParent = null;
	protected Transform cookieParent = null;
	protected Transform patrolParent = null;

	protected CatchingMiceLevelDefinition currentLevel = null;
	protected int width = 13;
	protected int height = 13;

	protected CatchingMiceCage cage = null;

	protected CatchingMiceTile[,] tiles = null;

	protected List<CatchingMiceWaypoint> waypointList = new List<CatchingMiceWaypoint>();
	protected List<CatchingMiceWaveDefinition> wavesList = new List<CatchingMiceWaveDefinition>();
	protected List<CatchingMiceTile> holeTiles = new List<CatchingMiceTile>();
	protected List<CatchingMiceTile> trapTiles = new List<CatchingMiceTile>();
	protected List<CatchingMiceTile> cheeseTiles = new List<CatchingMiceTile>();
	protected List<CatchingMiceTile> fakeCheeseTiles = new List<CatchingMiceTile>();
	protected List<CatchingMiceHole> miceHoles = new List<CatchingMiceHole>();
	protected List<CatchingMiceCharacterPlayer> playerList = new List<CatchingMiceCharacterPlayer>();
	protected List<CatchingMiceCharacterPatrol> patrols = new List<CatchingMiceCharacterPatrol>();
	protected List<CatchingMiceCharacterMouse> enemies = new List<CatchingMiceCharacterMouse>();
	protected List<GameObject> enemyParentList = new List<GameObject>();
	protected float maxDepth = 0;

	protected ILugusCoroutineHandle spawnRoutine = null;
	#endregion

	void Awake()
	{
		FindReferences();
	}
	
	void FindReferences()
	{
		// only do this once
		if (levelRoot != null)
		{
			return;
		}

		if (levelRoot == null)
		{
			levelRoot = GameObject.Find("LevelRoot").transform;

			if (levelRoot == null)
			{
				CatchingMiceLogVisualizer.use.LogError("LevelManager: Could not find level root.");
			}
		}


		levelParent = levelRoot.FindChild("LevelParent");
		objectParent = levelRoot.FindChild("ObjectParent");
		navigationParent = levelRoot.FindChild("NavigationParent");
		characterParent = levelRoot.FindChild("CharacterParent");
		enemyParent = levelRoot.FindChild("EnemyParent");
		spawnParent = levelRoot.FindChild("SpawnParent");
		cookieParent = levelRoot.FindChild("CookieParent");
		patrolParent = levelRoot.FindChild("PatrolParent");

		spawnRoutine = LugusCoroutines.use.GetHandle();
	}

	public void ClearLevel()
	{
		Debug.Log("Clearing level (playing in editor).");
		
		for (int i = levelParent.childCount - 1; i >= 0; i--)
		{
			DestroyGameObject(levelParent.GetChild(i).gameObject);
		}
		
		for (int i = objectParent.childCount - 1; i >= 0; i--)
		{
			DestroyGameObject(objectParent.GetChild(i).gameObject);
		}
		
		for (int i = navigationParent.childCount - 1; i >= 0; i--)
		{
			DestroyGameObject(navigationParent.GetChild(i).gameObject);
		}
		
		for (int i = characterParent.childCount - 1; i >= 0; i--)
		{
			DestroyGameObject(characterParent.GetChild(i).gameObject);
		}
		
		for (int i = enemyParent.childCount - 1; i >= 0; i--)
		{
			DestroyGameObject(enemyParent.GetChild(i).gameObject);
		}
		
		for (int i = spawnParent.childCount - 1; i >= 0; i--)
		{
			DestroyGameObject(spawnParent.GetChild(i).gameObject);
		}

		for (int i = patrolParent.childCount - 1; i >= 0; i--)
		{
			DestroyGameObject(patrolParent.GetChild(i).gameObject);
		}

		tiles = null;
		playerList.Clear();
		waypointList.Clear();
		wavesList.Clear();
		enemyParentList.Clear();
		trapTiles.Clear();
		cheeseTiles.Clear();
		holeTiles.Clear();
		miceHoles.Clear();
		patrols.Clear();

		if (spawnRoutine != null)
		{
			spawnRoutine.StopRoutine();
		}
	}

	// Only used for testing and for quickly building a level
	// Level is build without characters and enemies
	// because the generate errors when starting a game and the level
	// gets cleared
	public void BuildLevelEditor()
	{
		if (currentLevel == null)
		{
			CatchingMiceLogVisualizer.use.LogError("The level definition was null.");
			return;
		}

		FindReferences();

		ClearLevel();

		width = currentLevel.width;
		height = currentLevel.height;

		ParseTiles(width, height, currentLevel.layout);

		PlaceFurniture(currentLevel.furniture);

		CreateGrid();

		PlaceWaypoints();

		PlaceObstacles(currentLevel.obstacles);

		PlaceTraps(currentLevel.traps);

		PlaceCheeses(currentLevel.cheeses);

		PlaceCharacters(currentLevel.characters);

		PlaceMiceHoles(currentLevel.holeItems);

		PlaceWallPieces(currentLevel.wallPieces);

		if (OnLevelBuilt != null)
		{
			OnLevelBuilt();
		}
	}

	public void BuildLevel()
	{
		if (currentLevel == null)
		{
			CatchingMiceLogVisualizer.use.LogError("The level definition was null.");
			return;
		}

		FindReferences();

		ClearLevel();

		width = currentLevel.width;
		height = currentLevel.height;

		ParseTiles(width, height, currentLevel.layout);

		PlaceFurniture(currentLevel.furniture);

		CreateGrid();

		PlaceWaypoints();

		PlaceObstacles(currentLevel.obstacles);

		PlaceTraps(currentLevel.traps);

		PlaceCheeses(currentLevel.cheeses);
		
		PlaceCharacters(currentLevel.characters);

		PlacePatrollingCharacters(currentLevel.patrols);

		PlaceMiceHoles(currentLevel.holeItems);

		PlaceWallPieces(currentLevel.wallPieces);

		wavesList = new List<CatchingMiceWaveDefinition>(currentLevel.waves);

		if (OnLevelBuilt != null)
		{
			OnLevelBuilt();
		}
	}

	public void CreateGrid()
	{
		// iterate over entire grid
		for (int y = height - 1; y >= 0; y--)
		{
			for (int x = 0; x < width; x++)
			{
				if (tiles[x, y] == null)
				{
					continue;
				}

				GameObject floorTile = new GameObject(tiles[x, y].gridIndices.ToString());
				floorTile.transform.position = tiles[x, y].location;
				floorTile.transform.parent = levelParent;
				floorTile.transform.localScale = Vector3.one * 0.390625f; // this is the right scale to get a 256x256 sprite to be 1 Unity unit wide
				floorTile.AddComponent<SpriteRenderer>().sprite = floorTileSprite;



//				GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
//				quad.gameObject.name = tiles[x, y].gridIndices.ToString();
//				quad.transform.localScale = Vector3.one * 0.98f * scale;
//				quad.transform.position = tiles[x, y].location;
//				quad.transform.parent = levelParent;
//				quad.GetComponent<MeshCollider>().enabled = false;


				////if (levelTiles[x, y].tileType != CatchingMiceTile.TileType.Ground)
				////{
				////    Material tempMaterial = new Material(quad.renderer.sharedMaterial);
				////    tempMaterial.color = Color.red;
				////    //quad.transform.position = levelTile.location.v3().z(-0.5f);
				////    quad.transform.localScale = Vector3.one * 1.1f * scale;
				////    quad.renderer.sharedMaterial = tempMaterial;
				////}

			}
		}
	}

	public void PlaceWaypoints()
	{
		// Iterate over entire grid
		for (int y = height - 1; y >= 0; --y)
		{
			for (int x = 0; x < width; ++x)
			{
				if (tiles[x, y] == null)
				{
					waypointList.Add(null);
					continue;
				}

				// Make child object with waypoint script
				GameObject wayPoint = new GameObject();
				wayPoint.gameObject.name = "Waypoint " + tiles[x, y].gridIndices;


				wayPoint.transform.parent = navigationParent.transform;
				wayPoint.transform.position = tiles[x, y].location;

				CatchingMiceWaypoint wp = wayPoint.AddComponent<CatchingMiceWaypoint>();

				if ((tiles[x, y].tileType & CatchingMiceTile.TileType.Furniture) == CatchingMiceTile.TileType.Furniture)
				{
					wp.waypointType = CatchingMiceTile.TileType.Furniture;
					CatchingMiceWorldObject catchingMiceObject = tiles[x, y].furniture;
					if (catchingMiceObject != null)
					{
						wayPoint.transform.position = wayPoint.transform.position.yAdd(catchingMiceObject.yOffset);
					}
					else
					{
						CatchingMiceLogVisualizer.use.LogError("Tile " + tiles[x, y].gridIndices + " is of type furniture, but misses script CatchingMiceWorldObject");
					}
				}

				wp.parentTile = tiles[x, y];
				tiles[x, y].waypoint = wp;
				waypointList.Add(wp);
			}
		}

		AssignNeighbours();
	}

	protected void ParseTiles(int _width, int _height, string layout)
	{
		maxDepth = 0;

		// Clear grid
		tiles = new CatchingMiceTile[_width, _height];

		// Iterate over entire grid
		for (int y = height - 1; y >= 0; --y)
		{
			for (int x = 0; x < width; ++x)
			{
				//int index = y * _width + x;			// OLD VERSION: Forces level designer to enter string upside down, which is difficult.
	
				int index = (((height-1)-y)*width) + x; // e.g. tile at row 2, column 4 in a grid of 5*5 has index (((5-1)-2)*5) + 4 = 14

				switch (layout[index])
				{
					case 'x':
						tiles[x, y] = null;
						break;
					case 'o':
						CatchingMiceTile currentTile = new CatchingMiceTile();
						tiles[x, y] = currentTile;

						// Register this tile's grid indices and its true location, which is its index * scale
						currentTile.gridIndices = new Vector2(x, y);

						// We want to use the y position for the z axis because the tiles that are the highest are also the ones
						// that are the farthest away from us
						currentTile.location = currentTile.gridIndices.v3().z(y) * scale;

						if (currentTile.location.z > maxDepth)				
							maxDepth = currentTile.location.z;

						break;
				}
			}
		}
	}

	public void PlaceWallPieces(CatchingMiceWallPieceDefinition[] wallPieceDefinitions)
	{
		foreach (CatchingMiceWallPieceDefinition definition in wallPieceDefinitions)
		{
			// Find the furniture prefab
			GameObject wallPiecePrefab = null;
			foreach (GameObject go in wallPieces)
			{
				if (go.name == definition.prefabName)
				{
					wallPiecePrefab = go;
					break;
				}
			}
			
			if (wallPiecePrefab == null)
			{
				CatchingMiceLogVisualizer.use.LogError("Did not find wall piece ID: " + definition.prefabName);
				continue;
			}

			// in this case, we don't look for a tile with the coordinates from the definition - we also want to be able to place wall Pieces beyond the level bounds
			Vector3 position = definition.position * scale;

			
			// Create the furniture item
			GameObject wallPiece = (GameObject)Instantiate(wallPiecePrefab);
			wallPiece.transform.parent = objectParent;
			wallPiece.transform.name += " " + definition.position;
			wallPiece.transform.localPosition = position.z(maxDepth);
		}
	}

	protected void PlaceFurniture(CatchingMiceFurnitureDefinition[] furnitureDefinitions)
	{
		foreach (CatchingMiceFurnitureDefinition definition in furnitureDefinitions)
		{
			// Find the furniture prefab
			GameObject furniturePrefab = null;
			foreach (CatchingMiceFurniture go in furniturePrefabs)
			{
				if (go.name == definition.prefabName)
				{
					furniturePrefab = go.gameObject;
					break;
				}
			}

			if (furniturePrefab == null)
			{
				CatchingMiceLogVisualizer.use.LogError("Did not find furniture ID: " + definition.prefabName);
				continue;
			}

			// Get the tile on which the furniture is to be placed
			CatchingMiceTile targetTile = GetTile(definition.position, false);
			if (targetTile == null)
			{
				CatchingMiceLogVisualizer.use.LogError("Did not find tile with coordinates:" + definition.position + ". Skipping placing tile item: " + definition.prefabName);
				continue;
			}

			// Create the furniture item
			GameObject tileItem = (GameObject)Instantiate(furniturePrefab);
			tileItem.transform.parent = objectParent;
			tileItem.transform.name += " " + targetTile.gridIndices;
			tileItem.transform.localPosition = targetTile.location;

			// When placing the furniture, check if the it has the object script, so it can set the right tiles to furniture type
			CatchingMiceFurniture furniture = tileItem.GetComponent<CatchingMiceFurniture>();
			if (furniture != null)
			{
				if (furniture.CalculateColliders())
				{
					furniture.parentTile = targetTile;

					if ((targetTile.tileType & CatchingMiceTile.TileType.Furniture) != CatchingMiceTile.TileType.Furniture)
					{
						CatchingMiceLogVisualizer.use.LogWarning("The tile type of the tile has no furniture flag set!");
					}
				}
				else
				{
					CatchingMiceLogVisualizer.use.LogError("The furniture " + furniture.name + " could not be placed on the grid.");
					DestroyGameObject(furniture.gameObject);
				}
			}
			else
			{
				CatchingMiceLogVisualizer.use.LogError("The furniture prefab " + furniturePrefab.name + " does not have a WorldObject component attached to it.");
				DestroyGameObject(tileItem);
			}
		}
	}

	protected void PlaceObstacles(CatchingMiceObstacleDefinition[] obstacleDefinitions)
	{
		foreach(CatchingMiceObstacleDefinition definition in obstacleDefinitions)
		{
			if (string.IsNullOrEmpty(definition.prefabName))
			{
				CatchingMiceLogVisualizer.use.LogError("The obstacle prefab name is null or empty.");
				continue;
			}

			// Find the obstacle prefab
			GameObject obstaclePrefab = null;
			foreach(CatchingMiceObstacle go in obstaclePrefabs)
			{
				if (go.name == definition.prefabName)
				{
					obstaclePrefab = go.gameObject;
					break;
				}
			}

			if (obstaclePrefab == null)
			{
				CatchingMiceLogVisualizer.use.LogError("Did not find obstacle prefab name: " + definition.prefabName);
				continue;
			}

			// Get the tile where the obstacle is supposed to be placed
			CatchingMiceTile targetTile = GetTile(definition.position, false);
			if (targetTile == null)
			{
				CatchingMiceLogVisualizer.use.LogError("Did not find tile with coordinates:" + definition.position + ". Skipping placing obstacle: " + definition.prefabName);
				continue;
			}

			// Create the obstacle item
			GameObject obstacleItem = (GameObject)Instantiate(obstaclePrefab);
			obstacleItem.transform.parent = objectParent;
			obstacleItem.transform.name += " " + targetTile.gridIndices;
			obstacleItem.transform.localPosition = targetTile.location;

			CatchingMiceObstacle obstacle = obstacleItem.GetComponent<CatchingMiceObstacle>();
			if (obstacle != null)
			{
				if (obstacle.CalculateColliders())
				{
					obstacle.parentTile = targetTile;

					if ((targetTile.tileType & CatchingMiceTile.TileType.Obstacle) != CatchingMiceTile.TileType.Obstacle)
					{
						CatchingMiceLogVisualizer.use.LogWarning("The tile type of the tile has no obstacle flag set!");
					}

					obstacle.FromXMLObstacleDefinition(definition.obstacleData);
				}
				else
				{
					CatchingMiceLogVisualizer.use.LogError("The obstacle " + obstacle.name + " could not be placed on the grid.");
					DestroyGameObject(obstacle.gameObject);
				}
			}
			else
			{
				CatchingMiceLogVisualizer.use.LogError("The obstacle prefab " + obstaclePrefab.name + " does not have a Obstacle component attached.");
				DestroyGameObject(obstacleItem);
			}
		}
	}
	
	protected void PlaceCheeses(CatchingMiceCheeseDefinition[] cheeseDefinitions)
	{
		foreach (CatchingMiceCheeseDefinition definition in cheeseDefinitions)
		{
			// Find the cheese prefab
			GameObject cheesePrefab = null;
			foreach (CatchingMiceCheese go in cheesePrefabs)
			{
				if (go.name == definition.prefabName)
				{
					cheesePrefab = go.gameObject;
					break;
				}
			}

			if (cheesePrefab == null)
			{
				CatchingMiceLogVisualizer.use.LogError("Did not find tile item ID: " + definition.prefabName);
				return;
			}

			// Get the tile where the cheese is supposed to be placed
			CatchingMiceTile targetTile = GetTile(definition.position, false);
			if (targetTile == null)
			{
				CatchingMiceLogVisualizer.use.LogError("Did not find tile with coordinates:" + definition.position + ". Skipping placing cheese: " + definition.prefabName);
				return;
			}

			// Create the cheese item
			GameObject tileItem = (GameObject)Instantiate(cheesePrefab);
			tileItem.transform.parent = objectParent;
			tileItem.transform.name += " " + targetTile.gridIndices;
			tileItem.transform.localPosition = targetTile.location;

			CatchingMiceCheese cheese = tileItem.GetComponent<CatchingMiceCheese>();
			if (cheese != null)
			{
				if (cheese.CalculateColliders())
				{
					cheese.parentTile = targetTile;

					if ((targetTile.tileType & CatchingMiceTile.TileType.Cheese) != CatchingMiceTile.TileType.Cheese)
					{
						CatchingMiceLogVisualizer.use.LogWarning("The tile type of the tile has no cheese flag set!");
					}

					cheeseTiles.Add(targetTile);
					cheese.Health = definition.health;
				}
				else
				{
					CatchingMiceLogVisualizer.use.LogError("The cheese " + cheese.name + " could not be placed on the grid.");
					DestroyGameObject(cheese.gameObject);
				}
			}
			else
			{
				CatchingMiceLogVisualizer.use.LogError("The cheese prefab " + cheesePrefab.name + " does not have a Cheese component attached.");
				DestroyGameObject(tileItem);
			}
		}
	}

	public void InstantiateTrap(CatchingMiceTrap trap, CatchingMiceTile tile)
	{
		// Create the trap item
		GameObject tileItem = (GameObject)Instantiate(trap.gameObject);
		tileItem.transform.parent = objectParent;
		tileItem.transform.name += " " + tile.gridIndices;
		tileItem.transform.localPosition = tile.location.zAdd(-0.2f);
		CatchingMiceTrap instantiatedTrap = tileItem.GetComponent<CatchingMiceTrap>();

		if (instantiatedTrap != null)
		{
			if (instantiatedTrap.CalculateColliders())
			{
				instantiatedTrap.parentTile = tile;
				
				if ((tile.tileType & CatchingMiceTile.TileType.Trap) != CatchingMiceTile.TileType.Trap)
				{
					CatchingMiceLogVisualizer.use.LogWarning("The tile type of the tile has no trap flag set!");
				}
				
				trapTiles.Add(tile);
				instantiatedTrap.Ammo = trap.Ammo;
			}
			else
			{
				CatchingMiceLogVisualizer.use.LogError("The trap " + trap.name + " could not be placed on the grid.");
				DestroyGameObject(tileItem);
			}
		}
		else
		{
			CatchingMiceLogVisualizer.use.LogError("The trap to instantiate did not have a Trap component attached.");
			DestroyGameObject(tileItem);
		}
	}

	protected void PlaceTraps(CatchingMiceTrapDefinition[] trapdefinitions)
	{
		foreach (CatchingMiceTrapDefinition definition in trapdefinitions)
		{
			if (string.IsNullOrEmpty(definition.prefabName))
			{
				CatchingMiceLogVisualizer.use.LogError("The trap prefab name is null or empty!");
				continue;
			}

			// Find the trap prefab
			CatchingMiceTrap neededTrap = null;
			foreach (CatchingMiceTrap trap in trapPrefabs)
			{
				if (trap.name == definition.prefabName)
				{
					neededTrap = trap;
					break;
				}
			}

			if (neededTrap == null)
			{
				CatchingMiceLogVisualizer.use.LogError("Did not find trap prefab name: " + definition.prefabName);
				continue;
			}

			// Get the tile where the trap supposed to be placed
			CatchingMiceTile targetTile = GetTile(definition.position, false);
			if (targetTile == null)
			{
				CatchingMiceLogVisualizer.use.LogError("Did not find tile with coordinates:" + definition.position + ". Skipping placing trap: " + definition.prefabName);
				continue;
			}

			InstantiateTrap(neededTrap, targetTile);





//			// Create the trap item
//			GameObject tileItem = (GameObject)Instantiate(neededTrap);
//			tileItem.transform.parent = objectParent;
//			tileItem.transform.name += " " + targetTile.gridIndices;
//			tileItem.transform.localPosition = targetTile.location;
//
//			CatchingMiceTrap trap = tileItem.GetComponent<CatchingMiceTrap>();
//			if (trap != null)
//			{
//				if (trap.CalculateColliders())
//				{
//					trap.parentTile = targetTile;
//
//					if ((targetTile.tileType & CatchingMiceTile.TileType.Trap) != CatchingMiceTile.TileType.Trap)
//					{
//						CatchingMiceLogVisualizer.use.LogWarning("The tile type of the tile has no trap flag set!");
//					}
//
//					trapTiles.Add(targetTile);
//					trap.Stacks = definition.stacks;
//				}
//				else
//				{
//					CatchingMiceLogVisualizer.use.LogError("The trap " + trap.name + " could not be placed on the grid.");
//					DestroyGameObject(trap.gameObject);
//				}
//			}
//			else
//			{
//				CatchingMiceLogVisualizer.use.LogError("The trap prefab " + neededTrap.name + " does not have a Trap component attached.");
//				DestroyGameObject(tileItem);
//			}


		}
	}
	
	protected void PlaceMiceHoles(CatchingMiceHoleDefinition[] holeDefinitions)
	{
		if (holeDefinitions == null || holeDefinitions.Length < 1)
		{
			CatchingMiceLogVisualizer.use.LogError("This level has no mice holes!");
			return;
		}

		foreach (CatchingMiceHoleDefinition definition in holeDefinitions)
		{
			if (string.IsNullOrEmpty(definition.prefabName))
			{
				CatchingMiceLogVisualizer.use.LogError("The mice hole prefab name is null or empty!");
				continue;
			}

			// Find the hole prefab
			GameObject holePrefab = null;
			foreach (CatchingMiceHole go in holePrefabs)
			{
				if (go.name == definition.prefabName)
				{
					holePrefab = go.gameObject;
					break;
				}
			}

			if (holePrefab == null)
			{
				CatchingMiceLogVisualizer.use.LogError("Did not find mice hole prefab name: " + definition.prefabName);
				return;
			}

			// Get the tile where the mice hole supposed to be placed
			CatchingMiceTile targetTile = GetTile(definition.position, false);
			if (targetTile == null)
			{
				CatchingMiceLogVisualizer.use.LogError("Did not find tile with coordinates:" + definition.position + ". Skipping placing mice hole: " + definition.prefabName);
				continue;
			}

			// Create the trap item
			GameObject tileItem = (GameObject)Instantiate(holePrefab);
			tileItem.transform.parent = objectParent;
			tileItem.transform.name += " " + targetTile.gridIndices;
			tileItem.transform.localPosition = targetTile.location;

			CatchingMiceHole hole = tileItem.GetComponent<CatchingMiceHole>();
			if (hole != null)
			{
				if (hole.CalculateColliders())
				{
					hole.parentTile = targetTile;

					if ((targetTile.tileType & CatchingMiceTile.TileType.Hole) != CatchingMiceTile.TileType.Hole)
					{
						CatchingMiceLogVisualizer.use.LogWarning("The tile type of the tile has no hole flag set!");
					}

					miceHoles.Add(hole);
					holeTiles.Add(targetTile);

					hole.id = definition.holeId;
					hole.SetHoleSpawnPoint(definition.startDirection, targetTile);
				}
				else
				{
					CatchingMiceLogVisualizer.use.LogError("The mice hole " + hole.name + " could not be placed on the grid.");
					DestroyGameObject(hole.gameObject);
				}
			}
			else
			{
				CatchingMiceLogVisualizer.use.LogError("The hole prefab " + holePrefab.name + " does not have a MiceHole component attached.");
				DestroyGameObject(tileItem);
			}
		}
	}

	protected void PlaceCharacters(CatchingMiceCharacterDefinition[] characterDefinitions)
	{
		if (characterDefinitions == null || characterDefinitions.Length == 0)
		{
			CatchingMiceLogVisualizer.use.LogError("This level has no characters!");
			return;
		}

		foreach (CatchingMiceCharacterDefinition characterDefinition in characterDefinitions)
		{
			if (string.IsNullOrEmpty(characterDefinition.prefabName))
			{
				CatchingMiceLogVisualizer.use.LogError("Character ID is null or empty!");
				continue;
			}

			CatchingMiceCharacterPlayer characterPrefabFound = null;
			foreach (CatchingMiceCharacterPlayer characterPrefab in characterPrefabs)
			{
				if (characterDefinition.prefabName == characterPrefab.gameObject.name)
				{
					characterPrefabFound = characterPrefab;
					break;
				}
			}

			if (characterPrefabFound == null)
			{
				CatchingMiceLogVisualizer.use.LogError("Character prefab could not be found: " + characterDefinition.prefabName);
				continue;
			}

		
			CatchingMiceTile targetTile = GetTile(characterDefinition.position, false);
			if (targetTile == null)
			{
				CatchingMiceLogVisualizer.use.LogError("Did not find tile with coordinates:" + characterDefinition.position + ". Skipping placing character: " + characterDefinition.prefabName);
				continue;
			}

			CatchingMiceCharacterPlayer characterSpawned = (CatchingMiceCharacterPlayer)Instantiate(characterPrefabFound);
			
			characterSpawned.transform.parent = characterParent;
			characterSpawned.transform.localPosition = targetTile.waypoint.transform.position.zAdd(-characterSpawned.zOffset);
			characterSpawned.currentTile = targetTile;

			// set speed
			if (characterDefinition.speed < 0)
			{
				CatchingMiceLogVisualizer.use.LogError("Speed is negative for character " + characterDefinition.prefabName + ". Setting speed to 1.");
				characterSpawned.tileTraversalTime = 1;
			}
			else
			{
				characterSpawned.tileTraversalTime = characterDefinition.speed;
			}

			// set character type - TODO: Add functionality to make one of the cats fixed. Now they will all have the same color.

			characterSpawned.GetComponent<CatchingMiceCharacterAnimation>().characterNameAnimation = "Cat0" + LugusConfig.use.User.GetInt("CatIndex", 1).ToString();


			playerList.Add(characterSpawned);
		}
	}

	protected void PlacePatrollingCharacters(CatchingMicePatrolDefinition[] patrolDefinitions)
	{
		if (patrolDefinitions == null || patrolDefinitions.Length == 0)
		{
			CatchingMiceLogVisualizer.use.Log("This level has no patrols!");
			return;
		}

		foreach (CatchingMicePatrolDefinition patrolDefinition in patrolDefinitions)
		{
			if (string.IsNullOrEmpty(patrolDefinition.prefabName))
			{
				CatchingMiceLogVisualizer.use.LogError("Patrol ID is null or empty!");
				continue;
			}

			CatchingMiceCharacterPatrol patrolPrefabFound = null;
			foreach (CatchingMiceCharacterPatrol patrolPrefab in patrolPrefabs)
			{
				if (patrolDefinition.prefabName == patrolPrefab.gameObject.name)
				{
					patrolPrefabFound = patrolPrefab;
					break;
				}
			}

			if (patrolPrefabFound == null)
			{
				CatchingMiceLogVisualizer.use.LogError("Patrol prefab could not be found: " + patrolDefinition.prefabName);
				continue;
			}

			if (patrolDefinition.positions.Length == 0)
			{
				CatchingMiceLogVisualizer.use.LogError("The patrol has no waypoints.");
				continue;
			}

			// Get the initial tile where the patrol is supposed to be placed
			CatchingMiceTile targetTile = GetTile(patrolDefinition.positions[0], false);
			if (targetTile == null)
			{
				CatchingMiceLogVisualizer.use.LogError("Did not find tile with coordinates:" + patrolDefinition.positions[0] + ". Skipping placing patrol: " + patrolDefinition.prefabName);
				continue;
			}

			CatchingMiceCharacterPatrol patrolSpawned = (CatchingMiceCharacterPatrol)Instantiate(patrolPrefabFound);

			patrolSpawned.transform.parent = patrolParent;
			patrolSpawned.transform.localPosition = targetTile.waypoint.transform.position.zAdd(-patrolSpawned.zOffset);
			patrolSpawned.currentTile = targetTile;

			// Set speed
			if (patrolDefinition.speed < 0)
			{
				CatchingMiceLogVisualizer.use.LogError("Speed is negative for patrol " + patrolDefinition.prefabName + ". Setting speed to 1.");
				patrolSpawned.tileTraversalTime = 1;
			}
			else
			{
				patrolSpawned.tileTraversalTime = patrolDefinition.speed;
			}

			// Set the patrol route
			foreach (Vector2 patrolWaypoint in patrolDefinition.positions)
			{
				patrolSpawned.patrolRoute.Add(GetWaypointFromTile(patrolWaypoint));
			}

			patrols.Add(patrolSpawned);
		}
	}

	public void InstantiateWave(int waveIndex)
	{
		if (waveIndex < 0)
		{
			CatchingMiceLogVisualizer.use.LogError("Wave index can't be negative");
			return;
		}

		if (waveIndex >= wavesList.Count)
		{
			CatchingMiceLogVisualizer.use.LogError("Wave exceeds the waves list. Using last wave count.");
			waveIndex = wavesList.Count - 1;
		}

		// Now it's not pooling so delete every child in enemy parent
		for (int i = enemyParent.childCount - 1; i >= 0; i--)
		{
			Destroy(enemyParent.GetChild(i).gameObject);
		}
		enemyParentList.Clear();

		CatchingMiceWaveDefinition wave = wavesList[waveIndex];
		int amountToKill = 0;

		List<string> spawnHoles = new List<string>();

		for (int i = 0; i < wave.enemies.Length; i++)
		{
			// Get the prefab from string name
			if (string.IsNullOrEmpty(wave.enemies[i].prefabName))
			{
				CatchingMiceLogVisualizer.use.LogError("Character ID is null or empty!");
				continue;
			}

			// Get the hole name for this enemy and check whether isn't
			// already in the list
			string holeId = wave.enemies[i].holeId;
			if ((!string.IsNullOrEmpty(holeId)) && (!spawnHoles.Contains(holeId)))
			{
				spawnHoles.Add(holeId);
			}

			CatchingMiceCharacterMouse enemyPrefab = null;
			foreach (CatchingMiceCharacterMouse go in enemyPrefabs)
			{
				if (go.name == wave.enemies[i].prefabName)
				{
					enemyPrefab = go;
					break;
				}
			}

			if (enemyPrefab == null)
			{
				CatchingMiceLogVisualizer.use.LogError("Did not find enemy prefab with ID: " + wave.enemies[i].prefabName);
				return;
			}

			//everything has been found, start new coroutine with spawnwave
			//LugusCoroutines.use.StartRoutine(SpawnSubWave(enemyPrefab, spawnTile, enemyDefinition.amount, enemyDefinition.spawnTimeInterval, enemyDefinition.spawnDelay));

			InstantiateSubWave(i, enemyPrefab, wave.enemies[i].amount);

			amountToKill += wave.enemies[i].amount;
		}
		//foreach (CatchingMiceEnemyDefinition enemyDefinition in wave.enemies)
		//{
		//    //get the prefab from string name
		//    if (string.IsNullOrEmpty(enemyDefinition.prefabName))
		//    {
		//        Debug.LogError("Character ID is null or empty!");
		//        continue;
		//    }
		//    GameObject enemyPrefab = null;
		//    foreach (GameObject go in enemyPrefabs)
		//    {
		//        if (go.name == enemyDefinition.prefabName)
		//        {
		//            enemyPrefab = go;
		//            break;
		//        }
		//    }
		//    if (enemyPrefab == null)
		//    {
		//        Debug.LogError("Did not find hole item ID: " + enemyDefinition.prefabName);
		//        return;
		//    }

		//    //get spawnlocation from hole id
		//    CatchingMiceHole spawnTile = null;
		//    foreach (CatchingMiceHole hole in holeTiles)
		//    {
		//        if(hole.id == enemyDefinition.holeId)
		//        {
		//            spawnTile = hole;
		//        }
		//    }
		//    if (spawnTile == null)
		//    {
		//        Debug.LogError("hole id has not been found for " + enemyDefinition.prefabName);
		//        return;
		//    }
		//    //everything has been found, start new coroutine with spawnwave
		//    LugusCoroutines.use.StartRoutine(SpawnSubWave(enemyPrefab,spawnTile, enemyDefinition.amount, enemyDefinition.spawnTimeInterval, enemyDefinition.spawnDelay));

		//    amountToKill += enemyDefinition.amount;
		//}

		// Find each mice hole, and visualize the path of the mice
		foreach(string holeId in spawnHoles)
		{
			CatchingMiceHole spawnHole = null;
			foreach (CatchingMiceHole hole in miceHoles)
			{
				if (holeId == hole.id)
				{
					spawnHole = hole;
					break;
				}
			}

			if (spawnHole != null)
			{
				spawnHole.VisualizePath(Waypoints, CatchingMiceGameManager.use.preWaveTime);
			}
		}

		CatchingMiceGameManager.use.EnemiesAlive += amountToKill;
	}

	protected void InstantiateSubWave(int index, CatchingMiceCharacterMouse spawnGO, int amount)
	{
		//if the object already exist use it, else make new one
		GameObject waveParent = null;

		foreach (GameObject enemyParent in enemyParentList)
		{
			if (enemyParent.name == "subwave" + index)
			{
				waveParent = enemyParent;
				break;
			}
		}

		if (waveParent == null)
		{
			waveParent = new GameObject("subwave" + index);
		}

		waveParent.transform.position = new Vector3(-1000, -100, 0);
		waveParent.transform.parent = spawnParent;

		for (int i = 0; i < amount; i++)
		{
			CatchingMiceCharacterMouse enemy = GetNextEnemyFromPool(spawnGO);
			enemy.transform.parent = waveParent.transform;
			enemy.transform.localPosition = Vector3.zero;
			//enemy.SetActive(false);
		}

		enemyParentList.Add(waveParent);
	}

	public void SpawnInstantiatedWave(int waveIndex)
	{
		CatchingMiceWaveDefinition wave = wavesList[waveIndex];
		for (int i = 0; i < wave.enemies.Length; i++)
		{
			spawnRoutine.StartRoutine(SpawnInstantiatedSubWave(i, wave.enemies[i]));
		}
	}

	protected IEnumerator SpawnInstantiatedSubWave(int index, CatchingMiceEnemyDefinition enemy)
	{
		// 1. Find the spawn point for this enemy
		// 2. Find the subwave parent responsible for holding the game objects in the scene
		// 3. The enemies are one by one retrieved from the subwave parent, and place at the spawntile

		if (enemy.spawnDelay > 0)
		{
			yield return new WaitForSeconds(enemy.spawnDelay);
		}

		float spawnIntervalLower = enemy.spawnTimeInterval * 0.9f;
		float spawnIntervalUpper = enemy.spawnTimeInterval * 1.1f;

		// Get spawnlocation from hole id
		CatchingMiceHole spawnTile = null;
		foreach (CatchingMiceHole hole in miceHoles)
		{
			if (hole.id == enemy.holeId)
			{
				spawnTile = hole;
				break;
			}
		}

		if (spawnTile == null)
		{
			CatchingMiceLogVisualizer.use.LogError("Could not find the mice hole with name " + enemy.holeId + " for enemy " + enemy.prefabName);
			yield break;
		}

		// Find the parent object container for this subwave
		GameObject parentGO = null;
		foreach (GameObject parent in enemyParentList)
		{
			if (parent == null)
			{
				CatchingMiceLogVisualizer.use.LogError("Parent objects is null");
				break;
			}
			if (parent.name == "subwave" + index)
			{
				parentGO = parent;
				break;
			}
		}

		if (parentGO == null)
		{
			CatchingMiceLogVisualizer.use.LogError("SubwaveName has not been found");
			yield break;
		}

		// 
		for (int i = 0; i < enemy.amount; i++)
		{
			Transform child = parentGO.transform.GetChild(0);
			
			if (child == null)
			{
				CatchingMiceLogVisualizer.use.Log("No Child has been found");
				break;
			}
			
			CatchingMiceCharacterMouse mouseScript = child.GetComponent<CatchingMiceCharacterMouse>();
			
			if (mouseScript != null)
			{
				//child.gameObject.SetActive(true);
				child.transform.parent = enemyParent;
				child.transform.position = spawnTile.spawnPoint.zAdd(-mouseScript.zOffset);
				mouseScript.currentTile = spawnTile.parentTile;
				mouseScript.GetTarget();

				// Add the enemy to the list
				enemies.Add(mouseScript);
			}
			
			yield return new WaitForSeconds(LugusRandom.use.Uniform.Next(spawnIntervalLower, spawnIntervalUpper));
		}
		
		yield break;
	}

	public CatchingMiceCharacterMouse GetNextEnemyFromPool(CatchingMiceCharacterMouse gameObjectToGet)
	{
		//TODO pooling: search for inactive objects before instantiating new gameobject
		return (CatchingMiceCharacterMouse)Instantiate(gameObjectToGet);
	}

	public void AssignNeighbours()
	{
		for (int i = 0; i < waypointList.Count - 1; i++)
		{
			if (waypointList[i] == null)
			{
				continue;
			}

			// Last row does not need to (and can't) add their neighbor below
			if ((i < waypointList.Count - width) && (waypointList[i + width] != null))
			{
				waypointList[i].neighbours.Add(waypointList[i + width]);
				waypointList[i + width].neighbours.Add(waypointList[i]);
			}

			// Last column can't add his neighbor next to him
			// all waypoints are in one list, so every width length there will be a new row, is that waypoint is in the last column
			if (((i + 1) % width != 0) && (waypointList[i + 1] != null))
			{
				waypointList[i].neighbours.Add(waypointList[i + 1]);
				waypointList[i + 1].neighbours.Add(waypointList[i]);
			}

		}
	}

	public void EnemyDied(CatchingMiceCharacterMouse enemy)
	{
		// Removes the enemies from the list of active enemies
		enemies.Remove(enemy);
	}

	// Lookup methods--------------------------------------------------------------------

	// get tile by grid indices (contained in vector2)
	public CatchingMiceTile GetTile(Vector2 coords)
	{
		return GetTile(coords, false);
	}

	// Get tile by grid indices (contained in vector2)
	public CatchingMiceTile GetTile(Vector2 coords, bool clamp)
	{
		int x = Mathf.RoundToInt(coords.x);
		int y = Mathf.RoundToInt(coords.y);

		return GetTile(x, y, clamp);
	}

	// Get tile by grid indices
	public CatchingMiceTile GetTile(int x, int y)
	{
		return GetTile(x, y, false);
	}

	// Get tile by local position under level root
	public CatchingMiceTile GetTileByLocation(float x, float y)
	{
		int xIndex = Mathf.RoundToInt(x / scale);
		int yIndex = Mathf.RoundToInt(y / scale);

		// Instead of returning null, return closest tile?
		if (xIndex >= width)
		{
			xIndex = width - 1;
			//return null;
		}
		else if (xIndex < 0)
		{
			xIndex = 0;
			//return null;
		}
		if (yIndex >= height)
		{
			yIndex = height - 1;
			//return null;
		}
		else if (yIndex < 0)
		{
			yIndex = 0;
			//return null;
		}
		return GetTile(xIndex, yIndex, true);
	}

	public CatchingMiceTile GetTileFromMousePosition(bool clampToEdges = false)
	{
		return GetTileFromMousePosition(LugusCamera.game, clampToEdges);
	}

	public CatchingMiceTile GetTileFromMousePosition(Camera camera, bool clampToEdges = false)
	{
		Vector3 mouseWorldPosition = camera.ScreenToWorldPoint(LugusInput.use.lastPoint).z(0);
		
		int xIndex = Mathf.RoundToInt(mouseWorldPosition.x / scale);
		int yIndex = Mathf.RoundToInt(mouseWorldPosition.y / scale);
		
		return GetTile(xIndex, yIndex, clampToEdges);
	}

	public CatchingMiceTile GetTile(int x, int y, bool clamp)
	{
		if (tiles == null)
		{
			return null;
		}

		if (x >= width || x < 0)
		{
			if (clamp)
				x = Mathf.Clamp(x, 0, width - 1);
			else
				return null;
		}
		
		if (y >= height || y < 0)
		{
			if (clamp)
				y = Mathf.Clamp(y, 0, height - 1);
			else
				return null;
		}

		return tiles[x, y];
	}

	public CatchingMiceTile[] GetTilesInDirection(CatchingMiceTile startTile, int amount, Vector2 direction)
	{
		return GetTilesInDirection(startTile, amount, direction, false);
	}

	public CatchingMiceTile[] GetTilesInDirection(CatchingMiceTile startTile, int amount, Vector2 direction, bool clamp, bool reverseOrder = false)
	{
		List<CatchingMiceTile> tileList = new List<CatchingMiceTile>();

		int xStart = (int)startTile.gridIndices.x;
		int yStart = (int)startTile.gridIndices.y;

		if (direction.x > 0)
		{
			for (int x = xStart; x < xStart + amount; x++) 
			{
				tileList.Add(GetTile(x, yStart, clamp));
			}
		}
		else if (direction.x < 0)
		{
			for (int x = xStart; x > xStart - amount; x--)
			{
				tileList.Add(GetTile(x, yStart, clamp));
			}
		}
		else if (direction.y > 0)
		{
			for (int y = yStart; y < yStart + amount; y++) 
			{
				tileList.Add(GetTile(xStart, y, clamp));
			}
		}
		else if (direction.y < 0)
		{
			for (int y = yStart; y > yStart - amount; y--)
			{
				tileList.Add(GetTile(xStart, y, clamp));
			}
		}

		if (reverseOrder)
		{
			tileList.Reverse();
		}

		return tileList.ToArray();
	}

	public CatchingMiceWaypoint GetWaypointFromTile(Vector2 gridIndices)
	{
		return tiles[Mathf.RoundToInt(gridIndices.x), Mathf.RoundToInt(gridIndices.y)].waypoint;
	}

	public CatchingMiceWaypoint GetWaypointFromTile(int x, int y)
	{
		return tiles[x, y].waypoint;
	}

	public CatchingMiceTile[] GetTileAround(CatchingMiceTile tile)
	{
//		List<CatchingMiceTile> tiles = new List<CatchingMiceTile>();
//
//		for (int x = -1; x <= 1; x++)
//		{
//			for (int y = -1; y <= 1; y++)
//			{
//				//don't add your own tile
//				if (x == 0 && y == 0)
//					continue;
//				CatchingMiceTile inspectedTile = GetTile(tile.gridIndices.v3().xAdd(x).yAdd(y));
//				if (inspectedTile != null)
//					tiles.Add(inspectedTile);
//			}
//		}
//
//		return tiles.ToArray();

		return GetTilesAround(tile, 1);
	}

	public CatchingMiceTile[] GetTilesAround(CatchingMiceTile tile, int radius)
	{
		List<CatchingMiceTile> tiles = new List<CatchingMiceTile>();
	
		if (radius <= 0)
		{
			Debug.LogError("Cannot return tiles with a radius smaller than or equal to 0.");
			return null;
		}

		for (int x = -radius; x <= radius; x++)
		{
			for (int y = -radius; y <= radius; y++)
			{
				//don't add your own tile
				if (x == 0 && y == 0)
					continue;

				CatchingMiceTile inspectedTile = GetTile(tile.gridIndices.v3().xAdd(x).yAdd(y), false);

				if (inspectedTile != null)
					tiles.Add(inspectedTile);
			}
		}
		
		return tiles.ToArray();
	}



	public void RemoveTrapFromTile(CatchingMiceTile tile)
	{
		if (tile == null)
		{
			CatchingMiceLogVisualizer.use.LogError("Cannot remove trap from null tile.");
			return;
		}

		if ((tile.tileType & CatchingMiceTile.TileType.Trap) != CatchingMiceTile.TileType.Trap)
		{
			CatchingMiceLogVisualizer.use.LogError("Cannot remove trap from tile " + tile.ToString() + " that does not contain a trap.");
			return;
		}

		// Remove the references of the trap
		trapTiles.Remove(tile);
		
		if((tile.trap != null) && (OnTrapRemoved != null))
		{
			OnTrapRemoved(tile);
		}

		tile.tileType = tile.tileType ^ CatchingMiceTile.TileType.Trap;
		tile.trap = null;
	}

	public void RemoveCheeseFromTile(CatchingMiceTile tile)
	{
		if (tile == null)
		{
			CatchingMiceLogVisualizer.use.LogError("Cannot remove cheese from a null tile.");
			return;
		}

		if ((tile.tileType & CatchingMiceTile.TileType.Cheese) != CatchingMiceTile.TileType.Cheese)
		{
			CatchingMiceLogVisualizer.use.LogError("Cannot remove cheese from tile " + tile.ToString() + " that does not contain cheese.");
			return;
		}

		// Remove the references of the trap

		if (cheeseTiles.Contains(tile))
		{
			cheeseTiles.Remove(tile);

			if (!fakeCheeseTiles.Contains(tile))
			{
				tile.tileType = tile.tileType ^ CatchingMiceTile.TileType.Cheese;
				tile.cheese = null;
			}
		}

		if (fakeCheeseTiles.Contains(tile))
		{
			fakeCheeseTiles.Remove(tile);

			if (!cheeseTiles.Contains(tile))
			{
				tile.tileType = tile.tileType ^ CatchingMiceTile.TileType.Cheese;
				tile.cheese = null;
			}
		}


		if ((tile.cheese != null) && (OnCheeseRemoved != null))
		{
			OnCheeseRemoved(tile);
		}
	}

	protected void DestroyGameObject(GameObject obj)
	{
#if UNITY_EDITOR
		DestroyImmediate(obj);
#else
		obj.SetActive(false);
		Destroy(obj);
#endif

//#if UNITY_EDITOR
//		if (Application.isPlaying)
//		{
//			obj.SetActive(false);
//			Destroy(obj);
//		}
//		else
//		{
//			DestroyImmediate(obj);
//		}
//#endif
	}
}
