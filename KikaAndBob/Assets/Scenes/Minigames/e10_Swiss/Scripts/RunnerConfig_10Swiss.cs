using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunnerConfig_10Swiss : IRunnerConfig 
{
	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void LoadLevel(int index)
	{
		index--;

		if( index == 0 )
			Level0 ();
		else if( index == 1 )
			Level1 ();
		else if( index == 2 )
			Level2(); 
		else if( index == 666 )
			LevelCustom(); 
	}
	
	public void Level0()
	{
		RunnerCharacterControllerFasterSlower character = RunnerCharacterControllerFasterSlower.use;
		
		character.speedRange = new DataRange(10,10);
		character.timeToMaxSpeed = 60;
		RunnerInteractionManager.use.timeToMax = 60;
		RunnerInteractionManager.use.sectionSpanMultiplierRange = new DataRange( 1.0f, 1.0f ); 
		RunnerInteractionManager.use.difficultyRange = new DataRange(3,3);
	}
	
	public void Level1()
	{
		RunnerCharacterControllerFasterSlower character = RunnerCharacterControllerFasterSlower.use;
		
		character.speedRange = new DataRange(10,14);
		character.timeToMaxSpeed = 120;
		RunnerInteractionManager.use.timeToMax = 120;
		RunnerInteractionManager.use.sectionSpanMultiplierRange = new DataRange( 0.8f, 0.8f );  
		RunnerInteractionManager.use.difficultyRange = new DataRange(6,6);
	}
	
	public void Level2()
	{
		RunnerCharacterControllerFasterSlower character = RunnerCharacterControllerFasterSlower.use;
		
		character.speedRange = new DataRange(14,17);
		character.timeToMaxSpeed = 60;
		RunnerInteractionManager.use.timeToMax = 60;
		RunnerInteractionManager.use.sectionSpanMultiplierRange = new DataRange( 0.5f, 0.5f );  
		RunnerInteractionManager.use.difficultyRange = new DataRange(6,6);
		/*
		// the tasmanian devils appear too often underneath a sliding enemy, so disable them here
		List<string> inactiveZones = new List<string>();
		inactiveZones.Add("Enemy1"); // tasmanian devil 1
		inactiveZones.Add("Enemy2"); // tasmanian devil 2
		
		DisableInteractionZones( inactiveZones );
		*/
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
