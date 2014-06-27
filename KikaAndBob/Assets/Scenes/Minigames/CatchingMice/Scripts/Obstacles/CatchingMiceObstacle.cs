using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Obstacles are world objects that have specific functions inside a certain level
// The major difference with traps is that the behavior of an obstacle is
// not as strictly defined as a trap, and it cannot be attacked by Skinny Mice.
public abstract class CatchingMiceObstacle : CatchingMiceWorldObject
{
	public Sprite activeSprite = null;
	public Sprite inactiveSprite = null;
	public SpriteRenderer spriteRenderer = null;

	public virtual void SetupLocal()
	{

	}

	public virtual void SetupGlobal()
	{
		// Find the sprite renderer
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();

		if (spriteRenderer != null)
		{
			spriteRenderer.sprite = inactiveSprite;
		}
		else
		{
			CatchingMiceLogVisualizer.use.LogError("Could not find the sprite renderer for the obstacle.");
		}
	}

	public void Awake()
	{
		SetupLocal();
	}

	protected void Start()
	{
		SetupGlobal();
	}

	public abstract void FromXMLObstacleDefinition(string configuration);
}
