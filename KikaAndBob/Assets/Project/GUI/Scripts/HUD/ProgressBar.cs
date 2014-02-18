using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProgressBar : IHUDElement 
{
	public DataRange scaleRange = null;
	public DataRange valueRange = null;

	protected Transform filler = null;

	protected float currentValue = 0.0f;

	protected Vector3 originalFillerScale = Vector3.one;
	
	public override void Stop()
	{
		if( timerMode )
			StopTimer();
	}

	public override void AddValue(float value, bool animate = true, float minValue = Mathf.NegativeInfinity, float maxValue = Mathf.Infinity)
	{
		if( timerMode )
		{
			currentValue += value;
			value = currentValue;
		}
		else
		{
			value = currentValue + value;
		}

		value = Mathf.Clamp(value, minValue, maxValue);

		SetValue( value, animate );
	}

	public override void SetValue(float value, bool animate = true)
	{
		if( !timerMode )
		{
			currentValue = value;
		}
		else
		{
			// no animation if in timerMode, would fuck up the timer's constant counting
			animate = false;
		}

		if( valueRange == null )
		{
			Debug.LogError( transform.Path () + " : Cannot set value directly: no valueRange known!" );
			return;
		}

		float percentage = valueRange.PercentageInInterval(value);

		//Debug.LogError(transform.Path() + " : PROGRESSBAR " + value + " -> " + percentage + " in " + valueRange.from + " - " + valueRange.to);

		SetPercentage( percentage, animate );
	}

	protected ILugusCoroutineHandle animationHandle = null;

	public void SetPercentage(float percentage, bool animate = true )
	{
		float oldScale = filler.localScale.x;
		float newScale = scaleRange.ValueFromPercentage( percentage ); 
		//Debug.Log ("Set Percentage " + percentage + " // " + newScale + " //" + scaleRange.from + "-" + scaleRange.to);

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
		//Debug.LogError("SCALEROUTINE " + oldScale + " // " + newScale); 
		filler.gameObject.ScaleTo( originalFillerScale.x (newScale) ).EaseType(iTween.EaseType.easeOutBack).Time (0.5f).Execute();
		yield break;
	}

	protected bool timerMode = false;

	
	protected ILugusCoroutineHandle timerHandle = null;

	public void SetTimer(float seconds)
	{
		timerMode = true;
		valueRange = new DataRange(Time.time, Time.time + seconds);

		timerHandle = LugusCoroutines.use.StartRoutine( TimerRoutine() );
	}

	public void StopTimer()
	{
		if( timerHandle != null && timerHandle.Running )
		{
			timerHandle.StopRoutine();
			timerHandle = null;
		}
		
		timerMode = false;
	}

	protected IEnumerator TimerRoutine()
	{
		float percentage = 0.0f;
		while( Time.time < valueRange.to )
		{
			percentage = valueRange.PercentageInInterval(Time.time + currentValue);
			SetPercentage( percentage, false );

			yield return null;
		}
	}

	public void SetupLocal()
	{
		if( icon == null )
		{
			icon = transform.FindChild("Icon").GetComponent<SpriteRenderer>();
		}

		// assign variables that have to do with this class only
		if( filler == null )
		{
			filler = transform.FindChild("Bar/Filler");
		}

		if( filler == null )
		{
			Debug.LogError( transform.Path () + " : No filler found!" );
		}
		else
		{
			// we assume the filler is scaled all up to 100% (0% is 0 scale then)
			scaleRange = new DataRange(0, filler.localScale.x);
			originalFillerScale = filler.localScale; 
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
