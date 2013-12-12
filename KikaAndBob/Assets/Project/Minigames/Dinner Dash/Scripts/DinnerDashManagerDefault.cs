using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DinnerDashManager : LugusSingletonExisting<IDinnerDashManager> 
{

}

public abstract class IDinnerDashManager : MonoBehaviour
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

	public abstract void StartGame();
	public abstract void StopGame();
}

public class DinnerDashManagerDefault : IDinnerDashManager
{
	public void SetupLocal()
	{
		if( consumerManager == null )
			consumerManager = this.gameObject.AddComponent<ConsumableConsumerManager>();
	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
	}

	public override void StartGame()
	{
		StopGame ();
		Debug.Log ("Starting dinner dash");


		queueRoutineHandle = LugusCoroutines.use.StartRoutine( QueueRoutine() );

		consumerManager.StartConsumerGeneration();
	}

	public override void StopGame()
	{
		Debug.Log ("Stopping dinner dash");

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

	public List<IConsumableUser> queue = new List<IConsumableUser>();

	protected ILugusCoroutineHandle queueRoutineHandle = null;

	protected void Update()
	{
		Transform hit = LugusInput.use.RayCastFromMouseUp( LugusCamera.game );
		if( hit == null )
			return;

		IConsumableUser user = hit.GetComponent<IConsumableUser>();
		if( user != null )
		{
			Debug.Log ("Added user to queue : " + user.name);
			queue.Add(user);
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

		user.Use ();

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
