using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class AdjustBoneAnimationColor : MonoBehaviour 
{
	public Color newColor = Color.white;

	protected BoneAnimation boneAnimation = null;

	public void SetupLocal()
	{
		boneAnimation = GetComponent<BoneAnimation>();

		if (boneAnimation == null)
		{
			Debug.LogError("AdjustBoneAnimationColor: Missing bone animation! Disabling.");
			this.enabled = false;
			return;
		}

		boneAnimation.updateColors = true;
		boneAnimation.SetMeshColor(newColor);
	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}
	
	protected void Update () 
	{
	
	}
}
