using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FroggerCrossSceneInfo : MonoBehaviour, IMinigameCrossSceneInfo 
{
	public int levelToLoad = -1;

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

	public void SetLevelIndex(int index)
	{
		levelToLoad = index;
	}

	public int GetLevelIndex()
	{
		return levelToLoad;
	}
}

