using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanSoundEffects : LugusSingletonExisting<PacmanSoundEffects>
{
	public float maxEnemyDistance = 100;

	protected Dictionary<string, AudioClip> enemyAudioClips = new Dictionary<string, AudioClip>();	// load enemy sounds by LugusResources just once
	protected List<EnemyCharacter> enemies = new List<EnemyCharacter>();
	protected PacmanPlayerCharacter player = null;
	protected EnemyCharacter closestEnemy = null;
	protected LugusAudioTrackSettings enemyTrackSettings;
	protected ILugusAudioTrack enemiesTrack = null;

	public void SetupLocal()
	{
		enemiesTrack = LugusAudio.use.SFX().GetTrack();
		//enemiesTrack.Loop = true; 
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

	public void Reset(List<EnemyCharacter> _enemies)
	{
		player = (PacmanPlayerCharacter) FindObjectOfType(typeof(PacmanPlayerCharacter));
		if (player == null)
			Debug.LogError("Couldn't find player.");

		enemies = _enemies;

		enemyAudioClips.Clear();
		// dig enemy sounds up from Resources just once
		foreach(EnemyCharacter enemy in enemies)
		{
			if (!string.IsNullOrEmpty(enemy.walkSound) && !enemyAudioClips.ContainsKey(enemy.walkSound))
			{
				enemyAudioClips.Add(enemy.walkSound, LugusResources.use.Shared.GetAudio(enemy.walkSound));
			}
		}
	}
	
	protected void Update () 
	{
		float closestDistance = Mathf.Infinity;
		EnemyCharacter newClosestEnemy = null;
		foreach(EnemyCharacter enemy in enemies)
		{
			if (enemy.gameObject.activeInHierarchy && enemyAudioClips.ContainsKey(enemy.walkSound))
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
				enemiesTrack.Play(enemyAudioClips[newClosestEnemy.walkSound], enemyTrackSettings);
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
