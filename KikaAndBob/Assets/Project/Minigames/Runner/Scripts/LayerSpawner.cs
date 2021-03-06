﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LayerSpawner : MonoBehaviour 
{
	public LayerSection currentSection = null;
	public LayerSection nextSection = null;

	public Sprite[] baseLayer = null;
	public Sprite[] detailLayer = null;
	public float detailSpawnIntensity = 1.0f;

	public delegate void OnSectionSwitch(LayerSection currentSection, LayerSection nextSection);
	public OnSectionSwitch onSectionSwitch;

	public bool detailsRandomY = true; // should the details in the detail layer have a semi-random y position or stick to the default pivots?
	public bool detailsRandomX = true;

	public void SetupLocal()
	{

		if( currentSection == null )
		{
			GameObject section1 = transform.FindChildRecursively ("Section1").gameObject;

			if( section1 == null )
			{
				Debug.LogError(name + " : No Section1 known for the LayerSpawner");
			}
			else
			{
				currentSection = section1.GetComponent<LayerSection>();
				if( currentSection == null )
					currentSection = section1.AddComponent<LayerSection>();
				
				currentSection.spawner = this;
			}
		}
		if( nextSection == null )
		{
			GameObject section2 = transform.FindChildRecursively ("Section2").gameObject;

			if( section2 == null )
			{
				Debug.LogError(name + " : No Section2 known for the LayerSpawner");
			}
			else
			{
				nextSection = section2.GetComponent<LayerSection>();
				if( nextSection == null )
					nextSection = section2.AddComponent<LayerSection>();
				
				nextSection.spawner = this;
			}
		}

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

	public void StartSpawning()
	{
		LugusCoroutines.use.StartRoutine( SpawnRoutine() );
	}

	protected IEnumerator SpawnRoutine()
	{
		while( true )
		{
			//Debug.Log (Time.frameCount + "LAYER SPAWNING HAPPENING"); 

			// if currentSection is offscreen on the left side:
			// - respawn it on the right side using a new Sprite (if necessary)
			// - switch current to next section
		
			// EAST
			float rightBound = currentSection.transform.position.x + (currentSection.width / 2.0f);
			// TODO: add some pixels to UIWidth so we're 100% sure offscreen? 
			float leftBound = LugusCamera.game.transform.position.x - (LugusUtil.UIWidth / 2.0f);


			// SOUTH
			float bottomBound = currentSection.transform.position.y - (currentSection.height / 2.0f);
			float topBound = LugusCamera.game.transform.position.y + (LugusUtil.UIHeight / 2.0f);


			// TODO: find a better way? now we can only detect left an bottom, what about up and right?
			bool offscreenLeft = rightBound < leftBound;
			bool offscreenTop = bottomBound > topBound;

			// NORTH
			// moving up, so if we're offscreen on the bottom we can respawn
			topBound = currentSection.transform.position.y + (currentSection.height / 2.0f);
			bottomBound = LugusCamera.game.transform.position.y - (LugusUtil.UIHeight / 2.0f);

			bool offscreenBottom = bottomBound > topBound;


			if( offscreenLeft || offscreenTop || offscreenBottom ) 
			{
				// load new sprites and content to fill the section (which is now fully offscreen)
				currentSection.Reset();


				if( offscreenLeft )
				{
					// put it nicely to the right of the current section
					currentSection.transform.position = nextSection.transform.position.xAdd( (nextSection.width / 2.0f) + (currentSection.width / 2.0f) );
				}
				else if( offscreenTop )
				{	
					// put it nicely above the current section
					currentSection.transform.position = nextSection.transform.position.yAdd( ( -1.0f * (nextSection.height / 2.0f)) - (currentSection.height / 2.0f) );
				}
				else if( offscreenBottom )
				{
					//Debug.LogError("WE ARE OFFSCREEN BOTTOM " + currentSection.transform.Path() );
					currentSection.transform.position = nextSection.transform.position.yAdd( (nextSection.height / 2.0f) + (currentSection.height / 2.0f) );
				}

				LayerSection temp = currentSection;
				currentSection = nextSection;
				nextSection = temp; 


				if( onSectionSwitch != null )
					onSectionSwitch( currentSection, nextSection );

			} 





			yield return null;
		}
	}
	
	protected void Update () 
	{
		if( baseLayer == null )
		{
			Debug.LogError(name + " : No baseLayer entities known for this LayerSpawner!");
		}
		
		if( detailLayer == null )
		{
			Debug.LogError(name + " : No detailLayer entities known for this LayerSpawner!");
		}
	}
}
