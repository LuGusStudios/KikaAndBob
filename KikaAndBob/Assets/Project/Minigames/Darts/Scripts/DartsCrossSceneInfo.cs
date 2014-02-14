using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DartsCrossSceneInfo : MonoBehaviour, IMinigameCrossSceneInfo
{
	public int levelToLoad = -1;
	
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
	
	public void SetLevelIndex(int index)
	{
		levelToLoad = index;
	}
	
	public int GetLevelIndex()
	{
		return levelToLoad;
	}
}
