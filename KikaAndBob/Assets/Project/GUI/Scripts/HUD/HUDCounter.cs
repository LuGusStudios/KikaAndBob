using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUDCounter : IHUDElement 
{
	public enum Formatting
	{
		NONE = -1,

		Int = 1,
		Float = 4,
		TimeS = 2,
		TimeMS = 3,
	}

	public TextMeshWrapper text = null;

	public string prefix = "";
	public string suffix = "";

	public Formatting formatting = Formatting.Int;

	public float currentValue = 0.0f;

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
		if( icon == null )
		{
			icon = transform.FindChild("Icon").GetComponent<SpriteRenderer>();
		}

		if( text == null )
		{
			text = transform.FindChild("Text").GetComponent<TextMeshWrapper>();
		}
	}

	public override void Stop()
	{
		if( timerMode )
			StopTimer();
	}

	public override void AddValue(float value, bool animate = true)
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

		SetValue( value, animate );
	}

	public override void SetValue(float value, bool animate = true)
	{
		//Debug.LogError (transform.Path () + " : SetValue "+ value ); 
		
		float oldValue = currentValue;

		if( !timerMode )
		{
			currentValue = value; 
		}
		else
		{
			// no animation if in timerMode, would fuck up the timer's constant counting
			animate = false;
		}


		string text = "";
		if( this.formatting == Formatting.TimeS )
		{	
			//Debug.LogWarning(transform.Path () + " : SetValue "+ (value / 1000.0f) ); 

			int secs = Mathf.FloorToInt(value);
			text += secs + "s";
		}
		else if( this.formatting == Formatting.TimeMS )
		{
			int secs = Mathf.FloorToInt(value);
			int millisecs = Mathf.FloorToInt((value - secs) * 100.0f);

			//Debug.Log ("MILLI " + millisecs + " // " + value + " // " + secs);


			text += secs + "s";
			
			if( millisecs < 10 )
				text += "0" + millisecs; //+ "ms";
			else
				text += millisecs;// + "ms";
		}
		else // int or float
		{
			if( !animate || (Mathf.Abs(currentValue - oldValue) <= 1.0f) )
			{
				text = "" + value;
			}
			else
			{
				if( animationHandle != null )
				{
					animationHandle.StopRoutine();
					animationHandle = null; 
				}
				
				animationHandle = LugusCoroutines.use.StartRoutine( AnimationRoutine(oldValue, currentValue) );
			}
		}

		SetText( text );
	}
	
	
	protected ILugusCoroutineHandle animationHandle = null;
	protected IEnumerator AnimationRoutine(float oldValue, float newValue)
	{
		DataRange timeRange = new DataRange(Time.time, Time.time + 1.0f);
		DataRange valueRange = new DataRange(oldValue, newValue);

		while( Time.time < timeRange.to )
		{
			float val = timeRange.PercentageInInterval( Time.time );
			val = valueRange.ValueFromPercentage(val);
			
			//Debug.LogError( " ANIMATING " + val + " // " + this.formatting);

			if( formatting == Formatting.Int )
				val = Mathf.FloorToInt(val);

			SetText( "" + val );

			yield return null;
		}

		SetText( "" + newValue );
	}


	public void SetText(string content)
	{
		text.textMesh.text = prefix + "" + content + "" + suffix;
		text.UpdateWrapping();
	}

	protected bool timerMode = false;
	protected ILugusCoroutineHandle timerHandle = null;
	public void StartTimer(Formatting formatting = Formatting.TimeS)
	{
		timerMode = true;
		this.formatting = formatting;
		timerHandle = LugusCoroutines.use.StartRoutine( TimerRoutine(Time.time) );
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
	
	protected IEnumerator TimerRoutine(float startTime)
	{
		while( true )
		{
			SetValue( (Time.time - startTime) + currentValue );
			yield return null;
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
