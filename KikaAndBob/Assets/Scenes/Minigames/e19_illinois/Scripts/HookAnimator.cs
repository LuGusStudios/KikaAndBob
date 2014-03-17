using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HookAnimator : MonoBehaviour 
{
	public DataRange angleRange = new DataRange(-20, 20);

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}

	protected ILugusCoroutineHandle swingHandle = null;

	public void OnDisable()
	{
		if( swingHandle != null )
		{
			swingHandle.StopRoutine();
			swingHandle = null;
		}
	}

	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
		swingHandle = LugusCoroutines.use.StartRoutine( SwingRoutine() );
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

	protected IEnumerator SwingRoutine()
	{
		if( Random.value < 0.5f )
		{
			float temp = angleRange.from;
			angleRange.from = angleRange.to;
			angleRange.to = temp;
		}

		this.transform.eulerAngles = new Vector3( 0, 0, angleRange.from );



		Hashtable output = new Hashtable(); 
		output.Add ("time", 1.0f);
		output.Add ("rotation", new Vector3(0, 0, angleRange.to));
		output.Add ("easetype", iTween.EaseType.easeInOutSine);

		while( true )
		{
			output["rotation"] = new Vector3(0, 0, angleRange.to);
			iTween.RotateTo( this.gameObject, output );

			yield return new WaitForSeconds( 1.0f );
			
			output["rotation"] = new Vector3(0, 0, angleRange.from);
			iTween.RotateTo( this.gameObject, output );
			
			yield return new WaitForSeconds( 1.0f );
		}
	}
}
