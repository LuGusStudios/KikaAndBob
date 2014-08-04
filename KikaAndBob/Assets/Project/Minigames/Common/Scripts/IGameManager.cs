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

}
