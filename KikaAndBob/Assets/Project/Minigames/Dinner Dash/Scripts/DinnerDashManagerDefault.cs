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
	public GameObject checkMark = null;

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
			//checkMarkNew.transform.localPosition = Vector3.zero;

			Vector3 checkMarkPos = Vector3.zero;
			BoxCollider2D boxCollider = user.GetComponent<BoxCollider2D>();
			if( boxCollider == null )
			{
				checkMarkPos = user.transform.position + new Vector3(50.0f, 50.0f, 0.0f);
			}
			else
			{

				// position checkmark at top left corner of the bounding box
				float xOffset = (boxCollider.size.x / 2.0f) + boxCollider.center.x;
				float yOffset = ((-1.0f * boxCollider.size.y) / 2.0f) - boxCollider.center.y; 

				xOffset *= -1.0f;
				yOffset *= -1.0f;

				//xOffset *= user.transform.localScale.x;
				//yOffset *= user.transform.localScale.y;

				checkMarkPos = user.transform.TransformPoint( new Vector3(xOffset, yOffset, 0.0f ) );
				//xOffset = user.transform.position.x - offsets.x;
				//yOffset = user.transform.position.y - offsets.y;

				//Debug.LogError("CENTER : " + boxCollider.center + " / SIZE: " + boxCollider.size + " / " + xOffset + "," + yOffset); 
			}

			checkMarkNew.transform.position = checkMarkPos;//user.transform.position.xAdd(xOffset).yAdd (yOffset);

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
			user.onUsed( user );


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
