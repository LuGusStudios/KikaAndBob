using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DartsLevelConfiguration : LugusSingletonRuntime<DartsLevelConfiguration>
{
	public DartsLevelDefinition[] levels;
	protected DartsFunctionalityGroup[] groups;
	protected int currentIndex = 0;
	protected LevelLoaderDefault levelLoader = new LevelLoaderDefault();

	public void SetupLocal()
	{
		groups = (DartsFunctionalityGroup[])FindObjectsOfType(typeof(DartsFunctionalityGroup));
	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	//	ConfigureLevel(0);

		if (DartsCrossSceneInfo.use.GetLevelIndex() < 0)
		{
			MenuManager.use.ActivateMenu(MenuManagerDefault.MenuTypes.GameMenu);
		}
		else
		{
			MenuManager.use.ActivateMenu(MenuManagerDefault.MenuTypes.NONE);

			string levelData = levelLoader.GetLevelData(DartsCrossSceneInfo.use.GetLevelIndex());

			if (!string.IsNullOrEmpty(levelData))
			{
				DartsLevelDefinition newLevel = DartsLevelDefinition.FromXML(levelData);
				ConfigureLevel(newLevel);
			}
			else
			{
				Debug.LogError("DartsLevelConfiguration: Invalid level data!");
			}

			//ConfigureLevel(DartsCrossSceneInfo.use.GetLevelIndex() - 1);
		}
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

		for (int i = 0; i < levels.Length; i++) 
		{
			if (GUILayout.Button("Level " + i))
			{
				ConfigureLevel(i);
			}
		}
	}
}
