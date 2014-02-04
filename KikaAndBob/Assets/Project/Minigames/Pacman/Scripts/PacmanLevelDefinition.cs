using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

		List<PacmanCharacterDefinition> characters = new List<PacmanCharacterDefinition>();
		List<string> updaters = new List<string>();
		List<PacmanTileItemDefinition> tileitems = new List<PacmanTileItemDefinition>();
		while (parser.Read("Level"))
		{
			if (parser.tagType == TinyXmlReader.TagType.OPENING)
			{
				switch (parser.tagName)
				{
					case "BackgroundMusicName":
						level.backgroundMusicName = parser.content;
						break;
					case "Width":
						level.width = int.Parse(parser.content);
						break;
					case "Height":
						level.height = int.Parse(parser.content);
						break;
					case "CameraTracksPlayer":
						level.cameraTracksPlayer = bool.Parse(parser.content);
						break;
					case "Layout":
						char[] separators = { ' ', '\t', '\n', '\r' };
						string[] rows = parser.content.Split(separators);
						foreach (string row in rows)
						{
							level.level += row;
						}
						break;
					case "Character":
						characters.Add(PacmanCharacterDefinition.FromXML(parser));
						break;
					case "Updater":
						updaters.Add(parser.content);
						break;
					case "TileItem":
						tileitems.Add(PacmanTileItemDefinition.FromXML(parser));
						break;
				}
			}
		}

		level.characters = characters.ToArray();
		level.updaters = updaters.ToArray();
		level.tileItems = tileitems.ToArray();

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

		if ((parser.tagType != TinyXmlReader.TagType.OPENING) ||
			(parser.tagName != "Character"))
		{
			Debug.Log("PacmanCharacterDefinition.FromXML(): unexpected tag type or tag name.");
			return null;
		}

		List<Vector2> targettiles = new List<Vector2>();
		while (parser.Read("Character"))
		{
			if (parser.tagType == TinyXmlReader.TagType.OPENING)
			{
				switch(parser.tagName)
				{
					case "ID":
						character.id = parser.content;
						break;
					case "Speed":
						character.speed = float.Parse(parser.content);
						break;
					case "XLocation":
						character.xLocation = int.Parse(parser.content);
						break;
					case "YLocation":
						character.yLocation = int.Parse(parser.content);
						break;
					case "SpawnDelay":
						character.spawnDelay = float.Parse(parser.content);
						break;
					case "StartDirection":
						switch (parser.content)
						{
							case "down":
								character.startDirection = PacmanCharacter.CharacterDirections.Down;
								break;
							case "left":
								character.startDirection = PacmanCharacter.CharacterDirections.Left;
								break;
							case "up":
								character.startDirection = PacmanCharacter.CharacterDirections.Up;
								break;
							case "right":
								character.startDirection = PacmanCharacter.CharacterDirections.Right;
								break;
							case "undefined":
								character.startDirection = PacmanCharacter.CharacterDirections.Undefined;
								break;
						}
						break;
					case "Tile":
						Vector2 tile = Vector2.zero;
						while (parser.Read("Tile"))
						{
							if (parser.tagType == TinyXmlReader.TagType.OPENING)
							{
								switch (parser.tagName)
								{
									case "X":
										tile.x = float.Parse(parser.content);
										break;
									case "Y":
										tile.y = float.Parse(parser.content);
										break;
								}
							}
						}
						targettiles.Add(tile);
						break;
				}
			}
		}

		character.defaultTargetTiles = targettiles.ToArray();
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

		if ((parser.tagType != TinyXmlReader.TagType.OPENING) ||
			(parser.tagName != "Character"))
		{
			Debug.Log("PacmanTileItemDefinition.FromXML(): unexpected tag type or tag name.");
			return null;
		}

		while(parser.Read("TileItem"))
		{
			if (parser.tagType == TinyXmlReader.TagType.OPENING)
			{
				switch(parser.tagName)
				{
					case "ID":
						tileitem.id = parser.content;
						break;
					case "TileCoordinates":
						Vector2 coordinates = Vector2.zero;
						while (parser.Read("TileCoordinates"))
						{
							if (parser.tagType == TinyXmlReader.TagType.OPENING)
							{
								switch (parser.tagName)
								{
									case "X":
										coordinates.x = float.Parse(parser.content);
										break;
									case "Y":
										coordinates.y = float.Parse(parser.content);
										break;
								}
							}
						}
						tileitem.tileCoordinates = coordinates;
						break;
				}
			}
		}

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