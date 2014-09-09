using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class IGameManager : MonoBehaviour 
{
	public abstract void StartGame();
	public abstract void StopGame(); 
	
	public virtual void ReloadLevel()
	{
		Resources.UnloadUnusedAssets();

		Application.LoadLevel( Application.loadedLevelName );
	}
	
	public abstract bool GameRunning { get; }


	protected bool _paused = false;
	public bool Paused
	{
		get{ return _paused; }
		set
		{
			SetPaused( value );
		}
	}
	
	public void SetPaused(bool pause)
	{
		Debug.Log("IGameManager : setPaused : " + pause);
		if( pause )
		{
			// Try pause
			if( Paused )
			{
				Debug.LogError(transform.Path () + " : IGameManager:SetPaused : game was already paused. Doing nothing");
				return;
			}
			
			// pause
			Time.timeScale = 0.0001f;
			// update the physics timestep as well
			// otherwhise, moving objects with colliders (all our Buttons) wouldn't update collision correctly!
			Time.fixedDeltaTime = 0.02f * Time.timeScale;

			/*
			TrailRenderer[] trails = (TrailRenderer[]) GameObject.FindObjectsOfType( typeof(TrailRenderer) );
			foreach( TrailRenderer trail in trails )
			{
				trail.time = 1.0f * Time.timeScale;
			}
			*/

			_paused = true; 
			
		}
		else
		{
			// Try unpause
			if( !Paused )
			{
				Debug.LogWarning("GameManager:SetPaused : game was already UNpaused. Doing nothing");
				return;
			}
			
			// unpause
			Time.timeScale = 1.0f;
			// update the physics timestep as well
			// otherwhise, moving objects with colliders (all our Buttons) wouldn't update collision correctly!
			Time.fixedDeltaTime = 0.02f * Time.timeScale;

			/*
			TrailRenderer[] trails = (TrailRenderer[]) GameObject.FindObjectsOfType( typeof(TrailRenderer) );
			foreach( TrailRenderer trail in trails )
			{
				trail.time = 1.0f * Time.timeScale;
			}
			*/
			
			//GameObject pausePopup = GameObject.Find ("FrontLayer/PausePopup");
			//pausePopup.renderer.enabled = false;
			
			_paused = false;
		}
	}

	public IEnumerator StoreScore(int levelIndex, int score)
	{
		yield return LugusCoroutines.use.StartRoutine(StoreScore(levelIndex, (float)score));
	}

	public IEnumerator StoreScore(int levelIndex, float score)
	{
		if (!PlayerAuthCrossSceneInfo.use.loggedIn)
		{
			Debug.Log("GameManager: Player not logged in. Not storing score.");
			yield break;
		}

		yield return StartCoroutine(KBAPIConnection.use.CheckConnectionRoutine());
		
		if (!KBAPIConnection.use.hasConnection)
			yield break;

		List<int> foundGameIDs = new List<int>();
		
		yield return StartCoroutine(KBAPIConnection.use.GetGameIdRoutine(foundGameIDs, Application.loadedLevelName));
		
		if( foundGameIDs.Count >= 1 )
		{
			Debug.Log ("Received game-id " + foundGameIDs[0] + " for game " + Application.loadedLevelName);
		}
		else
		{
			Debug.LogError("No game id found for " + Application.loadedLevelName);

			yield break;
		}

		int gameID = foundGameIDs[0];

		yield return StartCoroutine(KBAPIConnection.use.AddScoreRoutine(gameID, levelIndex, score));

		if (KBAPIConnection.use.errorMessage == "")	// if it is not a new high score, an error message is returned
		{
			Debug.Log("New highscore achieved!");
		}


	}

	protected void OnDestroy()
	{
		//	Debug.Log("Clearing" + typeof (T).Name);
		
		this.enabled = false;

		PacmanLevelManager.Change(null);
		FroggerGameManager.Change(null);
		DanceHeroLevel.Change(null);
		DartsLevelConfiguration.Change(null);
		DinnerDashManager.Change(null);
		RunnerManager.Change(null);
		CatchingMiceGameManager.Change(null);

	}
}
