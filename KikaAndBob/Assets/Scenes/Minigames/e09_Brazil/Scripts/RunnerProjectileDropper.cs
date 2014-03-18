using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunnerProjectileDropper : MonoBehaviour 
{
	public GameObject projectile = null;
	public DataRange timeBetweenProjectiles = new DataRange( 1.0f, 3.0f );

	protected ILugusCoroutineHandle spawnHandle = null;

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
		if( projectile == null )
		{
			Debug.LogError(transform.Path () + " : projectileDropper has no projectile! disabling this component...");
			this.enabled = false;
		}

	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
		if( !this.enabled ) 
			return;

		OnDisable();
		spawnHandle = LugusCoroutines.use.StartRoutine( ShootRoutine() );
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}

	public void OnDisable()
	{
		if( spawnHandle != null )
		{
			spawnHandle.StopRoutine();
			spawnHandle = null;
		}
	}
	
	protected void Update () 
	{
		
	}

	protected IEnumerator ShootRoutine()
	{
		while( true )
		{
			GameObject newProjectile = (GameObject) GameObject.Instantiate( projectile );
			newProjectile.transform.position = this.transform.position.xAdd( Random.Range(-0.5f, 0.5f) ).yAdd( -0.3f ).zAdd( -1.0f );

			newProjectile.transform.parent = this.transform;

			//GameObject.Destroy( newProjectile, 20.0f );

			yield return new WaitForSeconds( timeBetweenProjectiles.Random () );
		}
	}
}
