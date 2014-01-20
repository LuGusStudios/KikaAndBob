using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DartsLevelDefinition : ScriptableObject 
{
	public DartsGroupDefinition[] groupDefinitions;

	// Arrays of serialized classes are not created with default values
	// Instead, initialize values once in OnEnable (which runs AFTER deserialization), checking for null / zero value
	// http://forum.unity3d.com/threads/155352-Serialization-Best-Practices-Megapost
	protected void OnEnable()
	{
		
	}

}

[System.Serializable]
public class DartsGroupDefinition
{
	public string id = "";
	public float itemsOnScreen = 1.0f;
	public float minTimeBetweenShows = 1.0f;
	public DataRange autoHideTimes = new DataRange(2.0f, 4.0f);
}
