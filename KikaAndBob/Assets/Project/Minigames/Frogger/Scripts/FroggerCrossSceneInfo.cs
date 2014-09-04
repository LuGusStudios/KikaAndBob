using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FroggerCrossSceneInfo : MonoBehaviour, IMinigameCrossSceneInfo 
{
	public static FroggerCrossSceneInfo use	
	{
		get
		{
			FroggerCrossSceneInfo info = (FroggerCrossSceneInfo) GameObject.FindObjectOfType ( typeof(FroggerCrossSceneInfo) );
			if( info == null )
			{
				GameObject container = new GameObject("FroggerCrossSceneInfo");
				info = container.AddComponent<FroggerCrossSceneInfo>();
				
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
			Debug.Log("FroggerCrossSceneInfo: Scene index was > 0, but we changed games.");
			levelToLoad = -1;
		}
		
		return levelToLoad;
	}

}

