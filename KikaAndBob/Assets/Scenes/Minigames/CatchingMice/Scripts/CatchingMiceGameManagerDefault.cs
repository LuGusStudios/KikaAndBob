using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceGameManager : LugusSingletonExisting<CatchingMiceGameManagerDefault>
{
}
public class CatchingMiceGameManagerDefault : IGameManager
{
	protected bool gameRunning = false;
	public float preWaveTime = 30.0f;
	public int collectedPickups = 0;

	#region Accessors
	public int EnemiesAlive
	{
		get
		{
			return enemiesAlive;
		}
		set
		{
			enemiesAlive = value;
		}
	}
	public int PickupCount
	{
		get
		{
			return pickupCount;
		}
		set
		{
			pickupCount = value;

			if (onPickupCountChanged != null)
				onPickupCountChanged(PickupCount);
		}
	}
	public int CurrentWave
	{
		get
		{
			return currentWave;
		}
	}
	#endregion

	#region Events
	public delegate void WaveStartedEventHandler(int waveIndex);
	public event WaveStartedEventHandler onWaveStarted;

	public delegate void WaveEndedEventHandler(int waveIndex);
	public event WaveEndedEventHandler onWaveEnded;

	public delegate void PickupCountChanged(int newAmount);
	public event PickupCountChanged onPickupCountChanged;
	#endregion

	#region Protected
	protected int pickupCount = 0;
	protected int currentWave = 0;
	protected int enemiesAlive = 0;

	protected float timer = 0;

	protected bool infiniteLevel = false;

	protected ILugusCoroutineHandle gameRoutineHandle = null;

	protected LevelLoaderDefault levelLoader = new LevelLoaderDefault();
	#endregion

	public override bool GameRunning
	{
		get { return gameRunning; }
	}

	public float Timer
	{
		get
		{
			return timer;
		}
	}
	public enum State
	{
		PreWave = 1, // Let the player place their traps
		Wave = 2,    // Object spawning and game playing
		PostWave = 3,// After wave has been killed or player lost
		Won = 4,     // All waves has been iterated and it's not an infinite level
		Lost = 5,    // Cheese has been eaten

		NONE = -1
	}

	public delegate void OnGameStateChange(CatchingMiceGameManagerDefault.State state);
	public OnGameStateChange onGameStateChange;

	protected CatchingMiceGameManagerDefault.State state = State.NONE;

	public void SetState(CatchingMiceGameManagerDefault.State st)
	{
		State oldState = state;
		state = st;
		if (state != oldState)
		{
			DoNewStateBehaviour(state);
			if (onGameStateChange != null)
				onGameStateChange(st);
		}
	}

	public void DoNewStateBehaviour(State newState)
	{
		//this can be usefull for ending the PreWavePhase early, so it can start with a wave
		if (gameRoutineHandle != null)
		{
			gameRoutineHandle.StopRoutine();
		}

		switch (newState)
		{
			case State.PreWave:
				gameRoutineHandle = LugusCoroutines.use.StartRoutine(PreWavePhase());
				break;
			case State.Wave:
				gameRoutineHandle = LugusCoroutines.use.StartRoutine(WavePhase());
				break;
			case State.PostWave:
				PostWavePhase();
				break;
			case State.Won:
				CatchingMiceLogVisualizer.use.Log("Game won!");
				WinState();
				break;
			case State.Lost:
				CatchingMiceLogVisualizer.use.Log("Game lost...");
				LoseState();
				break;
			case State.NONE:
				CatchingMiceLogVisualizer.use.LogError("New state can't be none.");
				break;
			default:
				break;
		}
	}

	public IEnumerator PreWavePhase()
	{
		CatchingMiceLogVisualizer.use.Log("Starting pre-wave phase");
		
		CatchingMiceLevelManager.use.InstantiateWave(currentWave);
		yield return new WaitForSeconds(preWaveTime);
		SetState(State.Wave);
	}

