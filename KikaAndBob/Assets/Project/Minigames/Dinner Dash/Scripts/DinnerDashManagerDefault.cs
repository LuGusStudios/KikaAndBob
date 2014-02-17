using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DinnerDashManager : LugusSingletonExisting<IDinnerDashManager> 
{

}

public abstract class IDinnerDashManager : IGameManager
{
	protected ConsumableMover _mover = null;
	public ConsumableMover Mover
	{
		get
		{
			if( _mover == null )
			{
				_mover = (ConsumableMover) GameObject.FindObjectOfType( typeof(ConsumableMover) ); 
			}
			
			return _mover;
		}
	}

	public ConsumableConsumerManager consumerManager = null;

	public float moneyScore = 0.0f;
	public float targetMoneyScore = -1.0f; // < 0 means no money score
	public float timeout = 300.0f; // how long the user can play in SECONDS (this is default 5 minutes). < 0 = no timeout

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
}

public class DinnerDashManagerDefault : IDinnerDashManager
{
	public GameObject checkMark = null;

	protected bool _gameRunning = false;
	public override bool GameRunning
	{
		get{ return _gameRunning; }
	}

	public void SetupLocal()
	{
		if( consumerManager == null )
			consumerManager = this.gameObject.AddComponent<ConsumableConsumerManager>();

		if( checkMark == null )
		{
			checkMark = GameObject.Find ("CheckMark");
		}

		if( checkMark == null )
		{
			Debug.LogError(name + " : could not find checkMark prefab!");
		}
	}
	
	public void SetupGlobal()
	{
		HUDManager.use.RepositionPauseButton( KikaAndBob.ScreenAnchor.Top);

		// lookup references to objects / scripts outside of this script

		// DEBUG: TODO: REMOVE THIS! just so we can directly play when starting in editor
#if UNITY_EDITOR
		//if( DinnerDashCrossSceneInfo.use.levelToLoad < 0 )
		//	DinnerDashCrossSceneInfo.use.levelToLoad = 1;
#endif

		//Debug.LogError("DINNER DASH TO LOAD" + DinnerDashCrossSceneInfo.use.levelToLoad);

		if( DinnerDashCrossSceneInfo.use.levelToLoad < 0 )
		{
			MenuManager.use.ActivateMenu(MenuManagerDefault.MenuTypes.GameMenu);
		}
		else
		{
			MenuManager.use.ActivateMenu(MenuManagerDefault.MenuTypes.NONE);

			DialogueBox introBox = DialogueManager.use.CreateBox(KikaAndBob.ScreenAnchor.Center, LugusResources.use.Localized.GetText(Application.loadedLevelName + "." + (DinnerDashCrossSceneInfo.use.levelToLoad) + ".intro") );  
			introBox.boxType = DialogueBox.BoxType.Continue;
			introBox.onContinueButtonClicked += OnStartButtonClicked;
			introBox.Show(); 
		}
	}

	protected void OnStartButtonClicked(DialogueBox box)
	{
		box.onContinueButtonClicked -= OnStartButtonClicked;
		box.Hide();

		StartGame ();
	}

	public override void StartGame()
	{
		_gameRunning = true;

		//StopGame ();
		Debug.Log ("Starting dinner dash");

		// empty the queue
		// when this was added, you could click on the items in the level while the beginning dialoguebox was still showing
		// the queue would fill up and the mover would start moving to sometimes no-longer existing items
		foreach( IConsumableUser obj in queue )
		{
			ToggleCheckmark( obj, false );
		}
		queue.Clear();

		
		IDinnerDashConfig.use.LoadLevel( DinnerDashCrossSceneInfo.use.levelToLoad );
		LugusCoroutines.use.StartRoutine( TimeoutRoutine() ); // timeout is possibly set by LoadLevel, so start the routine!

		queueRoutineHandle = LugusCoroutines.use.StartRoutine( QueueRoutine() );

		consumerManager.StartConsumerGeneration();
	}

	public override void StopGame() 
	{
		_gameRunning = false;

		HUDManager.use.StopAll();
		DialogueManager.use.HideAll();


		/*
		DialogueBox outroBox = DialogueManager.use.CreateBox(KikaAndBob.ScreenAnchor.Center, LugusResources.use.Localized.GetText(Application.loadedLevelName + "." + (DinnerDashCrossSceneInfo.use.levelToLoad + 1) + ".outro") );  
		outroBox.boxType = DialogueBox.BoxType.Continue;
		outroBox.onContinueButtonClicked += BackToMenu; 
		outroBox.Show(); 
		*/

		HUDManager.use.LevelEndScreen.Counter1.gameObject.SetActive(true);

		//float moneyScore = ((HUDCounter)HUDManager.use.GetElementForCommodity(KikaAndBob.CommodityType.Money)).currentValue;
		bool success = true;
		if( targetMoneyScore > 0.0f )
		{
			HUDManager.use.LevelEndScreen.Counter1.suffix = " / " + targetMoneyScore;

			if( moneyScore < targetMoneyScore )
			{
				success = false;
			}
		}

		HUDManager.use.LevelEndScreen.Counter1.commodity = KikaAndBob.CommodityType.Money;
		HUDManager.use.LevelEndScreen.Show(success);
		HUDManager.use.LevelEndScreen.Counter1.SetValue( moneyScore, true );

		if( success )
		{

			Debug.Log ("DinnerDash : set level success : " + (Application.loadedLevelName + "_level_" + DinnerDashCrossSceneInfo.use.levelToLoad) );
			LugusConfig.use.User.SetBool( Application.loadedLevelName + "_level_" + DinnerDashCrossSceneInfo.use.levelToLoad, true, true );
			LugusConfig.use.SaveProfiles();
		}


		Debug.Log ("Stopping dinner dash " + moneyScore + " >= " + targetMoneyScore);

		if( queueRoutineHandle != null )
		{
			queueRoutineHandle.StopRoutine();
			queueRoutineHandle = null;
		}

		if( consumerManager != null )
		{
			consumerManager.StopConsumerGeneration();
		}
	}

