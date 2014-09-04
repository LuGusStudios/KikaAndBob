using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DartsCrossSceneInfo : MonoBehaviour, IMinigameCrossSceneInfo
{	
	public static DartsCrossSceneInfo use	
	{
		get
		{
			DartsCrossSceneInfo info = (DartsCrossSceneInfo) GameObject.FindObjectOfType ( typeof(DartsCrossSceneInfo) );
			if( info == null )
			{
				GameObject container = new GameObject("DartsCrossSceneInfo");
				info = container.AddComponent<DartsCrossSceneInfo>();
				
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
			Debug.Log("DartsCrossSceneInfo: Scene index was > 0, but we changed games.");
			levelToLoad = -1;
		}
		
		return levelToLoad;
	}

}
