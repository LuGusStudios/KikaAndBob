using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

[CustomEditor(typeof(Consumable))]
public class ConsumableEditor : Editor 
{
	public override void OnInspectorGUI() 
	{
		Consumable subject = (Consumable) target;

		ConsumableDefinition definition = subject.definition;
		Lugus.ConsumableState state = subject.State;

		DrawDefaultInspector();

		// if user dragged a new definition or changed the default state, update the graphics
		if( definition != subject.definition || state != subject.State )
		{
			if( subject.definition != null )
			{
				subject.GetComponent<SpriteRenderer>().sprite = subject.definition.TextureForState( subject.State );
			}
		}
	}
	
	/*
	public void OnSceneGUI() 
	{
		Waypoint subject = (Waypoint) target;
		
		//Handles.color = Color.red;
		//Handles.Label(subject.transform.position + Vector3.up*45, subject.transform.position.ToString() + " ROBIN" );
		
		//GUIStyle style = new GUIStyle();
		
		// style.normal.textColor = Color.red;
		
		GUIStyle style = new GUIStyle();
		style.normal.textColor = Color.red;
		GUIStyle style2 = new GUIStyle();
		style2.normal.textColor = Color.black;
		style2.fontSize = 20;
		
		GUIStyle style3 = new GUIStyle();
		style3.normal.textColor = Color.blue; 
		style3.fontSize = 15;
		
		// http://forum.unity3d.com/threads/107333-very-small-request-Handle-Label
		Handles.BeginGUI();
		
		// draw for every waypoint. Otherwhise, we would only see the info for the currently selected one... 
		GameObject waypointParent = subject.transform.parent.gameObject;
		foreach( Transform waypointObj in waypointParent.transform )
		{
			Waypoint waypoint = waypointObj.GetComponent<Waypoint>();
			if( waypoint == null ) 
				continue; 
			
			Handles.Label(waypoint.transform.position, waypoint.name.Replace("Waypoint", "") , style2 );
			
			foreach( Waypoint neighbour in waypoint.neighbours ) 
			{
				Vector3 direction = neighbour.transform.position - waypoint.transform.position;
				//direction.Normalize(); 
				
				
				Vector3 pos = waypoint.transform.position; 
				float angleF = Vector3.Angle( direction, Vector3.right );
				
				int angle = Mathf.RoundToInt(angleF);
				
				
				direction.Normalize(); 
				Handles.Label(pos + direction * 45 , "" + angle , style );
				
			}
			
			if( waypoint.neighbours.Count == 1 )
			{
				// path end node
				Handles.Label(waypoint.transform.position + Vector3.up * 45 , "" + waypoint.exitToScene, style3 ); 
			}

		}
		
		Handles.EndGUI();
	}
	*/
}