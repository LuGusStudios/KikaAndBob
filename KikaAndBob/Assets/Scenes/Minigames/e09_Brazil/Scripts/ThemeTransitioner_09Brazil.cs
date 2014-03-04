using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ThemeTransitioner_09Brazil : MonoBehaviour 
{
	public LayerManagerDefault layerManager = null;
	public RunnerManagerDefault runnerManager = null;

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
		if( layerManager == null )
			layerManager = LayerManager.use;

		if( runnerManager == null )
			runnerManager = RunnerManager.use;
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}

	protected int phase = 0;

	protected void Update () 
	{
		/*
		if( phase == 0 && runnerManager.TraveledDistance > (runnerManager.targetDistance * 0.3f) )
		{
			Debug.LogError("Going to phase 1, next theme");
			phase = 1;
			layerManager.NextTheme();
		}
		else if( phase == 1 && runnerManager.TraveledDistance > (runnerManager.targetDistance * 0.6f) )
		{
			Debug.LogError("Going to phase 2, next theme");
			phase = 2;
			layerManager.NextTheme();
		}
		else if( phase == 2 && (runnerManager.targetDistance - runnerManager.TraveledDistance < LugusUtil.UIHeight * 1.5f ) )
		{
			Debug.LogError("Going to phase 3, next theme");
			phase = 3;
			layerManager.NextTheme();
		}
		*/
		
		if( phase == 0 && runnerManager.TraveledDistance > (runnerManager.targetDistance * 0.4f) )
		{
			//Debug.LogError("Going to phase 1, next theme");
			phase = 1;
			layerManager.NextTheme();
		}
		else if( phase == 1 && (runnerManager.targetDistance - runnerManager.TraveledDistance < LugusUtil.UIHeight * 3.0f ) )
		{
			//Debug.LogError("Going to phase 2, next theme");
			phase = 2;
			layerManager.NextTheme();
		}
	}
}
