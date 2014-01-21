using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugByTaps : MonoBehaviour 
{
	public bool activated = false;
	
	public bool topLeft = false;
	public bool topRight = false;
	public bool bottomRight = false;
	public bool bottomLeft = false;
	
	protected float resetFutureTime = 0.0f;
	
	protected bool HasTouch( Rect screenArea )
	{
		if( LugusInput.use.down && screenArea.Contains(LugusInput.use.lastPoint) )
			return true;
		
		return false;
	}


	void Update () 
	{
		// Touchpoints are bottom left based...
		// so top left is x = 0, y = max 

		// required pattern:
		// - top left
		// - top right
		// - bottom right
		// - bottom left
		// this within 5 seconds from the first touch
		
		if( topLeft && topRight && bottomRight && bottomLeft )
		{
			LugusDebug.debug = !LugusDebug.debug;

			resetFutureTime = Time.time - 1; 
		}
		else if(topLeft && topRight && bottomRight)
		{
			bottomLeft = HasTouch( new Rect(0, 0, 100, 100) );
		}
		else if(topLeft && topRight )
		{
			bottomRight = HasTouch( new Rect(Screen.width - 100, 0, 100, 100) );
		}
		else if( topLeft )
		{
			topRight = HasTouch( new Rect(Screen.width - 100, Screen.height - 100, 100, 100) );
		}
		else
		{
			topLeft = HasTouch( new Rect(0, Screen.height - 100, 100, 100) );
			resetFutureTime = Time.time + 5;
		}
		
		
		
		if( Time.time > resetFutureTime )
		{
			topLeft = false;
			topRight = false;
			bottomRight = false;
			bottomLeft = false;
		}
		
		//TrackingComponentBase.Touches;

		CalculateFPS();
	}

	public void OnGUI()
	{
		if( !LugusDebug.debug )
			return;

		GUILayout.BeginArea( new Rect(Screen.width - 200, 0, 200, Screen.height) );

		List<string> levels = new List<string>();  
		levels.Add( "DanceBuilder" );
		levels.Add( "e02_argentina" ); 
		levels.Add( "FroggerBuilder" );
		levels.Add( "PacmanBuilder" );
		levels.Add( "RunnerBuilder" );  
		levels.Add( "DartsBuilder" ); 

		foreach( string level in levels )
		{
			if( GUILayout.Button( "" + level) )
			{
				Application.LoadLevel( level );
			}

			if( Screen.height > 1000 )
			{
				GUILayout.Space( 20 );
			}
		}

		GUILayout.EndArea();

		DrawFPS();
	}


	// It calculates frames/second over each updateInterval,
	// so the display does not keep changing wildly.
	//
	// It is also fairly accurate at very low FPS counts (<10).
	// We do this not by simply counting frames per interval, but
	// by accumulating FPS for each frame. This way we end up with
	// correct overall FPS even if the interval renders something like
	// 5.5 frames.
	
	public  float updateInterval = 0.5F;
	
	private float accum   = 0; // FPS accumulated over the interval
	private int   frames  = 0; // Frames drawn over the interval
	private float timeleft; // Left time for current interval
	private float fps;
	//private int drawCalls;
	//public float posX = 200;
	//public float posY = 10;
	public Color textCol = Color.black;
	
	void Start()
	{
		//#if !UNITY_EDITOR  
		QualitySettings.vSyncCount = 1;
		//Time.fixedDeltaTime = 0.0333333f;
		//Application.targetFrameRate = 30;
		//Debug.LogError("Set the framerate to 30");
		//#endif


		timeleft = updateInterval;  
	}
	
	void CalculateFPS()
	{
		#if UNITY_EDITOR
		//Application.targetFrameRate = -1;  
		#endif
		
		timeleft -= Time.deltaTime;
		accum += Time.timeScale/Time.deltaTime;
		++frames;
		
		// Interval ended - update GUI text and start new interval
		if( timeleft <= 0.0 )
		{
			
			fps = accum/frames;
			//	DebugConsole.Log(format,level);
			timeleft = updateInterval;
			accum = 0.0F;
			frames = 0;
			//drawCalls = UnityStats.drawCalls;
		}		
	}
	
	void DrawFPS ()
	{
		//GUI.contentColor = textCol;
		float width = 120;
		float posX = Screen.width / 2.0f - (width / 2.0f);
		float posY = 0;
		GUI.TextField(new Rect(posX, posY, width, 40), "FPS: " + fps.ToString("F1") + " / " + Application.targetFrameRate + "\n" + Screen.width + " / " + Screen.height);
	}





	/*
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
	*/
}
