using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunnerMover : MonoBehaviour 
{
	public Vector3 direction = Vector3.zero;
	public float speed = 1.0f;

	public bool checkPlayerVicinity = false;
	public bool adjustForPlayerSpeed = false;
	
	
	protected RunnerInteractionManager directionStore = null;
	protected GameObject character = null;

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
		if( directionStore == null )
			directionStore = RunnerInteractionManager.use;

		if( character == null )
			character = ( (MonoBehaviour) RunnerCharacterController.use).gameObject;
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
		if( CanMove () )
		{
			float useSpeed = speed;
			if( adjustForPlayerSpeed )
			{
				if( RunnerCharacterControllerFasterSlower.Exists() )
				{
					RunnerCharacterControllerFasterSlower c = RunnerCharacterControllerFasterSlower.use;
					useSpeed = speed * c.SpeedRange().ValueFromPercentage( c.speedPercentage );
				}
			}

			transform.position += direction * useSpeed * Time.deltaTime;
		}
	}

	public bool CanMove()
	{
		if( !checkPlayerVicinity )
			return true;

		// only start moving if the player is less than 1 screen width / length away
		float distance = 0.0f;
		float minDistance = 0.0f;
		if( directionStore.direction == RunnerInteractionManager.Direction.EAST )
		{
			minDistance = LugusUtil.UIWidth / 2.0f;

			distance = this.transform.position.x - character.transform.position.x;
		}
		else if( directionStore.direction == RunnerInteractionManager.Direction.WEST )
		{
			minDistance = LugusUtil.UIWidth / 2.0f; 
			
			distance = character.transform.position.x - this.transform.position.x;
		}
		else if( directionStore.direction == RunnerInteractionManager.Direction.NORTH )
		{
			minDistance = LugusUtil.UIHeight / 2.0f;
			
			distance = this.transform.position.y - character.transform.position.y;
		}
		else if( directionStore.direction == RunnerInteractionManager.Direction.SOUTH )
		{
			minDistance = LugusUtil.UIHeight / 2.0f;
			
			distance = character.transform.position.y - this.transform.position.y;
		}

		//if( this.name.Contains("Devil") )
		//	Debug.LogError(transform.Path () + " : move distance? " + distance + " < " + minDistance);

		if( distance < (minDistance * 2.0f) )
		{
			return true;
		}
		else
			return false;

	}
}
