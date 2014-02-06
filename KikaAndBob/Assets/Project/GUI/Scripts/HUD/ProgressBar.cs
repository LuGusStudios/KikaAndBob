using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProgressBar : MonoBehaviour 
{
	public DataRange scaleRange = null;
	public DataRange valueRange = null;

	protected Transform filler = null;

	public void SetValue(float value, bool animate = true)
	{
		if( valueRange == null )
		{
			Debug.LogError( transform.Path () + " : Cannot set value directly: no valueRange known!" );
			return;
		}

		float percentage = valueRange.PercentageInInterval(value);
		SetPercentage( percentage, animate );
	}

	protected ILugusCoroutineHandle animationHandle = null;

	public void SetPercentage(float percentage, bool animate = true )
	{
		float oldScale = filler.localScale.x;
		float newScale = scaleRange.ValueFromPercentage( percentage ); 

		if( !animate )
		{
			filler.localScale = filler.localScale.x (newScale);
		}
		else
		{
			if( animationHandle != null )
			{
				iTween.Stop( filler.gameObject );
				animationHandle.StopRoutine();
				animationHandle = null; 
			}

			animationHandle = LugusCoroutines.use.StartRoutine( ScaleAnimationRoutine(oldScale, newScale) );
		}
	}

	protected IEnumerator ScaleAnimationRoutine(float oldScale, float newScale)
	{
		filler.gameObject.ScaleTo( new Vector3(newScale, 0, 0) ).Time (1.0f).Execute();
		yield break;
	}

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
		if( filler == null )
		{
			transform.FindChild("Bar/Filler");
		}
		if( filler == null )
		{
			Debug.LogError( transform.Path () + " : No filler found!" );
		}
		else
		{
			if( scaleRange == null )
			{
				// we assume the filler is scaled all up to 100% (0% is 0 scale then)
				scaleRange = new DataRange(0, filler.localScale.x);
			}
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
	
	}
}
