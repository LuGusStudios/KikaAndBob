using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunnerKangeroo : MonoBehaviour 
{
	public ParticleSystem dust = null;

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
		if( dust == null )
		{
			dust = this.transform.parent.GetComponentInChildren<ParticleSystem>();
		}
	}

	protected ILugusCoroutineHandle handle = null;
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
		handle = LugusCoroutines.use.StartRoutine( JumpRoutine() );
	}

	public void OnDestroy()
	{
		//Debug.LogWarning("Kangeroo destroyed : ending coroutine");
		if( handle != null )
		{
			handle.StopRoutine();
		}
	}

	protected IEnumerator JumpRoutine()
	{
		float halfLength = 0.5f; // jump animation is 1 sec long
		float speed = 5.0f;

		while( true )
		{
			gameObject.MoveTo( transform.localPosition.yAdd( 5.0f ) ).IsLocal(true).Time (halfLength).Execute();

			yield return new WaitForSeconds( halfLength / 2.0f );
			
			dust.Stop();
			
			yield return new WaitForSeconds( halfLength / 2.0f );
			
			gameObject.MoveTo( transform.localPosition.yAdd( -5.0f ) ).IsLocal(true).Time (halfLength).Execute();
			
			yield return new WaitForSeconds( halfLength * 0.8f );
			
			dust.Play ();

			yield return new WaitForSeconds( halfLength * 0.2f );

			/*
			// parabolic motion
			float startTime = Time.time;
			while( Time.time - startTime < halfLength )
			{
				transform.position += new Vector3(0.0f, Time.deltaTime * speed, 0.0f);
			}

			startTime = Time.time;

			while( Time.time - startTime < halfLength )
			{
				
			}
			*/
		}
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
		//Debug.Log ("KANGEROO Y " + transform.position.y);
	}
}
