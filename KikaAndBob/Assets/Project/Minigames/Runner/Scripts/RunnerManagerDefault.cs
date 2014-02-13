using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunnerManager : LugusSingletonExisting<RunnerManagerDefault> 
{

}

public class RunnerManagerDefault : IGameManager
{
	public float startTime = 0.0f;
	public float timeSpent = 0.0f;
	public int pickupCount = 0;
	//public float targetScore = -1.0f;
	public float timeout = -1.0f;

	
	protected bool _gameRunning = false;
	public override bool GameRunning
	{
		get{ return _gameRunning; }
	}


	public void AddTime(float amount)
	{
		timeSpent += amount;
	}

	public void AddPickup(int amount)
	{
		pickupCount += amount;
	}


	public IEnumerator TimeoutRoutine()
	{
		if( timeout < 0.0f )
		{
			// no timeout set (for example for tutorial levels)
			// so skip this function
			yield break;
		}
		
		yield return new WaitForSeconds(timeout);
		
		if( GameRunning )
		{
			StopGame();
		}
	}



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

	public void OnDisable()
	{
		// reset the singleton-connections
		// otherwhise, we get wrong references on scene re-start
		RunnerCharacterController.Reset();
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
		
		levelLoader.FindLevels(); 
		
		// DEBUG: TODO: REMOVE THIS! just so we can directly play when starting in editor
		#if UNITY_EDITOR
		if( RunnerCrossSceneInfo.use.levelToLoad < 0 )
			RunnerCrossSceneInfo.use.levelToLoad = 1;
		#endif
		
		if( RunnerCrossSceneInfo.use.levelToLoad < 0 )
		{
			( (MonoBehaviour) RunnerCharacterController.use).gameObject.SetActive(false);
			MenuManager.use.ActivateMenu(MenuManagerDefault.MenuTypes.GameMenu);
		}
		else
		{
			MenuManager.use.ActivateMenu(MenuManagerDefault.MenuTypes.NONE);

			StartGame();
		}


	}

	public override void StartGame()
	{
		_gameRunning = true;
		Debug.Log ("Starting Runner game");

		startTime = Time.time;

		IRunnerConfig.use.LoadLevel( RunnerCrossSceneInfo.use.levelToLoad );

		RunnerInteractionManager.use.StartTimer();
	
		LugusCoroutines.use.StartRoutine( TimeoutRoutine() ); // timeout is possibly set by LoadLevel, so start the routine!
	}

	public override void StopGame() 
	{
		_gameRunning = false;
		
		HUDManager.use.StopAll();
		DialogueManager.use.HideAll();
	

		HUDManager.use.LevelEndScreen.Counter1.gameObject.SetActive(true);
		HUDManager.use.LevelEndScreen.Counter1.commodity = KikaAndBob.CommodityType.Time;
		HUDManager.use.LevelEndScreen.Counter2.gameObject.SetActive(true);
		HUDManager.use.LevelEndScreen.Counter2.commodity = KikaAndBob.CommodityType.Feather;
		
		//float moneyScore = ((HUDCounter)HUDManager.use.GetElementForCommodity(KikaAndBob.CommodityType.Money)).currentValue;
		bool success = true;

		HUDManager.use.LevelEndScreen.Show(success);
		HUDManager.use.LevelEndScreen.Counter1.SetValue( timeSpent + (Time.time - startTime), true );
		HUDManager.use.LevelEndScreen.Counter2.SetValue( pickupCount, true );

		// TODO: move pickups towards time at the end of showing pickups (Extra coroutine needed)
		
		if( success )
		{
			
			Debug.Log ("Runner : set level success : " + (Application.loadedLevelName + "_level_" + RunnerCrossSceneInfo.use.levelToLoad) );
			LugusConfig.use.User.SetBool( Application.loadedLevelName + "_level_" + RunnerCrossSceneInfo.use.levelToLoad, true, true );
			LugusConfig.use.SaveProfiles();
		}
		
		
		Debug.Log ("Stopping Runner game " + (timeSpent + (Time.time - startTime)) + " - " + pickupCount);
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
