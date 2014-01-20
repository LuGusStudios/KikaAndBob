using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JumpingPiranha : MonoBehaviour 
{
	public float yAdd = 4.5f;

	protected IEnumerator JumpingRoutine()
	{
		Vector3 low = this.transform.localPosition;
		Vector3 high = low.yAdd ( yAdd );

		float animationTime = 0.6f;


		while( true )
		{
			gameObject.MoveTo( high ).Time (animationTime).EaseType(iTween.EaseType.easeOutSine).IsLocal(true).Execute();

			yield return new WaitForSeconds(animationTime);
			
			gameObject.MoveTo( low ).Time (animationTime).EaseType(iTween.EaseType.easeInSine).IsLocal(true).Execute();
			
			yield return new WaitForSeconds(animationTime + 1.0f);
		}
	}

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	protected ILugusCoroutineHandle handle = null;
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script

		handle = LugusCoroutines.use.StartRoutine( JumpingRoutine() );
	}

	
	public void OnDestroy()
	{
		if( handle != null )
		{
			handle.StopRoutine();
		}
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
