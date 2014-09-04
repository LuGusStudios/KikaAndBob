using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SetRenderQueue : MonoBehaviour 
{
	public int queue = 1000;
	private bool started = false; 
	public bool update = false;
	public bool onInstance = false;
	
	void Update () 
	{		
		if (started && !update)
			return;

		if (gameObject.renderer is SpriteRenderer)				// By default, all SpriteRenderers use the same material. You most definitely completely totally don't want to use sharedMaterial!
			gameObject.renderer.material.renderQueue = queue;	// Potentially screws up every SpriteRenderer in the project that uses the default material. SharedMaterial also persists after play mode!
		else if (onInstance)
			gameObject.renderer.material.renderQueue = queue;
		else
			gameObject.renderer.sharedMaterial.renderQueue = queue;
		
		started = true;
		update = false;
	}
	
}