	public IEnumerator WavePhase()
	{
		CatchingMiceLogVisualizer.use.Log("Starting wave phase");

		if (onWaveStarted != null)
		{
			onWaveStarted(currentWave);
		}
		
		//spawn waves
		CatchingMiceLevelManager.use.SpawnInstantiatedWave(currentWave);

		//wait until wave is done, or cheese has been eaten
		while (enemiesAlive > 0 && CatchingMiceLevelManager.use.CheeseTiles.Count > 0)
		{
			yield return null;
		}

		SetState(State.PostWave);
		yield break;
	}

	public void PostWavePhase()
	{
		CatchingMiceLogVisualizer.use.Log("Starting post-wave phase");

		if (onWaveEnded != null)
		{
			onWaveEnded(currentWave);
		}

		//check if start next wave (preWavePhase), cheese has been eaten or waves has been iterated
		if (CatchingMiceLevelManager.use.CheeseTiles.Count <= 0)
		{
			Debug.Log("CatchingMiceGameManager: Cheese tiles gone.");
			SetState(State.Lost);
		}
		else if (currentWave + 1 >= CatchingMiceLevelManager.use.Waves.Count)
		{
			Debug.Log("CatchingMiceGameManager: All waves are done and some cheese tiles survive.");

			//can be changed so when you want infinite levels
			SetState(State.Won);
		}
		else
		{
			//still waves left, get next wave
			currentWave++;
			SetState(State.PreWave);
		}
	}

	public void WinState()
	{
		gameRunning = false;

		CatchingMiceLogVisualizer.use.Log("Starting end phase: won");

		string saveKey = Application.loadedLevelName + "_level_" + CatchingMiceCrossSceneInfo.use.GetLevelIndex();
		LugusConfig.use.User.SetBool(saveKey, true, true);
		LugusConfig.use.SaveProfiles();

		CatchingMiceTrapSelector.use.SetVisible(false);

		HUDManager.use.StopAll();

		HUDManager.use.LevelEndScreen.Show(true, 1f);

		HUDManager.use.LevelEndScreen.Counter1.gameObject.SetActive(true);
		HUDManager.use.LevelEndScreen.Counter1.commodity = KikaAndBob.CommodityType.Cheese;
		HUDManager.use.LevelEndScreen.Counter1.SetValue(GetCheeseScore());

		HUDManager.use.LevelEndScreen.Counter2.gameObject.SetActive(true);
		HUDManager.use.LevelEndScreen.Counter2.commodity = KikaAndBob.CommodityType.Cookie;
		HUDManager.use.LevelEndScreen.Counter2.SetValue(collectedPickups);


		HUDManager.use.LevelEndScreen.Counter4.gameObject.SetActive(true);
		HUDManager.use.LevelEndScreen.Counter4.commodity = KikaAndBob.CommodityType.Score;
		HUDManager.use.LevelEndScreen.Counter4.SetValue(GetCheeseScore() + collectedPickups);

		CatchingMiceInputManager.use.ClearAllPaths();

		if (CatchingMiceCrossSceneInfo.use.GetLevelIndex() >= 4)
		{
			LugusConfig.use.User.SetBool("playroom" + "_unlock_" + CatchingMiceCrossSceneInfo.use.GetLevelIndex(), true, true);
			LugusConfig.use.SaveProfiles();
		}
	}

	public void LoseState()
	{
		gameRunning = false;

		CatchingMiceLogVisualizer.use.Log("Starting end phase: lost");

		HUDManager.use.StopAll();
		HUDManager.use.LevelEndScreen.Show(false, 1f);

		CatchingMiceTrapSelector.use.SetVisible(false);

		CatchingMiceInputManager.use.ClearAllPaths();
	}

	public int GetCheeseScore()
	{
		int score = 0;
		
		foreach(CatchingMiceTile tile in CatchingMiceLevelManager.use.CheeseTiles)
		{
			score += Mathf.FloorToInt(tile.cheese.GetHealthPercentage() * 1000);
		}
		
		return score;
	}
	
	public int GetCookieScore()
	{
		int score = 0;
		
		return score;
	}
	
