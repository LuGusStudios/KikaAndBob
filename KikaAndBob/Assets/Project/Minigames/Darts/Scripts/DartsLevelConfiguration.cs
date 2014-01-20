using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DartsLevelConfiguration : LugusSingletonRuntime<DartsLevelConfiguration>
{
	public DartsLevelDefinition[] levels;
	protected DartsFunctionalityGroup[] groups;

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
		ConfigureLevel(0);
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
