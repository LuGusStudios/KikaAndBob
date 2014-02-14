using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DanceHeroLevel : LugusSingletonRuntime<DanceHeroLevel> 
{
	public AudioClip musicClip; // TO DO: Remove

	public delegate void OnLevelStarted();
	public OnLevelStarted onLevelStarted = null;

	public delegate void OnLevelFinished();
	public OnLevelFinished onLevelFinished = null;

	public enum TimeProgressionMode
	{
		NONE = -1,

		PER_LANE = 1,
		GLOBAL_CUMULATIVE = 2
	}

	public TimeProgressionMode mode = TimeProgressionMode.PER_LANE;
	public List<DanceHeroLane> lanes = new List<DanceHeroLane>();
	public float cumulativeDelay = 0.0f;
	public int currentLevel = 0;
	public string[] levels;

	protected ILugusCoroutineHandle endLevelRoutine = null;
	protected ILugusAudioTrack backgroundMusic = null;
	protected LevelLoaderDefault levelLoader = new LevelLoaderDefault();

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


	
//	public void CreateLevel()
//	{
//		CreateLevel(currentLevel);
//	}
//
//	public void CreateLevel(int index)
//	{
//		if (index < 0 || index >= levels.Length)
//		{
//			Debug.LogError("DanceHeroLevel: Level index was out of bounds: " + index);
//			return;
//		}
//
//		CreateLevel(levels[index]);
//	}
	
	public void CreateLevel(string levelData)
	{
		Debug.Log("DanceHeroLevel: Clearing lanes.");
		foreach(DanceHeroLane lane in lanes)
		{
			lane.ClearLaneItems();
		}

		if (string.IsNullOrEmpty(levelData))
		{
			Debug.LogError("DanceHeroLevel: Level data was null or empty.");
			return;
		}

		// add new level items
		// also, set music file

		
		cumulativeDelay = 0;

		GetLane("Lane1").defaultActionType = KikaAndBob.LaneItemActionType.LEFT;
		GetLane("Lane2").defaultActionType = KikaAndBob.LaneItemActionType.DOWN;
		GetLane("Lane3").defaultActionType = KikaAndBob.LaneItemActionType.RIGHT;

		ParseLevelFromXML(levelData);

		LugusCoroutines.use.StartRoutine(MusicBufferDelay());
	}

	protected IEnumerator MusicBufferDelay()
	{
		Debug.Log("DanceHeroLevel: Waiting for song to be done buffering.");

		backgroundMusic = LugusAudio.use.Music().Play(musicClip); // replace with call to LugusResources

		while(backgroundMusic.Playing == false)
		{
			yield return new WaitForEndOfFrame();
		}

		if (endLevelRoutine != null && endLevelRoutine.Running)
			endLevelRoutine.StopRoutine();
		
		endLevelRoutine = LugusCoroutines.use.StartRoutine(LevelEndRoutine(GetTotalLevelDuration()));
		
		foreach( DanceHeroLane lane in lanes )
		{
			lane.Begin();
		}
		
		if (onLevelStarted != null)
		{
			onLevelStarted();
		}
		
		Debug.Log("Finished setting up new level.");
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

	

		/*
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

		// copy below
		lane2.AddItem( 1.3f );
		lane3.AddItem( 0.4f );
		
		lane2.AddItem( 1f );
		lane1.AddItem( 0.3f );
		lane2.AddItem( 0.3f );
		lane3.AddItem( 0.7f );
		
		lane1.AddItem( 1.2f );
		lane2.AddItem( 0.3f );
		lane3.AddItem( 0.3f );
		lane2.AddItem( 0.3f, 1 );*/
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
		if (onLevelFinished != null)
		{
			onLevelFinished();
		}

		HUDManager.use.LevelEndScreen.Show(true);

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

		levelLoader.FindLevels();

		if (DanceHeroCrossSceneInfo.use.GetLevelIndex() < 0)
		{
			MenuManager.use.ActivateMenu(MenuManagerDefault.MenuTypes.GameMenu);
		}
		else
		{
			MenuManager.use.ActivateMenu(MenuManagerDefault.MenuTypes.NONE);

			string levelData = levelLoader.GetLevelData(DanceHeroCrossSceneInfo.use.GetLevelIndex());
			
			if (!string.IsNullOrEmpty(levelData))
			{
				CreateLevel(levelData);
			}
			else
			{
				Debug.LogError("DanceHeroLevel: Invalid level data!");
			}

			DanceHeroFeedback.use.ResetGUI();
		}
	}

	void OnGUI()
	{
		if (!LugusDebug.debug)
			return;

		GUILayout.BeginVertical();

		foreach(int index in levelLoader.levelIndices)
		{
			if (GUILayout.Button("Load level: " + index))
			{
				levelLoader.LoadLevel(index);
			}
		}
		
		GUILayout.EndVertical();
	}

	void ParseLevelFromXML(string rawData)
	{
		TinyXmlReader parser = new TinyXmlReader(rawData);
		DanceHeroLevel.use.mode = DanceHeroLevel.TimeProgressionMode.PER_LANE;

		int laneCount = 0;
		while (parser.Read())
		{
			if ((parser.tagType == TinyXmlReader.TagType.OPENING) && (parser.tagName == "AudioClip"))
			{
				DanceHeroLevel.use.musicClip = LugusResources.use.Shared.GetAudio(parser.content);
			}
			else if ((parser.tagType == TinyXmlReader.TagType.OPENING) && (parser.tagName == "Lane"))
			{
				string laneName = "Lane" + (lanes.Count - laneCount).ToString();
				DanceHeroLane lane = GetLane(laneName);
				lane.ParseLaneFromXML(parser);
				++laneCount;
			}
		}
	}
}
