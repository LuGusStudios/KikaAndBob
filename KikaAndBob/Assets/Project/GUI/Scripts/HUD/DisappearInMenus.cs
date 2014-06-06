using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DisappearInMenus : MonoBehaviour 
{
	protected List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
	protected List<Collider2D> colliders2D = new List<Collider2D>();
	protected IGameManager gameManager = null;
	protected bool isBeingUsed = true;

	public void SetupLocal()
	{
		spriteRenderers.AddRange(GetComponentsInChildren<SpriteRenderer>());
		colliders2D.AddRange(GetComponentsInChildren<Collider2D>());
	}
	
	public void SetupGlobal()
	{
		if (gameManager == null)
		{
			gameManager = (IGameManager) FindObjectOfType(typeof(IGameManager));
		}
		
		if (gameManager == null)
		{
			Debug.Log("DisappearInMenus: No game manager in this scene! Disabling.");
			this.enabled = false;
		}
	}

	protected void SetEnabled(bool enabled)
	{
		isBeingUsed = enabled;
		
		foreach (SpriteRenderer sr in spriteRenderers)
		{
			sr.enabled = enabled;
		}
		
		foreach (Collider2D col in colliders2D)
		{
			col.enabled = enabled;
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
		if (gameManager != null)
		{
			if (gameManager.Paused)
			{
				if (isBeingUsed == true)
				{
					SetEnabled(false);
				}
			}
			else
			{
				if (isBeingUsed != gameManager.GameRunning)
				{
					SetEnabled(gameManager.GameRunning);
				}
			}
		}
	}
}
