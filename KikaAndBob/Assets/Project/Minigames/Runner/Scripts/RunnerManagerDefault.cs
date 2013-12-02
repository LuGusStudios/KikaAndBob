using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunnerManager : LugusSingletonExisting<RunnerManagerDefault> 
{

}

public class RunnerManagerDefault : MonoBehaviour
{
	// if the camera reaches this x value, the whole level is shifted to the left again
	// this is to prevent reaching very high x values (which float precision does not like)
	protected float shiftXTreshold = 1000.0f; // TODO: make this larger and test (should be a value of around 1000.0f in production)

	public void ShiftLevel(float units)
	{
		Debug.Log ("Shifting level " + units + " units");

		FollowCamera camera = LugusCamera.game.GetComponent<FollowCamera>();
		Vector3 cameraOriginal = camera.transform.position;
		float xOffset = camera.character.transform.position.x - camera.transform.position.x;

		List<string> layers = new List<string>();
		layers.Add ("LayerGround");
		layers.Add ("LayerSky");
		layers.Add ("Character");

		foreach( string layer in layers )
		{
			GameObject layerObj = GameObject.Find ( layer );
			if( layerObj.transform.childCount > 0 )
			{
				foreach( Transform child in layerObj.transform )
				{
					child.transform.position = child.transform.position.xAdd( units );
				}
			}
			else
			{
				layerObj.transform.position = layerObj.transform.position.xAdd( units );
			}
		}

		camera.transform.position = cameraOriginal.x( camera.character.transform.position.x + (-1 * xOffset) );
	}

	protected void LateUpdate()
	{
		if( LugusCamera.game.transform.position.x > shiftXTreshold || LugusInput.use.KeyDown(KeyCode.R) )
		{
			ShiftLevel( -1 * LugusCamera.game.transform.transform.position.x ); // shift back to 0.0f
		}
	}

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}
	
	protected void Update () 
	{
	
	}
}
