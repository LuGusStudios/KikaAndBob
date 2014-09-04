using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunnerCrossSceneInfo : MonoBehaviour, IMinigameCrossSceneInfo 
{
	public static RunnerCrossSceneInfo use	
	{
		get
		{
			RunnerCrossSceneInfo info = (RunnerCrossSceneInfo) GameObject.FindObjectOfType ( typeof(RunnerCrossSceneInfo) );
			if( info == null )
			{
				GameObject container = new GameObject("RunnerCrossSceneInfo");
				info = container.AddComponent<RunnerCrossSceneInfo>();
				
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
			Debug.Log("RunnerCrossSceneInfo: Scene index was > 0, but we changed games.");
			levelToLoad = -1;
		}
		
		return levelToLoad;
	}

}

