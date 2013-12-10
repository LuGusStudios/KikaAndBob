using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DartsDragon : IDartsHitable 
{
	public override void OnHit()
	{
		LugusCoroutines.use.StartRoutine( HitRoutine() );
	}

	protected IEnumerator HitRoutine()
	{
		Vector3 originalPos = this.transform.position;

		this.transform.position = LugusUtil.OFFSCREEN;

		yield return new WaitForSeconds(1.0f);

		this.transform.position = originalPos;
	}

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
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
	
	}
}
