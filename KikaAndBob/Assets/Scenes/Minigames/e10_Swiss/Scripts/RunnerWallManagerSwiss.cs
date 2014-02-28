using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunnerWallManagerSwiss : MonoBehaviour 
{
	public DataRange wallXRange = new DataRange(9.04f, 6.3f);
	public List<RunnerWall> walls = new List<RunnerWall>();
	
	public RunnerManagerDefault manager = null;
	
	public void SetupLocal()
	{
		// assign variables that have to do with this class only
		walls.AddRange( GameObject.FindObjectsOfType<RunnerWall>() );
		
		if( walls.Count != 4 )
		{
			Debug.LogError(transform.Path () + " : There should be 4 walls in this level. We can only find " + walls.Count);
		}
	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
		
		manager = RunnerManager.use;

		LugusCoroutines.use.StartRoutine( WallClosingRoutine() );
	}
	
	protected void Awake()
	{
		SetupLocal();
	}
	
	protected void Start () 
	{
		SetupGlobal();
	}

	protected IEnumerator WallClosingRoutine()
	{
		float halfPeriod = RunnerInteractionManager.use.timeToMax * 0.2f; // 1/5th of the timeToMax

		// small timeout at the beginning to let things start slow
		yield return new WaitForSeconds( halfPeriod );

		
		DataRange invertedXRange = new DataRange( wallXRange.from * -1.0f, wallXRange.to * -1.0f );

		while( true )
		{
			// CLOSING
			foreach( RunnerWall wall in walls )
			{
				if( wall.transform.localPosition.x < 0 ) // left walls
				{
					wall.gameObject.MoveTo( wall.transform.localPosition.x ( invertedXRange.to )).IsLocal(true).Time(halfPeriod).Execute();
				}
				else // right walls
				{
					wall.gameObject.MoveTo( wall.transform.localPosition.x ( wallXRange.to )    ).IsLocal(true).Time(halfPeriod).Execute();
				}
			}

			yield return new WaitForSeconds( halfPeriod );

			// OPENING
			foreach( RunnerWall wall in walls )
			{
				if( wall.transform.localPosition.x < 0 ) // left walls
				{
					wall.gameObject.MoveTo( wall.transform.localPosition.x ( invertedXRange.from )).IsLocal(true).Time(halfPeriod).Execute();
				}
				else // right walls
				{
					wall.gameObject.MoveTo( wall.transform.localPosition.x ( wallXRange.from )    ).IsLocal(true).Time(halfPeriod).Execute();
				}
			}
			
			yield return new WaitForSeconds( halfPeriod );
		}

	}
}
