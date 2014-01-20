using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IRunnerConfig : LugusSingletonRuntime<IRunnerConfig>  
{
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
}
