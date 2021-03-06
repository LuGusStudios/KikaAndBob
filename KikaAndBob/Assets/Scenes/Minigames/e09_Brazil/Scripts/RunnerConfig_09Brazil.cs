﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunnerConfig_09Brazil : IRunnerConfig 
{
	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	 
	public override void OnGameStopped()
	{		
		RunnerCameraPuller puller = GameObject.FindObjectOfType<RunnerCameraPuller>();
		puller.enabled = false;
		puller.rigidbody2D.isKinematic = true;

		Transform background = LugusCamera.game.transform.FindChild("Background");
		if( background != null )
		{
			background.gameObject.StopTweens();
		}
		else
		{
			Debug.LogError(name + " : No Background found under Game Camera!");
		}


		RunnerPlayerAnnoyer[] annoyers = GameObject.FindObjectsOfType<RunnerPlayerAnnoyer>();
		foreach( RunnerPlayerAnnoyer annoyer in annoyers )
		{
			if( !annoyer.gameObject.activeSelf )
				continue;
			
			annoyer.OnHit(null);
		}

	}
	
	public override void LoadLevel(int index)
	{
		RunnerManager.use.gameType = KikaAndBob.RunnerGameType.Distance;

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

		Transform background = LugusCamera.game.transform.FindChild("Background");
		if( background != null )
		{
			background.gameObject.MoveTo( background.transform.localPosition.y ( background.transform.localPosition.y * -1.0f ) ).IsLocal(true).Time(RunnerInteractionManager.use.timeToMax).Execute();
		}
		else
		{
			Debug.LogError(name + " : No Background found under Game Camera!");
		}
	}
	
	public void Level0()
	{
		RunnerCharacterControllerClimbing character = RunnerCharacterControllerClimbing.use;

		// in brazil, the camera is moving at at constant speed
		// the speed here is much lower than that of other runners!!!!
		character.speedRange = new DataRange(6,8);
		character.timeToMaxSpeed = 60; 
		RunnerInteractionManager.use.timeToMax = 60;
		RunnerInteractionManager.use.sectionSpanMultiplierRange = new DataRange( 1.5f, 1.0f ); 
		RunnerInteractionManager.use.difficultyRange = new DataRange(3,6);
		
		RunnerManager.use.targetDistance = 400.0f;
	}
	
	public void Level1()
	{
		RunnerCharacterControllerClimbing character = RunnerCharacterControllerClimbing.use;
		
		// in brazil, the camera is moving at at constant speed
		// the speed here is much lower than that of other runners!!!!
		character.speedRange = new DataRange(6,9);
		character.timeToMaxSpeed = 120;
		RunnerInteractionManager.use.timeToMax = 120;
		RunnerInteractionManager.use.sectionSpanMultiplierRange = new DataRange( 1.5f, 1.0f );  
		RunnerInteractionManager.use.difficultyRange = new DataRange(3,6);
		
		RunnerManager.use.targetDistance = 800.0f;
	}
	
	public void Level2()
	{
		RunnerCharacterControllerClimbing character = RunnerCharacterControllerClimbing.use;
		
		// in brazil, the camera is moving at at constant speed
		// the speed here is much lower than that of other runners!!!!
		character.speedRange = new DataRange(6,10); 
		character.timeToMaxSpeed = 60; 
		RunnerInteractionManager.use.timeToMax = 60;
		RunnerInteractionManager.use.sectionSpanMultiplierRange = new DataRange( 1.3f, 1.0f );  
		RunnerInteractionManager.use.difficultyRange = new DataRange(3,6);
		/*
		// the tasmanian devils appear too often underneath a sliding enemy, so disable them here
		List<string> inactiveZones = new List<string>();
		inactiveZones.Add("Enemy1"); // tasmanian devil 1
		inactiveZones.Add("Enemy2"); // tasmanian devil 2
		
		DisableInteractionZones( inactiveZones );
		*/

		RunnerManager.use.targetDistance = 1200.0f;
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
