using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceGameManager : LugusSingletonExisting<CatchingMiceGameManagerDefault>
{
}
public class CatchingMiceGameManagerDefault : MonoBehaviour
{
	public bool gameRunning = false;
	public float preWaveTime = 30.0f;

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
	public event WaveStartedEventHandler WaveStarted;

	public delegate void WaveEndedEventHandler(int waveIndex);
	public event WaveEndedEventHandler WaveEnded;
	#endregion

	#region Protected
	protected int pickupCount = 0;
	protected int currentWave = 0;
	protected int enemiesAlive = 0;

	protected float timer = 0;

	protected bool infiniteLevel = false;
	protected bool paused = false;
	
	protected ILugusCoroutineHandle gameRoutineHandle = null;

	protected CatchingMiceLevelLoader levelLoader = null;
	#endregion

	public bool GameRunning
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

	public delegate void OnGameStateChange();
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
				onGameStateChange();
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

		if (WaveStarted != null)
		{
			WaveStarted(currentWave);
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

		if (WaveEnded != null)
		{
			WaveEnded(currentWave);
		}

		currentWave++;

		//check if start next wave (preWavePhase), cheese has been eaten or waves has been iterated
		if (currentWave > CatchingMiceLevelManager.use.Waves.Count - 1)
		{
			//can be changed so when you want infinite levels
			SetState(State.Won);
		}
		else if (CatchingMiceLevelManager.use.CheeseTiles.Count <= 0)
		{
			SetState(State.Lost);
		}
		else
		{
			//still waves left, get next wave
			SetState(State.PreWave);
		}
	}

	public void WinState()
	{
		CatchingMiceLogVisualizer.use.Log("Starting end phase: won");
	}

	public void LoseState()
	{
		CatchingMiceLogVisualizer.use.Log("Starting end phase: lost");
	}

	// TODO: Something has to be done in here when the level selection menu is live
	public void StartGame()
	{
		CatchingMiceLevelManager.use.ClearLevel();

		// TODO: Modify this when the level selection menu goes live
		if (levelLoader == null)
		{
			levelLoader = new CatchingMiceLevelLoader();
			levelLoader.FindLevels();
		}

		if (CatchingMiceCrossSceneInfo.use.LevelToLoad == -1)
		{
			List<int> indexes = levelLoader.levelIndices;
			if (indexes.Count > 0)
			{
				CatchingMiceCrossSceneInfo.use.LevelToLoad = indexes[0];
			}
			else
			{
				CatchingMiceLogVisualizer.use.LogWarning("No levels could be found!");
				return;
			}
		}
		
		string levelData = levelLoader.GetLevelData(CatchingMiceCrossSceneInfo.use.LevelToLoad);
		CatchingMiceLevelDefinition levelDefinition = CatchingMiceLevelDefinition.FromXML(levelData);
		CatchingMiceLevelManager.use.CurrentLevel = levelDefinition;

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
		}	
	}

	public void StopGame()
	{

	}

	public virtual void ReloadLevel()
	{
		Application.LoadLevel(Application.loadedLevelName);
	}

	public bool Paused
	{
		get { return paused; }
		set
		{
			SetPaused(value);
		}
	}

	public void SetPaused(bool pause)
	{
		Debug.Log("GameManager : setPaused : " + pause);
		if (pause)
		{
			// Try pause
			if (Paused)
			{
				Debug.LogError(transform.Path() + " : IGameManager:SetPaused : game was already paused. Doing nothing");
				return;
			}

			// pause
			Time.timeScale = 0.0001f;
			// update the physics timestep as well
			// otherwhise, moving objects with colliders (all our Buttons) wouldn't update collision correctly!
			Time.fixedDeltaTime = 0.02f * Time.timeScale;

			paused = true;

		}
		else
		{
			// Try unpause
			if (!Paused)
			{
				Debug.LogWarning("GameManager:SetPaused : game was already Unpaused. Doing nothing");
				return;
			}

			// unpause
			Time.timeScale = 1.0f;
			// update the physics timestep as well
			// otherwhise, moving objects with colliders (all our Buttons) wouldn't update collision correctly!
			Time.fixedDeltaTime = 0.02f * Time.timeScale;

			paused = false;
		}
	}

	// Use this for initialization
	void Start()
	{
		StartGame();
	}
	
	// Update is called once per frame
	protected void Update()
	{
		if (gameRunning)
		{
			timer += Time.deltaTime;
		}
	}

	protected void OnGUI()
	{
		// Display available levels
		if (levelLoader == null)
		{
			levelLoader = new CatchingMiceLevelLoader();
			levelLoader.FindLevels();
		}

		GUILayout.BeginArea(new Rect(10, 10, 150, 25 * (levelLoader.levelIndices.Count + 2)));
		GUILayout.BeginVertical();

		GUILayout.Label("Catching Mice Levels:");

		if (GUILayout.Button("Refresh List"))
		{
			levelLoader.FindLevels();
		}

		for (int i = 0; i < levelLoader.levelIndices.Count; ++i)
		{
			if (GUILayout.Button("Level " + levelLoader.levelIndices[i]))
			{
				CatchingMiceCrossSceneInfo.use.LevelToLoad = levelLoader.levelIndices[i];
				Application.LoadLevel(Application.loadedLevelName);
			}
		}

		GUILayout.EndVertical();
		GUILayout.EndArea();

		// Display level information
		GUILayout.BeginArea(new Rect(Screen.width - 210, 10, 150, 100));
		GUILayout.BeginVertical();

		GUILayout.Label("Current phase: " + state);
		GUILayout.Label("Mice alive: " + enemiesAlive);
		GUILayout.Label("Cookies found: " + pickupCount);
		GUILayout.Label("Cheeses left: " + CatchingMiceLevelManager.use.CheeseTiles.Count);

		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
}
