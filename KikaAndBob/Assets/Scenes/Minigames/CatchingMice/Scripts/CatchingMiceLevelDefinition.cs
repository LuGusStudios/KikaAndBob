using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CatchingMiceLevelDefinition : ScriptableObject 
{
	public static CatchingMiceLevelDefinition FromXML(string rawData)
	{
		TinyXmlReader parser = new TinyXmlReader(rawData);

		while (parser.Read())
		{
			if ((parser.tagType == TinyXmlReader.TagType.OPENING) &&
				(parser.tagName == "Level"))
			{
				return CatchingMiceLevelDefinition.FromXML(parser);
			}
		}

		// If we end up here, then no level tag was found...
		return null;
	}

	public static CatchingMiceLevelDefinition FromXML(TinyXmlReader parser)
	{
		CatchingMiceLevelDefinition level = ScriptableObject.CreateInstance<CatchingMiceLevelDefinition>();

		if ((parser.tagType != TinyXmlReader.TagType.OPENING) ||
			(parser.tagName != "Level"))
		{
            CatchingMiceLogVisualizer.use.Log("CatchingMiceLevelDefinition.FromXML(): unexpected tag type or tag name.");
			return null;
		}

        //List<string> updaters = new List<string>();
        List<CatchingMiceCharacterDefinition> characters = new List<CatchingMiceCharacterDefinition>();
		List<CatchingMiceFurnitureDefinition> tileitems = new List<CatchingMiceFurnitureDefinition>();
        List<CatchingMiceCheeseDefinition> cheeses = new List<CatchingMiceCheeseDefinition>();
		List<CatchingMiceTrapDefinition> traps = new List<CatchingMiceTrapDefinition>();
        List<CatchingMiceHoleDefinition> holeItems = new List<CatchingMiceHoleDefinition>();
        List<CatchingMiceWaveDefinition> waves = new List<CatchingMiceWaveDefinition>();
		List<CatchingMicePatrolDefinition> patrols = new List<CatchingMicePatrolDefinition>();
		List<CatchingMiceObstacleDefinition> obstacles = new List<CatchingMiceObstacleDefinition>();

		while (parser.Read("Level"))
		{
			if (parser.tagType == TinyXmlReader.TagType.OPENING)
			{
				switch (parser.tagName)
				{
                    //case "BackgroundMusicName":
                    //    level.backgroundMusicName = parser.content;
                    //    break;
					case "Width":
						level.width = int.Parse(parser.content);
						break;
					case "Height":
						level.height = int.Parse(parser.content);
						break;
                    //case "CameraTracksPlayer":
                    //    level.cameraTracksPlayer = bool.Parse(parser.content);
                    //    break;
					case "Layout":
						char[] separators = { ' ', '\t', '\n', '\r' };
						string[] rows = parser.content.Split(separators);
						foreach (string row in rows)
						{
							level.layout += row;
						}
						break;
                    case "Character":
                        characters.Add(CatchingMiceCharacterDefinition.FromXML(parser));
                        break;
					case "Furniture":
						tileitems.Add(CatchingMiceFurnitureDefinition.FromXML(parser));
						break;
                    case "Cheese":
                        cheeses.Add(CatchingMiceCheeseDefinition.FromXML(parser));
                        break;
					case "Trap":
						traps.Add(CatchingMiceTrapDefinition.FromXML(parser));
						break;
                    case "HoleItem":
                        holeItems.Add(CatchingMiceHoleDefinition.FromXML(parser));
                        break;
                    case "Wave":
                        waves.Add(CatchingMiceWaveDefinition.FromXML(parser));
                        break;
					case "Patrol":
						patrols.Add(CatchingMicePatrolDefinition.FromXML(parser));
						break;
					case "Obstacle":
						obstacles.Add(CatchingMiceObstacleDefinition.FromXML(parser));
						break;
				}
			}
		}

		level.characters = characters.ToArray();
		level.furniture = tileitems.ToArray();
		level.cheeses = cheeses.ToArray();
		level.traps = traps.ToArray();
		level.holeItems = holeItems.ToArray();
		level.waves = waves.ToArray();
		level.patrols = patrols.ToArray();
		level.obstacles = obstacles.ToArray();

		return level;
	}

	public static string ToXML(CatchingMiceLevelDefinition level)
	{
		string rawdata = string.Empty;

		if (level == null)
		{
            CatchingMiceLogVisualizer.use.Log("CatchingMiceLevelDefinition.ToXML(): The level to be serialized is null.");
			return rawdata;
		}

		rawdata += "<Level>\r\n";

		//rawdata += "\t<BackgroundMusicName>" + level.backgroundMusicName + "</BackgroundMusicName>\r\n";
		rawdata += "\t<Width>" + level.width.ToString() + "</Width>\r\n";
		rawdata += "\t<Height>" + level.height.ToString() + "</Height>\r\n";
		//rawdata += "\t<CameraTracksPlayer>" + level.cameraTracksPlayer.ToString() + "</CameraTracksPlayer>\r\n";

		rawdata += "\t<Layout>\r\n";

		// Pad the end of the level layout if it does not meet the length requirements
		if (level.layout.Length < (level.width * level.height))
		{
			CatchingMiceLogVisualizer.use.LogWarning("CatchingMiceLevelDefinition.ToXML(): The layout length does not match the level's dimensions.\nThe layout will be padded with o's.");
			level.layout = level.layout.PadRight(level.width * level.height, 'o');
		}

		for (int i = 0; i < level.height; ++i)
		{
			rawdata += "\t\t" + level.layout.Substring(i * level.width, level.width) + "\r\n";
		}
		rawdata += "\t</Layout>\r\n";

		rawdata += "\t<Furnitures>\r\n";
		foreach (CatchingMiceFurnitureDefinition furniture in level.furniture)
		{
			rawdata += CatchingMiceFurnitureDefinition.ToXML(furniture, 2);
		}
		rawdata += "\t</Furnitures>\r\n";

		rawdata += "\t<Obstacles>\r\n";
		foreach (CatchingMiceObstacleDefinition obstacle in level.obstacles)
		{
			rawdata += CatchingMiceObstacleDefinition.ToXML(obstacle, 2);
		}
		rawdata += "\t</Obstacles>\r\n";

        rawdata += "\t<Cheeses>\r\n";
        foreach (CatchingMiceCheeseDefinition cheese in level.cheeses)
        {
            rawdata += CatchingMiceCheeseDefinition.ToXML(cheese, 2);
        }
        rawdata += "\t</Cheeses>\r\n";

        rawdata += "\t<HoleItems>\r\n";
        foreach (CatchingMiceHoleDefinition holeItem in level.holeItems)
        {
            rawdata += CatchingMiceHoleDefinition.ToXML(holeItem, 2);
        }
        rawdata += "\t</HoleItems>\r\n";

		rawdata += "\t<Traps>\r\n";
		foreach (CatchingMiceTrapDefinition trap in level.traps)
		{
			rawdata += CatchingMiceTrapDefinition.ToXML(trap, 2);
		}
		rawdata += "\t</Traps>\r\n";

        rawdata += "\t<Characters>\r\n";
        foreach (CatchingMiceCharacterDefinition character in level.characters)
        {
            rawdata += CatchingMiceCharacterDefinition.ToXML(character, 2);
        }
		rawdata += "\t</Characters>\r\n";

		rawdata += "\t<Patrols>\r\n";
		foreach (CatchingMicePatrolDefinition patrol in level.patrols)
		{
			rawdata += CatchingMicePatrolDefinition.ToXML(patrol, 2);
		}
		rawdata += "\t</Patrols>\r\n";

        rawdata += "\t<Waves>\r\n";
        foreach (CatchingMiceWaveDefinition wave in level.waves)
        {
            rawdata += CatchingMiceWaveDefinition.ToXML(wave, 2);
        }
        rawdata += "\t</Waves>\r\n";

		rawdata += "</Level>\r\n";

		return rawdata;
	}

	//public string backgroundMusicName = "";
	public int width = 13;
	public int height = 13;
	//public bool cameraTracksPlayer = false;
	public string layout = string.Empty;
	public CatchingMiceFurnitureDefinition[] furniture;
	public CatchingMiceObstacleDefinition[] obstacles;
    public CatchingMiceCheeseDefinition[] cheeses;
    public CatchingMiceHoleDefinition[] holeItems;
	public CatchingMiceTrapDefinition[] traps;
    public CatchingMiceCharacterDefinition[] characters;
	public CatchingMicePatrolDefinition[] patrols;
    public CatchingMiceWaveDefinition[] waves;
    
	// Arrays of serialized classes are not created with default values
	// Instead, initialize values once in OnEnable (which runs AFTER deserialization), checking for null / zero value
	// http://forum.unity3d.com/threads/155352-Serialization-Best-Practices-Megapost
	void OnEnable()
	{
	
	}
}

