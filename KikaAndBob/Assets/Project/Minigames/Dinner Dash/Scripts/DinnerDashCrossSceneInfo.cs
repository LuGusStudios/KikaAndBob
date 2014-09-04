using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DinnerDashCrossSceneInfo : MonoBehaviour, IMinigameCrossSceneInfo 
{
	public static DinnerDashCrossSceneInfo use	
	{
		get
		{
			DinnerDashCrossSceneInfo info = (DinnerDashCrossSceneInfo) GameObject.FindObjectOfType ( typeof(DinnerDashCrossSceneInfo) );
			if( info == null )
			{
				GameObject container = new GameObject("DinnerDashCrossSceneInfo");
				info = container.AddComponent<DinnerDashCrossSceneInfo>();
				
				DontDestroyOnLoad( container );
			}
			
			return info;
		}
	}

	public void Destroy()
	{
		GameObject.Destroy( this.gameObject );
	}
	
	protected int levelToLoad = -1;
	protected string lastLoadedScene = "";


	public void SetLevelIndex(int index)
	{
		lastLoadedScene = Application.loadedLevelName;
		levelToLoad = index;
	}
	
	public int GetLevelIndex()
	{
		if (Application.loadedLevelName != lastLoadedScene)
		{
			Debug.Log("DinnerDashCrossSceneInfo: Scene index was > 0, but we changed games.");
			levelToLoad = -1;
		}

		return levelToLoad;
	}

}
