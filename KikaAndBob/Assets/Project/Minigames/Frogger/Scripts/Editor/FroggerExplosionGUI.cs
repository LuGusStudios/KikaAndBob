#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(FroggerExplosion))]
public class FroggerExplosionGUI : Editor
{

	protected void OnSceneGUI()
	{
		FroggerExplosion explosion = (FroggerExplosion)target;

		// Display range box in world coordinates
		Vector2 blastCenter = explosion.BlastCenterWorld;
		Vector2 blastSize = explosion.blastSize * 0.5f;
		float z = explosion.transform.position.z;

		Vector3[] points = new Vector3[4];
		points[0] = new Vector3(blastCenter.x - blastSize.x, blastCenter.y - blastSize.y, z);
		points[1] = new Vector3(blastCenter.x - blastSize.x, blastCenter.y + blastSize.y, z);
		points[2] = new Vector3(blastCenter.x + blastSize.x, blastCenter.y + blastSize.y, z);
		points[3] = new Vector3(blastCenter.x + blastSize.x, blastCenter.y - blastSize.y, z);

		Handles.color = Color.red;
		Handles.DrawLine(points[0], points[1]);
		Handles.DrawLine(points[1], points[2]);
		Handles.DrawLine(points[2], points[3]);
		Handles.DrawLine(points[3], points[0]);
	}

}
#endif