using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanGameManager : LugusSingletonExisting<PacmanGameManagerDefault> {
}

public class PacmanGameManagerDefault : IGameManager {

	public bool gameRunning = false;
	protected float timer = 0.0f;
	protected int lives = 3;
	protected bool gameDone = false;
	protected List<PacmanEnemyCharacter> enemies = new List<PacmanEnemyCharacter>();
	protected PacmanPlayerCharacter activePlayer;
	protected Transform level = null;
	protected List<PacmanPlayerCharacter> playerChars = new List<PacmanPlayerCharacter>();
	protected int currentLevelIndex = 0;
	protected LevelLoaderDefault levelLoader = new LevelLoaderDefault();

	public override void StartGame()
	{

	}

	public override void StopGame()
	{
		
	}

	public override bool GameRunning
	{
		get{ return gameRunning; }
	}

	protected void Awake () 
	{
		SetupLocal();
	}

	public void SetupLocal()
	{
		level = GameObject.Find("LevelRoot").transform;
	}

	protected void Start()
	{
		levelLoader.FindLevels();
		
		PacmanLevelManager.use.ClearLevel();
		
		if (PacmanCrossSceneInfo.use.GetLevelIndex() < 0)
		{
			MenuManager.use.ActivateMenu(MenuManagerDefault.MenuTypes.GameMenu);
		}
		else
		{
			MenuManager.use.ActivateMenu(MenuManagerDefault.MenuTypes.NONE);
			
			string levelData = levelLoader.GetLevelData(PacmanCrossSceneInfo.use.GetLevelIndex());
			
			if (!string.IsNullOrEmpty(levelData))
			{
				PacmanLevelDefinition newLevel = PacmanLevelDefinition.FromXML(levelData);
				PacmanLevelManager.use.levels = new PacmanLevelDefinition[]{newLevel};
			}
			else
			{
				Debug.LogError("FroggerGameManager: Invalid level data!");
			}
			
			// if a level wasn't found above, we can still load a default level
			StartNewLevel();
		}
	}

	protected void Update()
	{
		if (!gameRunning || Paused)
			return;


		timer += Time.deltaTime;
	}

	public List<PacmanPlayerCharacter> GetPlayerChars()
	{
		return playerChars;
	}

	// starts a completely new level
	public void StartNewLevel()
	{
		StartNewLevel(currentLevelIndex);
	}

	// starts a completely new level
	public void StartNewLevel(int levelIndex)
	{
		PacmanPickups.use.ClearPickups();
		PacmanCameraFollower.use.ResetCamera();

		PacmanLevelManager.use.BuildLevel(levelIndex);
		
		// find and reset any player characters
		playerChars.Clear();
		// NOTE: GetComponentsInChildren skips inactive objects! This is intentional because the Destroy method used to clear the level does not act immediately.
		playerChars = new List<PacmanPlayerCharacter>(level.GetComponentsInChildren<PacmanPlayerCharacter>());

		if (playerChars.Count >= 1)
		{
			activePlayer = playerChars[0];
		}
		else
		{
			Debug.LogError("No player characters found!");
			return;
		}

		ResetPlayerChars();

		// find and reset enemy characters
		enemies.Clear();
		// NOTE: GetComponentsInChildren skips inactive objects! This is intentional because the Destroy method used to clear the level does not act immediately.
		enemies = new List<PacmanEnemyCharacter>(level.GetComponentsInChildren<PacmanEnemyCharacter>());
		ResetEnemies();

		// start character spawn routine (will only enable certain enemies after a time set in the level definition has passed)
		PacmanLevelManager.use.StartCharacterSpawnRoutine();
		
		// start all level updaters
		foreach (PacmanLevelUpdater updater in (PacmanLevelUpdater[]) FindObjectsOfType(typeof(PacmanLevelUpdater)))
		{
			updater.Activate();
		}

		// reset sound effects
		PacmanSoundEffects.use.Reset(enemies);

		// reset lives
		lives = 3;

		PacmanGUIManager.use.ResetGUI();
		PacmanGUIManager.use.UpdateLives(lives);

		gameRunning = true;

		Debug.Log("Finished starting up new level.");
	}