[System.Serializable]
public class CatchingMiceObstacleDefinition
{
	// The obstacle definition is somewhat special compared
	// to the other definitions. An obstacle is dynamic,
	// and depending on the object, the tags, values and
	// structure may vary. Every obstacle definition has a
	// prefab name which refers to its prefab, and a configuration
	// which the obstacle will parse on its own. This configuration
	// needs to be reconstructed, because there currently
	// no simple method of the TinyXMLReader parser object
	// to simply extract all data between a certain tag pair.

	public static CatchingMiceObstacleDefinition FromXML(TinyXmlReader parser)
	{
		CatchingMiceObstacleDefinition obstacle = new CatchingMiceObstacleDefinition();

		if ((parser.tagType != TinyXmlReader.TagType.OPENING) ||
			(parser.tagName != "Obstacle"))
		{
			CatchingMiceLogVisualizer.use.Log("CatchingMiceObstacleDefinition.FromXML(): unexpected tag type or tag name.");
			return null;
		}

		while (parser.Read("Obstacle"))
		{
			if (parser.tagType == TinyXmlReader.TagType.OPENING)
			{
				switch (parser.tagName)
				{
					case "PrefabName":
						obstacle.prefabName = parser.content;
						break;
					case "Configuration":

						obstacle.obstacleData += "<" + parser.tagName + ">";

						while (parser.Read("Configuration"))
						{
							switch(parser.tagType)
							{
								case TinyXmlReader.TagType.OPENING:
									obstacle.obstacleData += "<" + parser.tagName + ">";
									obstacle.obstacleData += parser.content;
									break;
								case TinyXmlReader.TagType.CLOSING:
									obstacle.obstacleData += "</" + parser.tagName + ">";
									break;
							}
						}

						obstacle.obstacleData += "</" + parser.tagName + ">";

						break;
					case "Position":
						Vector2 coordinates = Vector2.zero;
						while (parser.Read("Position"))
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
						obstacle.position = coordinates;
						break;
				}
			}
		}

		return obstacle;
	}

