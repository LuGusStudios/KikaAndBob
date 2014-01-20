using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunnerDestroyZoneWhenOffscreen : MonoBehaviour 
{
	protected void OnBecameInvisible() 
	{
		RunnerInteractionZone zone = transform.parent.GetComponent<RunnerInteractionZone>();

		if( zone == null )
		{
			Debug.LogError(name + " : parent had no zone to destroy!");
			return;
		}

		GameObject.Destroy( zone.gameObject );
	}
}
