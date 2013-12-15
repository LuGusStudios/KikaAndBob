using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunnerInteractionManager : LugusSingletonExisting<RunnerInteractionManager>  
{
	public List<RunnerInteractionZone> zones = new List<RunnerInteractionZone>();
	public LayerSpawner groundLayer = null;

	protected int nextZoneCountdown = 0;

	public void OnSectionSwitch(LayerSection currentSection, LayerSection newSection)
	{
		Debug.LogError("SECTION SWITCH ACCEPTED");

		--nextZoneCountdown;
		if( nextZoneCountdown < 0 )
			nextZoneCountdown = 0;

		if( nextZoneCountdown == 0 )
		{
			RunnerInteractionZone newZone = (RunnerInteractionZone) GameObject.Instantiate( zones[ Random.Range(0, zones.Count) ] );
			
			newZone.gameObject.SetActive(true);
			newZone.transform.position = newSection.transform.position.zAdd( -30.0f );
			newZone.transform.parent = newSection.transform;

			// TODO: do this at build time? or at least at level startup once, not every time we spawn!
			GameObject.Destroy( newZone.transform.FindChild("Background").gameObject );

			nextZoneCountdown += newZone.sectionWidth;
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
			GameObject zoneContainer = GameObject.Find ("Zones");

			zones.AddRange( zoneContainer.GetComponentsInChildren<RunnerInteractionZone>() );
		}
		
		if( zones.Count == 0 )
		{
			Debug.LogError(name + " : no InteractionZones found!");
		}

		foreach( RunnerInteractionZone zone in zones )
		{
			zone.gameObject.SetActive(false);
		}
	}

	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
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
