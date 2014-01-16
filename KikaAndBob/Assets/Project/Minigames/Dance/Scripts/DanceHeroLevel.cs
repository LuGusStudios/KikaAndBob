using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DanceHeroLevel : LugusSingletonRuntime<DanceHeroLevel> 
{
	public enum TimeProgressionMode
	{
		NONE = -1,

		PER_LANE = 1,
		GLOBAL_CUMULATIVE = 2
	}

	public TimeProgressionMode mode = TimeProgressionMode.PER_LANE;

	public List<DanceHeroLane> lanes = new List<DanceHeroLane>();

	public float cumulativeDelay = 0.0f;

	protected ILugusCoroutineHandle endLevelRoutine = null;

	public void SetupLocal()
	{
		if( lanes.Count == 0 )
		{
			lanes.AddRange( GameObject.FindObjectsOfType<DanceHeroLane>() );
		}

		if( lanes.Count == 0 )
		{
			Debug.LogError(name + " : no Lanes in level!");
		}
	}
	
	public void SetupGlobal()
	{
		// levels:
		// - working with state machines : put something on for a certain duration
		// - if duration > single unit, you need to keep the button pressed. If not: single press of button

		// - make it possible to change the speed of the lane in itself (speed of movement)?

		// ex. list.Add( new LaneItem( 25 (offset from previous item START in seconds), Enum.ButtonType, duration [, speedmodifier (in percentage)) )
		// lane.LoadLevel( list )

		// laneItem( 0, Enum.Delay, -x) to adjust the time to next -> so that if we change a part, we can easily keep timing on the next parts?
		// -> is this really needed... probably it is only change the starting one of the sequence and we're good to go?

		// lanes moeten aan/uit kunnen gezet worden

		CreateLevels();

		foreach( DanceHeroLane lane in lanes )
		{
			lane.Begin();
		}
	}

	public DanceHeroLane GetLane(string name)
	{
		foreach( DanceHeroLane lane in lanes )
		{
			if( lane.name == name )
				return lane;
		}

		return null;
	}


	protected void CreateLevels()
	{
		//LoadLevelMetallica();

		if (endLevelRoutine != null && endLevelRoutine.Running)
			endLevelRoutine.StopRoutine();

		cumulativeDelay = 0;
		//LoadLevelChina();
		GetLane("Lane1").defaultActionType = KikaAndBob.LaneItemActionType.LEFT;
		GetLane("Lane2").defaultActionType = KikaAndBob.LaneItemActionType.DOWN;
		GetLane("Lane3").defaultActionType = KikaAndBob.LaneItemActionType.RIGHT;
		LaneLoader.LoadLanes();

		endLevelRoutine = LugusCoroutines.use.StartRoutine(LevelEndRoutine(GetTotalLevelDuration()));
	}

	protected void LoadLevelChina()
	{
		Debug.Log("Loading level China");

		DanceHeroLane lane1 = GetLane("Lane1");
		DanceHeroLane lane2 = GetLane("Lane2");
		DanceHeroLane lane3 = GetLane("Lane3");
		
		lane1.defaultActionType = KikaAndBob.LaneItemActionType.LEFT;
		lane2.defaultActionType = KikaAndBob.LaneItemActionType.DOWN;
		lane3.defaultActionType = KikaAndBob.LaneItemActionType.RIGHT;
		
		
		// GLOBAL_CUMULATIVE
		mode = TimeProgressionMode.GLOBAL_CUMULATIVE;
		lane1.AddItem( 0.0f );
		lane2.AddItem( 0.3f );
		lane3.AddItem( 0.4f );

		lane2.AddItem( 1f );
		lane1.AddItem( 0.3f );
		lane2.AddItem( 0.3f );
		lane3.AddItem( 0.7f );

		lane1.AddItem( 1.2f );
		lane2.AddItem( 0.3f );
		lane3.AddItem( 0.3f );
		lane2.AddItem( 0.3f );

		lane1.AddItem( 6.0f );
		lane2.AddItem( 0.3f );
		lane3.AddItem( 0.3f );
		lane2.AddItem( 0.3f );
		lane1.AddItem( 0.3f );
		lane3.AddItem( 0.3f, 1 );

	}

	protected void LoadLevel1()
	{
		DanceHeroLane lane1 = GetLane("Lane1");
		DanceHeroLane lane2 = GetLane("Lane2");
		DanceHeroLane lane3 = GetLane("Lane3");

		lane2.Hide ();
		
		lane1.AddItem( 1.0f, KikaAndBob.LaneItemActionType.LEFT );
		lane1.AddItem( 1.0f, KikaAndBob.LaneItemActionType.LEFT, 1.2f );

		lane3.AddItem( 4.0f, KikaAndBob.LaneItemActionType.RIGHT );
		lane3.AddItem( 1.0f, KikaAndBob.LaneItemActionType.RIGHT, 1.2f );
	}

	protected void LoadLevelMetallica()
	{

		DanceHeroLane lane1 = GetLane("Lane1");
		DanceHeroLane lane2 = GetLane("Lane2");
		DanceHeroLane lane3 = GetLane("Lane3");

		lane1.defaultActionType = KikaAndBob.LaneItemActionType.LEFT;
		lane2.defaultActionType = KikaAndBob.LaneItemActionType.DOWN;
		lane3.defaultActionType = KikaAndBob.LaneItemActionType.RIGHT;

		
		// GLOBAL_CUMULATIVE
		mode = TimeProgressionMode.GLOBAL_CUMULATIVE;
		lane1.AddItem( 0.0f );
		lane2.AddItem( 0.2f );
		lane3.AddItem( 0.2f );
		lane1.AddItem( 0.3f ); 
		
		lane1.AddItem( 1.6f );
		lane2.AddItem( 0.2f );
		lane3.AddItem( 0.2f );
		lane1.AddItem( 0.3f ); 

		
		lane3.AddItem( 0.6f, 0.8f  ); 

		
		lane1.AddItem( 3.6f );
		lane2.AddItem( 0.2f );
		lane3.AddItem( 0.2f );
		lane1.AddItem( 0.3f ); 

		/*
        // PER_LANE
		lane1.AddItem( 0.1f );
		lane2.AddItem( 0.3f );
		lane3.AddItem( 0.5f );
		lane1.AddItem( 0.7f );
		*/

		//lane1.AddItem( 1.0f, KikaAndBob.LaneItemActionType.LEFT );
		//lane1.AddItem( 1.0f, KikaAndBob.LaneItemActionType.LEFT, 1.2f );
		
		//lane3.AddItem( 4.0f, KikaAndBob.LaneItemActionType.RIGHT );
		//lane3.AddItem( 1.0f, KikaAndBob.LaneItemActionType.RIGHT, 1.2f );
	}

	protected IEnumerator LevelEndRoutine(float levelDuration)
	{
		yield return new WaitForSeconds(levelDuration);
		
		LevelFinished();
	}
	
	protected void LevelFinished()
	{
		Debug.Log("Level finished!");
	}
	
	protected float GetTotalLevelDuration()
	{
		// total level length consists of a number of things:
		// the amount of time it takes the first point of a lane to reach the input point
		// the total delay of that lane
		// the duration of the last item in that lane
		// we compare all lanes and see which one is the longest in total, based on the values above
		// finally, add a little delay so the level's not immediately over

		float longestLaneDuration = 0;

		foreach(DanceHeroLane lane in lanes)
		{
			if (lane.GetFullDuration() > longestLaneDuration)
				longestLaneDuration = lane.GetFullDuration();
		}
		
		return longestLaneDuration + 0;
	}

	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}

	float interval = 0;
	protected void Update () 
	{
		interval += Time.deltaTime;
		if (Input.GetKeyDown(KeyCode.Space))
			interval = 0;
	}

	void OnGUI()
	{
		if (!LugusDebug.debug)
			return;

		GUILayout.Box(Time.time.ToString("0.000"));
		GUILayout.Box(interval.ToString("0.000"));
	}
}
