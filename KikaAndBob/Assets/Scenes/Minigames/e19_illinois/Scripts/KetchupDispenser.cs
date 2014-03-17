using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KetchupDispenser : MonoBehaviour 
{
	public Transform squirt = null;
	public Animator splash = null;

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
		if( squirt == null )
		{ 
			squirt = this.transform.parent.FindChild("Squirt");
		}

		if( squirt != null )
		{
			squirt.GetComponent<SpriteRenderer>().enabled = false;
		}

		if( splash == null )
		{
			splash = this.transform.parent.GetComponentInChildren<Animator>();
		}

		if( splash != null )
		{
			splash.enabled = false;
			splash.gameObject.SetActive(false);
		}
	}

	public void OnDisable()
	{
		if( squirtHandle != null ) 
		{
			squirtHandle.StopRoutine();
		}

		squirtHandle = null;
	}

	protected ILugusCoroutineHandle squirtHandle = null;

	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script

		squirtHandle = LugusCoroutines.use.StartRoutine( SquirtRoutine() );
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

	protected IEnumerator SquirtRoutine()
	{
		while( !CanMove() )
			yield return null;

		yield return new WaitForSeconds( Random.Range(0.4f, 1.0f) );


		squirt.GetComponent<SpriteRenderer>().enabled = true;

		squirt.gameObject.MoveTo( squirt.localPosition.yAdd( -4.5f) ).IsLocal( true ).Time (0.2f).Execute();
		
		this.gameObject.MoveTo( this.transform.localPosition.yAdd( 3.0f) ).IsLocal( true ).Time (0.3f).Execute();

		LugusAudio.use.SFX().Play ( LugusResources.use.Shared.GetAudio("Spit02") );

		yield return new WaitForSeconds(0.2f);

		splash.gameObject.SetActive(true);
		splash.enabled = true;

		squirt.gameObject.SetActive(false);

		yield return null;
	}

	protected bool CanMove()
	{
		// only start moving if the player is less than 1 screen width / length away
		float distance = 0.0f;
		float minDistance = 0.0f;
		//if( directionStore.direction == RunnerInteractionManager.Direction.EAST )
		//{
			minDistance = LugusUtil.UIWidth / 2.0f;
			
			distance = this.transform.position.x - RunnerCharacterController.useBehaviour.transform.position.x;
		//}

		if( distance < (minDistance * 2.0f) )
		{
			return true;
		}
		else
			return false;
	}
}
