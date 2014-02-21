using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DartsLevelConfiguration : LugusSingletonRuntime<DartsLevelConfigurationDefault>
{

}

public class DartsLevelConfigurationDefault :  IGameManager
{
	public DartsLevelDefinition[] levels;
	protected DartsFunctionalityGroup[] groups;
	protected int currentIndex = 0;
	protected LevelLoaderDefault levelLoader = new LevelLoaderDefault();
	protected bool gameRunning = false;

	public void SetupLocal()
	{
		groups = (DartsFunctionalityGroup[])FindObjectsOfType(typeof(DartsFunctionalityGroup));
	}
	
	public void SetupGlobal()
	{
		levelLoader.FindLevels();
	}

	public override bool GameRunning {
		get 
		{
			return gameRunning;
		}
	}

	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();

		if (DartsCrossSceneInfo.use.GetLevelIndex() < 0)
		{
			MenuManager.use.ActivateMenu(MenuManagerDefault.MenuTypes.GameMenu);
		}
		else
		{
			MenuManager.use.ActivateMenu(MenuManagerDefault.MenuTypes.NONE);
			StartGame();
		}
	}

	public override void StartGame ()
	{
		string levelData = levelLoader.GetLevelData(DartsCrossSceneInfo.use.GetLevelIndex());
		
		if (!string.IsNullOrEmpty(levelData))
		{
			gameRunning = true;
			DartsLevelDefinition newLevel = DartsLevelDefinition.FromXML(levelData);
			ConfigureLevel(newLevel);
		}
		else
		{
			Debug.LogError("DartsLevelConfiguration: Invalid level data!");
		}
	}

	public override void StopGame ()
	{
		gameRunning = false;
	}
	
	protected void Update () 
	{
	
	}

	public void ConfigureLevel(int index)
	{
		if (levels == null || levels.Length <= 0)
		{
			Debug.LogError("Level array was null or empty.");
			return;
		}
		
		if (index >= levels.Length || index < 0)
		{
			Debug.LogError("Level index out of bounds.");
			return;
		}
		
		Debug.Log("Loading level: " + index);
		
		DartsLevelDefinition level = levels[index];

		ConfigureLevel(level);
	}

	public void ConfigureLevel(DartsLevelDefinition level)
	{
		HUDManager.use.CounterLargeLeft1.gameObject.SetActive(true);
		HUDManager.use.CounterLargeLeft1.commodity = KikaAndBob.CommodityType.Score;
		HUDManager.use.CounterLargeLeft1.formatting = HUDCounter.Formatting.Int;
		HUDManager.use.CounterLargeLeft1.SetValue(0, false);

		// first disable groups
		foreach (DartsFunctionalityGroup group in groups) 
		{
			group.SetEnabled(false);
		}
		
		foreach(DartsGroupDefinition groupDefinition in level.groupDefinitions)
		{
			DartsFunctionalityGroup foundGroup = null;
			
			foreach (DartsFunctionalityGroup group in groups) 
			{
				if (group.gameObject.name == groupDefinition.id)
				{
					foundGroup = group;
					break;
				}
			}
			
			if (foundGroup == null)
			{
				Debug.LogError("Level id: " + groupDefinition.id + " not found.");
				continue;
			}
			
			foundGroup.SetEnabled(true);
			foundGroup.itemsOnScreen = groupDefinition.itemsOnScreen;
			foundGroup.minTimeBetweenShows = groupDefinition.minTimeBetweenShows;
			foundGroup.autoHideTimes = groupDefinition.autoHideTimes;
			foundGroup.avoidRepeat = groupDefinition.avoidRepeat;
		}
	}



	protected void OnGUI()
	{
		if (!LugusDebug.debug)
			return;

		foreach(int index in levelLoader.levelIndices)
		{
			if (GUILayout.Button("Load level: " + index))
			{
				levelLoader.LoadLevel(index);
			}
		}	
	}
}
