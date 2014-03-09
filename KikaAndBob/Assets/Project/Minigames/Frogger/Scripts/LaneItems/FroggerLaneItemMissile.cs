using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FroggerLaneItemMissile : MonoBehaviour 
{
	public SpriteRenderer missileSprite = null;
	public ParticleSystem missileSmoke = null;
	public SpriteRenderer missileShadow = null;
	public FroggerExplosion explosion = null;
	
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

	public void Launch(Vector3 targetPosition, float velocity)
	{
		StartCoroutine(LaunchRoutine(targetPosition, velocity));
	}

	private IEnumerator LaunchRoutine(Vector3 targetPosition, float velocity)
	{
		// This routine makes the missile fly towards the targetPosition of
		// where the player stood. The missile is launched upward, and flies
		// to a position off-screen. When the missile is off-screen, it turns
		// around and flies towards the targetPosition. This will also spawn
		// a drop shadow of the missile at the targetPosition. When the missile
		// reaches its destination, the missile, its shadow and smoke particles
		// aren't needed anymore. Afterwards, an explosion happens that will search
		// for the player and other destroyable objects in its area.

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
		transform.Rotate(0f, 0f, 180f);
		transform.position = new Vector3(targetPosition.x, offScreenPos.y, targetPosition.z);

		time = Mathf.Abs(transform.position.y - targetPosition.y) / velocity;

		// Animate the missile by letting it fall down and the shadow by making it bigger over time
		gameObject.MoveTo(targetPosition).Time(time).Execute();
		missileShadowCopy.ScaleTo(missileShadow.transform.localScale).Time(time).Execute();
		yield return new WaitForSeconds(time);

		// Destroy the shadow copy, and disable the missile sprite and particle emission
		GameObject.Destroy(missileShadowCopy.gameObject);
		missileSprite.enabled = false;
		missileSmoke.enableEmission = false;

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