	public static string ToXML(CatchingMiceObstacleDefinition obstacle, int depth)
	{
		string rawdata = string.Empty;

		if (obstacle == null)
		{
			CatchingMiceLogVisualizer.use.Log("CatchingMiceCharacterDefinition.ToXML(): The character to be serialized is null.");
			return rawdata;
		}

		string tabs = string.Empty;
		for (int i = 0; i < depth; ++i)
		{
			tabs += "\t";
		}

		rawdata += tabs + "<Obstacle>\r\n";

		rawdata += tabs + "\t<PrefabName>" + obstacle.prefabName + "</PrefabName>\r\n";

		rawdata += tabs + "\t<Position>\r\n";
		rawdata += tabs + "\t\t<X>" + obstacle.position.x.ToString() + "</X>\r\n";
		rawdata += tabs + "\t\t<Y>" + obstacle.position.y.ToString() + "</Y>\r\n";
		rawdata += tabs + "\t</Position>\r\n";

		rawdata += tabs + "\t<Configuration>" + obstacle.obstacleData + "</Configuration>\r\n";

		rawdata += tabs + "</Obstacle>\r\n";

		return rawdata;
	}

	public string prefabName = "";
	public Vector2 position;
	public string obstacleData = "";
}

[System.Serializable]
public class CatchingMiceCharacterDefinition
{
    public static CatchingMiceCharacterDefinition FromXML(TinyXmlReader parser)
    {
        CatchingMiceCharacterDefinition character = new CatchingMiceCharacterDefinition();

        if ((parser.tagType != TinyXmlReader.TagType.OPENING) ||
            (parser.tagName != "Character"))
        {
            CatchingMiceLogVisualizer.use.Log("CatchingMiceCharacterDefinition.FromXML(): unexpected tag type or tag name.");
            return null;
        }

        while (parser.Read("Character"))
        {
            if (parser.tagType == TinyXmlReader.TagType.OPENING)
            {
                switch (parser.tagName)
                {
                    case "PrefabName":
                        character.prefabName = parser.content;
                        break;
                    case "Speed":
                        character.speed = float.Parse(parser.content);
                        break;
					case "Position":
						Vector2 coordinates = Vector2.zero;
						while (parser.Read("Position"))
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
						character.position = coordinates;
						break;
                    
                }
            }
        }

        return character;
    }

