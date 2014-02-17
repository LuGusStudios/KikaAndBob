#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class LevelGenerator : MonoBehaviour 
{

	public int sectionCount = 50;
	//public bool transitionThemes = true;

	public void Generate()
	{
		LugusCoroutines.use.StartRoutine( GenerationRoutine() );
		RunnerInteractionManager.use.ResetSeed();
	}

	public IEnumerator GenerationRoutine()
	{
		EditorApplication.isPaused = true;
		
		// step 0 : clear this transform by deleting its children
		foreach( Transform child in this.transform )
		{
			GameObject.Destroy( child.gameObject ); // note: if we use DestroyImmediate here, with isPaused = true, it won't work consistently... so use Destroy and a small WaitForSeconds later :)
		}

		// Give GameObject.Destroy() some time to perform the section Reset()
		EditorApplication.isPaused = false;
		yield return new WaitForSeconds(0.01f);
		EditorApplication.isPaused = true;


		// step 1: place ourselves at LayerRoot, copy over all contents of LayerRoot, move back to original position
		GameObject layerRoot = GameObject.Find("LayerRoot");
		Vector3 originalPosition = this.transform.position;
		
		this.transform.position = layerRoot.transform.position;
		foreach( Transform child in layerRoot.transform )
		{
			GameObject child2 = (GameObject) GameObject.Instantiate( child.gameObject );
			child2.name = child.name;
			child2.transform.parent = this.transform;
		}
		
		this.transform.position = originalPosition;
		
		// step 2: setup the needed vars
		LayerSpawner groundLayer = this.transform.FindChild ("LayerGround").GetComponent<LayerSpawner>();
		List<LayerSection> sections = new List<LayerSection>();
		
		// step 3: spawn the sections by duplicating them and resetting the sections as needed
		LayerSection newSectionPrefab = groundLayer.currentSection;
		LayerSection newSectionReferencePoint = groundLayer.nextSection;

		newSectionPrefab.Reset();
		newSectionReferencePoint.Reset ();
		
		// Give GameObject.Destroy() some time to perform the section Reset()
		EditorApplication.isPaused = false;
		yield return new WaitForSeconds(0.01f);
		EditorApplication.isPaused = true;


		sections.Add( newSectionReferencePoint ); // we only spawn from the "nextSection" standardly
		
		for( int i = 0; i < sectionCount; ++i )
		{
			// for starters : duplicate currentSection and place it behind nextSection
			LayerSection newSection = (LayerSection) GameObject.Instantiate( newSectionPrefab ); 
			newSection.transform.parent = newSectionPrefab.transform.parent;
			newSection.name = "Section " + i;

			sections.Add ( newSection ); 

			
			Vector3 newPos = Vector3.zero;
			
			// code taken from LayerSpawner
			if( RunnerInteractionManager.use.direction == RunnerInteractionManager.Direction.EAST ) // moving to the RIGHT
			{
				// put it nicely to the right of the reference section
				newPos = newSectionReferencePoint.transform.position.xAdd( (newSectionReferencePoint.width / 2.0f) + (newSection.width / 2.0f) );
			}
			else if( RunnerInteractionManager.use.direction == RunnerInteractionManager.Direction.SOUTH )
			{	
				// put it nicely above the current section
				newPos = newSectionReferencePoint.transform.position.yAdd( ( -1.0f * (newSectionReferencePoint.height / 2.0f)) - (newSection.height / 2.0f) );
			}
			else if( RunnerInteractionManager.use.direction == RunnerInteractionManager.Direction.NORTH )
			{
				//Debug.LogError("WE ARE OFFSCREEN BOTTOM " + currentSection.transform.Path() );
				newPos = newSectionReferencePoint.transform.position.yAdd( (newSectionReferencePoint.height / 2.0f) + (newSection.height / 2.0f) );
			}
			
			newSection.transform.position = newPos;
			newSection.Reset();
			
			newSectionPrefab = newSectionReferencePoint;
			newSectionReferencePoint = newSection;
			
			/*
			if( newSectionPrefab == groundLayer.currentSection )
			{
				newSectionPrefab = groundLayer.nextSection;
				newSectionReferencePoint = groundLayer.currentSection;
			}
			else
			{
				newSectionPrefab = groundLayer.currentSection;
				newSectionReferencePoint = groundLayer.nextSection;
			}
			*/
		}

		// Give GameObject.Destroy() some time to perform the section Reset()
		EditorApplication.isPaused = false;
		yield return new WaitForSeconds(0.01f);
		EditorApplication.isPaused = true;


		DataRange sectionSpanRange = RunnerInteractionManager.use.sectionSpanMultiplierRange;
		DataRange difficultyRange = RunnerInteractionManager.use.difficultyRange;

		// 1/5th at the end is full difficulty (as if we're playing after timeToMax has ended)
		// this means 4/5th at the beginning should progress through the ranges

		DataRange sectionRange = new DataRange(0, sectionCount * 0.8f);

		int currentSectionNr = 0;
		// 5. spawn new InteractionZones
		foreach( LayerSection section in sections )
		{
			currentSectionNr++;

			float sectionPercentage = sectionRange.PercentageInInterval( currentSectionNr );

			// hard-set the range to the same from-and-to to make sure this value is chosen
			RunnerInteractionManager.use.sectionSpanMultiplierRange = new DataRange( sectionSpanRange.ValueFromPercentage(sectionPercentage), sectionSpanRange.ValueFromPercentage(sectionPercentage) );
			RunnerInteractionManager.use.difficultyRange = new DataRange( difficultyRange.ValueFromPercentage(sectionPercentage), difficultyRange.ValueFromPercentage(sectionPercentage) );

			//Debug.Log ("InteractionManager for " + section.name);
			RunnerInteractionManager.use.activated = true; // combat that nasty eagle and other zones that disable the interactionManager...
			RunnerInteractionManager.use.OnSectionSwitch( null, section );
		}
	}

	public void SetupLocal() 
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{
		EditorApplication.isPaused = true;
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
#endif
