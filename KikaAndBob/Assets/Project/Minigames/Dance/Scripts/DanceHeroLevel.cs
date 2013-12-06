using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DanceHeroLevel : MonoBehaviour 
{
	public List<DanceHeroLane> lanes = new List<DanceHeroLane>();

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

	protected DanceHeroLane GetLane(string name)
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
		LoadLevelMetallica();
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

		// TODO: option to have the lanes add their delays as one
		// without, this wouldn't work for metallica one, but we want it to:
		/*
		 * 
		lane1.AddItem( 0.0f );
		lane2.AddItem( 0.6f );
		lane3.AddItem( 0.4f );
		lane1.AddItem( 0.4f ); 
		 * 
		 */ 

		lane1.AddItem( 0.1f );
		lane2.AddItem( 0.3f );
		lane3.AddItem( 0.5f );
		lane1.AddItem( 0.7f );

		//lane1.AddItem( 1.0f, KikaAndBob.LaneItemActionType.LEFT );
		//lane1.AddItem( 1.0f, KikaAndBob.LaneItemActionType.LEFT, 1.2f );
		
		//lane3.AddItem( 4.0f, KikaAndBob.LaneItemActionType.RIGHT );
		//lane3.AddItem( 1.0f, KikaAndBob.LaneItemActionType.RIGHT, 1.2f );
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
