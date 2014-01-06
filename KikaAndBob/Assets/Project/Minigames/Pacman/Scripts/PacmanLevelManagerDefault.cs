using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanLevelManager : LugusSingletonExisting<PacmanLevelManagerDefault> {
}

public class PacmanLevelManagerDefault : MonoBehaviour {

	/*
	 * x: collision 
	 * o: open
	 * p: pickup
	 * d: door; switches between being open and closed
	 * t: teleport; will teleport player and enemies to the other side of the field
	 * e: exit; this tile is inaccessible until all pickups have been collected. Afterwards, the player wins the game by entering this tile.
	 * u: upgrade; if picked up, makes enemies afraid and run away for some time
	 */

	public PacmanLevelDefinition[] levels = null;
	public PacmanCharacter[] characterPrefabs = null;

	private string testLevel ="" +
		"xxxxxxxxxxxxx"+
		"xpopopopopopx"+
		"xoxxxxoxxxxox"+
		"xopopxpxpopox"+
		"xoxxoxoxoxxox"+
		"xpxopopopoxpx"+
		"xoooxxoxxooox"+
		"xxxoxopoxoxxx"+
		"xopoxpxpxopox"+
		"xoxoooxoooxox"+
		"xoxxoxxxoxxox"+
		"xpoopopopoopx"+
		"xxxxxxxxxxxxx";

//	private string testLevel ="" +
//		"ooxxxxxxxxxxxxxxxxxxxxxoo"+
//		"ooxpopxpdpxpxpxpdpopxpxoo"+
//		"ooxoxoooxoxoxoxoxoxoxoxoo"+
//		"ooxpxpxpxpopxpopxpxpopxoo"+
//		"xxxoxoxoxoxoooxoxoxxxoxxx"+
//		"optpxpxpxpxxaxxpxpopoptpo"+
//		"xxxoxoxodoxaaexodoxxxoxxx"+
//		"ooxpopxpxpxaaaxpxpxxxpxoo"+
//		"ooxxxoooxoxxxxxoxoxxxoxoo"+
//		"ooxxopxpdpxpxpxpdpxxxpxoo"+
//		"ooxxoxxoxoxoxoxoxoxxxoxoo"+
//		"ooxxopopxpopopopxpxxxpxoo"+
//		"ooxxxxxxxxxxxxxxxoooooxoo";
	
	public int width = 13;
	public int height = 13;
	public float scale = 64;

	// MOVE TO SCRIPTABLE OBJECT
	public Sprite[] blockSprites = null;
	public Sprite[] blockShadows = null;
	public Sprite[] blockDecorations = null;
	public Sprite pickupSprite = null;
	public float wallTileScaleFactor = 0.6f;
	
	public enum LevelQuadrant
	{
		None,
		NE,
		SE,
		SW,
		NW
	}
	
	protected Transform levelRoot = null;
	protected Transform pickupParent = null;
	protected Transform levelParent = null;
	protected Transform prefabParent = null;
	protected Transform characterParent = null;
	protected ILugusCoroutineHandle spawnRoutine = null;
	protected List<PacmanCharacter> spawnedCharacters = new List<PacmanCharacter>();
	
	[HideInInspector]
	public Transform effectsParent = null;
	
	public PacmanTile[,] levelTiles;
	public PacmanTile[] teleportTiles = new PacmanTile[2];

	public delegate void OnLevelBuilt();
	public OnLevelBuilt onLevelBuilt;
	
	protected int itemsToBePickedUp = 0;
	protected int itemsPickedUp = 0;
	protected int doorIteration = 0;

	protected GameObject doorPrefab = null;

	void Awake () 
	{
		FindReferences();
	}

