using UnityEngine;
using System.Collections;

public abstract class IConsumableUser : MonoBehaviour
{
	public abstract bool Use();

	public delegate void OnUsed(IConsumableUser user);
	public OnUsed onUsed;

	// TODO:
	/*
	public Vector3 GetTarget()
	{
		return this.transform.position;
	}
	*/

	public Waypoint GetTarget()
	{
		// child of current object
		Waypoint waypoint = GetComponentInChildren<Waypoint>();
		if( waypoint != null )
			return waypoint;

		// waypoint with name : WaypointThisName
		// TODO: store the overview of all waypoitns somewhere central so there's no need to constantly FindObjectsOfType
		Waypoint[] waypoints = (Waypoint[]) GameObject.FindObjectsOfType( typeof(Waypoint) );
		foreach( Waypoint wp in waypoints )
		{
			if( wp.name == "Waypoint" + this.name )
				return wp;
		}

		// no waypoint found yet: return closest waypoint
		float smallestDistance = float.MaxValue;
		foreach( Waypoint wp in waypoints )
		{
			float distance = Vector2.Distance( this.transform.position.v2 (), wp.transform.position.v2 () );
			if( distance < smallestDistance )
			{
				waypoint = wp;
				smallestDistance = distance;
			}
		}

		return waypoint;
	}
}
