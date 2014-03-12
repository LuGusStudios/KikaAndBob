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
		if( LugusCamera.game.transform.position.y > shiftYTreshold /*|| LugusInput.use.KeyDown(KeyCode.R)*/ )
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
		// TODO: remove me! jsut for debugging!
		LugusAudio.use.Music().BaseTrackSettings = new LugusAudioTrackSettings().Volume(0.0f);


		// lookup references to objects / scripts outside of this script
		
		levelLoader.FindLevels(); 
		
		// DEBUG: TODO: REMOVE THIS! just so we can directly play when starting in editor
		#if UNITY_EDITOR
		if( RunnerCrossSceneInfo.use.levelToLoad < 0 )
			RunnerCrossSceneInfo.use.levelToLoad = 667;
		#endif

		
		if( RunnerCrossSceneInfo.use.levelToLoad < 0 )
		{
			MonoBehaviour cc = RunnerCharacterController.useBehaviour;
			cc.gameObject.SetActive(false);
			MenuManager.use.ActivateMenu(MenuManagerDefault.MenuTypes.GameMenu);
		}
		else 
		{
			MenuManager.use.ActivateMenu(MenuManagerDefault.MenuTypes.NONE);
			
			AudioClip background = LugusResources.use.Shared.GetAudio(Application.loadedLevelName + "_background");
			if( background != LugusResources.use.errorAudio ) 
			{
				LugusAudio.use.Music().Play(background, true, new LugusAudioTrackSettings().Loop(true).Volume(0.5f));
			}

			StartGame();
		}


	}

	public override void StartGame()
	{
		LugusCoroutines.use.StartRoutine( StartGameRoutine() );
	}

	protected IEnumerator StartGameRoutine()
	{
		Debug.Log ("Starting Runner game");

		RunnerCameraPuller puller = null;
		if( RunnerCameraPuller.Exists() )
			puller = RunnerCameraPuller.use;
		
		RunnerInteractionManager.use.Deactivate();
		RunnerCharacterController.useBehaviour.enabled = false;

		if( puller != null )
		{
			puller.rigidbody2D.isKinematic = true;
			puller.enabled = false;
		}

		yield return new WaitForSeconds(0.01f); // give other SetupGlobal()s the time to do their work (especially HUDManager)
		
		if( Application.loadedLevelName == "e09_Brazil" )
		{
			RunnerCharacterAnimatorFasterSlower fs = RunnerCharacterController.useBehaviour.GetComponent<RunnerCharacterAnimatorFasterSlower>();
			fs.PlayAnimation(fs.stillAnimation);
		}

		yield return new WaitForSeconds(0.5f); // give fade-out time to finish

		//HUDManager.use.CountdownScreen.gameObject.SetActive(true); // make sure this is active. Is done in SetupGlobal 

		if( Application.loadedLevelName == "e10_Swiss" ||
		    Application.loadedLevelName == "e13_pacific" )
		{
			HUDManager.use.CountdownScreen.countText.textMesh.color = Color.black;
			HUDManager.use.CountdownScreen.countTextShadow.textMesh.color = Color.white;
		}

		if( Application.loadedLevelName == "e10_Swiss" )
		{
			// there are trail renderers on the skies in switserland
			// everytime we shift, the renderers fill the screen for a second, which looks buggy
			// so try to delay the shift as long as possible.  
			shiftYTreshold = 1800.0f;
		}

		#if !UNITY_EDITOR
		HUDManager.use.CountdownScreen.StartCountdown(3, 3.0f);

		yield return new WaitForSeconds(3.0f);
		#endif

		_gameRunning = true;
		IRunnerConfig.use.LoadLevel( RunnerCrossSceneInfo.use.levelToLoad );


		RunnerCharacterController.useBehaviour.enabled = true;
		
		if( puller != null )
		{
			puller.rigidbody2D.isKinematic = false;
			puller.enabled = true;
		}

		startTime = Time.time;
		
		RunnerInteractionManager.use.Activate();
		RunnerInteractionManager.use.SpawnForFirstSection ();
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

		// endless runners can only stop when the character dies
		// so: start death animation and stop camera

		// distance-based runners: just stop camera and let the character move out of the screen

		float endscreenDelay = 1.0f;

		if( this.gameType == KikaAndBob.RunnerGameType.Distance )
		{
			RunnerCharacterController.useBehaviour.rigidbody2D.isKinematic = true;

			if( RunnerCameraPuller.Exists() )
			{
				// climber: also disable the controller and animation, cam stops automatically
				RunnerCharacterController.useBehaviour.enabled = false;
				
				RunnerCharacterAnimatorFasterSlower fs = RunnerCharacterController.useBehaviour.GetComponent<RunnerCharacterAnimatorFasterSlower>();
				fs.PlayAnimation(fs.stillAnimation);

				//RunnerCharacterController.useBehaviour.GetComponent<RunnerCharacterAnimator>().StopAll();
				//RunnerCharacterController.useBehaviour.GetComponent<RunnerCharacterAnimator>().enabled = false;
			}
			else
			{
				FollowCameraContinuous cam = LugusCamera.game.GetComponent<FollowCameraContinuous>();
				if( cam != null )
				{
					cam.DisableParallax();
					cam.enabled = false;

					RunnerCharacterController.useBehaviour.rigidbody2D.isKinematic = true;
					//RunnerCharacterController.useBehaviour.rigidbody2D.velocity = Vector3.zero;
					RunnerCharacterController.useBehaviour.Disable( 10.0f ); 

					if( Application.loadedLevelName == "e19_illinois" )
					{
						// make kika run out of screen a little more slowly please
						RunnerCharacterControllerJumpSlide c = RunnerCharacterControllerJumpSlide.use;
						c.speedRange.to = c.speedRange.to / 2.0f;
					}
				}
				else
				{
					Debug.LogError(transform.Path () + " : No FollowCameraContinuous found!");
				}
			}
		}
		else
		{
			FollowCameraContinuous cam = LugusCamera.game.GetComponent<FollowCameraContinuous>();
			if( cam != null )
			{
				cam.DisableParallax();
				cam.enabled = false;
			}
			else
			{
				Debug.LogError(transform.Path () + " : No FollowCameraContinuous found!");
			}

			RunnerCharacterAnimatorJumpSlide animator = RunnerCharacterController.useBehaviour.GetComponent<RunnerCharacterAnimatorJumpSlide>();

			if( animator != null )
			{
				//RunnerCharacterController.useBehaviour.rigidbody2D.velocity = Vector3.zero;
				//RunnerCharacterController.useBehaviour.rigidbody2D.isKinematic = true;
				
				( (RunnerCharacterControllerJumpSlide) RunnerCharacterController.jumpSlide).EnlargeShadow();
				RunnerCharacterController.useBehaviour.enabled = false;
				animator.characterDead = true;
				animator.dust.enableEmission = false;
				animator.PlayAnimation( animator.deathAnimation );

				endscreenDelay = 2.0f;
			}
			else
			{
				Debug.LogError(transform.Path () + " : No RunnerCharacterAnimatorJumpSlide found!");
			}

			//RunnerCharacterController.useBehaviour.enabled = false;
			//RunnerCharacterController.useBehaviour.rigidbody2D.isKinematic = true;
			//RunnerCharacterController.useBehaviour.GetComponent<RunnerCharacterAnimator>().StopAll();
			//RunnerCharacterController.useBehaviour.GetComponent<RunnerCharacterAnimator>().enabled = false;
		}

		/*
		RunnerCharacterController.useBehaviour.enabled = false;
		RunnerCharacterController.useBehaviour.rigidbody2D.isKinematic = true;
		RunnerCharacterController.useBehaviour.GetComponent<RunnerCharacterAnimator>().StopAll();
		RunnerCharacterController.useBehaviour.GetComponent<RunnerCharacterAnimator>().enabled = false;
		*/

		RunnerInteractionManager.use.Deactivate();

		IRunnerConfig.use.OnGameStopped();
		
		HUDManager.use.StopAll();
		DialogueManager.use.HideAll();
		HUDManager.use.PauseButton.gameObject.SetActive(false);
	

		LugusCoroutines.use.StartRoutine( ScoreAnimationRoutine(endscreenDelay) );
		
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

	//public int pickupsPerSecondConversion = 1; // how many pickups a user should score before he gets 1 second added or subtracted from his timescore

	protected IEnumerator ScoreAnimationRoutine(float delay)
	{
		yield return new WaitForSeconds( delay );

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

		// this is for Endless runner
		// there, we just take the time as score
		float timeScore = Mathf.FloorToInt(timeSpent + (Time.time - startTime));



		HUDManager.use.LevelEndScreen.Show(true);
		HUDManager.use.LevelEndScreen.Counter1.SetValue( timeScore, true );
		HUDManager.use.LevelEndScreen.Counter2.SetValue( pickupCount, true );
		 
		// for fixed distance levels, we want a HIGHscore, but the goal is to reach the end as quickly as possible...
		// so we need to convert low numbers to high scores
		// approach: 
		// - 0s is 10.000 points
		// - every second spent is -50 points
		// - every enemy hit was 5 seconds extra, so basically -250 points
		// - every feather counts for 50 points at the end (so 1 feather = win 1 second back. 5 seconds = 1 enemy hit)

		/*
		 * target is 200s

		0 sec = 10.000 punten
		elke 1 sec = -50 punten?
		elke hit = 5 sec bij = -250 punten
		elke feather = + 50 punten
		*/
		if( gameType == KikaAndBob.RunnerGameType.Distance )
		{
			timeScore = 10000 - (timeScore * 50);
		}

		timeScore = Mathf.Max (0.0f, timeScore); // prevent going under 0

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

		int pickupsPerBatch = 1;

		float minTimePerItem = 0.3f;
		float timePerItem = minTimePerItem;
		float totalTime = 3.0f;
		int batchCount = pickupCount;

		int restValue = 0;

		if( (totalTime / pickupCount) < minTimePerItem )
		{
			// too many pickups to process them one by one (would go over totalTime)
			// group them into batches of pickups, so we retain the minTimePerItem and totalTime
			batchCount = Mathf.RoundToInt( totalTime / minTimePerItem );
			pickupsPerBatch = Mathf.CeilToInt( ( (float) pickupCount ) / ( (float) batchCount) ); 
			
			//Debug.LogError ("SCORE PICKUPS 1 : " + pickupCount + " pickups : " + batchCount + " batches with "+ pickupsPerBatch + " per batch and rest " +  restValue); 

			if( pickupsPerBatch * batchCount > pickupCount )
			{
				// ex. 26 pickups, batchCount = 15 -> ceiled pickupsPerBatch is 2, but there are no 30 pickups
				// so -> reduce pickups per batch and use a different approach for the rest value
				pickupsPerBatch -= 1;
				pickupsPerBatch = Mathf.Max (1, pickupsPerBatch); // make sure it's never 0. Shouldn't be necessary, but do it just in case ;)

				restValue = pickupCount - (batchCount * pickupsPerBatch);

				if( restValue < 0 )
				{
					// once again: shouldn't happen, but if it does...
					Debug.LogError(transform.Path () + " : Rest value was < 0 : " + restValue + " // " + pickupCount + " - (" + batchCount + " * " + pickupsPerBatch );
					restValue = 0;
				}
			} 

			//Debug.LogError ("SCORE PICKUPS 2 : " + pickupCount + " pickups : " + batchCount + " batches with "+ pickupsPerBatch + " per batch and rest " +  restValue); 
		} 
		else 
		{ 
			timePerItem = totalTime / pickupCount; // prolongue the timePerItem a little if possible
		}

		for( int i = 0; i < batchCount; ++i )
		{ 
			
			//Debug.Log("LOOP STart " + i + " // " + batchCount + " // per batch " + pickupsPerBatch); 

			// note: this should not animate the counter.
			// otherwhise, if the timePerItem < counterAnimationTime, we can sometimes see jumpy values
			HUDManager.use.LevelEndScreen.Counter2.AddValue( -pickupsPerBatch, false );

			float score = 50.0f; // DISTANCE 
			if( gameType == KikaAndBob.RunnerGameType.Endless ) 
			{
				// ENDLESS: picups give more time (longer untill death) = better
				score = 1.0f; 
			}

			score *= pickupsPerBatch;


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


			// now we've done all the batches, we might have to process the resting values
			if( (i == (batchCount - 1)) && restValue > 0 )
			{
				i = -1; // account for the ++i at the end of this loop: effectively restart it
				batchCount = restValue;
				pickupsPerBatch = 1;
				timePerItem = minTimePerItem; 

				restValue = 0;

				//Debug.LogError("RESET THE LOOP " + i + " //" + batchCount);
			}

			//Debug.Log("LOOP score " + i + " // " + score); 

			/*
			if( i == (batchCount - 1) ) 
			{
				HUDManager.use.LevelEndScreen.Counter2.SetValue(0);
			}
			*/

			
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

			pickupCount = Random.Range(20, 50);

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