	/*
	protected void BackToMenu(DialogueBox box)
	{
		box.onContinueButtonClicked -= BackToMenu;
		box.Hide(); 

		HUDManager.use.DisableAll();

		MenuManager.use.ActivateMenu( MenuManagerDefault.MenuTypes.LevelMenu );
	}
	*/

	public List<IConsumableUser> queue = new List<IConsumableUser>();

	protected ILugusCoroutineHandle queueRoutineHandle = null;

	protected void Update()
	{
		if( LugusInput.use.KeyDown(KeyCode.S) )
			StopGame();

		Transform hit = LugusInput.use.RayCastFromMouseUp( LugusCamera.game );
		if( hit == null )
			return;

		IConsumableUser user = hit.GetComponent<IConsumableUser>();
		if( user != null )
		{
			Debug.Log ("Added user to queue : " + user.name);
			queue.Add(user);

			ToggleCheckmark( user, true );
		}
	}

	protected void ToggleCheckmark(IConsumableUser user, bool show)
	{
		if( show )
		{
			// TODO: pool these things!
			GameObject checkMarkNew = (GameObject) GameObject.Instantiate( checkMark );
			checkMarkNew.transform.parent = user.transform;
			checkMarkNew.name = "CheckMark";

			Vector3 checkMarkPos = user.GetCheckmarkPosition();

			checkMarkNew.transform.position = checkMarkPos;

			checkMarkNew.transform.position = checkMarkNew.transform.position.z(-400.0f);
		}
		else 
		{
			Transform checkMarkOld = user.transform.FindChild("CheckMark");
			if( checkMarkOld != null )
			{
				// TODO: pool these things!
				GameObject.Destroy( checkMarkOld.gameObject );
			}
		}
	}




	//protected bool processing = false;

	protected IEnumerator QueueRoutine()
	{
		while( true )
		{
			if( queue.Count == 0 /*|| processing*/ )
			{
				yield return new WaitForSeconds(0.1f);
			}
			else
			{
				//processing = true;
				processHandle = LugusCoroutines.use.GetHandle();
				yield return processHandle.StartRoutine( Process(this.queue[0]) );

				if( queue.Count == 0 )
				{
					if( ConsumableMover.use.onStopped != null )
						ConsumableMover.use.onStopped();
				}
			}
		}
	}

	protected ILugusCoroutineHandle processHandle = null;
	protected IEnumerator Process(IConsumableUser user)
	{
		//Debug.Log ("Processing next item in queue : " + user.name );
		
		queue.Remove (user);
		
		Waypoint target = user.GetTarget(); 

		ILugusCoroutineHandle h = LugusCoroutines.use.GetHandle();
		yield return h.StartRoutine( ConsumableMover.use.MoveToRoutine(target) );
		
		ToggleCheckmark( user, false );

		bool result = user.Use ();

		if( result && user.onUsed != null )
		{
			//Debug.LogWarning("ICOnsumableuser " + user.name + " used!" );
			user.onUsed( user );
		}


		/*
		//Mover.gameObject.StopTweens();
		//Mover.gameObject.MoveTo( target.transform.position ).Speed( 600.0f ).Execute(); 

		float maxDistance = 5.0f; // units (in this setup = pixels)

		bool reachedTarget = false;
		while( !reachedTarget )
		{
			yield return new WaitForSeconds(0.1f);

			reachedTarget = (Vector2.Distance( Mover.transform.position.v2 (), target.transform.position.v2 () ) < maxDistance);
		}

		//Mover.gameObject.StopTweens();

		user.Use ();
		*/
	}
	


	protected void Awake()
	{
		SetupLocal();
	}
	
	protected void Start()
	{
		SetupGlobal();
	}

	protected bool debugAutoProcessing = true;
	protected void OnGUI()
	{
		if( !LugusDebug.debug )
			return;

		GUILayout.BeginArea( new Rect(0, 0, 200, 400) );
		GUILayout.BeginVertical();

		if( GUILayout.Button("AutoProcessing : "+ debugAutoProcessing) )
		{
			debugAutoProcessing = !debugAutoProcessing;

			if( !debugAutoProcessing )
				queueRoutineHandle.StopRoutine();
			else
				queueRoutineHandle = LugusCoroutines.use.StartRoutine( QueueRoutine() );
		}

		if( !debugAutoProcessing && queue.Count > 0 )
		{
			bool go = true;
			if( processHandle != null && processHandle.Running )
				go = false;
			 
			if( go )
			{
				if( GUILayout.Button ("Process one") )
				{
					processHandle = LugusCoroutines.use.GetHandle();
					processHandle.StartRoutine( Process(this.queue[0]) );
				}
			}
		}


		GUILayout.Label( "Queue:" + ( (processHandle != null && processHandle.Running) ? "Busy" : "") );
		GUILayout.Label( "------");
		foreach( IConsumableUser user in queue )
		{
			GUILayout.Label( "" + user.name );
		}

		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
}
