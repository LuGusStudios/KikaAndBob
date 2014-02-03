using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(RandomFlip))]
[CanEditMultipleObjects]
public class RandomFlipInspector : Editor 
{
	protected bool showDefault = true;
	
	public override void OnInspectorGUI() 
	{
		RandomFlip subject = (RandomFlip) target;
		
		//EditorGUIUtility.LookLikeInspector(); 

		/*
		showDefault = EditorGUILayout.Foldout(showDefault, "Show original");
		if( showDefault )
		{
			DrawDefaultInspector();
		}
		*/
		
		DrawDefaultInspector();
		
		if( GUILayout.Button("Flip Random") )
		{
			subject.FlipRandom();
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