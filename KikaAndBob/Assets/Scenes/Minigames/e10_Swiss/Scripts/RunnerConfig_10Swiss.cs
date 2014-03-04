﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunnerConfig_10Swiss : IRunnerConfig 
{
	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}

	public override void OnGameStopped()
	{
		RunnerPlayerAnnoyerSkier[] annoyers = GameObject.FindObjectsOfType<RunnerPlayerAnnoyerSkier>();
		foreach( RunnerPlayerAnnoyerSkier annoyer in annoyers )
		{
			if( !annoyer.gameObject.activeSelf )
				continue;

			annoyer.OnHit(null);
		}
	}
	
	public override void LoadLevel(int index)
	{
		RunnerManager.use.gameType = KikaAndBob.RunnerGameType.Distance;
		RunnerManager.use.targetDistance = 600.0f;

		index--;

		if( index == 0 )
			Level0 ();
		else if( index == 1 )
			Level1 ();
		else if( index == 2 )
			Level2(); 
		else if( index == 666 )
			LevelCustom(); 
		
		SetupHUDForGame();
		
		LoadGUIVarsFromRealSetup();
	}
	
	public void Level0()
	{
		RunnerCharacterControllerSkiing character = RunnerCharacterControllerSkiing.use;
		
		character.speedRange = new DataRange(10,10);
		character.timeToMaxSpeed = 60;
		RunnerInteractionManager.use.timeToMax = 60;
		RunnerInteractionManager.use.sectionSpanMultiplierRange = new DataRange( 1.1f, 1.0f ); 
		RunnerInteractionManager.use.difficultyRange = new DataRange(3,5);

		RunnerManager.use.targetDistance = 600.0f;
	}
	
	public void Level1()
	{
		RunnerCharacterControllerSkiing character = RunnerCharacterControllerSkiing.use;
		
		character.speedRange = new DataRange(10,14);
		character.timeToMaxSpeed = 90;
		RunnerInteractionManager.use.timeToMax = 90;
		RunnerInteractionManager.use.sectionSpanMultiplierRange = new DataRange( 1.0f, 0.8f );  
		RunnerInteractionManager.use.difficultyRange = new DataRange(3,6);
		
		RunnerManager.use.targetDistance = 900.0f; 
	}
	
	public void Level2()
	{
		RunnerCharacterControllerSkiing character = RunnerCharacterControllerSkiing.use;
		
		character.speedRange = new DataRange(14,17);
		character.timeToMaxSpeed = 60;
		RunnerInteractionManager.use.timeToMax = 60;
		RunnerInteractionManager.use.sectionSpanMultiplierRange = new DataRange( 1.0f, 0.5f );  
		RunnerInteractionManager.use.difficultyRange = new DataRange(3,6);
		/*
		// the tasmanian devils appear too often underneath a sliding enemy, so disable them here
		List<string> inactiveZones = new List<string>();
		inactiveZones.Add("Enemy1"); // tasmanian devil 1
		inactiveZones.Add("Enemy2"); // tasmanian devil 2
		
		DisableInteractionZones( inactiveZones );
		*/
		
		RunnerManager.use.targetDistance = 1500.0f;
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
	
	protected void Update () 
	{
		
	}

	/*
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
				RunnerCrossSceneInfo.use.levelToLoad = i + 1;
				LugusCoroutines.use.StopAllRoutines();
				Application.LoadLevel( Application.loadedLevelName );
			}
		}
		GUILayout.EndArea();
	}
	*/
}
