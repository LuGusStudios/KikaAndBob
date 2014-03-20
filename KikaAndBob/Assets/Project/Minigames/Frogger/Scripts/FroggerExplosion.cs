using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// The Frogger Explosion class will, once Explode() is called,
/// look for the player object, and see whether it is in range.
/// It will also look for FroggerDestructible objects in the
/// scene that lie in range.
[RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
public class FroggerExplosion : MonoBehaviour 
{
	public enum ExplosionState
	{
		NONE = -1,
		ANIMATING = 1,
		STOPPED = 2
	};

	public ExplosionState State
	{
		get
		{
			return explosionState;
		}
	}

	// Return the location of the center in world coordinates
	public Vector2 BlastCenterWorld
	{
		get
		{
			return new Vector2(blastCenter.x * transform.localScale.x + transform.position.x,
				blastCenter.y * transform.localScale.y + transform.position.y);
		}

		set
		{
			blastCenter = new Vector2((value.x - transform.position.x) / transform.localScale.x,
				(value.y - transform.position.y) / transform.localScale.y);
		}
	}

	// Return the location of the center in local coordinates
	public Vector2 BlastCenterLocal
	{
		get
		{
			return new Vector2(blastCenter.x * transform.localScale.x, blastCenter.y * transform.localScale.y);
		}

		set
		{
			blastCenter = new Vector2(value.x / transform.localScale.x, value.y / transform.localScale.y);
		}
	}

	public Vector2 BlastSize
	{
		get
		{
			return blastSize;
		}

		set
		{
			blastSize = value;
		}
	}

	public float animatonLength = 1f;
	public Vector2 blastCenter = Vector2.zero;
	public Vector2 blastSize = Vector2.one;
	public string explosionSFXName = "";
	public float explosionVolume = 1f;

	protected ExplosionState explosionState = ExplosionState.STOPPED;

	public void EnforceBlastRangeHeight(BoxCollider2D coll2D)
	{
		Vector2 blastCenter = BlastCenterLocal;
		blastCenter.y = coll2D.center.y;
		BlastCenterLocal = blastCenter;

		Vector2 newBlastSize = BlastSize;
		newBlastSize.y = coll2D.size.y * 0.9f;
		BlastSize = newBlastSize;
	}

	public void Explode()
	{
		if (explosionState == ExplosionState.STOPPED)
		{
			StartCoroutine(ExplodeRoutine());
		}
	}

	private IEnumerator ExplodeRoutine()
	{
		// Start animating the explosion
		explosionState = ExplosionState.ANIMATING;
		
		Animator animator = GetComponent<Animator>();
		SpriteRenderer renderer = GetComponent<SpriteRenderer>();

		animator.enabled = true;
		renderer.enabled = true;

		// Play the explosion sound effect
		AudioClip explosionSFX = LugusResources.use.Shared.GetAudio(explosionSFXName);
		if (explosionSFX != LugusResources.use.errorAudio)
		{
			ILugusAudioTrack track = LugusAudio.use.SFX().Play(explosionSFX);
			track.Volume = explosionVolume;
		}

		// Place the explosion just before the camera, because it should appear over everything else
		transform.position = transform.position.z(Camera.main.transform.position.z + 1f);

		// Check whether the player is within the blast radius
		GameObject playerObj = GameObject.Find("Player");
		if (playerObj != null)
		{
			FroggerCharacter player = playerObj.GetComponent<FroggerCharacter>();
			if (player != null)
			{
				Vector2 centerWorld = BlastCenterWorld;
				Vector2 sizeWorld = blastSize;
				Bounds bounds = new Bounds(new Vector3(centerWorld.x, centerWorld.y, transform.position.z),
					new Vector3(sizeWorld.x, sizeWorld.y, float.PositiveInfinity));

				if (bounds.Contains(new Vector3(player.transform.position.x, player.transform.position.y, 0f))
					&& FroggerGameManager.use.gameRunning)
				{
					player.Blink(Color.red, 1f, 3);
					FroggerGameManager.use.LoseGame();
					player.DoHitAnimation();
				}
			}
		}
		
		// Check for other objects within the blast radius that can be destroyed
		float xOffset = blastSize.x * 0.5f;
		float yOffset = blastSize.y * 0.5f;

		Vector2[] hitCorners = new Vector2[2];
		hitCorners[0] = new Vector2(transform.position.x + blastCenter.x - xOffset, transform.position.y + blastCenter.y - yOffset);
		hitCorners[1] = new Vector2(transform.position.x + blastCenter.x + xOffset, transform.position.y + blastCenter.y + yOffset);

		Collider2D[] hits = Physics2D.OverlapAreaAll(hitCorners[0], hitCorners[1]);
		foreach (Collider2D hit in hits)
		{
			FroggerLaneItemDestructible destructible = hit.GetComponent<FroggerLaneItemDestructible>();
			if (destructible != null)
			{
				destructible.Destruct();
			}
		}

		yield return new WaitForSeconds(animatonLength);

		explosionState = ExplosionState.STOPPED;
		animator.enabled = false;
		renderer.enabled = false;

		yield break;
	}
}
