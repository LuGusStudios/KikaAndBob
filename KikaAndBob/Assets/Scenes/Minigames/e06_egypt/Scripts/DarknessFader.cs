using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class DarknessFader : MonoBehaviour 
{
	public float minAlpha = 0.8f;

	protected SpriteRenderer image = null;

	public void SetupLocal()
	{
	}
	
	public void SetupGlobal()
	{
		if (image == null)
		{
			image = GetComponent<SpriteRenderer>();
		}
		if (image == null)
		{
			Debug.LogError("DarknessFader: Missing sprite renderer.");
			this.enabled = false;
		}
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
		if (!PacmanGameManager.use.GameRunning)
			return;

		image.color = image.color.a(Mathf.Lerp(minAlpha, 1.0f, Mathf.Sin(1.0f * Time.time)));
	}
	
}
