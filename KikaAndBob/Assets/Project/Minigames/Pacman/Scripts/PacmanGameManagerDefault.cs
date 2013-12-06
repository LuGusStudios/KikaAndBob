using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanGameManager : LugusSingletonExisting<PacmanGameManagerDefault> {
}

public class PacmanGameManagerDefault : MonoBehaviour {

	public bool gameRunning = false;
	public Vector2 enemySpawnLocation = new Vector2(5, 5);
	public Vector2 playerSpawnTile = new Vector2(2, 1);
	protected float timer = 120;
	protected int lives = 3;
	protected bool gameDone = false;
	protected List<EnemyCharacter> enemies = new List<EnemyCharacter>();
	protected PacmanPlayerCharacter player;
	
	void Awake () 
	{
		Transform level = GameObject.Find("LevelRoot").transform;

		foreach(EnemyCharacter enemy in (EnemyCharacter[])FindObjectsOfType(typeof(EnemyCharacter)))
		{
			enemies.Add(enemy);
		}

		player = (PacmanPlayerCharacter) FindObjectOfType(typeof(PacmanPlayerCharacter));

		StartNewGame();
	}
	
	// starts new round in the same level
	public void StartNewRound()
	{
		ResetPlayer();
		
		foreach (EnemyCharacter enemy in enemies)
		{
			DisableEnemy(enemy);
		}

		StopAllCoroutines();
		StartCoroutine(EnemySpawning());
		
		gameRunning = true;
	}

	// starts a completely new level
	public void StartNewGame()
	{
		StartNewGame("levelDefault");
	}

	// starts a completely new level
	public void StartNewGame(string levelName)
	{
		PacmanLevelManager.use.BuildLevel(levelName);

		ResetPlayer();
		
		//initially disable enemies; they will be gradually re-enabled later
		foreach (EnemyCharacter enemy in enemies)
		{
			DisableEnemy(enemy);
		}

		// start all level updaters
		foreach (PacmanLevelUpdater updater in (PacmanLevelUpdater[]) FindObjectsOfType(typeof(PacmanLevelUpdater)))
		{
			updater.Activate();
		}
		
		gameRunning = true;

		// enable enemies at regular intervals
		StartCoroutine(EnemySpawning());
	}

	// puts the player in the start location and resets their movement
	protected void ResetPlayer()
	{
		player.PlayAnimation("Idle", PacmanCharacter.CharacterDirections.Undefined);
		player.transform.localPosition = PacmanLevelManager.use.GetTile(2,1).location;
		player.DetectCurrentTile();
		player.ResetMovement();
	}

	protected IEnumerator EnemySpawning()
	{
		// first enemy takes a fraction of a second to spawn
		yield return new WaitForSeconds(0.1f);
		
		int enemyIndex = 0;

		// the rest spawns at regular intervals
		while(enemyIndex < enemies.Count)
		{	
			EnableEnemy(enemies[enemyIndex]);
			enemyIndex++;
			yield return new WaitForSeconds(4);
		}
	}
	
	protected void DisableEnemy(EnemyCharacter target)
	{
		foreach (SpriteRenderer spriteRenderer in (SpriteRenderer[])target.gameObject.GetComponentsInChildren<SpriteRenderer>())
		{
			spriteRenderer.enabled = false;
		}

		foreach (SkinnedMeshRenderer skinnedmeshRenderer in (SkinnedMeshRenderer[])target.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
		{
			skinnedmeshRenderer.enabled = false;
		}

		target.enabled = false;
		target.Reset(enemySpawnLocation);
	}

	void EnableEnemy(EnemyCharacter target)
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
		
		lives--;
		
		if (lives < 0)
			LoseGame();
		else
			PacmanGUIManager.use.UpdateLives(lives);
	}
	
	public void WinGame()
	{
		Debug.Log("You win!");
		gameRunning = false;

		foreach (PacmanLevelUpdater updater in (PacmanLevelUpdater[]) FindObjectsOfType(typeof(PacmanLevelUpdater)))
		{
			updater.Deactivate();
		}

		PacmanGUIManager.use.ShowWinMessage();
	}
	
	public void LoseGame()
	{
		Debug.Log("You lose!");
		gameRunning = false;
		PacmanGUIManager.use.ShowGameOverMessage();
	}
}

