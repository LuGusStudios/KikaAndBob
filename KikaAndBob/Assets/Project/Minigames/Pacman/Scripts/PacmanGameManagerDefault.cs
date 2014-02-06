using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanGameManager : LugusSingletonExisting<PacmanGameManagerDefault> {
}

public class PacmanGameManagerDefault : MonoBehaviour {

	public bool gameRunning = false;
	protected float timer = 120;
	protected int lives = 3;
	protected bool gameDone = false;
	protected List<PacmanEnemyCharacter> enemies = new List<PacmanEnemyCharacter>();
	protected PacmanPlayerCharacter activePlayer;
	protected Transform level = null;
	protected List<PacmanPlayerCharacter> playerChars = new List<PacmanPlayerCharacter>();
	protected int currentLevelIndex = 0;

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
		StartNewLevel();
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
		PacmanGUIManager.use.UpdateLives(lives);
	//	PacmanGUIManager.use.UpdateKeyGUIItems();

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
		timer = 120;
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
			PacmanGUIManager.use.UpdateLives(lives);
			StartNewRound();
			Debug.Log("You lose one life!");
		}
		else
		{
			PacmanGUIManager.use.ShowGameOverMessage();
			yield return new WaitForSeconds(1f);
			StartNewLevel();
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

		PacmanGUIManager.use.ShowWinMessage();
	}
	

}

