using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(LevelGenerator))]
[CanEditMultipleObjects]
public class LevelGeneratorInspector : Editor 
{
	protected bool showDefault = true;
	
	public override void OnInspectorGUI() 
	{
		LevelGenerator subject = (LevelGenerator) target;
		
		//EditorGUIUtility.LookLikeInspector(); 
		
		/*
		showDefault = EditorGUILayout.Foldout(showDefault, "Show original");
		if( showDefault )
		{
			DrawDefaultInspector();
		}
		*/
		
		DrawDefaultInspector();
		
		if( Application.isPlaying && GUILayout.Button("Generate") )
		{
			subject.Generate();
		}
		
		/*
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
		*/
		
	}
}