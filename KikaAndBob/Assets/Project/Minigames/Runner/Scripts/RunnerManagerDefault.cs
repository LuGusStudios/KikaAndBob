using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KikaAndBob
{
	public enum RunnerGameType
	{
		NONE = -1,

		Endless = 1,
		Distance = 2
	}
}

public class RunnerManager : LugusSingletonExisting<RunnerManagerDefault> 
{

}

public class RunnerManagerDefault : IGameManager
{
	public KikaAndBob.RunnerGameType gameType = KikaAndBob.RunnerGameType.NONE;

	public float startTime = 0.0f;
	public float timeSpent = 0.0f;
	public int pickupCount = 0;
	public int lifeCount = 0;
	public float targetDistance = -1.0f;
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

	public void AddLives(int amount)
	{
		lifeCount += amount;

		if( lifeCount <= 0 )
		{
			StopGame();
		}
		//Debug.LogError("ADD LIVES " + amount + " -> " + lifeCount);
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

	public float TraveledDistance
	{
		get
		{
			bool horizontal = ( RunnerInteractionManager.use.direction == RunnerInteractionManager.Direction.EAST ||
			                   RunnerInteractionManager.use.direction == RunnerInteractionManager.Direction.WEST );
			
			MonoBehaviour character = RunnerCharacterController.useBehaviour;

			if( horizontal )
			{
				return distanceTraveledStore + Mathf.Abs( character.transform.position.x - characterReferencePosition.x );
			}
			else
			{
				return distanceTraveledStore + Mathf.Abs( character.transform.position.y - characterReferencePosition.y );
			}
		}
	}

	protected bool finishLinePositioned = false;

	public Vector3 characterReferencePosition = Vector3.zero;
	public float distanceTraveledStore = 0.0f;
	public IEnumerator DistanceRoutine()
	{		
		if( targetDistance < 0.0f || this.gameType != KikaAndBob.RunnerGameType.Distance )
		{
			yield break;
		}
		
		MonoBehaviour character = RunnerCharacterController.useBehaviour;
		characterReferencePosition = character.transform.position;


		float distance = 0.0f;
		IHUDElement visualizer = HUDManager.use.GetElementForCommodity(KikaAndBob.CommodityType.Distance);
		
		bool horizontal = ( RunnerInteractionManager.use.direction == RunnerInteractionManager.Direction.EAST ||
		                   RunnerInteractionManager.use.direction == RunnerInteractionManager.Direction.WEST );

		while( distance < targetDistance )
		{
			distance = TraveledDistance;

			visualizer.SetValue( distance, false );

			if( !finishLinePositioned && !horizontal )
			{
				// vertical: show finish line if almost there!
				if( targetDistance - distance < (LugusUtil.UIHeight))
				{
					finishLinePositioned = true;
					allowShifting = false;

					GameObject finishLine = GameObject.Find ("FinishLine01");

					// SOUTH
					float yPos = -1.0f * LugusUtil.UIHeight + 2.0f; // +2f so we stop when kika is halfway in the line instead of before
					if( RunnerInteractionManager.use.direction == RunnerInteractionManager.Direction.NORTH )
					{
						// NORTH
						yPos = LugusUtil.UIHeight;// - 2.0f; // +2f so we stop when kika is halfway in the line instead of before
					}

					finishLine.transform.position = character.transform.position.x (0.0f).yAdd( yPos ); 

					//Debug.LogError ("POSITIONING FINISHLINE " + finishLine.transform.position );

					//finishLine.transform.parent = LayerManager.use.groundLayer.currentSection.transform;
				}
			}

			yield return null;
		}

		if( GameRunning )
		{
			StopGame();
		}

		yield break;
	}


	// if the camera reaches this x value, the whole level is shifted to the left again
	// this is to prevent reaching very high x values (which float precision does not like)
	protected float shiftXTreshold = 500.0f; // TODO: make this larger and test (should be a value of around 1000.0f in production)
	protected float shiftYTreshold = 500.0f;
	protected LevelLoaderDefault levelLoader = new LevelLoaderDefault();

	protected bool allowShifting = true;


	// if horizontal == false -> it's vertical
	public void ShiftLevel(float units, bool horizontal)
	{
		if( !allowShifting )
			return;

		MonoBehaviour character = RunnerCharacterController.useBehaviour;
		if( horizontal )
			distanceTraveledStore += Mathf.Abs(character.transform.position.x - characterReferencePosition.x);
		else
			distanceTraveledStore += Mathf.Abs(character.transform.position.y - characterReferencePosition.y);



		// TODO: make this decent... this is kind of hacky with the layer list etc.

		FollowCameraContinuous camera = LugusCamera.game.GetComponent<FollowCameraContinuous>();
		Vector3 cameraOriginal = camera.transform.position;
		float xOffset = camera.character.transform.position.x - camera.transform.position.x;
		float yOffset = camera.character.transform.position.y - camera.transform.position.y;

		
		Debug.Log ("Shifting level " + units + " units (horizontal=" + horizontal + "). Cam offset x : " + xOffset + ", y : " + yOffset);

		List<string> layers = new List<string>();
		layers.Add ("LayerGround");
		layers.Add ("LayerSky");
		layers.Add ("LayerFront");
		layers.Add ("Character");

		if( character is RunnerCharacterControllerClimbing )
		{
			layers.Add ("CameraPuller"); 
		}

		foreach( string layer in layers )
		{
			GameObject layerObj = GameObject.Find ( layer );
			//if( layerObj == null )
			//	Debug.LogWarning("LAYER " + layer + " WAS NULL!");

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
		
		characterReferencePosition = character.transform.position;
	}

	public void OnDisable()
	{
		// reset the singleton-connections
		// otherwhise, we get wrong references on scene re-start
		RunnerCharacterController.Reset();
	}

	protected void LateUpdate()
	{
		// TODO: FIXME: make this work for top and left moving levels as well!!

		// EAST
		if( LugusCamera.game.transform.position.x > shiftXTreshold /*|| LugusInput.use.KeyDown(KeyCode.R)*/ )
		{
			ShiftLevel( -1 * LugusCamera.game.transform.position.x, true ); // shift back to 0.0f
		}

		// SOUTH

		if( LugusCamera.game.transform.position.y < -shiftYTreshold /*|| LugusInput.use.KeyDown(KeyCode.R)*/ )
		{
			ShiftLevel( -1 * LugusCamera.game.transform.position.y, false ); // shift back to 0.0f
		}


		// NORTH // THIS DOES NOT YET SEEM TO WORK... (in Brazil only?) WEIRD
		if( LugusCamera.game.transform.position.y > shiftYTreshold || LugusInput.use.KeyDown(KeyCode.R) )
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
		//if( RunnerCrossSceneInfo.use.levelToLoad < 0 )
		//	RunnerCrossSceneInfo.use.levelToLoad = 667;
		#endif

		AudioClip background = LugusResources.use.Shared.GetAudio(Application.loadedLevelName + "_background");
		if( background != LugusResources.use.errorAudio ) 
		{
			LugusAudio.use.Music().Play(background, true, new LugusAudioTrackSettings().Loop(true).Volume(0.5f));
		}

		
		if( RunnerCrossSceneInfo.use.levelToLoad < 0 )
		{
			MonoBehaviour cc = RunnerCharacterController.useBehaviour;
			cc.gameObject.SetActive(false);
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
		LugusCoroutines.use.StartRoutine( DistanceRoutine() ); // timeout is possibly set by LoadLevel, so start the routine!

		if( this.gameType == KikaAndBob.RunnerGameType.NONE )
		{
			Debug.LogError(transform.Path () + " : No RunnerGameType set! Config should do this!!!");
		}
	}

	public override void StopGame() 
	{
		_gameRunning = false;



		RunnerCharacterController.useBehaviour.enabled = false;
		RunnerCharacterController.useBehaviour.rigidbody2D.isKinematic = true;
		RunnerCharacterController.useBehaviour.GetComponent<RunnerCharacterAnimator>().StopAll();
		RunnerCharacterController.useBehaviour.GetComponent<RunnerCharacterAnimator>().enabled = false;

		RunnerInteractionManager.use.Deactivate();

		IRunnerConfig.use.OnGameStopped();
		
		HUDManager.use.StopAll();
		DialogueManager.use.HideAll();
		HUDManager.use.PauseButton.gameObject.SetActive(false);
	

		LugusCoroutines.use.StartRoutine( ScoreAnimationRoutine() );
		
		//float moneyScore = ((HUDCounter)HUDManager.use.GetElementForCommodity(KikaAndBob.CommodityType.Money)).currentValue;
		bool success = true;


		// TODO: move pickups towards time at the end of showing pickups (Extra coroutine needed)
		
		if( success )
		{
			
			Debug.Log ("Runner : set level success : " + (Application.loadedLevelName + "_level_" + RunnerCrossSceneInfo.use.levelToLoad) );
			LugusConfig.use.User.SetBool( Application.loadedLevelName + "_level_" + RunnerCrossSceneInfo.use.levelToLoad, true, true );
			LugusConfig.use.SaveProfiles();
		}
		
		
		Debug.Log ("Stopping Runner game " + (timeSpent + (Time.time - startTime)) + " - " + pickupCount);
	}

	public int pickupsPerSecondConversion = 1; // how many pickups a user should score before he gets 1 second added or subtracted from his timescore

	protected IEnumerator ScoreAnimationRoutine()
	{
		// 1. let time and pickups count up individually
		// 2. move time over to score (in 1 time)
		// 3. move pickups over to score (in groups of x, where x = number of pickups to warrant 1 second extra score)
		HUDManager.use.LevelEndScreen.Counter1.gameObject.SetActive(true);
		HUDManager.use.LevelEndScreen.Counter1.commodity = KikaAndBob.CommodityType.Time;
		HUDManager.use.LevelEndScreen.Counter1.formatting = HUDCounter.Formatting.Int;
		HUDManager.use.LevelEndScreen.Counter1.suffix = "s";

		HUDManager.use.LevelEndScreen.Counter2.gameObject.SetActive(true);
		HUDManager.use.LevelEndScreen.Counter2.commodity = KikaAndBob.CommodityType.Feather;
		
		HUDManager.use.LevelEndScreen.Counter6.gameObject.SetActive(true);
		HUDManager.use.LevelEndScreen.Counter6.commodity = KikaAndBob.CommodityType.Score;
		HUDManager.use.LevelEndScreen.Counter6.SetValue(0);

		float timeScore = Mathf.FloorToInt(timeSpent + (Time.time - startTime));


		HUDManager.use.LevelEndScreen.Show(true);
		HUDManager.use.LevelEndScreen.Counter1.SetValue( timeScore, true );
		HUDManager.use.LevelEndScreen.Counter2.SetValue( pickupCount, true );

		yield return new WaitForSeconds( Mathf.Max(HUDManager.use.LevelEndScreen.Counter1.animationTime, HUDManager.use.LevelEndScreen.Counter2.animationTime) + 1.0f );

		ScoreVisualizer
				.Score(KikaAndBob.CommodityType.Time, timeScore) 
				.Position( HUDManager.use.LevelEndScreen.Counter1.transform.position )
				.HUDElement( HUDManager.use.LevelEndScreen.Counter6 )
				.UseGUICamera(true)
				.Time (1.5f)
				.Audio("Blob01")
				.Execute();

		yield return new WaitForSeconds( 1.5f + 1.0f);

		float timePerItem = 0.1f;
		float maxTotalTime = 3.0f;
		if( pickupCount * timePerItem > maxTotalTime )
			timePerItem = maxTotalTime / pickupCount;

		int convertedPickupCount = Mathf.FloorToInt(pickupCount / pickupsPerSecondConversion);

		for( int i = 0; i < convertedPickupCount; ++i )
		{

			HUDManager.use.LevelEndScreen.Counter2.AddValue( -pickupsPerSecondConversion, true );

			float score = -1.0f; // DISTANCE = pickups give lower time = better
			if( gameType == KikaAndBob.RunnerGameType.Endless )
			{
				// ENDLESS: picups give more time (longer untill death) = better
				score = 1.0f;
			}
			 

			Vector3 position = HUDManager.use.LevelEndScreen.Counter2.transform.position.xAdd ( Random.Range(-1.0f, 1.0f) ).yAdd( 0.3f );

			ScoreVisualizer
				.Score(KikaAndBob.CommodityType.Time, score )
				.Position( position )
				.HUDElement( HUDManager.use.LevelEndScreen.Counter6 )
				.UseGUICamera(true)
				.Time (1.0f)
				.Audio("Blob01")
				.MinValue(0)
				.Execute();

			// sometimes we get stuck on 1 or 2 (because of the FloorToInt)
			if( i == (convertedPickupCount - 1) )
			{
				HUDManager.use.LevelEndScreen.Counter2.SetValue(0);
			}

			yield return new WaitForSeconds( timePerItem );
		}

		yield break;

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
		/*
		if( LugusInput.use.Key( KeyCode.S) )
		{
			timeSpent = Random.Range(50, 100);
			startTime = Time.time - Random.Range(50, 100);

			pickupCount = Random.Range(10, 50);

			StopGame();
		}  
		*/
	}

	protected void OnGUI()
	{
		if( !LugusDebug.debug )
			return;


		bool horizontal = ( RunnerInteractionManager.use.direction == RunnerInteractionManager.Direction.EAST ||
		                   RunnerInteractionManager.use.direction == RunnerInteractionManager.Direction.WEST );
		
		MonoBehaviour character = RunnerCharacterController.useBehaviour;

		float distance = 0.0f;
		if( horizontal )
		{
			distance = distanceTraveledStore + Mathf.Abs( character.transform.position.x );
		}
		else
		{
			distance = distanceTraveledStore + Mathf.Abs( character.transform.position.y );
		}

		GUI.Button( new Rect( Screen.width / 2.0f - 50, 20, 100, 40 ),  "DIST: " + distance );
	}
}
