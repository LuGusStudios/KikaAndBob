using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUDCounter : IHUDElement 
{
	public enum Formatting
	{
		NONE = -1,

		Text = 1,
		TimeS = 2,
		TimeMS = 3,
	}

	public TextMeshWrapper text = null;

	public string prefix = "";
	public string suffix = "";

	public Formatting formatting = Formatting.Text;

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

	public void SetValue(float value)
	{
		string text = "";
		if( this.formatting == Formatting.TimeS )
		{	
			int secs = Mathf.FloorToInt(value);
			text += secs + "s ";
		}
		else if( this.formatting == Formatting.TimeMS )
		{
			int secs = Mathf.FloorToInt(value);
			int millisecs = Mathf.FloorToInt((value - secs) * 100.0f);

			//Debug.Log ("MILLI " + millisecs + " // " + value + " // " + secs);


			text += secs + "s";
			
			if( millisecs < 10 )
				text += "0" + millisecs;// + "ms";
			else
				text += millisecs;// + "ms";
		}
		else
			text = "" + value;

		SetText( text );
	}

	public void SetText(string content)
	{
		text.textMesh.text = prefix + "" + content + "" + suffix;
	}


	protected ILugusCoroutineHandle timerHandle = null;
	public void StartTimer(Formatting formatting = Formatting.TimeS)
	{
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
	}
	
	protected IEnumerator TimerRoutine(float startTime)
	{
		while( true )
		{
			SetValue( Time.time - startTime );
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
