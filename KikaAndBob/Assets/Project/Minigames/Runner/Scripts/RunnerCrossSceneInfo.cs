using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunnerCrossSceneInfo : MonoBehaviour 
{
	public int levelToLoad = 0;

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
}

