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
	
	public int levelToLoad = -1;

	public void SetLevelIndex(int index)
	{
		levelToLoad = index;
	}
	
	public int GetLevelIndex()
	{
		return levelToLoad;
	}

}