	public override void StartGame()
	{
		collectedPickups = 0;

		string levelData = levelLoader.GetLevelData(CatchingMiceCrossSceneInfo.use.GetLevelIndex());
		
		if (!string.IsNullOrEmpty(levelData))
		{
			CatchingMiceLevelDefinition newLevel = CatchingMiceLevelDefinition.FromXML(levelData);
			CatchingMiceLevelManager.use.CurrentLevel = newLevel;

			if (CatchingMiceLevelManager.use.CurrentLevel != null)
			{
				CatchingMiceLevelManager.use.BuildLevel();
				LugusCoroutines.use.StopAllRoutines();
				gameRunning = true;
	
				if (CatchingMiceLevelManager.use.Waves.Count > 0)
				{
					currentWave = 0;
					SetState(State.PreWave);
				}

				if (CatchingMiceLevelManager.use.Players.Count > 0)
				{
					CatchingMiceCameraMover.use.FocusOnPlayer(CatchingMiceLevelManager.use.Players[0], false);
				}

				CatchingMiceGUI.use.ResetHUD();

				CatchingMiceTrapSelector.use.CreateTrapList(PickupCount);
			}
		}
		else 
		{
			Debug.LogError("CatchingGameManager: Invalid level data!");
		}
	}

	public override void StopGame()
	{
		gameRunning = false;
	}




//	public void SetPaused(bool pause)
//	{
//		Debug.Log("GameManager : setPaused : " + pause);
//		if (pause)
//		{
//			// Try pause
//			if (Paused)
//			{
//				Debug.LogError(transform.Path() + " : IGameManager:SetPaused : game was already paused. Doing nothing");
//				return;
//			}
//
//			// pause
//			Time.timeScale = 0.0001f;
//			// update the physics timestep as well
//			// otherwhise, moving objects with colliders (all our Buttons) wouldn't update collision correctly!
//			Time.fixedDeltaTime = 0.02f * Time.timeScale;
//
//			paused = true;
//
//		}
//		else
//		{
//			// Try unpause
//			if (!Paused)
//			{
//				Debug.LogWarning("GameManager:SetPaused : game was already Unpaused. Doing nothing");
//				return;
//			}
//
//			// unpause
//			Time.timeScale = 1.0f;
//			// update the physics timestep as well
//			// otherwhise, moving objects with colliders (all our Buttons) wouldn't update collision correctly!
//			Time.fixedDeltaTime = 0.02f * Time.timeScale;
//
//			paused = false;
//		}
//	}


	void Start()
	{
		CatchingMiceLevelManager.use.ClearLevel();

		levelLoader.SetLevelLoadCountCap(30);	// the standard cap is 20, but this requires more
		levelLoader.FindLevels();
		
		if (CatchingMiceCrossSceneInfo.use.GetLevelIndex() < 0)
		{
			MenuManager.use.ActivateMenu(MenuManagerDefault.MenuTypes.GameMenu);
		}
		else
		{
			MenuManager.use.ActivateMenu(MenuManagerDefault.MenuTypes.NONE);
			StartGame();
		}
	}
	
	// Update is called once per frame
	protected void Update()
	{
		if (gameRunning)
		{
			timer += Time.deltaTime;
		}

		if (Input.GetKeyDown(KeyCode.A))
		{
			WinState();
		}
	}

	protected void OnGUI()
	{
//		// Display available levels
//		if (levelLoader == null)
//		{
//			levelLoader = new LevelLoaderDefault();
//			levelLoader.FindLevels();
//		}

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


//		// Display level information
//		GUILayout.BeginArea(new Rect(Screen.width - 210, 10, 150, 100));
//		GUILayout.BeginVertical();
//
//		GUILayout.Label("Current phase: " + state);
//		GUILayout.Label("Mice alive: " + enemiesAlive);
//		GUILayout.Label("Cookies found: " + pickupCount);
//		GUILayout.Label("Cheeses left: " + CatchingMiceLevelManager.use.CheeseTiles.Count);
//
//		GUILayout.EndVertical();
//		GUILayout.EndArea();
	}
}
