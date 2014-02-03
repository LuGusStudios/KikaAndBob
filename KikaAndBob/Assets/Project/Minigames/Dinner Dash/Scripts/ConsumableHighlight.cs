using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConsumableHighlight : MonoBehaviour 
{
	public float minY = -20.0f;
	public float maxY = 20.0f;
	public float speed = 10.0f; 

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}

	protected ILugusCoroutineHandle bobbingHandle = null;
	public void SetupGlobal()
	{
		bobbingHandle = LugusCoroutines.use.StartRoutine( BobbingRoutine() );
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

	public void OnDestroy()
	{
		if( bobbingHandle != null && bobbingHandle.Running )
			bobbingHandle.StopRoutine();
	}

	protected IEnumerator BobbingRoutine()
	{
		Vector3 minPos = transform.position.yAdd( minY );
		Vector3 maxPos = transform.position.yAdd( maxY );


		while( true )
		{
			if( speed > 0 && transform.position.y >= maxPos.y )
			{
				speed = -1 * speed;
			}
			if( speed < 0 && transform.position.y <= minPos.y )
			{
				speed = -1 * speed;
			}

			transform.position = transform.position.yAdd( speed * Time.deltaTime );

			yield return null;
		}
	}
}
