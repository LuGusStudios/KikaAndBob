using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FroggerLaneItemMissile : MonoBehaviour 
{
	public SpriteRenderer missileSprite = null;
	public ParticleSystem missileSmoke = null;
	public SpriteRenderer missileShadow = null;
	public FroggerExplosion explosion = null;
	public string missileFallingSFXName = "";
	public float missileFallingVolume = 1f;
	public bool enforceLaneOnly = false;
	
	public void SetupGlobal()
	{
		if (missileSprite == null)
		{
			missileSprite = transform.FindChild("MissileSprite").GetComponent<SpriteRenderer>();
			if (missileSprite == null)
			{
				Debug.LogError("Could not find the sprite of the missile!");
			}
			else
			{
				missileSprite.enabled = false;
			}
		}

		if (missileSmoke == null)
		{
			missileSmoke = transform.FindChild("MissileSmoke").GetComponent<ParticleSystem>();
			if (missileSmoke == null)
			{
				Debug.LogError("Could not find the smoke for the missile!");
			}
			else
			{
				missileSmoke.enableEmission = false;
			}
		}

		if (missileShadow == null)
		{
			missileShadow = transform.FindChild("MissileShadow").GetComponent<SpriteRenderer>();
			if (missileShadow == null)
			{
				Debug.LogError("Could not find the drop shadow for the missile!");
			}
			else
			{
				missileShadow.enabled = false;
			}
		}

		if (explosion == null)
		{
			explosion = transform.FindChild("Explosion").GetComponent<FroggerExplosion>();
			if (explosion == null)
			{
				Debug.LogError("Could not find the explosion for the missile!");
			}
		}
	}

	protected void Start () 
	{
		SetupGlobal();
	}

	public void Launch(FroggerCharacter target, float velocity)
	{
		StartCoroutine(LaunchRoutine(target, velocity));
	}

	private IEnumerator LaunchRoutine(FroggerCharacter target, float velocity)
	{
		// This routine makes the missile fly towards the targetPosition of
		// where the player stood. The missile is launched upward, and flies
		// to a position off-screen. When the missile is off-screen, it turns
		// around and flies towards the targetPosition. This will also spawn
		// a drop shadow of the missile at the targetPosition. When the missile
		// reaches its destination, the missile, its shadow and smoke particles
		// aren't needed anymore. Afterwards, we check if it hits on a water lane
		// or not, so that maybe the missile just splashes in the water. Else, it
		// just explode.

		SetupGlobal();

		Vector3 targetPosition = target.transform.position;
		FroggerLane targetLane = target.CurrentLane;

		if (enforceLaneOnly)
		{
			BoxCollider2D coll2D = targetLane.GetComponent<BoxCollider2D>();
			if (coll2D != null)
			{
				explosion.EnforceBlastRangeHeight(coll2D);
			}
		}

		// Enable the necessary sprite renderers and particle systems
		missileSprite.enabled = true;
		missileSmoke.enableEmission = true;

		// Let the missile fly upward at first, until it leaves the screen
		Vector3 offScreenPos = Camera.main.ScreenToWorldPoint(new Vector3(0f, Camera.main.pixelHeight, 0f));
		offScreenPos.x = transform.position.x;
		offScreenPos.y += missileSprite.sprite.bounds.extents.y * missileSprite.transform.localScale.y;
		offScreenPos.z = transform.position.z;

		// Calculate the time it takes for the missile to fly off-screen
		float time = Mathf.Abs(transform.position.y - offScreenPos.y) / velocity;

		// Animate the missile by letting it fly upward
		gameObject.MoveTo(offScreenPos).Time(time).Execute();
		yield return new WaitForSeconds(time);

		// Modify the target position a little bit so that it falls just
		// before the target
		targetPosition.z = targetPosition.z - 1f;

		// Spawn a shadow of the missile and place the shadow at the
		// target position and make it small
		GameObject missileShadowCopy = (GameObject)GameObject.Instantiate(missileShadow.gameObject);
		missileShadowCopy.transform.position = targetPosition;
		missileShadowCopy.transform.localScale = missileShadow.transform.localScale * 0.1f;
		missileShadowCopy.transform.parent = transform.parent;
		missileShadowCopy.GetComponent<SpriteRenderer>().enabled = true;

		// Let the missile turn around and fall down on the targetPosition
		transform.localScale = transform.localScale.y(-transform.localScale.y);
		transform.position = new Vector3(targetPosition.x, offScreenPos.y, targetPosition.z);

		time = Mathf.Abs(transform.position.y - targetPosition.y) / velocity;

		// Animate the missile by letting it fall down and the shadow by making it bigger over time
		gameObject.MoveTo(targetPosition).Time(time).Execute();
		missileShadowCopy.ScaleTo(missileShadow.transform.localScale).Time(time).Execute();

		// Play the falling SFX of the missile
		AudioClip fallingSFX = LugusResources.use.Shared.GetAudio(missileFallingSFXName);
		if (fallingSFX != LugusResources.use.errorAudio)
		{
			ILugusAudioTrack track = LugusAudio.use.SFX().Play(fallingSFX);
			track.Volume = missileFallingVolume;
		}

		yield return new WaitForSeconds(time);

		// Destroy the shadow copy, and disable the missile sprite and particle emission
		GameObject.Destroy(missileShadowCopy.gameObject);
		missileSprite.enabled = false;
		missileSmoke.enableEmission = false;

		// See if the missile falls down on a water lane
		if (targetLane != null)
		{
			FroggerLaneWater waterLane = targetLane.GetComponent<FroggerLaneWater>();
			if (waterLane != null)
			{
				RaycastHit2D[] hits = Physics2D.RaycastAll(new Vector3(targetPosition.x, targetPosition.y, float.NegativeInfinity), Vector3.forward);
				bool laneItemFound = false;
				foreach (RaycastHit2D hit in hits)
				{
					if (hit.transform.GetComponent<FroggerLaneItem>() != null)
					{
						laneItemFound = true;
						break;
					}
				}

				// If we're in a water lane, and didn't fell on a lane item, then let is splash
				if (!laneItemFound)
				{
					waterLane.DoSplashAnimation(targetPosition);
					waterLane.PlaySplashSFX();
					GameObject.Destroy(gameObject);
					yield break;
				}
			}
		}

		// If we come here, then EXPLOSION!
		StartCoroutine(ExplosionRoutine());

	}

	private IEnumerator ExplosionRoutine()
	{
		// Display the explosion and destroy this missile object once the explosion is done
		explosion.Explode();
		while (true)
		{
			if (explosion.State == FroggerExplosion.ExplosionState.STOPPED)
			{
				GameObject.Destroy(gameObject);
				yield break;
			}
			yield return new WaitForEndOfFrame();
		}
	}
}