    public static string ToXML(CatchingMiceCharacterDefinition character, int depth)
    {
        string rawdata = string.Empty;

        if (character == null)
        {
            CatchingMiceLogVisualizer.use.Log("CatchingMiceCharacterDefinition.ToXML(): The character to be serialized is null.");
            return rawdata;
        }

        string tabs = string.Empty;
        for (int i = 0; i < depth; ++i)
        {
            tabs += "\t";
        }

        rawdata += tabs + "<Character>\r\n";
        rawdata += tabs + "\t<PrefabName>" + character.prefabName + "</PrefabName>\r\n";
        rawdata += tabs + "\t<Speed>" + character.speed.ToString() + "</Speed>\r\n";

		rawdata += tabs + "\t<Position>\r\n";
		rawdata += tabs + "\t\t<X>" + character.position.x.ToString() + "</X>\r\n";
		rawdata += tabs + "\t\t<Y>" + character.position.y.ToString() + "</Y>\r\n";
		rawdata += tabs + "\t</Position>\r\n";

        //rawdata += tabs + "\t<SpawnDelay>" + character.spawnDelay.ToString() + "</SpawnDelay>\r\n";
        rawdata += tabs + "</Character>\r\n";

        return rawdata;
    }

    public string prefabName = "";
    public float speed = 0.5f;
	public Vector2 position;
}

[System.Serializable]
public class CatchingMiceFurnitureDefinition
{
    public static CatchingMiceFurnitureDefinition FromXML(TinyXmlReader parser)
    {
        CatchingMiceFurnitureDefinition furniture = new CatchingMiceFurnitureDefinition();

        if ((parser.tagType != TinyXmlReader.TagType.OPENING) ||
            (parser.tagName != "Furniture"))
        {
            CatchingMiceLogVisualizer.use.Log("CatchingMiceFurnitureDefinition.FromXML(): unexpected tag type or tag name.");
            return null;
        }

		while (parser.Read("Furniture"))
        {
            if (parser.tagType == TinyXmlReader.TagType.OPENING)
            {
                switch (parser.tagName)
                {
                    case "PrefabName":
                        furniture.prefabName = parser.content;
                        break;
                    case "Position":
                        Vector2 coordinates = Vector2.zero;
                        while (parser.Read("Position"))
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
                        furniture.position = coordinates;
                        break;
                }
            }
        }

        return furniture;
    }

