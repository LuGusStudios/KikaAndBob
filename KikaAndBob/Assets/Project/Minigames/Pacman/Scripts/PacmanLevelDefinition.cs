using UnityEngine;
using System.Collections;

[System.Serializable]
public class PacmanLevelDefinition : ScriptableObject {
	
	public static PacmanLevelDefinition FromXML(TinyXmlReader parser)
	{
		PacmanLevelDefinition level = ScriptableObject.CreateInstance<PacmanLevelDefinition>();

		if ((parser.tagType != TinyXmlReader.TagType.OPENING) ||
			(parser.tagName != "Level"))
		{
			Debug.Log("PacmanLevelDefinition.FromXML(): unexpected tag type or tag name.");
			return null;
		}


		return level;
	}

	public static string ToXML(PacmanLevelDefinition level)
	{
		string rawdata = string.Empty;

		if (level == null)
		{
			Debug.Log("PacmanLevelDefinition.ToXML(): The level to be serialized is null.");
			return rawdata;
		}

		rawdata += "<Level>\r\n";
		rawdata += "\t<BackgroundMusicName>" + level.backgroundMusicName + "</BackgroundMusicName>\r\n";
		rawdata += "\t<Width>" + level.width.ToString() + "</Width>\r\n";
		rawdata += "\t<Height>" + level.height.ToString() + "</Height>\r\n";
		rawdata += "\t<CameraTracksPlayer>" + level.cameraTracksPlayer.ToString() + "</CameraTracksPlayer>\r\n";
		rawdata += "\t<Layout>\r\n";
		for (int i = 0; i < level.height; ++i)
		{
			rawdata += "\t\t" + level.level.Substring(i * level.width, level.width) + "\r\n";
		}
		rawdata += "\t</Layout>\r\n";
		rawdata += "\t<Characters>\r\n";
		foreach (PacmanCharacterDefinition character in level.characters)
		{
			rawdata += PacmanCharacterDefinition.ToXML(character, 2);
		}
		rawdata += "\t</Characters>\r\n";
		rawdata += "\t<Updaters>\r\n";
		foreach (string updater in level.updaters)
		{
			rawdata += "\t\t<Updater>" + updater + "</Updater>\r\n";
		}
		rawdata += "\t</Updaters>\r\n";
		rawdata += "\t<TileItems>\r\n";
		foreach(PacmanTileItemDefinition tileitem in level.tileItems)
		{
			rawdata += PacmanTileItemDefinition.ToXML(tileitem, 2);
		}
		rawdata += "\t</TileItems>\r\n";
		rawdata += "</Level>\r\n";

		return rawdata;
	}

	public string backgroundMusicName = "";
	public int width = 13;
	public int height = 13;
	public bool cameraTracksPlayer = false;
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

	public static PacmanCharacterDefinition FromXML(TinyXmlReader parser)
	{
		PacmanCharacterDefinition character = new PacmanCharacterDefinition();

		return character;
	}

	public static string ToXML(PacmanCharacterDefinition character, int depth)
	{
		string rawdata = string.Empty;

		if (character == null)
		{
			Debug.Log("PacmanCharacterDefinition.ToXML(): The character to be serialized is null.");
			return rawdata;
		}

		string tabs = string.Empty;
		for (int i = 0; i < depth; ++i)
		{
			tabs += "\t";
		}

		rawdata += tabs + "<Character>\r\n";
		rawdata += tabs + "\t<ID>" + character.id + "</ID>\r\n";
		rawdata += tabs + "\t<Speed>" + character.speed.ToString() + "</Speed>\r\n";
		rawdata += tabs + "\t<XLocation>" + character.xLocation.ToString() + "</XLocation>\r\n";
		rawdata += tabs + "\t<YLocation>" + character.yLocation.ToString() + "</YLocation>\r\n";
		rawdata += tabs + "\t<SpawnDelay>" + character.spawnDelay.ToString() + "</SpawnDelay>\r\n";
		rawdata += tabs + "\t<StartDirection>";
		switch (character.startDirection)
		{
			case PacmanCharacter.CharacterDirections.Down:
				rawdata += "down";
				break;
			case PacmanCharacter.CharacterDirections.Left:
				rawdata += "left";
				break;
			case PacmanCharacter.CharacterDirections.Up:
				rawdata += "up";
				break;
			case PacmanCharacter.CharacterDirections.Right:
				rawdata += "right";
				break;
			case PacmanCharacter.CharacterDirections.Undefined:
			default:
				rawdata += "undefined";
				break;
		}
		rawdata += "</StartDirection>\r\n";
		rawdata += tabs + "\t<DefaultTargetTiles>\r\n";
		foreach (Vector2 targettile in character.defaultTargetTiles)
		{
			rawdata += tabs + "\t\t<Tile>\r\n";
			rawdata += tabs + "\t\t\t<X>" + targettile.x.ToString() + "</X>\r\n";
			rawdata += tabs + "\t\t\t<Y>" + targettile.y.ToString() + "</Y>\r\n";
			rawdata += tabs + "\t\t</Tile>\r\n";
		}
		rawdata += tabs + "\t</DefaultTargetTiles>\r\n";
		rawdata += tabs + "</Character>\r\n";

		return rawdata;
	}

	public string id = "";
	public float speed = 120;
	public int xLocation = 0;
	public int yLocation = 0;
	public float spawnDelay = 0;
	public PacmanCharacter.CharacterDirections startDirection = PacmanCharacter.CharacterDirections.Undefined;
	public Vector2[] defaultTargetTiles;
}

[System.Serializable]
public class PacmanTileItemDefinition
{
	public static PacmanTileItemDefinition FromXML(TinyXmlReader parser)
	{
		PacmanTileItemDefinition tileitem = new PacmanTileItemDefinition();

		return tileitem;
	}

	public static string ToXML(PacmanTileItemDefinition tileitem, int depth)
	{
		string rawdata = string.Empty;

		if (tileitem == null)
		{
			Debug.Log("PacmanTileItemDefinition.ToXML(): The tile item to be serialized is null.");
			return rawdata;
		}

		string tabs = string.Empty;
		for (int i = 0; i < depth; ++i)
		{
			tabs += "\t";
		}

		rawdata += tabs + "<TileItem>\r\n";
		rawdata += tabs + "\t<ID>" + tileitem.id + "</ID>\r\n";
		rawdata += tabs + "\t<TileCoordinates>\r\n";
		rawdata += tabs + "\t\t<X>" + tileitem.tileCoordinates.x.ToString() + "</X>\r\n";
		rawdata += tabs + "\t\t<Y>" + tileitem.tileCoordinates.y.ToString() + "</Y>\r\n";
		rawdata += tabs + "\t</TileCoordinates>\r\n";
		rawdata += tabs + "</TileItem>\r\n";

		return rawdata;
	}


	public string id;
	public Vector2 tileCoordinates;
}