	void FindReferences()
	{
		// only do this once
		if (levelRoot != null)
			return;

		if (levelRoot == null)
			levelRoot = GameObject.Find ("LevelRoot").transform;

		if (levelRoot == null)
			Debug.Log("LevelManager: Could not find level root.");
		
		pickupParent = levelRoot.FindChild("PickupParent");
		levelParent = levelRoot.FindChild("LevelParent");
		effectsParent = levelRoot.FindChild("EffectsParent");
		characterParent = levelRoot.FindChild("CharacterParent");
		prefabParent = levelRoot.FindChild("Prefabs");
		doorPrefab = GetPrefab("Door");
	}

	public void ClearLevel()
	{
		for (int i = levelParent.childCount - 1; i >= 0; i--) 
		{
			Destroy(levelParent.GetChild(i).gameObject);
		}

		for (int i = pickupParent.childCount - 1; i >= 0; i--) 
		{
			Destroy(pickupParent.GetChild(i).gameObject);
		}

		for (int i = characterParent.childCount - 1; i >= 0; i--) 
		{
			Destroy(characterParent.GetChild(i).gameObject);
		}
	}

	public void ClearLevelEditor()
	{
		for (int i = levelParent.childCount - 1; i >= 0; i--) 
		{
			DestroyImmediate(levelParent.GetChild(i).gameObject);
		}
		
		for (int i = pickupParent.childCount - 1; i >= 0; i--) 
		{
			DestroyImmediate(pickupParent.GetChild(i).gameObject);
		}

		for (int i = characterParent.childCount - 1; i >= 0; i--) 
		{
			DestroyImmediate(characterParent.GetChild(i).gameObject);
		}
	}

	public void BuildLevel(int levelIndex)
	{
		if (levelIndex < 0 || levelIndex >= levels.Length)
		{
			Debug.LogError("Level index was out of bounds!");
			return;
		}

		PacmanLevelDefinition level = levels[levelIndex];

		FindReferences();

		#if UNITY_EDITOR
		ClearLevelEditor();
		#else
		ClearLevel();
		#endif

		ParseLevelTiles(level.level);

		PlaceLevelTiles();

		PlaceCharacters(level.characters);

		if (onLevelBuilt != null)
			onLevelBuilt();
		
		//StartCoroutine(DoorUpdateRoutine());
	}
		
	public void ParseLevelTiles(string levelData)
	{
		if (string.IsNullOrEmpty(levelData))
		{
			Debug.LogError("LevelManager: Level string is null or empty!");
			return;
		}

		// clear grid
		levelTiles = new PacmanTile[width, height];
		itemsPickedUp = 0;
		itemsToBePickedUp = 0;

		// iterate over entire grid
		for (int y = height-1; y >= 0; y--)
		{
			for (int x = 0; x < width; x++)
			{
				PacmanTile currentTile = new PacmanTile();
				levelTiles[x,y] = currentTile;

				// register this tile's grid indices and its true location, which is its index * scale
				currentTile.gridIndices = new Vector2(x, y);
				currentTile.location = currentTile.gridIndices*scale;

				// get index of this tile's char in the level string
				// e.g. tile at row 2, column 4 in a grid of 5*5 has index (((5-1)-2)*5) + 4 = 14
				int currentStringIndex = (((height-1)-y)*width) + x;

				char tileChar = testLevel[currentStringIndex];

				// assign tiletype depending on character
				// could be done with a lookup dictionary, but per type some custom actions are required anyway
				if (tileChar == 'x')
					currentTile.tileType = PacmanTile.TileType.Collide;
				else if (tileChar == 'o')
					currentTile.tileType = PacmanTile.TileType.Open;
				else if (tileChar == 'p')
				{
					currentTile.tileType = PacmanTile.TileType.Pickup;
					itemsToBePickedUp++;
				}
				else if (tileChar == 'u')
				{
//					currentTile.tileType = GameTile.TileType.Upgrade;
//			
//					GameObject powerUpPickup = (GameObject)Instantiate(pickUpPrefab);
//					powerUpPickup.transform.parent = levelRoot;
//					powerUpPickup.transform.localPosition = new Vector3(currentTile.location.x, currentTile.location.y, 1);
//					currentTile.sprite = powerUpPickup;
//					powerUpPickup.transform.localScale = powerUpPickup.transform.localScale * 2;
//					powerUpPickup.transform.parent = pickupParent;
				}
				else if (testLevel[currentStringIndex] == 'd') // Door tiles start off as open tiletype, but are kept in a list of doors which are regularly switched between Open and Collide
				{
					currentTile.tileType = PacmanTile.TileType.Door;
			
					GameObject door = (GameObject)Instantiate(doorPrefab);
					door.transform.parent = levelRoot;
					door.transform.localPosition = new Vector3(currentTile.location.x, currentTile.location.y, 1);
					door.transform.Translate(new Vector3(0, 2, 0));		// TO DO: Quick and dirty. The doorjambs on the background image are not entirely identical in size.
																		// Slightly scaling and then moving the doors by a few pixels results in a good average.
					currentTile.sprite = door;
				}
				else if (testLevel[currentStringIndex] == 't')	// teleport tiles on either side of the field
				{
					currentTile.tileType = PacmanTile.TileType.Teleport;
					for (int i = 0; i < teleportTiles.Length; i++)
					{
						if (teleportTiles[i] == null)
						{
							teleportTiles[i] = currentTile;
						}
					}
				}
				else if (testLevel[currentStringIndex] == 'e')	// entering this tile finishes the game
					currentTile.tileType = PacmanTile.TileType.LevelEnd;
				else if (testLevel[currentStringIndex] == 'a')	
					currentTile.tileType = PacmanTile.TileType.Locked;
				else
					currentTile.tileType = PacmanTile.TileType.Open;
			}
		}
		
		//PacmanGUIManager.use.UpdatePickupCounter(itemsToBePickedUp);
		
		// count tile exits for each tile. Enemies use this value to figure out whether they're in a cul-de-sac and are allowed to turn around.
		foreach(PacmanTile tile in levelTiles)
		{
			tile.exitCount = GetNumberOfExits(tile);
		}
	}

