using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DanceHeroCrossSceneInfo : MonoBehaviour, IMinigameCrossSceneInfo 
{
	public static DanceHeroCrossSceneInfo use	
	{
		get
		{
			DanceHeroCrossSceneInfo info = (DanceHeroCrossSceneInfo) GameObject.FindObjectOfType ( typeof(DanceHeroCrossSceneInfo) );
			if( info == null )
			{
				GameObject container = new GameObject("DanceHeroCrossSceneInfo");
				info = container.AddComponent<DanceHeroCrossSceneInfo>();
				
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
			Debug.Log("DanceHeroCrossSceneInfo: Scene index was > 0, but we changed games.");
			levelToLoad = -1;
		}
		
		return levelToLoad;
	}

}

