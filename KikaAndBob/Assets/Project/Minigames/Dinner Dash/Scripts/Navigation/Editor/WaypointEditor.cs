using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

[CustomEditor(typeof(Waypoint))]
[CanEditMultipleObjects]
public class WaypointEditor : Editor 
{
	protected bool showDefault = true;
	protected bool showData = true; 
	
	protected string message = "";
	protected Waypoint previousNeighbour = null;  
	
	public override void OnInspectorGUI() 
	{
		Waypoint subject = (Waypoint) target;
		
		EditorGUIUtility.LookLikeInspector(); 
		
		showDefault = EditorGUILayout.Foldout(showDefault, "Show original");
		if( showDefault )
		{
			DrawDefaultInspector();
		}
		
		/*
		EditorGUILayout.LabelField("------------");
		
		bool defaultPos = subject.defaultSceneStartPosition;
		subject.defaultSceneStartPosition = GUILayout.Toggle(subject.defaultSceneStartPosition, "Is default start position"); 
		
		EditorGUILayout.LabelField("------------");
		
		BoScene previousExit = subject.exitToScene;
		subject.exitToScene = (BoScene) EditorGUILayout.ObjectField( "Exit to: ", subject.exitToScene, typeof(BoScene), true);
		
		*/
		
		
		EditorGUILayout.LabelField("------------"); 
		
		Waypoint newNeighbour = (Waypoint) EditorGUILayout.ObjectField( "Link to: ", null, typeof(Waypoint), true);

		if( newNeighbour != null && newNeighbour != previousNeighbour )
		{
			bool alreadyLinked = false; 
			foreach( Waypoint neighbour in subject.neighbours )
			{
				if( neighbour == newNeighbour )
				{
					alreadyLinked = true;
					Debug.LogError("WaypointEditor: tried to add the same link twice : " + newNeighbour.name); 
				}
			}
			
			if( newNeighbour == subject )
			{
				alreadyLinked = true;
				Debug.LogError("WaypointEditor: tried to add a link to itself! : " + newNeighbour.name); 
			}
			
			if( !alreadyLinked )
			{
				/*
				Waypoint[] currentNeighbours = subject.neighbours;
				subject.neighbours = new Waypoint[ currentNeighbours.Length + 1 ];
				if( currentNeighbours.Length != 0 )
					currentNeighbours.CopyTo( subject.neighbours, 0 );
				subject.neighbours[ subject.neighbours.Length - 1 ] = newNeighbour;
				*/
				
				subject.neighbours.Add( newNeighbour );
				 
				// FIXME: do an extra check at the neighbour as well! and only continue if both are correct!
				newNeighbour.neighbours.Add( subject );
			}
			
			previousNeighbour = newNeighbour;  
		}
			
		
		EditorGUILayout.LabelField("------------");
		
		showData = EditorGUILayout.Foldout(showData, "Show data");
		if( showData )
		{
			// cannot remove element from List while enumerating!
			Waypoint toBeRemoved = null;
			
			foreach( Waypoint neighbour in subject.neighbours )
			{
				if( neighbour == null )
				{
					// neighbour has probably been destroyed...
					toBeRemoved = neighbour;
				}
				else
				{
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField( neighbour.name ); 
					if( GUILayout.Button("Remove link") )
					{
						toBeRemoved = neighbour;
					}
					EditorGUILayout.EndHorizontal();
				}
			}
			
			if( toBeRemoved != null )
			{
				subject.neighbours.Remove( toBeRemoved );
				toBeRemoved.neighbours.Remove( subject );
			}
		}
		
		
	}
	
	
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
		style2.fontSize = 14;
		
		GUIStyle style3 = new GUIStyle();
		style3.normal.textColor = Color.blue; 
		style3.fontSize = 15;
 
		// http://forum.unity3d.com/threads/107333-very-small-request-Handle-Label
		Handles.BeginGUI();
		
		// draw for every waypoint. Otherwhise, we would only see the info for the currently selected one... 
		//GameObject waypointParent = subject.transform.parent.gameObject;
		Waypoint[] allWaypoints = (Waypoint[]) GameObject.FindObjectsOfType( typeof(Waypoint) );
		foreach( Waypoint waypoint in allWaypoints )
		{
			//Waypoint waypoint = waypointObj.GetComponent<Waypoint>();
			if( waypoint == null ) 
				continue; 
		
	 		Handles.Label(waypoint.transform.position, waypoint.name.Replace("Waypoint", ""), style2 );
			
			/*
			foreach( Waypoint neighbour in waypoint.neighbours ) 
			{
				Vector3 direction = neighbour.transform.position - waypoint.transform.position;
				//direction.Normalize(); 
				
			
				Vector3 pos = waypoint.transform.position; 
				float angleF = Vector3.Angle( direction, Vector3.right );
				
				int angle = Mathf.RoundToInt(angleF);
				
				
				direction.Normalize(); 
	 			Handles.Label(pos + direction * 45  , "" + angle , style );
				
			}
			*/
			
			if( waypoint.neighbours.Count == 1 )
			{
				// path end node
	 			Handles.Label(waypoint.transform.position + Vector3.up * 45 , "", style3 ); 
			}
		}
		
		Handles.EndGUI();
	}
}