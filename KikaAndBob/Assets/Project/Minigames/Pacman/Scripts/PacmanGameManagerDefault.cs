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
	protected Transform level = null;
	protected List<PacmanPlayerCharacter> playerChars = new List<PacmanPlayerCharacter>();

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
		StartNewGame();
	}

	public List<PacmanPlayerCharacter> GetPlayerChars()
	{
		return playerChars;
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

		// find and reset any player characters
		playerChars.Clear();
		playerChars = new List<PacmanPlayerCharacter>(level.GetComponentsInChildren<PacmanPlayerCharacter>(true));

		if (playerChars.Count >= 1)
		{
			player = playerChars[0];
		}
		else
		{
			Debug.LogError("No player characters found!");
			return;
		}

		ResetPlayerChars();

		// find and reset enemy characters
		enemies.Clear();
		enemies = new List<EnemyCharacter>(level.GetComponentsInChildren<EnemyCharacter>(true));

		//initially disable enemies; they will be gradually re-enabled later
		DisableEnemies();

		// start all level updaters
		foreach (PacmanLevelUpdater updater in (PacmanLevelUpdater[]) FindObjectsOfType(typeof(PacmanLevelUpdater)))
		{
			updater.Activate();
		}
	
		PacmanSoundEffects.use.Reset(enemies);

		// enable enemies at regular intervals
		StartCoroutine(EnemySpawning());

		lives = 3;
		PacmanGUIManager.use.UpdateLives(lives);

		gameRunning = true;
	}

	// starts new round in the same level
	public void StartNewRound()
	{
		ResetPlayerChars();

		DisableEnemies();

		StopAllCoroutines();
		StartCoroutine(EnemySpawning());
		
		gameRunning = true;
	}

	// puts the player in the start location and resets their movement
	protected void ResetPlayerChars()
	{
		foreach(PacmanPlayerCharacter ppc in playerChars)
		{
			ppc.PlayAnimationObject("Idle", PacmanCharacter.CharacterDirections.Undefined);
			ppc.transform.localPosition = PacmanLevelManager.use.GetTile(2,1).location;	// TO DO make customizable and per character
			ppc.DetectCurrentTile();
			ppc.ResetMovement();
		}
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
	
	protected void DisableEnemies()
	{
		foreach(EnemyCharacter target in enemies)
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
			StartNewGame();
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

		PacmanGUIManager.use.ShowWinMessage();
	}
	

}

