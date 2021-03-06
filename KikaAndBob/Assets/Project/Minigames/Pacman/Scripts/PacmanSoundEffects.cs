using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanSoundEffects : LugusSingletonExisting<PacmanSoundEffects>
{
	public float maxEnemyDistance = 100;

	protected Dictionary<string, AudioClip> enemyAudioClips = new Dictionary<string, AudioClip>();	// load enemy sounds by LugusResources just once
	protected List<PacmanEnemyCharacter> enemies = new List<PacmanEnemyCharacter>();
	protected PacmanEnemyCharacter closestEnemy = null;
	protected LugusAudioTrackSettings enemyTrackSettings = null;
	protected ILugusAudioTrack enemiesTrack = null;

	public void SetupLocal()
	{
		enemiesTrack = LugusAudio.use.SFX().GetTrack();
		enemiesTrack.Claim();
		enemyTrackSettings = new LugusAudioTrackSettings().Loop(true);
	}
	
	public void SetupGlobal()
	{	

	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}

	public void Reset(List<PacmanEnemyCharacter> _enemies)
	{
		Debug.Log("PacmanSoundEffects: Resetting sound effects.");

		enemies = _enemies;

		enemyAudioClips.Clear();
		// dig enemy sounds up from Resources just once
		foreach(PacmanEnemyCharacter enemy in enemies)
		{
			if (!string.IsNullOrEmpty(enemy.walkSoundKey) && !enemyAudioClips.ContainsKey(enemy.walkSoundKey))
			{
				enemyAudioClips.Add(enemy.walkSoundKey, LugusResources.use.Shared.GetAudio(enemy.walkSoundKey));
			}
		}
	}

	public void Stop()
	{
		enemyAudioClips.Clear();

		if (enemiesTrack != null && enemiesTrack.Playing)
			enemiesTrack.Stop();

		closestEnemy = null;
	}
	
	protected void Update () 
	{
		if (!PacmanGameManager.use.gameRunning)
			return;

		// if the closest enemy was just defeated, always make sure to immediately stop sound
		if (closestEnemy != null && closestEnemy.enemyState == PacmanEnemyCharacter.EnemyState.Defeated && enemiesTrack.Playing)
		{
			enemiesTrack.Stop();
			closestEnemy = null;
		}

		PacmanPlayerCharacter player = PacmanGameManager.use.GetActivePlayer();
		if (player == null)
		{
			Debug.LogError("PacmanSoundEffect: Player was null!");
			return;
		}

		float closestDistance = Mathf.Infinity;
		PacmanEnemyCharacter newClosestEnemy = null;
		foreach(PacmanEnemyCharacter enemy in enemies)
		{
			if (enemy == null || enemy.enemyState == PacmanEnemyCharacter.EnemyState.Defeated || !enemy.enabled)
				continue;

			if (enemyAudioClips.ContainsKey(enemy.walkSoundKey))
			{
				float distance = Vector2.Distance(player.transform.position.v2(), enemy.transform.position.v2());

				if (distance < maxEnemyDistance & distance < closestDistance)
				{
					closestDistance = distance;
					newClosestEnemy = enemy;
				}
			}
		}

		if (newClosestEnemy != null)
		{
			if (newClosestEnemy != closestEnemy || !enemiesTrack.Playing)
			{
				enemiesTrack.Play(enemyAudioClips[newClosestEnemy.walkSoundKey], enemyTrackSettings);
				closestEnemy = newClosestEnemy;
			}
			enemiesTrack.Volume = Mathf.Lerp(1, 0, closestDistance/maxEnemyDistance);
		}
		else
		{
			if (enemiesTrack.Playing)
			{
				enemiesTrack.Stop();
			}
		}
	}
}
