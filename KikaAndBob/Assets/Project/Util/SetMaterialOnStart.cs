using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SetMaterialOnStart : MonoBehaviour 
{
	public Material replaceMaterial = null;
	protected bool firstFrame = true;

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
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

			firstFrame = false;

			Renderer renderer = GetComponent<Renderer>();

			if (renderer != null && replaceMaterial != null)
			{
				print (renderer.material);
				renderer.material = replaceMaterial;
			}

			//this.enabled = false;

	}
}
