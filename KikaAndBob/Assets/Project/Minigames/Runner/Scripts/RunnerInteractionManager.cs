using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunnerInteractionManager : LugusSingletonExisting<RunnerInteractionManager>  
{
	public bool activated = true;
	public void Activate()
	{
		activated = true;
	}
	public void Deactivate()
	{
		activated = false;
	}

	public List<RunnerInteractionZone> zones = new List<RunnerInteractionZone>();
	public LayerSpawner groundLayer = null;

	private int listCount = 3;
	public List<RunnerInteractionZone>[] zoneDiff ;
	public LugusRandomGeneratorSequence[] zoneRndSeq;
	public LugusRandomGeneratorUniform zoneRndUni;
	public LugusRandomGeneratorDistribution zoneRndDist;
	private int rndSeed = 5;

	public enum Direction
	{
		NONE = -1,

		EAST = 1, // character is running to the right
		WEST = 2, // character running to the left
		NORTH = 3, // character running to the top
		SOUTH = 4 // character running to the bottom
	}

	public Direction direction = Direction.EAST;
	
	public DataRange sectionSpanMultiplierRange = null;
	public DataRange difficultyRange = null;
	public float timeToMax = 60.0f;
	protected float startTime = -1.0f;

	// READ_ONLY for external classes: constantly adjusted. Use the Range-vars above to adjust these values over time
	public float sectionSpanMultiplier = 1.0f;
	public float maximumDifficulty = 6;

	//protected int nextZoneCountdown = 0;
	protected float sectionSpanOverflow = 0.0f;
	protected RunnerInteractionZone lastSpawned = null;

	public void StartTimer()
	{
		// needed to correctly interpolate between the sectionspans and difficulty ranges
		startTime = Time.time;
	}

	public void OnSectionSwitch(LayerSection currentSection, LayerSection newSection)
	{
		if( !activated )
		{
			return;
		}

		//Debug.LogError("SECTION SWITCH ACCEPTED " + zones.Count + " // " + sectionSpanOverflow);

		//return;

		float progressionPercentage = 1.0f;
		if( (Time.time - startTime) < timeToMax )
		{
			DataRange timeRange = new DataRange( startTime, startTime + timeToMax );
			progressionPercentage = timeRange.PercentageInInterval( Time.time );
		}

		sectionSpanMultiplier = sectionSpanMultiplierRange.ValueFromPercentage( progressionPercentage );
		maximumDifficulty = difficultyRange.ValueFromPercentage( progressionPercentage );


		float sectionSpan = sectionSpanOverflow;

		// when spawning new sections, we can't really do this on a separate clock (separate from section generation)
		// because we don't know how long a section is going to be on screen...
		// so, instead : spawn everything that resides within this section (And possibly a part of the next section)
		// and keep how "far over the edge" we went, so we can take that into account on the next section spawn 
		while( sectionSpan < 1.0f )
		{
			if( !activated )
				break;

			RunnerInteractionZone zonePrefab = null;
			int maxIterations = 100;
			int iteration = 0;
			bool zoneOK = true;

			do
			{
				int nextZone = Mathf.Clamp((int)zoneRndDist.Next(),0,zoneDiff[0].Count-1);
				//Debug.Log("nextzone : " + nextZone);
				//zonePrefab = zoneDiff[nextZone][(int)zoneRndSeq[nextZone].Next()];
				zonePrefab = zoneDiff[0][nextZone];
				//zonePrefab = zones[ Random.Range(0, zones.Count) ];
				//zoneOK = (zonePrefab.sectionSpan != lastSpawned.sectionSpan);

				if( zoneOK ) // if equal to the previous, we're probably not going to spawn it anyway, so avoid these next checks
				{

					// InteractionZones can be limited to 1 (or more) specific BackgroundThemes
					// if this is the case, only spawn them if the current BackgroundTheme is in fact appropriate for this zone

					// if themes is empty, no restrictions: can always spawn
					if( zonePrefab.themes != null && zonePrefab.themes.Length > 0 )
					{
						// If we're transitioning, we only want to spawn zones without specific requirements
						// this is because transition layers often have special graphic characteristics that won't match specific zones
						if( LayerManager.use.themeTransitionInProgress )
						{
							zoneOK = false;
						}
						else
						{
							bool ok = false;

							// no transition: check if the current theme is in our list
							foreach( BackgroundTheme theme in zonePrefab.themes )
							{
								if( theme == LayerManager.use.CurrentTheme )
								{
									ok = true;
									break;
								}
							}
							if( !ok ) // themes don't match: skip this interactionZone
								zoneOK = false;
						}
					}
					
					if( zonePrefab.difficulty > maximumDifficulty )
					{
						zoneOK = false;
					}
				}


				++iteration;


				// for example if all zones are too difficult or we're just testing 1 zone
				// won't happen in "real" situations, but can easily happen in testing if we're not carefull
				if( iteration >= maxIterations || zones.Count == 1 )
				{
					zoneOK = true;
				}
			}
			while( !zoneOK ); 


			float newSectionSpan = zonePrefab.sectionSpan * sectionSpanMultiplier;
			if( newSectionSpan < zonePrefab.minimumSectionSpan )
				newSectionSpan = zonePrefab.minimumSectionSpan;

			if( (sectionSpan + newSectionSpan > 0.9f) && zonePrefab.autoDestroy ) // not 1.0f but 0.9f, to provide some extra padding
			{
				// if we spawn the zones outside of the section, chances are big they will "disappear" at the end
				// because they are parented to the section, which is being re-used constantly
				// so: make sure the zones don't surpass the section's area on the end side

				// unless autoDestroy is OFF: then we don't need to worry about disappearances out of time :)
				
				//Debug.LogError("DISMISSED " + zonePrefab.name + " with span " + newSectionSpan );
				break;
			}


			lastSpawned = zonePrefab;
			
			RunnerInteractionZone newZone = (RunnerInteractionZone) GameObject.Instantiate( zonePrefab );


			sectionSpan += (newSectionSpan / 2.0f); // only half for now, because we want the zone to spawn in "it's center"

			
			newZone.gameObject.SetActive(true);
			newZone.transform.position = newSection.transform.position.zAdd( (sectionSpan + 1.0f) * -30.0f ); // zone is now at CENTER of new section

			Vector3 offset = Vector3.zero;
			
			// offset = sectionSpan/2 * width/height - [width/height]/2
			//          (add from START (left or bottom) of the section) - (half, because we're at the CENTER now: need to undo)
			if( this.direction == Direction.EAST )
			{
				// EAST: start counting on the left, as we normally do on the x axis
				offset = new Vector3( (sectionSpan * newSection.width) - (newSection.width / 2.0f), 0.0f, 0.0f );
			}
			else if( this.direction == Direction.WEST )
			{
				// WEST: start couting on the RIGHT : flip x basically
				offset = new Vector3( (-1.0f * (sectionSpan * newSection.width)) + (newSection.width / 2.0f), 0.0f, 0.0f );
			}
			else if( this.direction == Direction.NORTH )
			{
				// NORTH: start counting on the bottom, as we normally do on the y axis
				offset = new Vector3(0.0f, (sectionSpan * newSection.height) - (newSection.height / 2.0f), 0.0f );
			}
			else
			{
				// SOUTH : start counting on the TOP : flip y 
				offset = new Vector3(0.0f, (-1.0f * (sectionSpan * newSection.height)) + (newSection.height / 2.0f), 0.0f );
			}

			newZone.transform.position += offset; 


			newZone.transform.parent = newSection.transform;
			
			// TODO: do this at build time? or at least at level startup once, not every time we spawn!
			GameObject.Destroy( newZone.transform.FindChild("Background").gameObject );
			
			sectionSpan += (newSectionSpan / 2.0f); 
			 
			//Debug.LogWarning("Spawned " + newZone.name + " with span " + newSectionSpan + " so total is now " + sectionSpan + " // " + offset + " of " + newSection.height );
		}

		sectionSpanOverflow = sectionSpan - 1.0f; // what remains for the next section
		sectionSpanOverflow = Mathf.Max ( sectionSpanOverflow, -0.4f ); // make sure we don't spawn too far in the "previous" section or we might see some popping there

		//Debug.LogError("sectionSpanOverflow = " + sectionSpanOverflow);

		zoneRndDist.Delta ++;
		if (zoneRndDist.Delta >= zoneDiff[0].Count) 
		{
			zoneRndDist.Delta = 1;
		}
	}

	public void SetupLocal()
	{
		if( groundLayer == null )
		{
			groundLayer = GameObject.Find ("LayerGround").GetComponent<LayerSpawner>();
		}
		
		if( groundLayer == null )
		{
			Debug.LogError(name + " : no groundLayer found!");
		}
		else
		{
			groundLayer.onSectionSwitch += OnSectionSwitch;
		}

		if( zones.Count == 0 )
		{
			CacheInteractionZones();
		}

		if( zones.Count == 0 )
		{
			Debug.LogError(name + " : no InteractionZones found!");
		}

	}

	public void CacheInteractionZones()
	{
		zoneRndUni = new LugusRandomGeneratorUniform(rndSeed);

		if( zones.Count > 0 )
			zones.Clear();

		GameObject zoneContainer = GameObject.Find ("Zones");
		
		zones.AddRange( zoneContainer.GetComponentsInChildren<RunnerInteractionZone>(true) );

		zoneDiff = new List<RunnerInteractionZone>[listCount];
		zoneRndSeq = new LugusRandomGeneratorSequence[listCount];

		for (int i = 0; i < listCount; i++) 
		{
			zoneDiff[i] = new List<RunnerInteractionZone>();
		}

		foreach( RunnerInteractionZone zone in zones )
		{
			zone.gameObject.SetActive(false);
		}

		for (int i = 0; i < maximumDifficulty; i++) 
		{
			foreach( RunnerInteractionZone zone in zones )
			{
				if(zone.difficulty == i)
				{
					zoneDiff[0].Add(zone);
				}
			}
		}

		zoneRndDist = new LugusRandomGeneratorDistribution(Distribution.Triangular,-2,zoneDiff[0].Count+1,9999,rndSeed);

		ResetSeed();
//		for (int i = 0; i < listCount; i++) 
//		{
//			zoneRndSeq[i] = new LugusRandomGeneratorSequence(0,zoneDiff[i].Count-1);
//			zoneRndSeq[i].SetSeed(rndSeed);
//			Debug.Log("zone count : " + zoneDiff[i].Count);
//		}
	}

	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script

		if( groundLayer != null )
		{
			// normally, we have 2 sections of the same layer already setup at the scen estart
			// this means that we have to wait 2 full sections before OnSectionSwitch is called and we finally see the first InteractionZones
			// so: counter that by forcing a spawn on the nextSection at the beginning of the level
			OnSectionSwitch( groundLayer.currentSection, groundLayer.nextSection );
		}

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

	public void ResetSeed()
	{
		zoneRndDist.Delta = 0;
		zoneRndUni.SetSeed(rndSeed);
		zoneRndDist.SetSeed(rndSeed);
		sectionSpanOverflow = 0.0f;
	}
	private string _seed = "";
	void OnGUI()
	{
		if (!LugusDebug.debug)
			return;

		GUILayout.BeginArea( new Rect(210, Screen.height - 150, 200, 150) );
		GUILayout.Label("Current Seed : " + rndSeed);
		_seed = GUILayout.TextField(rndSeed.ToString());
		int.TryParse(_seed, out rndSeed);
		if (GUILayout.Button("Change Seed"))
		{

			ResetSeed();
		}

		GUILayout.EndArea();
	}
}