    public static string ToXML(CatchingMiceFurnitureDefinition furniture, int depth)
    {
        string rawdata = string.Empty;

        if (furniture == null)
        {
            CatchingMiceLogVisualizer.use.Log("CatchingMiceFurnitureDefinition.ToXML(): The furniture to be serialized is null.");
            return rawdata;
        }

        string tabs = string.Empty;
        for (int i = 0; i < depth; ++i)
        {
            tabs += "\t";
        }

		rawdata += tabs + "<Furniture>\r\n";
        rawdata += tabs + "\t<PrefabName>" + furniture.prefabName + "</PrefabName>\r\n";
        rawdata += tabs + "\t<Position>\r\n";
        rawdata += tabs + "\t\t<X>" + furniture.position.x.ToString() + "</X>\r\n";
        rawdata += tabs + "\t\t<Y>" + furniture.position.y.ToString() + "</Y>\r\n";
        rawdata += tabs + "\t</Position>\r\n";
		rawdata += tabs + "</Furniture>\r\n";

        return rawdata;
    }

    public string prefabName;
    public Vector2 position;
}

[System.Serializable]
public class CatchingMiceCheeseDefinition
{
    public static CatchingMiceCheeseDefinition FromXML(TinyXmlReader parser)
    {
        CatchingMiceCheeseDefinition cheese = new CatchingMiceCheeseDefinition();

        if ((parser.tagType != TinyXmlReader.TagType.OPENING) ||
            (parser.tagName != "Cheese"))
        {
            CatchingMiceLogVisualizer.use.Log("CatchingMiceCheeseDefinition.FromXML(): unexpected tag type or tag name.");
            return null;
        }

        while (parser.Read("Cheese"))
        {
            if (parser.tagType == TinyXmlReader.TagType.OPENING)
            {
                switch (parser.tagName)
                {
                    case "PrefabName":
                        cheese.prefabName = parser.content;
                        break;
                    case "Position":
                        Vector2 coordinates = Vector2.zero;
                        while (parser.Read("Position"))
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
                        cheese.position = coordinates;
                        break;
                    case "Stacks":
                        cheese.stacks = int.Parse(parser.content);
                        break;
                }
            }
        }

        return cheese;
    }

    public static string ToXML(CatchingMiceCheeseDefinition tileitem, int depth)
    {
        string rawdata = string.Empty;

        if (tileitem == null)
        {
            CatchingMiceLogVisualizer.use.Log("CatchingMiceCheeseDefinition.ToXML(): The tile item to be serialized is null.");
            return rawdata;
        }

        string tabs = string.Empty;
        for (int i = 0; i < depth; ++i)
        {
            tabs += "\t";
        }

        rawdata += tabs + "<Cheese>\r\n";
        rawdata += tabs + "\t<PrefabName>" + tileitem.prefabName + "</PrefabName>\r\n";
        rawdata += tabs + "\t<Position>\r\n";
        rawdata += tabs + "\t\t<X>" + tileitem.position.x.ToString() + "</X>\r\n";
        rawdata += tabs + "\t\t<Y>" + tileitem.position.y.ToString() + "</Y>\r\n";
        rawdata += tabs + "\t</Position>\r\n";
        rawdata += tabs + "\t<Stacks>" + tileitem.stacks + "</Stacks>\r\n";
        rawdata += tabs + "</Cheese>\r\n";

        return rawdata;
    }

    public string prefabName;
    public Vector2 position;
    public int stacks = 3;
}

[System.Serializable]
public class CatchingMiceHoleDefinition
{
    public static CatchingMiceHoleDefinition FromXML(TinyXmlReader parser)
    {
        CatchingMiceHoleDefinition holeTile = new CatchingMiceHoleDefinition();

        if ((parser.tagType != TinyXmlReader.TagType.OPENING) ||
            (parser.tagName != "HoleItem"))
        {
            CatchingMiceLogVisualizer.use.Log("CatchingMiceHoleDefinition.FromXML(): unexpected tag type or tag name.");
            return null;
        }

        while (parser.Read("HoleItem"))
        {
            if (parser.tagType == TinyXmlReader.TagType.OPENING)
            {
                switch (parser.tagName)
                {
                    case "PrefabName":
                        holeTile.prefabName = parser.content;
                        break;
                    case "HoleID":
                        holeTile.holeId = parser.content;
                        break;
                    case "Position":
                        Vector2 coordinates = Vector2.zero;
                        while (parser.Read("Position"))
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
                        holeTile.position = coordinates;
                        break;
                    case "StartDirection":
                        switch (parser.content)
                        {
                            case "down":
                                holeTile.startDirection = CatchingMiceHole.CharacterDirections.Down;
                                break;
                            case "left":
                                holeTile.startDirection = CatchingMiceHole.CharacterDirections.Left;
                                break;
                            case "up":
                                holeTile.startDirection = CatchingMiceHole.CharacterDirections.Up;
                                break;
                            case "right":
                                holeTile.startDirection = CatchingMiceHole.CharacterDirections.Right;
                                break;
                            case "undefined":
                                holeTile.startDirection = CatchingMiceHole.CharacterDirections.Undefined;
                                break;
                        }
                        break;
                }
            }
        }

        return holeTile;
    }