	public void PlaceCharacters(PacmanCharacterDefinition[] characters)
	{
		if (characters == null || characters.Length < 1)
		{
			Debug.LogError("This level has no characters!");
			return;
		}

		foreach(PacmanCharacterDefinition characterDefinition in characters)
		{
			spawnedCharacters.Clear();

			if (string.IsNullOrEmpty(characterDefinition.id))
			{
				Debug.LogError("Character ID is null or empty!");
				continue;
			}

			PacmanCharacter characterPrefabFound = null;
			foreach(PacmanCharacter characterPrefab in characterPrefabs)
			{
				if (characterDefinition.id == characterPrefab.gameObject.name)
				{
					characterPrefabFound = characterPrefab;
					break;
				}
			}

			if (characterPrefabFound == null)
			{
				Debug.LogError("Character prefab could not be found: " + characterDefinition.id);
				continue;
			}

			PacmanCharacter characterSpawned = (PacmanCharacter)Instantiate(characterPrefabFound);

			characterSpawned.transform.parent = characterParent;
			PacmanTile startTile = GetTile(characterDefinition.xLocation, characterDefinition.yLocation);
			characterSpawned.transform.localPosition = Vector3.zero;
			
			// check if character doesn't happen to be on weird tile (not a problem per se, but probably unwanted)
			if (startTile.tileType != PacmanTile.TileType.Open && startTile.tileType != PacmanTile.TileType.Pickup)
			{
				Debug.LogWarning("Caution. Character " + characterDefinition.id + " placed on non-open tile: " + startTile + ": " + startTile.tileType );
			}

			// set speed
			if (characterDefinition.speed < 0)
			{
				Debug.LogError("Speed is negative for character " + characterDefinition.id + ". Setting speed to 1.");
				characterSpawned.speed = 1;
			}
			else
			{
				characterSpawned.speed = characterDefinition.speed;
			}

			// set spawndelay - i.e. how long before various characters become visible 
			// characters are then gradually enabled by CharacterSpawnRoutine()
			if (characterDefinition.spawnDelay < 0)
			{
				Debug.LogError("Spawn delay is negative for character " + characterDefinition.id + ". Setting spawn delay to 0.");
				characterSpawned.spawnDelay = 0;
			}
			else
			{
				characterSpawned.spawnDelay = characterDefinition.spawnDelay;
			}

			characterSpawned.SetSpawnLocation(new Vector2(characterDefinition.xLocation, characterDefinition.yLocation));

			spawnedCharacters.Add(characterSpawned);
		}
	}

