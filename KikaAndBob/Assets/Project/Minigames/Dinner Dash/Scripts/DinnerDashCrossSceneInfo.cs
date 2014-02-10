using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DinnerDashCrossSceneInfo : MonoBehaviour 
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
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

}