    public static string ToXML(CatchingMiceHoleDefinition holeTile, int depth)
    {
        string rawdata = string.Empty;

        if (holeTile == null)
        {
            CatchingMiceLogVisualizer.use.Log("CatchingMiceHoleDefinition.ToXML(): The tile item to be serialized is null.");
            return rawdata;
        }

        string tabs = string.Empty;
        for (int i = 0; i < depth; ++i)
        {
            tabs += "\t";
        }
        rawdata += tabs + "<HoleItem>\r\n";
        rawdata += tabs + "\t<PrefabName>" + holeTile.prefabName + "</PrefabName>\r\n";
        rawdata += tabs + "\t<HoleID>" + holeTile.holeId + "</HoleID>\r\n";
        rawdata += tabs + "\t<Position>\r\n";
        rawdata += tabs + "\t\t<X>" + holeTile.position.x.ToString() + "</X>\r\n";
        rawdata += tabs + "\t\t<Y>" + holeTile.position.y.ToString() + "</Y>\r\n";
        rawdata += tabs + "\t</Position>\r\n";
        rawdata += tabs + "\t<StartDirection>";
        switch (holeTile.startDirection)
        {
            case CatchingMiceHole.CharacterDirections.Down:
                rawdata += "down";
                break;
            case CatchingMiceHole.CharacterDirections.Left:
                rawdata += "left";
                break;
            case CatchingMiceHole.CharacterDirections.Up:
                rawdata += "up";
                break;
            case CatchingMiceHole.CharacterDirections.Right:
                rawdata += "right";
                break;
            case CatchingMiceHole.CharacterDirections.Undefined:
            default:
                rawdata += "undefined";
                break;
        }
        rawdata += "</StartDirection>\r\n";
        rawdata += tabs + "</HoleItem>\r\n";

        return rawdata;
    }

    public string prefabName = "";
    public string holeId = "";
    public Vector2 position = Vector2.zero;
    public CatchingMiceHole.CharacterDirections startDirection = CatchingMiceHole.CharacterDirections.Undefined;
}

[System.Serializable]
public class CatchingMiceWaveDefinition
{
    public static CatchingMiceWaveDefinition FromXML(TinyXmlReader parser)
    {
        CatchingMiceWaveDefinition wave = new CatchingMiceWaveDefinition();

        if ((parser.tagType != TinyXmlReader.TagType.OPENING) ||
            (parser.tagName != "Wave"))
        {
            CatchingMiceLogVisualizer.use.Log("CatchingMiceWaveDefinition.FromXML(): unexpected tag type or tag name.");
            return null;
        }
        List<CatchingMiceEnemyDefinition> enemies = new List<CatchingMiceEnemyDefinition>();

        while (parser.Read("Wave"))
        {
            if (parser.tagType == TinyXmlReader.TagType.OPENING)
            {
                switch (parser.tagName)
                {
                    case "Enemy":
                        enemies.Add(CatchingMiceEnemyDefinition.FromXML(parser));
                        break;
                }
            }
        }

		wave.enemies = enemies.ToArray();

        return wave;
    }

