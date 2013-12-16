using UnityEngine;
using System.Collections;

[System.Serializable]
public class FroggerLevelDefinition : ScriptableObject {

	public FroggerLaneDefinition[] lanes;

	// Arrays of serialized classes are not created with default values
	// Instead, initialize values once in OnEnable (which runs AFTER deserialization), checking for null / zero value
	// http://forum.unity3d.com/threads/155352-Serialization-Best-Practices-Megapost
	void OnEnable()
	{
		if (lanes == null || lanes.Length < 1)
		{
			// Since we always want at least one lane in the Frogger games, it makes sense to initially provide one lane already.
			// This way, the constructor for the array's type will get called, which sets the default values.
			// Further array entries are always copied from the last entry anyway, so any default values are copied.
			Debug.Log("Initializing FroggerLevelDefinition");
			lanes = new FroggerLaneDefinition[1];
		}
	}
}
