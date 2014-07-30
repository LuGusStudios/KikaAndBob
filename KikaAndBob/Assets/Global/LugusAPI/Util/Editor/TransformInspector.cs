using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Transform))]
public class TransformInspector : Editor
{
	protected static Vector3 savedPosition = Vector3.zero;
	protected static Vector3 savedEulerAngles = Vector3.zero;
	protected static Vector3 savedScale = Vector3.zero;


	public override void OnInspectorGUI()
	{
		
		Transform t = (Transform)target;
		
		// Replicate the standard transform inspector gui
		EditorGUIUtility.labelWidth = 58;
		EditorGUI.indentLevel = 0;


		GUILayout.BeginHorizontal();
		Vector3 position = EditorGUILayout.Vector3Field("Position", t.localPosition);

		if (GUILayout.Button("0", GUILayout.Width(20)))
		{
			position = Vector3.zero;
		}

		if (GUILayout.Button("Copy", GUILayout.Width(40)))
		{
			savedPosition = position;
		}

		if (GUILayout.Button("Paste", GUILayout.Width(44)))
		{
			position = savedPosition;
		}

		if (GUILayout.Button("String", GUILayout.Width(48)))
		{
			EditorGUIUtility.systemCopyBuffer = SerializeV3ForCode(position);
		}

		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		Vector3 eulerAngles = EditorGUILayout.Vector3Field("Rotation", t.localEulerAngles);

		if (GUILayout.Button("0", GUILayout.Width(20)))
		{
			eulerAngles = Vector3.zero;
		}

		if (GUILayout.Button("Copy", GUILayout.Width(40)))
		{
			savedEulerAngles = eulerAngles;
		}
		
		if (GUILayout.Button("Paste", GUILayout.Width(44)))
		{
			eulerAngles = savedEulerAngles;
		}

		if (GUILayout.Button("String", GUILayout.Width(48)))
		{
			EditorGUIUtility.systemCopyBuffer = SerializeV3ForCode(eulerAngles);
		}

		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		Vector3 scale = EditorGUILayout.Vector3Field("Scale", t.localScale);

		if (GUILayout.Button("1", GUILayout.Width(20)))
		{
			scale = Vector3.one;
		}

		if (GUILayout.Button("Copy", GUILayout.Width(40)))
		{
			savedScale = scale;
		}
		
		if (GUILayout.Button("Paste", GUILayout.Width(44)))
		{
			scale = savedScale;
		}

		if (GUILayout.Button("String", GUILayout.Width(48)))
		{
			EditorGUIUtility.systemCopyBuffer = SerializeV3ForCode(scale);
		}

		GUILayout.EndHorizontal();


		EditorGUIUtility.labelWidth = 0;
		EditorGUIUtility.fieldWidth = 0;

		GUILayout.Space(10);

		GUILayout.BeginHorizontal();
		
		if (GUILayout.Button("Copy all", GUILayout.Width(60)))
		{
			savedPosition = position;
			savedEulerAngles = eulerAngles;
			savedScale = scale;
			
		}
		
		if (GUILayout.Button("Paste all", GUILayout.Width(64)))
		{
			position = savedPosition;
			eulerAngles = savedEulerAngles;
			scale = savedScale;
		}

		GUILayout.FlexibleSpace();
		
		if (GUILayout.Button("Uniform X scale", GUILayout.Width(120)))
		{
			scale = new Vector3(scale.x, scale.x, scale.x);
		}

		GUILayout.FlexibleSpace();

		if (GUILayout.Button("Create child", GUILayout.Width(84)))
		{
			GameObject child = new GameObject("Child");
			child.transform.parent = t;
			child.transform.localPosition = Vector3.zero;
			child.transform.localEulerAngles = Vector3.zero;
			child.transform.localScale = Vector3.one;

			Selection.activeTransform = child.transform;
		}

		GUILayout.EndHorizontal();
		


		if (GUI.changed)
		{
			Undo.RecordObject(t, "Transform Change");
			
			t.localPosition = FixIfNaN(position);
			t.localEulerAngles = FixIfNaN(eulerAngles);
			t.localScale = FixIfNaN(scale);
		}
	}
	
	private Vector3 FixIfNaN(Vector3 v)
	{
		if (float.IsNaN(v.x))
		{
			v.x = 0;
		}
		if (float.IsNaN(v.y))
		{
			v.y = 0;
		}
		if (float.IsNaN(v.z))
		{
			v.z = 0;
		}
		return v;
	}

	private string SerializeV3ForCode(Vector3 input)
	{
		string result = "";

		result = "Vector3(" + 
			input.x.ToString() + "f, " +
			input.y.ToString() + "f, " +
			input.z.ToString() + "f)";

		return result;
	}
	
}