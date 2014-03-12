using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunnerCameraOffsetByDistance : MonoBehaviour 
{
	public DataRange xOffsetRange = new DataRange(7, -0.54f);

	public FollowCameraContinuous followCamera = null;
	public RunnerManagerDefault manager = null;

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
		if( followCamera == null )
		{
			followCamera = GetComponent<FollowCameraContinuous>();
		}

		if( manager == null )
		{
			manager = RunnerManager.use;
		}
	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
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
		float distancePercentage = manager.TraveledDistance / manager.targetDistance;

		float offset = xOffsetRange.ValueFromPercentage( distancePercentage );

		Debug.Log("DISTANCE percent : " + distancePercentage + ", so offset : " + offset );

		followCamera.xOffset = offset;


	}
}
