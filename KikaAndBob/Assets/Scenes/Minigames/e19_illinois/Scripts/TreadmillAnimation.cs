using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreadmillAnimation : MonoBehaviour 
{
	public DataRange speedRange = new DataRange(7, 12);

	public GameObject other1 = null;
	public GameObject other2 = null;

	public RunnerManagerDefault manager = null;
	public RunnerInteractionManager interactionManager = null;

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{
		if( manager == null )
			manager = RunnerManager.use;
		
		if( interactionManager == null )
			interactionManager = RunnerInteractionManager.use;

		// lookup references to objects / scripts outside of this script
		other1 = (GameObject) GameObject.Instantiate( this.gameObject );
		other2 = (GameObject) GameObject.Instantiate( this.gameObject );
		
		GameObject.Destroy( other1.GetComponent<TreadmillAnimation>() );
		GameObject.Destroy( other2.GetComponent<TreadmillAnimation>() );

		other1.transform.position = other1.transform.position.xAdd( LugusUtil.UIWidth );
		other2.transform.position = other2.transform.position.xAdd( LugusUtil.UIWidth * 2.0f );

		other1.transform.parent = this.transform.parent;
		other2.transform.parent = this.transform.parent;
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}

	//public float offset = 0.0f;

	protected void Update () 
	{
		if( !manager.GameRunning )
			return;

		float progressionPercentage = 1.0f;
		if( (Time.time - interactionManager.startTime) < interactionManager.timeToMax )
		{
			DataRange timeRange = new DataRange( interactionManager.startTime, interactionManager.startTime + interactionManager.timeToMax );
			progressionPercentage = timeRange.PercentageInInterval( Time.time );
		}


		float offset = speedRange.ValueFromPercentage(progressionPercentage) * Time.deltaTime;

		this.transform.position = this.transform.position.xAdd( -offset );
		other1.transform.position = other1.transform.position.xAdd( -offset );
		other2.transform.position = other2.transform.position.xAdd( -offset );

		//Debug.Log("Offset : " + offset + " // " + progressionPercentage );
		//this.GetComponent<SpriteRenderer>().material.SetTextureOffset("_MainTex", new Vector3(offset, 0.0f) );


		if( this.transform.localPosition.x < -LugusUtil.UIWidth )
		{
			this.transform.position = other2.transform.position.xAdd( LugusUtil.UIWidth );
		}

		if( other1.transform.localPosition.x < -LugusUtil.UIWidth )
		{
			other1.transform.position = this.transform.position.xAdd( LugusUtil.UIWidth );
		}

		
		if( other2.transform.localPosition.x < -LugusUtil.UIWidth )
		{
			other2.transform.position = other1.transform.position.xAdd( LugusUtil.UIWidth );
		}
	}
}