	// TO DO: make this useful
	// Idea is to allow multiple characters, e.g. Kika and Bob, but only one is active at a time
	public PacmanPlayerCharacter GetActivePlayer()
	{
		// currently just returns the first of any placed players - multiple player characters isn't implemented currently
		return activePlayer;
	}

	// starts new round in the same level
	public void StartNewRound()
	{
		ResetPlayerChars();

		ResetEnemies();

		PacmanLevelManager.use.StartCharacterSpawnRoutine();
		
		gameRunning = true;

		Debug.Log("Finished starting up new round.");
	}
	
	protected void ResetPlayerChars()
	{
		foreach(PacmanPlayerCharacter ppc in playerChars)
		{
			ppc.Reset();
		}
	}
	
	protected void ResetEnemies()
	{
		foreach(PacmanEnemyCharacter target in enemies)
		{
			target.Reset();
		}
	}

	void EnableEnemy(PacmanEnemyCharacter target)
	{
		foreach (SpriteRenderer spriteRenderer in (SpriteRenderer[])target.gameObject.GetComponentsInChildren<SpriteRenderer>())
		{
			spriteRenderer.enabled = true;
		}

		foreach (SkinnedMeshRenderer skinnedmeshRenderer in (SkinnedMeshRenderer[])target.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
		{
			skinnedmeshRenderer.enabled = true;
		}

		target.enabled = true;
	}
	
	public void ResetGame()
	{
		timer = 0.0f;
		lives = 3;
		gameDone = false;
		
		StopAllCoroutines();
		
		Debug.Log("Game manager: Game reset.");
	}
	
	public void LoseLife()
	{
		Debug.Log ("Lost one life!");

		LugusCoroutines.use.StartRoutine(LoseLifeRoutine());
	}

	public IEnumerator LoseLifeRoutine()
	{
		lives--;
		
		gameRunning = false;

		if (lives > 0)
		{
			Debug.Log("You lose one life!");

			PacmanGUIManager.use.UpdateLives(lives);
			PacmanGUIManager.use.PauseTimer(true);
			HUDManager.use.FailScreen.Show(2.5f);

			yield return new WaitForSeconds(2.5f);

			PacmanGUIManager.use.PauseTimer(false);
			StartNewRound();
		}
		else
		{
			PacmanGUIManager.use.UpdateLives(lives);
			PacmanGUIManager.use.ShowGameOverMessage(timer);
			Debug.Log("You lost the game!");
		}
	}
	
	public void WinGame()
	{
		Debug.Log("You win!");
		gameRunning = false;

		foreach (PacmanLevelUpdater updater in (PacmanLevelUpdater[]) FindObjectsOfType(typeof(PacmanLevelUpdater)))
		{
			updater.Deactivate();
		}

		if (currentLevelIndex < PacmanLevelManager.use.levels.Length - 1)
		{
			currentLevelIndex ++;
		}

		PacmanGUIManager.use.ShowWinMessage(timer);

		
		Debug.Log ("Pacman : set level success : " + (Application.loadedLevelName + "_level_" + PacmanCrossSceneInfo.use.levelToLoad) );
		LugusConfig.use.User.SetBool( Application.loadedLevelName + "_level_" + PacmanCrossSceneInfo.use.levelToLoad, true, true );
		LugusConfig.use.SaveProfiles();
	}

	protected void OnGUI()
	{
		if (!LugusDebug.debug)
			return;
		
		foreach(int index in levelLoader.levelIndices)
		{
			if (GUILayout.Button("Load level: " + index))
			{
				levelLoader.LoadLevel(index);
			}
		}	
	}
}

