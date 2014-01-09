using UnityEngine;
using System.Collections;

[System.Serializable]
public class PacmanLevelDefinition : ScriptableObject {
	
	public string backgroundMusicName = "";
	public int width = 13;
	public int height = 13;
	public string level;
	public PacmanCharacterDefinition[] characters;
	public string[] updaters;
	public PacmanTileItemDefinition[] tileItems;

	// Arrays of serialized classes are not created with default values
	// Instead, initialize values once in OnEnable (which runs AFTER deserialization), checking for null / zero value
	// http://forum.unity3d.com/threads/155352-Serialization-Best-Practices-Megapost
	void OnEnable()
	{
	
	}
}

[System.Serializable]
public class PacmanCharacterDefinition 
{
	public string id = "";
	public float speed = 120;
	public int xLocation = 0;
	public int yLocation = 0;
	public float spawnDelay = 0;
	public PacmanCharacter.CharacterDirections startDirection = PacmanCharacter.CharacterDirections.Undefined;
	
}

[System.Serializable]
public class PacmanTileItemDefinition
{
	public string id;
	public Vector2 tileCoordinates;
}