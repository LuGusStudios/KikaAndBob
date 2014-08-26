﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceTrap : CatchingMiceWorldObject, ICatchingMiceWorldObjectTrap
{
	public Sprite activeSprite = null;
	public Sprite inactiveSprite = null;
	public SpriteRenderer spriteRenderer = null;
	public string attackSoundKey = null;


	protected int originalAmmoCount = 1;
	protected Vector3 originalScale = Vector3.zero;

	public float Health
	{
		get
		{
			return health;
		}
		set
		{
			health = value;
			if (health <= 0)
			{
				OnEmptyHealth();
			}
		}
	}
	public int Ammo
	{
		get
		{
			return ammo;
		}
		set
		{
			ammo = value;
			if (ammo <= 0)
			{
				OnEmptyAmmo();
			}
		}
	}
	public float Cost
	{
		get
		{
			return cost;
		}
		set
		{
			cost = value;
		}
	}
	public float Damage
	{
		get
		{
			return damage;
		}
		set
		{
			damage = value;
		}
	}
	public float Interval
	{
		get
		{
			return interval;
		}
		set
		{
			interval = value;
		}
	}
	public int TileRange
	{
		get
		{
			return tileRange;
		}
		set
		{
			tileRange = value;
		}
	}
	public CatchingMiceTrap TrapObject
	{
		get
		{
			return this;
		}
	}

	[SerializeField]
	protected float health = 100.0f;
	[SerializeField]
	protected int ammo = 3;
	[SerializeField]
	protected float cost = 1.0f;
	[SerializeField]
	protected float damage = 1.0f;
	[SerializeField]
	protected float interval = 1.0f;
	[SerializeField]
	protected int tileRange = 0;
	
	public virtual void SetupLocal()
	{

	}

	public virtual void SetupGlobal()
	{
		// Find the sprite renderer
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();

		if (spriteRenderer != null)
		{
			if (ammo > 0)
			{
				spriteRenderer.sprite = activeSprite;
			}
			else
			{
				spriteRenderer.sprite = inactiveSprite;
			}
		}
		else
		{
			CatchingMiceLogVisualizer.use.LogError("Could not find the sprite renderer for the trap.");
		}

		originalAmmoCount = ammo;
		originalScale = transform.localScale;
	}

	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start()
	{
		SetupGlobal();
	}

	public void OnHit(ICatchingMiceCharacter character)
	{
		character.Health -= damage;
	}

	public void DestroySelf()
	{
		// Remove the trap from the list and tiletype
		CatchingMiceLevelManager.use.RemoveTrapFromTile(parentTile);
		gameObject.SetActive(false);
	}

	public void PlayerInteraction()
	{
		// for the the time being, this has been disabled -  traps cannot be reset right now
		//OnPlayerInteract();
	}

	public void CalculateTrapBounds(out Vector2 min, out Vector2 max)
	{
		// Get y offset from furniture, if any
		float yOffset = 0f;
		if ((parentTile != null) && (parentTile.furniture != null))
		{
			yOffset = parentTile.furniture.yOffset;
		}

		// When traps are bigger than 1 tile, check the collider for its center
		min = transform.position.xAdd(-tileRange - 0.5f).yAdd(-tileRange - yOffset - 0.5f).v2() * CatchingMiceLevelManager.use.scale;
		max = transform.position.xAdd(tileRange + 0.5f).yAdd(tileRange - yOffset + 0.5f).v2() * CatchingMiceLevelManager.use.scale;
		/*BoxCollider2D trapCollider = GetComponentInChildren<BoxCollider2D>();
		if (trapCollider == null)
		{
			min = transform.position.xAdd(-tileRange).yAdd(-tileRange - yOffset).v2() * CatchingMiceLevelManager.use.scale;
			max = transform.position.xAdd(tileRange).yAdd(tileRange - yOffset).v2() * CatchingMiceLevelManager.use.scale;
		}
		else
		{
			// Get middle location from one tile
			Vector3 tile = Vector3.zero;

			// Only do this when range is bigger then 1, if not just use collider bounds
			if (tileRange > 1)
			{
				tile = (CatchingMiceLevelManager.use.scale * Vector3.one * 0.5f);
			}

			// Gets the bound from the first and last tile in world space
			Vector3 trapBoundLeft = trapCollider.transform.TransformPoint(trapCollider.Bounds().min) + tile;
			Vector3 trapBoundRight = trapCollider.transform.TransformPoint(trapCollider.Bounds().max) - tile;

			min = trapBoundLeft.xAdd(-tileRange).yAdd(-tileRange - yOffset).v2() * CatchingMiceLevelManager.use.scale;
			max = trapBoundRight.xAdd(tileRange).yAdd(tileRange - yOffset).v2() * CatchingMiceLevelManager.use.scale;
		}*/
	}

	// The basic trap routine checks every fixed update frame
	// for enemies within its range (collider or tile range)
	// When a different behavior is necessary, then it
	// should be overridden in the sub-class
	protected virtual IEnumerator TrapRoutine()
	{
		Vector2 min, max;
		CalculateTrapBounds(out min, out max);

		while (CatchingMiceGameManager.use.GameRunning
			&& (ammo > 0)
			&& (health > 0))
		{
			// Check whether an enemy is near
			List<CatchingMiceCharacterMouse> enemies = new List<CatchingMiceCharacterMouse>();
			Collider2D[] colliders = Physics2D.OverlapAreaAll(min, max);

			foreach (Collider2D coll2D in colliders)
			{
				CatchingMiceCharacterMouse enemy = null;

				// colliders are often parented to the object with the character script
				// Still: should do a null check on parent to prevent exceptions.
				if (coll2D.transform.parent != null)
					enemy = coll2D.transform.parent.GetComponent<CatchingMiceCharacterMouse>();

				if (enemy != null)
				{
					enemies.Add(enemy);
				}
			}

			// Hit all the enemies in range
			if (enemies.Count > 0)
			{
				foreach (CatchingMiceCharacterMouse enemy in enemies)
				{
					OnHit(enemy);
				}

				AttackEffect();

				Ammo = Ammo - 1;

				yield return new WaitForSeconds(Interval);
			}
			else
			{
				yield return new WaitForFixedUpdate();
			}
		}
	}

	protected virtual void AttackEffect()
	{
		iTween.Stop(gameObject);
		transform.localScale = originalScale;

		iTween.PunchScale(gameObject, iTween.Hash(
			"amount", Vector3.one * 0.1f,
			"time", 0.5f));
	}

	// The basic action for when a trap's health drops
	// to 0. The default behavior is to destroy the trap.
	// When a different behavior is necessary,
	// then it should be overridden in the sub-class.
	protected virtual void OnEmptyHealth()
	{
		DestroySelf();
	}

	// The basic action for when a trap's ammo drops
	// to 0. The default behavior is to destroy the trap.
	// When a different behavior is necessary,
	// then it should be overridden in the sub-class.
	protected virtual void OnEmptyAmmo()
	{
		if (spriteRenderer != null)
		{
			spriteRenderer.sprite = inactiveSprite;
		}
	}

	protected virtual void OnPlayerInteract()
	{
		if (ammo <= 0)
		{
			if (CatchingMiceGameManager.use.PickupCount >= this.Cost)
			{
				if (spriteRenderer != null)
				{
					spriteRenderer.sprite = activeSprite;
				}

				CatchingMiceGameManager.use.PickupCount -= (int) this.Cost;

				Ammo = originalAmmoCount;

				// this will not be running it anymore - it terminates itself the frame that ammo goes 0
				StartCoroutine(TrapRoutine());

				CatchingMiceLogVisualizer.use.LogError("Refilling ammo on trap: " + transform.Path() + " to " + originalAmmoCount);
			}
		}
		else
		{
			CatchingMiceLogVisualizer.use.LogError("No interaction options for trap: " + transform.Path());
		}
	}

	protected void PlayAttackSound()
	{
		if (!string.IsNullOrEmpty(attackSoundKey))
		{
			LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio(attackSoundKey));
		}
	}

	protected void OnDrawGizmos()
	{
		Vector2 min = Vector2.zero;
		Vector2 max = Vector2.zero;

		CalculateTrapBounds(out min, out max);

		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(transform.position, new Vector3(max.x - min.x, max.y - min.y, 1f));
	}
}
