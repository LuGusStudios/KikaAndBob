using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
		//Waypoint[] waypoints = (Waypoint[]) GameObject.FindObjectsOfType( typeof(Waypoint) );
		
		List<Waypoint> waypoints = ConsumableMover.use.navigationGraph;
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

	public Vector3 GetCheckmarkPosition()
	{
		Transform checkmark = this.transform.FindChild("CheckmarkPosition");
		if( checkmark != null )
		{
			return checkmark.position;
		}
		else
		{
			Vector3 checkMarkPos = Vector3.zero;
			BoxCollider2D boxCollider = this.GetComponent<BoxCollider2D>();

			if( boxCollider == null )
			{
				checkMarkPos = this.transform.position + new Vector3(50.0f, 50.0f, 0.0f);
				Debug.LogError("No boxcollider found for Checkmark Placement : put checkMark at 50,50");
			}
			else
			{
				// position checkmark at top left corner of the bounding box
				float xOffset = (boxCollider.size.x / 2.0f) + boxCollider.center.x;
				float yOffset = ((-1.0f * boxCollider.size.y) / 2.0f) - boxCollider.center.y; 
				
				xOffset *= -1.0f;
				yOffset *= -1.0f;
				
				checkMarkPos = this.transform.TransformPoint( new Vector3(xOffset, yOffset, 0.0f ) );


				
				if( this.transform.localScale != Vector3.one )
				{
					Debug.LogError("CheckMark parent has non-one scale! " + this.transform.Path () );
				}
			}



			return checkMarkPos;
		}
	}
}
