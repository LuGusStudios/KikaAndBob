using UnityEngine;
using System.Collections;

public class MainCrossSceneInfo : MonoBehaviour {

	public static MainCrossSceneInfo use
	{
		get
		{
			MainCrossSceneInfo info = (MainCrossSceneInfo)GameObject.FindObjectOfType(typeof(MainCrossSceneInfo));
			if (info == null)
			{
				GameObject container = new GameObject("MainCrossSceneInfo");
				info = container.AddComponent<MainCrossSceneInfo>();
				
				DontDestroyOnLoad(container);
			}
			
			return info;
		}
	}

	public string lastLoadedGameLevel = "";
}
