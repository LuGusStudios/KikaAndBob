using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(ConsumableConsumer))]
[CanEditMultipleObjects]
public class ConsumableConsumerInspector : Editor 
{
	protected bool showDefault = true;
	
	public override void OnInspectorGUI() 
	{
		ConsumableConsumer subject = (ConsumableConsumer) target;
		
		//EditorGUIUtility.LookLikeInspector(); 
		
		/*
		showDefault = EditorGUILayout.Foldout(showDefault, "Show original");
		if( showDefault )
		{
			DrawDefaultInspector();
		}
		*/
		
		DrawDefaultInspector();
		
		if( GUILayout.Button("Move to place") )
		{
			GameObject pos = GameObject.Find("ConsumerPlaces/" + subject.name);
			if( pos != null )
				subject.transform.position = pos.transform.position;
		}
		
		if( GUILayout.Button("Move away") )
		{
			subject.transform.position = subject.transform.position.xAdd(-3000.0f);
		}
		
	}
}