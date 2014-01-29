﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunnerConfig_13Pacific : IRunnerConfig 
{
	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void LoadLevel(int index)
	{
		if( index == 0 )
			Level0 ();
		else if( index == 1 )
			Level1 ();
		else if( index == 2 )
			Level2(); 
	}

	public void Level0()
	{
		RunnerCharacterControllerJumpSlide character = RunnerCharacterControllerJumpSlide.use;

		character.speedRange = new DataRange(13,13);
		RunnerInteractionManager.use.sectionSpanMultiplier = 1.0f; 
		RunnerInteractionManager.use.maximumDifficulty = 2; // TODO: change this, only good for testing :)bg
	}
	
	public void Level1()
	{
		RunnerCharacterControllerJumpSlide character = RunnerCharacterControllerJumpSlide.use;
		
		character.speedRange = new DataRange(13,20);
		character.timeToMaxSpeed = 120;
		RunnerInteractionManager.use.sectionSpanMultiplier = 1.0f; 
		RunnerInteractionManager.use.maximumDifficulty = 6;
	}
	
	public void Level2()
	{
		RunnerCharacterControllerJumpSlide character = RunnerCharacterControllerJumpSlide.use;
		
		character.speedRange = new DataRange(16,20);
		character.timeToMaxSpeed = 60;
		RunnerInteractionManager.use.sectionSpanMultiplier = 0.8f; 
		RunnerInteractionManager.use.maximumDifficulty = 6;

		// the tasmanian devils appear too often underneath a sliding enemy, so disable them here
		List<string> inactiveZones = new List<string>();
		inactiveZones.Add("Enemy1"); // tasmanian devil 1
		inactiveZones.Add("Enemy2"); // tasmanian devil 2

		DisableInteractionZones( inactiveZones );
	}

	public void SetupGlobal()
	{
		LoadLevel( RunnerCrossSceneInfo.use.levelToLoad );
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}
	
	protected void Update () 
	{
	
	}

	void OnGUI()
	{
		if (!LugusDebug.debug)
			return;
		
		GUILayout.BeginArea( new Rect(0, Screen.height - 150, 200, 150) );
		GUILayout.Label("Current level : " + RunnerCrossSceneInfo.use.levelToLoad);
		for (int i = 0; i < 3; i++) 
		{
			if (GUILayout.Button("Start Level " + i))
			{
				RunnerCrossSceneInfo.use.levelToLoad = i;
				LugusCoroutines.use.StopAllRoutines();
				Application.LoadLevel( Application.loadedLevelName );
			}
		}
		GUILayout.EndArea();
	}
}