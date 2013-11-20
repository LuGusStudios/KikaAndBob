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
}

public class DinnerDashManagerDefault : IDinnerDashManager
{

	protected void Awake()
	{

	}

	public List<IConsumableUser> queue = new List<IConsumableUser>();

	protected ILugusCoroutineHandle queueRoutineHandle = null;

	protected void Start()
	{
		queueRoutineHandle = LugusCoroutines.use.StartRoutine( QueueRoutine() );
	}

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
			}
		}
	}

	protected ILugusCoroutineHandle processHandle = null;
	protected IEnumerator Process(IConsumableUser user)
	{
		Debug.Log ("Processing next item in queue : " + user.name );
		
		queue.Remove (user);
		
		Vector2 target = user.GetTarget().v2 (); 

		// TODO: move Mover to user location before actually using it : properly
		//Mover.transform.positionTo(3.0f, target); 
		//GoTween tween = Go.to( Mover.transform, 3.0f, new GoTweenConfig().position( target ) );
		
		Mover.gameObject.StopTweens();
		Mover.gameObject.MoveTo( target ).Speed( 300.0f ).Execute(); 

		float maxDistance = 5.0f; // units (in this setup = pixels)

		bool reachedTarget = false;
		while( !reachedTarget )
		{
			yield return new WaitForSeconds(0.1f);

			reachedTarget = (Vector2.Distance( Mover.transform.position.v2 (), target ) < maxDistance);
		}

		Mover.gameObject.StopTweens();

		user.Use ();
	}

	protected bool debugAutoProcessing = true;
	protected void OnGUI()
	{
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
