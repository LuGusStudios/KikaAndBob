using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAuthCrossSceneInfo : MonoBehaviour
{
	public bool loggedIn = false;
	public string userId = "";
	public JSONObject userDataObj = null;
	public string userDataString = "";

	public bool hasConnection = false;
	public bool checkedAuthentication = false;


	protected static PlayerAuthCrossSceneInfo instance = null;

	public static PlayerAuthCrossSceneInfo use	
	{
		get
		{
			PlayerAuthCrossSceneInfo instance = (PlayerAuthCrossSceneInfo) GameObject.FindObjectOfType ( typeof(PlayerAuthCrossSceneInfo) );
			if( instance == null )
			{
				GameObject container = new GameObject("PlayerAuthCrossSceneInfo");
				instance = container.AddComponent<PlayerAuthCrossSceneInfo>();
				
				DontDestroyOnLoad( container );
			}
			
			return instance;
		}
	}
	
	public void Destroy()
	{
		GameObject.Destroy( this.gameObject );
	}

}
