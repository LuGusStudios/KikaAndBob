using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunnerCrossSceneInfo : MonoBehaviour, IMinigameCrossSceneInfo 
{
	public int levelToLoad = -1;

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

	public void SetLevelIndex(int index)
	{
		levelToLoad = index;
	}

	public int GetLevelIndex()
	{
		return levelToLoad;
	}
}

