using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanCrossSceneInfo : MonoBehaviour, IMinigameCrossSceneInfo 
{

	public static PacmanCrossSceneInfo use	
	{
		get
		{
			PacmanCrossSceneInfo info = (PacmanCrossSceneInfo) GameObject.FindObjectOfType ( typeof(PacmanCrossSceneInfo) );
			if( info == null )
			{
				GameObject container = new GameObject("PacmanCrossSceneInfo");
				info = container.AddComponent<PacmanCrossSceneInfo>();
				
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
			Debug.Log("PacmanCrossSceneInfo: Scene index was > 0, but we changed games.");
			levelToLoad = -1;
		}
		
		return levelToLoad;
	}

}

