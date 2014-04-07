using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MJ : MonoBehaviour 
{
	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script

		LugusCoroutines.use.StartRoutine( MoonwalkRoutine() );
	}

	protected IEnumerator MoonwalkRoutine()
	{
		IDinnerDashManager manager = DinnerDashManager.use;

		while( !manager.GameRunning )
		{
			yield return null;
		}

		// when game started, still wait a little while to make sure level is actually loaded
		yield return new WaitForSeconds(0.5f);

		if( manager.timeout <= 0.0f ) // moneyScore
		{
			Debug.LogError("MJ starting in 30 seconds");
			yield return new WaitForSeconds(30.0f); // show MJ after 30 seconds of playtime in the tutorials
		}
		else
		{
			// show MJ halfway through
			Debug.LogError("MJ starting in "+ (manager.timeout / 2.0f) +" seconds");
			yield return new WaitForSeconds( manager.timeout / 2.0f );
		}
		
		gameObject.MoveTo( this.transform.position.x ( this.transform.position.x - 3000 ) ).Time (7.0f).Execute();

		yield return new WaitForSeconds( 3.5f );

		LugusAudio.use.SFX().Play( LugusResources.use.Shared.GetAudio("Jackson01") );

		yield return new WaitForSeconds( 5.0f ); // wait long enough so we're surely done

		this.gameObject.SetActive(false);
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
}
