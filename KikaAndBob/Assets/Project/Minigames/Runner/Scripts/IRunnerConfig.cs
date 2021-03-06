﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IRunnerConfig : LugusSingletonRuntime<IRunnerConfig>  
{
	public virtual void LoadLevel(int index)
	{
		Debug.LogError(transform.Path () + " : LoadLevel not implemented!");
	}

	// ideally this should be implemented individually,
	// but this is late in the project and all the runners have 3 levels right now...
	public virtual bool IsLastLevel(int levelIndex)
	{
		return levelIndex >= 3;
	}

	public virtual void OnGameStopped()
	{

	}

	public void SetupHUDForGame()
	{
		HUDManager.use.DisableAll();
		
		HUDManager.use.PauseButton.gameObject.SetActive(true);
		
		HUDManager.use.CounterLargeLeft1.gameObject.SetActive(true);
		HUDManager.use.CounterLargeLeft1.commodity = KikaAndBob.CommodityType.Time;
		HUDManager.use.CounterLargeLeft1.StartTimer(HUDCounter.Formatting.TimeMS);

		//Debug.LogError("Setting up GUI for " + RunnerManager.use.gameType + " // " + RunnerManager.use.lifeCount);

		if( RunnerManager.use.gameType == KikaAndBob.RunnerGameType.Distance )
		{
			HUDManager.use.ProgressBarCenter.gameObject.SetActive(true);
			HUDManager.use.ProgressBarCenter.commodity = KikaAndBob.CommodityType.Distance;
			HUDManager.use.ProgressBarCenter.valueRange = new DataRange(0, RunnerManager.use.targetDistance);
			HUDManager.use.ProgressBarCenter.SetValue(0);

		}
		else
		{
			if( RunnerManager.use.lifeCount > 0 )
			{
				HUDManager.use.CounterSmallLeft2.gameObject.SetActive(true);
				HUDManager.use.CounterSmallLeft2.commodity = KikaAndBob.CommodityType.Life;
				HUDManager.use.CounterSmallLeft2.SetValue( RunnerManager.use.lifeCount, false );
			}
		}


		// TODO: use this for ipad version
		//HUDManager.use.RepositionPauseButton( KikaAndBob.ScreenAnchor.Top );

		HUDManager.use.CounterLargeRight1.gameObject.SetActive(true);
		HUDManager.use.CounterLargeRight1.commodity = KikaAndBob.CommodityType.Feather;
		HUDManager.use.CounterLargeRight1.SetValue(0);
	}

	protected void DisableInteractionZones(List<string> zones)
	{
		GameObject inactiveZones = GameObject.Find ("ZonesInactive");
		if( inactiveZones == null )
		{
			Debug.LogError(name + " : ZonesInactive could not be found!" );
			return;
		}

		GameObject zoneContainer = GameObject.Find ("Zones");
		if( zoneContainer == null )
		{
			Debug.LogError(name + " : ZoneContainer could not be found!" );
			return;
		}

		
		foreach( string zoneName in zones )
		{
			Transform zone = zoneContainer.transform.FindChild ( zoneName );
			if( zone == null )
			{
				Debug.LogError(name + " : Zone " + zoneName + " could not be found and not disabled!" );
				continue;
			}
			
			zone.parent = inactiveZones.transform;
		}

		RunnerInteractionManager.use.CacheInteractionZones();
	}

	
	
	
	float speed1 = 10.0f;
	float speed2 = 20.0f;

	int lifeCount = 1;
	
	float timeToMax = 60.0f;
	float sectionSpan1 = 1.0f;
	float sectionSpan2 = 0.8f;
	float difficulty1 = 3.0f;
	float difficulty2 = 6.0f;

	float targetDistance = 500.0f;

	public void LevelCustom()
	{
		//RunnerCharacterControllerJumpSlide character = RunnerCharacterControllerJumpSlide.use;

		RunnerInteractionManager.use.sectionSpanMultiplierRange = new DataRange( 0.8f, 0.8f );  
		RunnerInteractionManager.use.difficultyRange = new DataRange(6,6);


		IRunnerCharacterController character = RunnerCharacterController.use;

		character.SpeedRange().from = LugusConfig.use.User.GetFloat("runner.custom.speed1", speed1);
		character.SpeedRange().to   = LugusConfig.use.User.GetFloat("runner.custom.speed2", speed2);

		float timeToMax2 = LugusConfig.use.User.GetFloat("runner.custom.timeToMax", timeToMax);
		RunnerInteractionManager.use.timeToMax = timeToMax2;

		if( RunnerCharacterControllerJumpSlide.Exists() )
		{
			RunnerCharacterControllerJumpSlide.use.timeToMaxSpeed = timeToMax2;
		}
		else if( RunnerCharacterControllerFasterSlower.Exists() )
		{
			RunnerCharacterControllerFasterSlower.use.timeToMaxSpeed = timeToMax2;
		}
		else if( RunnerCharacterControllerClimbing.Exists() )
		{
			RunnerCharacterControllerClimbing.use.timeToMaxSpeed = timeToMax2;
		}
		else if( RunnerCharacterControllerSkiing.Exists() )
		{
			RunnerCharacterControllerSkiing.use.timeToMaxSpeed = timeToMax2;
		}

		
		RunnerInteractionManager.use.sectionSpanMultiplierRange = new DataRange( LugusConfig.use.User.GetFloat("runner.custom.sectionSpan1", sectionSpan1), 
		                                                                         LugusConfig.use.User.GetFloat("runner.custom.sectionSpan2", sectionSpan2) );  

		
		RunnerInteractionManager.use.difficultyRange = new DataRange( LugusConfig.use.User.GetFloat("runner.custom.difficulty1", difficulty1), 
		                                                              LugusConfig.use.User.GetFloat("runner.custom.difficulty2", difficulty2) );  
	
		if( RunnerManager.use.gameType == KikaAndBob.RunnerGameType.Endless )
			RunnerManager.use.lifeCount = LugusConfig.use.User.GetInt("runner.custom.lives", lifeCount); 
		else if( RunnerManager.use.gameType == KikaAndBob.RunnerGameType.Distance )
			RunnerManager.use.targetDistance = LugusConfig.use.User.GetFloat("runner.custom.targetDistance", targetDistance); 
	}

	public void LoadGUIVarsFromRealSetup()
	{
		IRunnerCharacterController character = RunnerCharacterController.use;

		speed1 = character.SpeedRange().from;
		speed2 = character.SpeedRange().to;

		
		if( RunnerCharacterControllerJumpSlide.Exists() )
		{
			timeToMax = RunnerCharacterControllerJumpSlide.use.timeToMaxSpeed;
		}
		else if( RunnerCharacterControllerFasterSlower.Exists() )
		{
			timeToMax = RunnerCharacterControllerFasterSlower.use.timeToMaxSpeed;
		}
		else if( RunnerCharacterControllerClimbing.Exists() )
		{
			timeToMax = RunnerCharacterControllerClimbing.use.timeToMaxSpeed;
		}

		sectionSpan1 = RunnerInteractionManager.use.sectionSpanMultiplierRange.from;
		sectionSpan2 = RunnerInteractionManager.use.sectionSpanMultiplierRange.to;
	
		difficulty1 = RunnerInteractionManager.use.difficultyRange.from;
		difficulty2 = RunnerInteractionManager.use.difficultyRange.to;

		lifeCount = RunnerManager.use.lifeCount;

		targetDistance = RunnerManager.use.targetDistance;
	}

	public void ShowAdjustmentGUI()
	{
		//if( RunnerCrossSceneInfo.use.levelToLoad != 667 )
		//	return;

		float speed = 0.0f;
		if( RunnerCharacterControllerJumpSlide.Exists() )
		{
			speed = RunnerCharacterControllerJumpSlide.use.SpeedRange().ValueFromPercentage( RunnerCharacterControllerJumpSlide.use.speedPercentage );
		}
		else if( RunnerCharacterControllerFasterSlower.Exists() )
		{
			speed = RunnerCharacterControllerFasterSlower.use.SpeedRange().ValueFromPercentage( RunnerCharacterControllerFasterSlower.use.speedPercentage );
			speed *= RunnerCharacterControllerFasterSlower.use.speedModifiers.ValueFromPercentage( RunnerCharacterControllerFasterSlower.use.speedModifierPercentage );
		}
		else if( RunnerCharacterControllerClimbing.Exists() )
		{
			speed = RunnerCharacterControllerClimbing.use.SpeedRange().ValueFromPercentage( RunnerCharacterControllerClimbing.use.speedPercentage );
		}
		else if( RunnerCharacterControllerSkiing.Exists() )
		{
			speed = RunnerCharacterControllerSkiing.use.SpeedRange().ValueFromPercentage( RunnerCharacterControllerSkiing.use.speedPercentage );
			speed *= RunnerCharacterControllerSkiing.use.speedModifiers.ValueFromPercentage( RunnerCharacterControllerSkiing.use.speedModifierPercentage );

		}

		GUILayout.BeginArea( new Rect(0, 50, 190, Screen.height / 1.5f ) );
		GUILayout.BeginVertical(GUI.skin.box);

		GUILayout.Label("Speed (1 to 30)\nCurrent: " + speed);
		GUILayout.BeginHorizontal();

		GUILayout.Label("From");
		speed1 = float.Parse( GUILayout.TextField( "" + speed1 ) );
		GUILayout.Label("To");
		speed2 = float.Parse( GUILayout.TextField( "" + speed2 ) );

		GUILayout.EndHorizontal();


		GUILayout.Label("Distance between zones (1.5f to 0.5f) Current: " + RunnerInteractionManager.use.sectionSpanMultiplier);
		GUILayout.Label("(lower is more hectic)");
		GUILayout.BeginHorizontal();
		
		GUILayout.Label("From");
		sectionSpan1 = float.Parse( GUILayout.TextField( "" + sectionSpan1 ) );
		GUILayout.Label("To");
		sectionSpan2 = float.Parse( GUILayout.TextField( "" + sectionSpan2 ) );
		
		GUILayout.EndHorizontal();

		
		GUILayout.Label("Difficulty level (1 to 6) Current: " + RunnerInteractionManager.use.maximumDifficulty);
		GUILayout.Label("(1 = positive pickups, 6 = most difficult)");
		GUILayout.BeginHorizontal();
		
		GUILayout.Label("From");
		difficulty1 = float.Parse( GUILayout.TextField( "" + difficulty1 ) );
		GUILayout.Label("To");
		difficulty2 = float.Parse( GUILayout.TextField( "" + difficulty2 ) );
		
		GUILayout.EndHorizontal();


		GUILayout.Label("Time to max values in seconds");
		GUILayout.BeginHorizontal();
		
		GUILayout.Label("Time"); 
		timeToMax = float.Parse( GUILayout.TextField( "" + timeToMax ) );
		
		GUILayout.EndHorizontal();

		if( RunnerManager.use.gameType == KikaAndBob.RunnerGameType.Endless )
		{
			GUILayout.Label("Starting lives");
			GUILayout.BeginHorizontal();
			
			GUILayout.Label("Lives"); 
			lifeCount = int.Parse( GUILayout.TextField( "" + lifeCount ) );
			
			GUILayout.EndHorizontal();
		}
		else
		{
			GUILayout.Label("Target distance (100 - 2000)");
			GUILayout.BeginHorizontal();
			
			GUILayout.Label("Distance"); 
			targetDistance = float.Parse( GUILayout.TextField( "" + targetDistance ) );
			
			GUILayout.EndHorizontal();
		}




		if( GUILayout.Button("Load level with these settings") )
		{
			LugusConfig.use.User.SetFloat("runner.custom.speed1", speed1, true);
			LugusConfig.use.User.SetFloat("runner.custom.speed2", speed2, true);
			
			LugusConfig.use.User.SetFloat("runner.custom.timeToMax", timeToMax, true);

			LugusConfig.use.User.SetFloat("runner.custom.sectionSpan1", sectionSpan1, true);
			LugusConfig.use.User.SetFloat("runner.custom.sectionSpan2", sectionSpan2, true);  

			LugusConfig.use.User.SetFloat("runner.custom.difficulty1", difficulty1, true);
			LugusConfig.use.User.SetFloat("runner.custom.difficulty2", difficulty2, true); 

			if( RunnerManager.use.gameType == KikaAndBob.RunnerGameType.Endless )
				LugusConfig.use.User.SetInt("runner.custom.lives", lifeCount, true); 
			else
				LugusConfig.use.User.SetFloat("runner.custom.targetDistance", targetDistance, true); 

			LugusConfig.use.SaveProfiles();

			RunnerCrossSceneInfo.use.SetLevelIndex(667);
			LugusCoroutines.use.StopAllRoutines();

			Resources.UnloadUnusedAssets();
			Application.LoadLevel( Application.loadedLevelName );
		}

		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	
	public virtual void OnGUI()
	{
		if (!LugusDebug.debug)
			return;
		
		ShowAdjustmentGUI();
		
		GUILayout.BeginArea( new Rect(0, Screen.height - 150, 200, 150) );
	//	GUILayout.Label("Current level : " + (RunnerCrossSceneInfo.use.GetLevelIndex() - 1));
		for (int i = 0; i < 3; i++) 
		{
			if (GUILayout.Button("Start Level " + i ))
			{
				RunnerCrossSceneInfo.use.SetLevelIndex(i + 1);
				LugusCoroutines.use.StopAllRoutines();
				Resources.UnloadUnusedAssets();
				Application.LoadLevel( Application.loadedLevelName );
			}
		}
		
		if (GUILayout.Button("Custom settings level"))
		{
			RunnerCrossSceneInfo.use.SetLevelIndex(667);
			LugusCoroutines.use.StopAllRoutines();
			Resources.UnloadUnusedAssets();
			Application.LoadLevel( Application.loadedLevelName );
		}
		GUILayout.EndArea();
	}
}
