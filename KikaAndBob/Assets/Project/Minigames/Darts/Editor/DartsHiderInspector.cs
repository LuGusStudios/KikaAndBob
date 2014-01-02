using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

[CustomEditor(typeof(DartsHider))]
[CanEditMultipleObjects]
public class DartsHiderInspector : Editor 
{
	protected bool showDefault = true;
	
	public override void OnInspectorGUI() 
	{
		DartsHider subject = (DartsHider) target;
		
		//EditorGUIUtility.LookLikeInspector(); 
		
		showDefault = EditorGUILayout.Foldout(showDefault, "Show original");
		if( showDefault )
		{
			DrawDefaultInspector();
		}

		if( GUILayout.Button("Set hidden") )
		{
			subject.hiddenPosition = subject.transform.position;
		}
		if( GUILayout.Button ("Move to hidden") )
		{
			subject.transform.position = subject.hiddenPosition;
		}
		
		EditorGUILayout.LabelField("------------"); 

		if( GUILayout.Button("Set shown") )
		{
			subject.shownPosition = subject.transform.position;
		}
		if( GUILayout.Button ("Move to shown") )
		{
			subject.transform.position = subject.shownPosition;
		}
		
	}
}
