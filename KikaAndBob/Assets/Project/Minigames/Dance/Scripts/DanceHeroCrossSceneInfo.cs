using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DanceHeroCrossSceneInfo : MonoBehaviour, IMinigameCrossSceneInfo 
{
	public int levelToLoad = -1;

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

	public void SetLevelIndex(int index)
	{
		levelToLoad = index;
	}

	public int GetLevelIndex()
	{
		return levelToLoad;
	}
}

