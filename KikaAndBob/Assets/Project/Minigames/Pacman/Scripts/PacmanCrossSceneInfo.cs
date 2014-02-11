using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanCrossSceneInfo : MonoBehaviour, IMinigameCrossSceneInfo 
{
	public int levelToLoad = -1;

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

	public void SetLevelIndex(int index)
	{
		levelToLoad = index;
	}

	public int GetLevelIndex()
	{
		return levelToLoad;
	}
}

