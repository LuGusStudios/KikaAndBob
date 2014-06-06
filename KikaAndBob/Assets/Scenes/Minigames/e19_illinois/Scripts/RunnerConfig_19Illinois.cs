using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunnerConfig_19Illinois : IRunnerConfig 
{
	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public override void LoadLevel(int index)
	{
		RunnerManager.use.gameType = KikaAndBob.RunnerGameType.Distance;
		RunnerManager.use.targetDistance = 300.0f; 

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

		HUDManager.use.RepositionPauseButton(KikaAndBob.ScreenAnchor.Bottom, KikaAndBob.ScreenAnchor.Bottom); 

		LoadGUIVarsFromRealSetup();
	}

	public void Level0()
	{

		RunnerCharacterControllerJumpSlide character = RunnerCharacterControllerJumpSlide.use;

		character.speedRange = new DataRange(10,14);
		character.timeToMaxSpeed = 60;
		RunnerInteractionManager.use.timeToMax = 60;
		RunnerInteractionManager.use.sectionSpanMultiplierRange = new DataRange( 1.1f, 0.9f );  
		RunnerInteractionManager.use.difficultyRange = new DataRange(3,6);
		
		RunnerManager.use.targetDistance = 600.0f;

	}
	
	public void Level1()
	{

		RunnerCharacterControllerJumpSlide character = RunnerCharacterControllerJumpSlide.use;
		
		character.speedRange = new DataRange(12,14); 
		character.timeToMaxSpeed = 120;
		RunnerInteractionManager.use.timeToMax = 120; 
		RunnerInteractionManager.use.sectionSpanMultiplierRange = new DataRange( 1.0f, 0.9f );    
		RunnerInteractionManager.use.difficultyRange = new DataRange(3,6);
		
		RunnerManager.use.targetDistance = 900.0f;
	}
	
	public void Level2()
	{

		RunnerCharacterControllerJumpSlide character = RunnerCharacterControllerJumpSlide.use;
		
		character.speedRange = new DataRange(12,16); 
		character.timeToMaxSpeed = 60;
		RunnerInteractionManager.use.timeToMax = 60;
		RunnerInteractionManager.use.sectionSpanMultiplierRange = new DataRange( 1.0f, 0.8f );  
		RunnerInteractionManager.use.difficultyRange = new DataRange(3,6);

		// the tasmanian devils appear too often underneath a sliding enemy, so disable them here
		List<string> inactiveZones = new List<string>();
		//inactiveZones.Add("Enemy1"); // tasmanian devil 1
		//inactiveZones.Add("Enemy2"); // tasmanian devil 2

		DisableInteractionZones( inactiveZones );
		
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

}