    public static string ToXML(CatchingMiceWaveDefinition wave, int depth)
    {
        string rawdata = string.Empty;

        if (wave == null)
        {
            CatchingMiceLogVisualizer.use.Log("CatchingMiceWaveDefinition.ToXML(): The tile item to be serialized is null.");
            return rawdata;
        }

        string tabs = string.Empty;
        for (int i = 0; i < depth; ++i)
        {
            tabs += "\t";
        }

        rawdata += tabs + "<Wave>\r\n";
        foreach (CatchingMiceEnemyDefinition enemy in wave.enemies)
        {
            rawdata += CatchingMiceEnemyDefinition.ToXML(enemy, 2);
        }
        rawdata += tabs + "</Wave>\r\n";

        return rawdata;
    }

    public CatchingMiceEnemyDefinition[] enemies;
}

[System.Serializable]
public class CatchingMiceEnemyDefinition
{
    public static CatchingMiceEnemyDefinition FromXML(TinyXmlReader parser)
    {
        CatchingMiceEnemyDefinition enemy = new CatchingMiceEnemyDefinition();

        if ((parser.tagType != TinyXmlReader.TagType.OPENING) ||
            (parser.tagName != "Enemy"))
        {
            CatchingMiceLogVisualizer.use.Log("CatchingMiceEnemyDefinition.FromXML(): unexpected tag type or tag name.");
            return null;
        }
        while (parser.Read("Enemy"))
        {
            if (parser.tagType == TinyXmlReader.TagType.OPENING)
            {
                switch (parser.tagName)
                {
                    case "PrefabName":
                        enemy.prefabName = parser.content;
                        break;
                    case "HoleID":
                        enemy.holeId = parser.content;
                        break;
                    case "Amount":
                        enemy.amount = int.Parse(parser.content);
                        break;
                    case "SpawnTimeInterval":
                        enemy.spawnTimeInterval = float.Parse(parser.content);
                        break;
                    case "SpawnDelay":
                        enemy.spawnDelay = float.Parse(parser.content);
                        break;
                }
            }
        }
        return enemy;
    }

    public static string ToXML(CatchingMiceEnemyDefinition character, int depth)
    {
        string rawdata = string.Empty;

        if (character == null)
        {
            CatchingMiceLogVisualizer.use.Log("CatchingMiceEnemyDefinition.ToXML(): The character to be serialized is null.");
            return rawdata;
        }

        string tabs = string.Empty;
        for (int i = 0; i < depth; ++i)
        {
            tabs += "\t";
        }

        rawdata += tabs + "<Enemy>\r\n";
        rawdata += tabs + "\t<PrefabName>" + character.prefabName + "</PrefabName>\r\n";
        rawdata += tabs + "\t<HoleID>" + character.holeId + "</HoleID>\r\n";
        rawdata += tabs + "\t<Amount>" + character.amount + "</Amount>\r\n";
        rawdata += tabs + "\t<SpawnTimeInterval>" + character.spawnTimeInterval + "</SpawnTimeInterval>\r\n";
        rawdata += tabs + "\t<SpawnDelay>" + character.spawnDelay + "</SpawnDelay>\r\n";
        rawdata += tabs + "</Enemy>\r\n";

        return rawdata;
    }

    public string prefabName = "";
    public string holeId = "";
    public int amount = 1;
    public float spawnTimeInterval = 0.5f;
    public float spawnDelay = 0.0f;
}

[System.Serializable]
public class CatchingMiceTrapDefinition
{
	public static CatchingMiceTrapDefinition FromXML(TinyXmlReader parser)
	{
		CatchingMiceTrapDefinition trap = new CatchingMiceTrapDefinition();

		if ((parser.tagType != TinyXmlReader.TagType.OPENING) ||
			(parser.tagName != "Trap"))
		{
			CatchingMiceLogVisualizer.use.Log("CatchingMiceTrapDefinition.FromXML(): unexpected tag type or tag name.");
			return null;
		}

		while (parser.Read("Trap"))
		{
			if (parser.tagType == TinyXmlReader.TagType.OPENING)
			{
				switch (parser.tagName)
				{
					case "PrefabName":
						trap.prefabName = parser.content;
						break;
					case "Position":
						Vector2 coordinates = Vector2.zero;
						while (parser.Read("Position"))
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
						trap.position = coordinates;
						break;
					case "Stacks":
						trap.stacks = int.Parse(parser.content);
						break;
				}
			}
		}

		return trap;
	}