	public void StartCharacterSpawnRoutine()
	{
		// stop and restart character spawn routine
		if (spawnRoutine != null && spawnRoutine.Running)
		{
			spawnRoutine.StopRoutine();
		}
		
		spawnRoutine = LugusCoroutines.use.StartRoutine(CharacterSpawnRoutine(spawnedCharacters));
	}

	protected IEnumerator CharacterSpawnRoutine(List<PacmanCharacter> spawnedCharacters)
	{
		List<PacmanCharacter> toBeEnabled = spawnedCharacters;
		
		foreach(PacmanCharacter c in spawnedCharacters)
		{
			if (c.spawnDelay > 0)
				c.DisableCharacter();
			else
				toBeEnabled.Remove(c);
		}

		float time = 0;
		while (toBeEnabled.Count > 0)
		{
			// each from, iterate over all characters until they've all been re-enabled
			for (int i = toBeEnabled.Count-1; i >= 0; i--) 
			{
				if (time >= toBeEnabled[i].spawnDelay)
				{
					toBeEnabled[i].EnableCharacter();
					toBeEnabled.Remove(toBeEnabled[i]);
				}
			}

			time += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
	}

	public GameObject GetPrefab(string name)
	{
		Transform child = prefabParent.FindChild(name);

		if (child != null)
			return child.gameObject;
		else
		{
			Debug.LogError("Child " + name + " not found.");
			return null;
		}
	}

	protected void PlaceLevelTiles()
	{
		foreach(PacmanTile tile in levelTiles)
		{
			if (tile.tileType == PacmanTile.TileType.Collide)
			{
				GameObject block = new GameObject(tile.gridIndices.ToString());

				int randomIndex = Random.Range(0, blockSprites.Length);

				block.transform.localScale = block.transform.localScale * wallTileScaleFactor;
				block.transform.parent = levelParent;
				block.transform.localPosition = new Vector3(tile.location.x, tile.location.y, 0);
				SpriteRenderer spriteRenderer = block.AddComponent<SpriteRenderer>();
				spriteRenderer.sprite = blockSprites[randomIndex];

				GameObject shadow = new GameObject("Shadow");
				shadow.transform.localScale = shadow.transform.localScale * wallTileScaleFactor;
				shadow.transform.parent = block.transform;
				shadow.transform.localPosition = new Vector3(0, 0, 1);
				SpriteRenderer spriteRenderer2 = shadow.AddComponent<SpriteRenderer>();
				spriteRenderer2.sprite = blockShadows[randomIndex];

				tile.sprite = block;

				// also randomly add a tile decorator to some tiles
				if (Random.value > 0.8f && blockDecorations.Length > 0)
				{
					GameObject decoration = new GameObject("Decoration");
					decoration.transform.localScale = decoration.transform.localScale * wallTileScaleFactor;
					decoration.transform.parent = block.transform;
					decoration.transform.localPosition = new Vector3(0, 0, -1);
					SpriteRenderer decorationSpriteRenderer = decoration.AddComponent<SpriteRenderer>();
					decorationSpriteRenderer.sprite = blockDecorations[Random.Range(0, blockDecorations.Length)];
				}
			}
			else if (tile.tileType == PacmanTile.TileType.Pickup)
			{
				GameObject pickUp = new GameObject(tile.gridIndices.ToString());
				pickUp.AddComponent<SpriteRenderer>().sprite = pickupSprite;

				pickUp.transform.parent = pickupParent;
				pickUp.transform.localPosition = new Vector3(tile.location.x, tile.location.y, -1);

				tile.sprite = pickUp;
			}
			else if (tile.tileType == PacmanTile.TileType.LevelEnd)
			{
				GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
				
				cube.transform.localScale = cube.transform.localScale * scale;
				cube.transform.parent = levelParent;
				cube.transform.localPosition = new Vector3(tile.location.x, tile.location.y, 0);

				cube.renderer.material.color = Color.red;
			}
		}
	}

	// Lookup methods--------------------------------------------------------------------

	// get tile by grid indices (contained in vector2)
	public PacmanTile GetTile(Vector2 coords)
	{
		return GetTile(coords, false);
	}

	// get tile by grid indices (contained in vector2)
	public PacmanTile GetTile(Vector2 coords, bool clamp)
	{
		int x = Mathf.RoundToInt(coords.x);
		int y = Mathf.RoundToInt(coords.y);
		
		return GetTile(x, y, clamp);
	}

	// get tile by grid indices
	public PacmanTile GetTile(int x, int y)
	{
		return GetTile(x, y, false);
	}

	// get tile by local position under level root
	public PacmanTile GetTileByLocation(float x, float y)
	{
		int xIndex = Mathf.RoundToInt(x/scale);
		int yIndex = Mathf.RoundToInt(y/scale);
		
		if (xIndex >= width || x < 0)
			return null;
		else if (yIndex >= height || y < 0)
			return null;

		return GetTile(xIndex, yIndex, true);
	}

	public PacmanTile GetTile(int x, int y, bool clamp)
	{
		if (x >= width || x < 0)
		{
			if (clamp)
				x = Mathf.Clamp(x, 0, width-1);
			else
				return null;
		}
		else if (y >= height || y < 0)
		{
			if (clamp)
				y = Mathf.Clamp(y, 0, height-1);
			else
				return null;
		}
		
		return levelTiles[x, y];
	}

	public PacmanTile GetTileInDirection(PacmanTile startTile, PacmanCharacter.CharacterDirections direction)
	{
		if (direction == PacmanCharacter.CharacterDirections.Undefined)
		{
			Debug.LogError("Direction was undefined.");
			return null;
		}

		if (direction == PacmanCharacter.CharacterDirections.Up)
		{
			return GetTile(startTile.gridIndices + new Vector2(0, 1), false);
		}
		else if (direction == PacmanCharacter.CharacterDirections.Right)
		{
			return GetTile(startTile.gridIndices + new Vector2(1, 0), false);
		}
		else if (direction == PacmanCharacter.CharacterDirections.Down)
		{
			return GetTile(startTile.gridIndices + new Vector2(0, -1), false);
		}
		else if (direction == PacmanCharacter.CharacterDirections.Left)
		{
			return GetTile(startTile.gridIndices + new Vector2(-1, 0), false);
		}

		return null;
	}
	
	public PacmanTile[] GetTilesInDirection(PacmanTile startTile, int amount, PacmanCharacter.CharacterDirections direction)
	{
		return GetTilesInDirection(startTile, amount, direction, false);
	}

	public PacmanTile[] GetTilesInDirection(PacmanTile startTile, int amount, PacmanCharacter.CharacterDirections direction, bool clamp)
	{
		List<PacmanTile> tileList = new List<PacmanTile>();
		
		int xStart = (int)startTile.gridIndices.x;
		int yStart = (int)startTile.gridIndices.y;
		
		if (direction == PacmanCharacter.CharacterDirections.Undefined)
		{	
			Debug.LogError("Direction is undefined.");
			return null;
		}
		
		if (direction == PacmanCharacter.CharacterDirections.Up)
		{
			for (int y = yStart; y < yStart + amount; y++) 
			{
				tileList.Add(GetTile(xStart, y, clamp));
			}
		}
		else if (direction == PacmanCharacter.CharacterDirections.Right)
		{
			for (int x = xStart; x < xStart + amount; x++) 
			{
				tileList.Add(GetTile(x, yStart, clamp));
			}
		}
		else if (direction == PacmanCharacter.CharacterDirections.Down)
		{
			for (int y = yStart; y > yStart - amount; y--) 
			{
				tileList.Add(GetTile(xStart, y, clamp));
			}
		}
		else if (direction == PacmanCharacter.CharacterDirections.Left)
		{
			for (int x = xStart; x > xStart - amount; x--) 
			{
				tileList.Add(GetTile(x, yStart, clamp));
			}
		}
		
		return tileList.ToArray();
	}

	public PacmanTile[] GetTilesAroundStraight(PacmanTile tile)
	{
		List<PacmanTile> result = new List<PacmanTile>();

		foreach(PacmanCharacter.CharacterDirections direction in System.Enum.GetValues(typeof(PacmanCharacter.CharacterDirections)))
		{
			if (direction == PacmanCharacter.CharacterDirections.Undefined)
				continue;

			if (GetTileInDirection(tile, direction) != null)
			{
				result.Add(GetTileInDirection(tile, direction));
			}
		}

		return result.ToArray();
	}
	
	public LevelQuadrant GetOppositeQuadrant(LevelQuadrant quadrant)
	{
		if (quadrant == LevelQuadrant.None)
		{
			Debug.Log("Quadrant was undefined!");
			return LevelQuadrant.None;
		}

		if (quadrant == LevelQuadrant.NE)
		{
			return LevelQuadrant.SW;
		}
		else if (quadrant == LevelQuadrant.SE)
		{
			return LevelQuadrant.NW;
		}
		else if (quadrant == LevelQuadrant.SW)
		{
			return LevelQuadrant.NE;
		}
		else if (quadrant == LevelQuadrant.NW)
		{
			return LevelQuadrant.SE;
		}

		return LevelQuadrant.None;
	}

	public LevelQuadrant GetQuadrantOfTile(PacmanTile tile)
	{
		if (tile.gridIndices.x >= width / 2 &&
		    tile.gridIndices.y >= height / 2)
		{
			return LevelQuadrant.NE;
		}
		else if (tile.gridIndices.x >= width / 2 &&
		    tile.gridIndices.y < height / 2)
		{
			return LevelQuadrant.SE;
		}
		else if (tile.gridIndices.x < width / 2 &&
		         tile.gridIndices.y < height / 2)
		{
			return LevelQuadrant.SW;
		}
		else if (tile.gridIndices.x < width / 2 &&
		         tile.gridIndices.y >= height / 2)
		{
			return LevelQuadrant.NW;
		}

		return LevelQuadrant.None;
	}

	public PacmanTile[] GetTilesForQuadrant(LevelQuadrant quadrant)
	{
		List<PacmanTile> returnTiles = new List<PacmanTile>();

		if (quadrant == LevelQuadrant.NE)
		{
			foreach(PacmanTile tile in levelTiles)
			{
				if (tile.gridIndices.x >= width / 2 &&
				    tile.gridIndices.y >= height / 2)
				{
					returnTiles.Add(tile);
				}
			}
		}
		else if (quadrant == LevelQuadrant.SE)
		{
			foreach(PacmanTile tile in levelTiles)
			{
				if (tile.gridIndices.x >= width / 2 &&
				    tile.gridIndices.y < height / 2)
				{
					returnTiles.Add(tile);
				}
			}
		}
		else if (quadrant == LevelQuadrant.SW)
		{
			foreach(PacmanTile tile in levelTiles)
			{
				if (tile.gridIndices.x < width / 2 &&
				    tile.gridIndices.y < height / 2)
				{
					returnTiles.Add(tile);
				}
			}
		}
		else if (quadrant == LevelQuadrant.NW)
		{
			foreach(PacmanTile tile in levelTiles)
			{
				if (tile.gridIndices.x < width / 2 &&
				    tile.gridIndices.y >= height / 2)
				{
					returnTiles.Add(tile);
				}
			}
		}
		return returnTiles.ToArray();
	}
	
	public PacmanTile GetTileByClick(Vector3 clickPoint)
	{
		Vector3 pointOnLevel = (LugusCamera.game.ScreenToWorldPoint(clickPoint) - levelRoot.position);

		// A bit of a weird trick here. Since tiles 
		if (pointOnLevel.x < 0)
		{
			if (Mathf.Abs(pointOnLevel.x) < scale / 2)
			{
				pointOnLevel.x = 0;
			}
		}

		if (pointOnLevel.y < 0)
		{
			if (Mathf.Abs(pointOnLevel.y) < scale / 2)
			{
				pointOnLevel.y = 0;
			}
		}

		return GetTileByLocation(pointOnLevel.x, pointOnLevel.y);
	}

	public float GetDistanceBetweenTiles(PacmanTile tile1, PacmanTile tile2)
	{
		if (tile1 == null)
		{
			Debug.LogError("Tile 1 was null.");
			return 0;
		}

		if (tile2 == null)
		{
			Debug.LogError("Tile 2 was null.");
			return 0;
		}

		return Vector2.Distance(tile1.location, tile2.location);
	}

	public Vector2 GetTileDistanceBetweenTiles(PacmanTile tile1, PacmanTile tile2)
	{
		if (tile1 == null)
		{
			Debug.LogError("Tile 1 was null.");
			return Vector2.zero;
		}
		
		if (tile2 == null)
		{
			Debug.LogError("Tile 2 was null.");
			return Vector2.zero;
		}

		return new Vector2(Mathf.Abs(tile1.gridIndices.x - tile2.gridIndices.x),  Mathf.Abs(tile1.gridIndices.y - tile2.gridIndices.y));
	}

	public void IncreasePickUpCount()
	{
		itemsPickedUp++;
		PacmanGUIManager.use.UpdatePickupCounter(itemsToBePickedUp - itemsPickedUp);
	}
	
	public bool AllItemsPickedUp()
	{
		return itemsPickedUp >= itemsToBePickedUp;
	}
	
	public void CheckPickedUpItems()
	{
		if (AllItemsPickedUp())
		{
			PacmanGameManager.use.WinGame();
		}
	}
	
	public void UnlockCenter()
	{
		foreach(PacmanTile tile in levelTiles)
		{
			if (tile.tileType == PacmanTile.TileType.Locked)
				tile.tileType = PacmanTile.TileType.EnemyAvoid;
		}
	}

	public int GetNumberOfExits(PacmanTile tile)
	{
		int exitCounter = 0;
		int x = (int)tile.gridIndices.x;
		int y = (int)tile.gridIndices.y;
		PacmanTile inspectedTile;
	
		// TO DO: Optimize. We're currently doing excess work for neighboring tiles that share an exit.
		
		// check above
		inspectedTile = GetTile(x, y+1);		// will return null for invalid coords
		if (inspectedTile != null)
		{
			if (PacmanEnemyCharacter.IsEnemyWalkable(inspectedTile))
				exitCounter++;
		}
		
		// check right
		inspectedTile = GetTile(x+1, y);				// will return null for invalid coords
		if (inspectedTile != null)
		{
			if (PacmanEnemyCharacter.IsEnemyWalkable(inspectedTile))
				exitCounter++;
		}
		
		// check down
		inspectedTile = GetTile(x, y-1);				// will return null for invalid coords
		if (inspectedTile != null)
		{
			if (PacmanEnemyCharacter.IsEnemyWalkable(inspectedTile))
				exitCounter++;
		}
		
		// check left
		inspectedTile = GetTile(x-1, y);				// will return null for invalid coords
		if (inspectedTile != null)
		{
			if (PacmanEnemyCharacter.IsEnemyWalkable(inspectedTile))
				exitCounter++;
		}
		
		return exitCounter;	
	}

	void OnGUI()
	{
		if (LugusDebug.debug)
		{
			for (int i = 0; i < levels.Length; i++) 
			{
				if (GUILayout.Button("Level " + i))
				{
					PacmanGameManager.use.StartNewGame(i);
				}
			}
		}
	}
}

