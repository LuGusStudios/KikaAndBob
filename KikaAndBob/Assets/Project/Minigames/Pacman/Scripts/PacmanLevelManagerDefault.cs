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
	public int width = 13;
	public int height = 13;
	public float scale = 64;
	public float wallTileScaleFactor = 0.6f;
	public float pickupScaleFactor = 0.15f;
	
	public PacmanCharacter[] characterPrefabs = null;
	public GameObject[] tileItems = null;

	// MOVE TO SCRIPTABLE OBJECT
	public Sprite[] blockSprites = null;
	public Sprite[] blockShadows = null;
	public Sprite[] blockDecorations = null;
	public Sprite pickupSprite = null;
	public Sprite powerUpSprite = null;
	public Sprite doorSprite = null;
	public Sprite teleportSprite = null;

	
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
	public List<PacmanTile> teleportTiles = new List<PacmanTile>();

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
		Debug.Log("Clearing level (build).");
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
			// NOTE: Ideally, destroying this gameObject would mean it is immediately gone!
			// But Destroy() is delayed until the end of the Update loop, which means the scan for new player characters will still find them!
			// So: Make sure to also set them disabled, which will make them invisible to GetComponents (provided includeInactive is false)
			if (characterParent.GetChild(i).GetComponent<PacmanCharacter>() != null)
			{
				characterParent.GetChild(i).GetComponent<PacmanCharacter>().enabled = false;
				characterParent.GetChild(i).gameObject.SetActive(false);
			}
			Destroy(characterParent.GetChild(i).gameObject);
		}
	}

	public void ClearLevelEditor()
	{
		Debug.Log("Clearing level (playing in editor).");
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

	// only used for testing and for quickly building a level
	public void BuildLevelDebug(string levelData, int _width, int _height)
	{
		FindReferences();
		
		#if UNITY_EDITOR
		ClearLevelEditor();
		#else
		ClearLevel();
		#endif

		width = _width;
		height = _height;
		
		ParseLevelTiles(levelData, _width, _height);
		
		PlaceLevelTiles();
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

		width = level.width;
		height = level.height;

		ParseLevelTiles(level.level, width, height);

		PlaceLevelTiles();

		PlaceLevelTileItems(level.tileItems);

		PlaceCharacters(level.characters);

		ApplyUpdaters(level.updaters);

		PacmanCameraFollower.use.track = level.cameraTracksPlayer;

		LugusAudio.use.Ambient().StopAll();
		if (!string.IsNullOrEmpty(level.backgroundMusicName))
		{
			AudioClip music = LugusResources.use.Shared.GetAudio(level.backgroundMusicName);
			LugusAudio.use.Ambient().Play(music, true, new LugusAudioTrackSettings().Loop(true));
		}

		if (onLevelBuilt != null)
			onLevelBuilt();
	}
		
	public void ParseLevelTiles(string levelData, int _width, int _height)
	{
		if (string.IsNullOrEmpty(levelData))
		{
			Debug.LogError("LevelManager: Level string is null or empty!");
			return;
		}

		// clear grid
		levelTiles = new PacmanTile[_width, _height];
		itemsPickedUp = 0;
		itemsToBePickedUp = 0;

		teleportTiles.Clear();

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

				// default value is open tile
				char tileChar = 'o';
				if (currentStringIndex < levelData.Length)
					tileChar = levelData[currentStringIndex];

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
					currentTile.tileType = PacmanTile.TileType.Upgrade;
				}
				else if (tileChar == 'd') // Door tiles start off as open tiletype, but are kept in a list of doors which are regularly switched between Open and Collide
				{
					currentTile.tileType = PacmanTile.TileType.Door;
				}
				else if (tileChar == 't')	// teleport tiles on either side of the field
				{
					currentTile.tileType = PacmanTile.TileType.Teleport;
					teleportTiles.Add(currentTile);
				}
				else if (tileChar == 'e')	// entering this tile finishes the game
					currentTile.tileType = PacmanTile.TileType.LevelEnd;
				else if (tileChar == 'a')	
					currentTile.tileType = PacmanTile.TileType.Locked;
				else
					currentTile.tileType = PacmanTile.TileType.Open;
			}
		}
		
		//PacmanGUIManager.use.UpdatePickupCounter(itemsToBePickedUp);
		
		// count tile exits for each tile. Enemies use this value to figure out whether they're in a cul-de-sac and are allowed to turn around.
		foreach(PacmanTile tile in levelTiles)
		{
			if (tile == null)
			{
				Debug.LogError("Tile was null!");
				continue;
			}

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

		spawnedCharacters.Clear();
		
		foreach(PacmanCharacterDefinition characterDefinition in characters)
		{
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

			PacmanTile startTile = GetTile(characterDefinition.xLocation, characterDefinition.yLocation);

			if (startTile == null)
			{
				Debug.LogWarning("Character " + characterDefinition.id + " was placed on a non-existing tile. Not placing.");
				return;
			}
			
			// check if character doesn't happen to be on a non-open tile (not a problem per se, but probably unwanted)
			if (startTile.tileType != PacmanTile.TileType.Open && startTile.tileType != PacmanTile.TileType.Pickup)
			{
				Debug.LogWarning("Caution. Character " + characterDefinition.id + " placed on non-open tile: " + startTile + ": " + startTile.tileType );
			}

			PacmanCharacter characterSpawned = (PacmanCharacter)Instantiate(characterPrefabFound);

			characterSpawned.transform.parent = characterParent;
			characterSpawned.transform.localPosition = Vector3.zero;

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
			characterSpawned.SetStartDirection(characterDefinition.startDirection);
			characterSpawned.SetDefaultTargetTiles(characterDefinition.defaultTargetTiles);


			spawnedCharacters.Add(characterSpawned);
		}
	}

	protected void PlaceLevelTileItems(PacmanTileItemDefinition[] tileItemDefinitions)
	{
		foreach(PacmanTileItemDefinition definition in tileItemDefinitions)
		{
			GameObject tileItemPrefab = null;

			foreach(GameObject go in tileItems)
			{
				if (go.name == definition.id)
				{
					tileItemPrefab = go;
					break;
				}
			}

			if (tileItemPrefab == null)
			{
				Debug.LogError("Did not find tile item ID: " + definition.id);
				return;
			}

			PacmanTile targetTile = GetTile(definition.tileCoordinates, false);

			if (targetTile == null)
			{
				Debug.LogError("Did not find tile with coordinates:" + definition.tileCoordinates + ". Skipping placing tile item: " + definition.id);
				return;
			}

			GameObject tileItem = (GameObject)Instantiate(tileItemPrefab);

			tileItem.transform.parent = pickupParent;
			tileItem.transform.localPosition = targetTile.location.v3().z(1);

			PacmanTileItem tileItemScript = tileItem.GetComponent<PacmanTileItem>();

			if (tileItemScript != null)
			{
				tileItemScript.parentTile = targetTile;
				tileItemScript.Initialize();
			}

			targetTile.tileItems.Add(tileItem);
		}
	}

	protected void ApplyUpdaters(string[] ids)
	{
		GameObject updaterContainer = GameObject.Find("Updaters");

		if (updaterContainer == null)
		{
			updaterContainer = new GameObject("Updaters");
		}

		PacmanLevelUpdater[] updaters = updaterContainer.GetComponents<PacmanLevelUpdater>();

		for (int i = updaters.Length - 1; i >= 0; i--) 
		{
			updaters[i].Deactivate();

			#if UNITY_EDITOR
			DestroyImmediate(updaters[i]);
			#else
			Destroy(updaters[i]);
			#endif

		}

		foreach(string id in ids)
		{
			if (id == "DoorUpdater" && updaterContainer.GetComponent<DoorUpdater>() == null)
			{
				updaterContainer.AddComponent<DoorUpdater>();
			}
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
		List<PacmanCharacter> toBeEnabled = new List<PacmanCharacter>(spawnedCharacters);

		for (int i = toBeEnabled.Count-1; i >= 0; i--) 
		{
			if (toBeEnabled[i].spawnDelay > 0)
			{
				toBeEnabled[i].DisableCharacter();
			}
			else
			{
				toBeEnabled.Remove(toBeEnabled[i]);
			}
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
		if (levelTiles == null)
			return;

		foreach(PacmanTile tile in levelTiles)
		{
			if (tile == null)
				continue;

			if (tile.tileType == PacmanTile.TileType.Collide)
			{
				GameObject block = new GameObject(tile.gridIndices.ToString() + ": Block");

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

				tile.rendered = block;

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
				GameObject pickUp = new GameObject(tile.gridIndices.ToString() + ": Pick up");
				pickUp.AddComponent<SpriteRenderer>().sprite = pickupSprite;

				pickUp.transform.parent = pickupParent;
				pickUp.transform.localPosition = new Vector3(tile.location.x, tile.location.y, -1);

				pickUp.transform.localScale = Vector3.one * pickupScaleFactor;

				tile.rendered = pickUp;
			}
			else if (tile.tileType == PacmanTile.TileType.LevelEnd)
			{
				GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
				
				cube.transform.localScale = cube.transform.localScale * scale;
				cube.transform.parent = levelParent;
				cube.transform.localPosition = new Vector3(tile.location.x, tile.location.y, 0);

				cube.renderer.sharedMaterial.color = Color.red;

				tile.rendered = cube;
			}
			else if (tile.tileType == PacmanTile.TileType.Door)
			{
				GameObject door = new GameObject(tile.gridIndices.ToString() + ": Door");
				door.AddComponent<SpriteRenderer>().sprite = doorSprite;
				
				door.transform.parent = levelParent;
				door.transform.localScale = door.transform.localScale * wallTileScaleFactor;
				door.transform.localPosition = new Vector3(tile.location.x, tile.location.y, 0);
			
				tile.rendered = door;
			}
			else if (tile.tileType == PacmanTile.TileType.Upgrade)
			{
				GameObject powerUp = new GameObject(tile.gridIndices.ToString() + ": Power Up");
				powerUp.AddComponent<SpriteRenderer>().sprite = powerUpSprite;
				
				powerUp.transform.parent = pickupParent;
				powerUp.transform.localScale = powerUp.transform.localScale * wallTileScaleFactor;
				powerUp.transform.localPosition = new Vector3(tile.location.x, tile.location.y, 0);

				tile.rendered = powerUp;
			}
			else if (tile.tileType == PacmanTile.TileType.Teleport)
			{
				GameObject teleport = new GameObject(tile.gridIndices.ToString() + ": Teleport");
				teleport.AddComponent<SpriteRenderer>().sprite = teleportSprite;
				
				teleport.transform.parent = levelParent;
				teleport.transform.localScale = teleport.transform.localScale * wallTileScaleFactor;
				teleport.transform.localPosition = new Vector3(tile.location.x, tile.location.y, 0);
				
				tile.rendered = teleport;
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
		if (tile == null)
		{
			Debug.LogError("Tile was null!");
			return 0;
		}

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

	public float GetLevelWidthInPixels()
	{
		return width * scale;
	}

	public float GetLevelHeightInPixels()
	{
		return height * scale;
	}

	public Transform GetLevelRoot()
	{
		return levelRoot;
	}

	void OnGUI()
	{
		if (LugusDebug.debug)
		{
			for (int i = 0; i < levels.Length; i++) 
			{
				if (GUILayout.Button("Level " + i))
				{
					PacmanGameManager.use.StartNewLevel(i);
				}
			}
		}
	}
}