	public static string ToXML(CatchingMiceTrapDefinition trap, int depth)
	{
		string rawdata = string.Empty;

		if (trap == null)
		{
			CatchingMiceLogVisualizer.use.Log("CatchingMiceTrapDefinition.ToXML(): The trap item to be serialized is null.");
			return rawdata;
		}

		string tabs = string.Empty;
		for (int i = 0; i < depth; ++i)
		{
			tabs += "\t";
		}

		rawdata += tabs + "<Trap>\r\n";

		rawdata += tabs + "\t<PrefabName>" + trap.prefabName + "</PrefabName>\r\n";
		rawdata += tabs + "\t<Position>\r\n";
		rawdata += tabs + "\t\t<X>" + trap.position.x.ToString() + "</X>\r\n";
		rawdata += tabs + "\t\t<Y>" + trap.position.y.ToString() + "</Y>\r\n";
		rawdata += tabs + "\t</Position>\r\n";
		rawdata += tabs + "\t<Stacks>" + trap.stacks + "</Stacks>\r\n";

		rawdata += tabs + "</Trap>\r\n";

		return rawdata;
	}

	public string prefabName = "";
	public Vector2 position = Vector2.zero;
	public int stacks = 1;
}

[System.Serializable]
public class CatchingMicePatrolDefinition
{
	public static CatchingMicePatrolDefinition FromXML(TinyXmlReader parser)
	{
		CatchingMicePatrolDefinition patrol = new CatchingMicePatrolDefinition();

		List<Vector2> positions = new List<Vector2>();

		if ((parser.tagType != TinyXmlReader.TagType.OPENING) ||
			(parser.tagName != "Patrol"))
		{
			CatchingMiceLogVisualizer.use.Log("CatchingMicePatrolDefinition.FromXML(): unexpected tag type or tag name.");
			return null;
		}

		while (parser.Read("Patrol"))
		{
			if (parser.tagType == TinyXmlReader.TagType.OPENING)
			{
				switch (parser.tagName)
				{
					case "PrefabName":
						patrol.prefabName = parser.content;
						break;
					case "Speed":
						patrol.speed = float.Parse(parser.content);
						break;
					case "Position":
						Vector2 coordinates = Vector2.zero;
						while (parser.Read("Position"))
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
						positions.Add(coordinates);
						break;
				}
			}
		}

		patrol.positions = positions.ToArray();

		return patrol;
	}

	public static string ToXML(CatchingMicePatrolDefinition patrol, int depth)
	{
		string rawdata = string.Empty;

		if (patrol == null)
		{
			CatchingMiceLogVisualizer.use.Log("CatchingMicePatrolDefinition.ToXML(): The patrol route to be serialized is null.");
			return rawdata;
		}

		string tabs = string.Empty;
		for (int i = 0; i < depth; ++i)
		{
			tabs += "\t";
		}

		rawdata += tabs + "<Patrol>\r\n";

		rawdata += tabs + "\t<PrefabName>" + patrol.prefabName + "</PrefabName>\r\n";
		rawdata += tabs + "\t<Speed>" + patrol.speed + "</Speed>\r\n";

		rawdata += tabs + "\t<Positions>\r\n";
		foreach (Vector2 position in patrol.positions)
		{
			rawdata += tabs + "\t\t<Position>\r\n";
			rawdata += tabs + "\t\t\t<X>" + position.x.ToString() + "</X>\r\n";
			rawdata += tabs + "\t\t\t<Y>" + position.y.ToString() + "</Y>\r\n";
			rawdata += tabs + "\t\t</Position>\r\n";
		}
		rawdata += tabs + "\t</Positions>\r\n";

		rawdata += tabs + "</Patrol>\r\n";

		return rawdata;
	}

	public string prefabName = "";
	public float speed = 0.5f;
	public Vector2[] positions;
}