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
	protected float shiftXTreshold = 500.0f; // TODO: make this larger and test (should be a value of around 1000.0f in production)
	protected float shiftYTreshold = -500.0f;
	protected LevelLoaderDefault levelLoader = new LevelLoaderDefault();

	// if horizontal == false -> it's vertical
	public void ShiftLevel(float units, bool horizontal)
	{
		// TODO: make this decent... this is kind of hacky with the layer list etc.

		FollowCameraContinuous camera = LugusCamera.game.GetComponent<FollowCameraContinuous>();
		Vector3 cameraOriginal = camera.transform.position;
		float xOffset = camera.character.transform.position.x - camera.transform.position.x;
		float yOffset = camera.character.transform.position.y - camera.transform.position.y;

		
		Debug.Log ("Shifting level " + units + " units. Cam offset x : " + xOffset + ", y : " + yOffset);

		List<string> layers = new List<string>();
		layers.Add ("LayerGround");
		layers.Add ("LayerSky");
		layers.Add ("LayerFront");
		layers.Add ("Character");

		foreach( string layer in layers )
		{
			GameObject layerObj = GameObject.Find ( layer );
			if( layerObj.transform.childCount > 0 && layer != "Character" )
			{
				foreach( Transform child in layerObj.transform )
				{
					if( horizontal )
						child.transform.position = child.transform.position.xAdd( units );
					else
						child.transform.position = child.transform.position.yAdd( units );

				}
			}
			else
			{
				if( horizontal )
					layerObj.transform.position = layerObj.transform.position.xAdd( units );
				else
					layerObj.transform.position = layerObj.transform.position.yAdd( units );
			}
		}

		if( horizontal )
		{
			camera.transform.position = cameraOriginal.x( camera.character.transform.position.x + (-1 * xOffset) );
		}
		else
		{
			camera.transform.position = cameraOriginal.y( camera.character.transform.position.y + (-1 * yOffset) );
		}
	}

	protected void LateUpdate()
	{
		// TODO: make this work for top and left moving levels as well!!

		if( LugusCamera.game.transform.position.x > shiftXTreshold || LugusInput.use.KeyDown(KeyCode.R) )
		{
			ShiftLevel( -1 * LugusCamera.game.transform.position.x, true ); // shift back to 0.0f
		}
		
		if( LugusCamera.game.transform.position.y < shiftYTreshold || LugusInput.use.KeyDown(KeyCode.R) )
		{
			ShiftLevel( -1 * LugusCamera.game.transform.position.y, false ); // shift back to 0.0f
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

		levelLoader.FindLevels();
		
		if (RunnerCrossSceneInfo.use.GetLevelIndex() < 0)
		{
			MenuManager.use.ActivateMenu(MenuManagerDefault.MenuTypes.GameMenu);
		}
		else
		{
			MenuManager.use.ActivateMenu(MenuManagerDefault.MenuTypes.NONE);

			// TO DO: Add code for clearing level, setting correct level variables, etc. Or leave that a more specific config loader.
		}
	}
	
	protected void Update () 
	{
	
	}
}
