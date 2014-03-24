using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FroggerLaneItemBomber : FroggerCollider 
{

	public SpriteRenderer bomb = null;
	public SpriteRenderer bombDropShadow = null;
	public FroggerExplosion explosion = null;
	public bool enforceLaneOnly = false;
	public float bombDropSpeed = 5f;
	public string bomberPresenceSound = "";
	public float bomberPresenceVolume = 1f;

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public override void SetupGlobal()
	{
		base.SetupGlobal();

		if (bomb == null)
		{
			bomb = transform.FindChild("Bomb").GetComponent<SpriteRenderer>();
			if (bomb == null)
			{
				Debug.LogError("Could not find the bomb!");
			}
			else
			{
				bomb.enabled = false;
			}
		}

		if (bombDropShadow == null)
		{
			bombDropShadow = transform.FindChild("BombDropShadow").GetComponent<SpriteRenderer>();
			if (bombDropShadow == null)
			{
				Debug.LogError("Could not find the bomb drop shadow!");
			}
			else
			{
				bombDropShadow.enabled = false;
			}
		}

		if (explosion == null)
		{
			explosion = transform.FindChild("Explosion").GetComponent<FroggerExplosion>();
			if (explosion == null)
			{
				Debug.LogError("Could not find the explosion!");
			}
		}

		if (FroggerGameManager.use.GameRunning)
		{
			// Play the bomber fly-over sound when the bomber spawns
			AudioClip flyOverSFX = LugusResources.use.Shared.GetAudio(bomberPresenceSound);
			if (flyOverSFX != LugusResources.use.errorAudio)
			{
				// First check if its not playing already, because we don't want to clutter the sound buffer
				bool isAlreadyPlaying = false;
				List<ILugusAudioTrack> tracks = LugusAudio.use.SFX().Tracks;
				foreach (ILugusAudioTrack track in tracks)
				{
					if (track.Source.isPlaying && (track.Source.clip.name == bomberPresenceSound))
					{
						isAlreadyPlaying = true;
						break;
					}
				}


				if (!isAlreadyPlaying)
				{
					ILugusAudioTrack track = LugusAudio.use.SFX().Play(flyOverSFX);
					track.Volume = bomberPresenceVolume;
				}
			}
		}
	}
	
	private void Awake()
	{
		SetupLocal();
	}

	private void Start () 
	{
		SetupGlobal();
	}

	protected override void EnterSurfaceEffect(FroggerCharacter character)
	{
		// When the character enters triggers the collider, drop a bomb on the current location of the character
		Vector2 charPos = new Vector2(character.transform.position.x, character.transform.position.y);
		StartCoroutine(DropBombRoutine(charPos));
	}

	private IEnumerator DropBombRoutine(Vector2 charPos)
	{
		// This routine drops a bomb at the current location of the player
		// It spawns a bomb off-screen and initializes a drop shadow that gets smaller
		// as the bomb closes in on its target position.
		// When the bomb reaches its target position, the bomb and its shadow
		// are no longer needed and are destroyed. Afterwards, an explosion
		// is created that will search for the player and other destroyable objects
		// in its area.

		// Find a position off screen to spawn the bomb
		Vector3 spawnPos = Camera.main.ScreenToWorldPoint(new Vector3(0f, Camera.main.pixelHeight, 0f));
		spawnPos.x = charPos.x;
		spawnPos.y += bomb.sprite.bounds.extents.y * bomb.transform.localScale.y;
		spawnPos.z = bomb.transform.position.z;

		// Spawn the bomb
		GameObject bombCopy = (GameObject)GameObject.Instantiate(bomb.gameObject);
		bombCopy.transform.position = spawnPos;
		bombCopy.transform.parent = transform.parent;
		bombCopy.GetComponent<SpriteRenderer>().enabled = true;

		// Spawn the shadow of the bomb, starting with a very small scale
		GameObject bombShadowCopy = (GameObject)GameObject.Instantiate(bombDropShadow.gameObject);
		bombShadowCopy.transform.position = new Vector3(spawnPos.x, bombDropShadow.transform.position.y, bombDropShadow.transform.position.z);
		bombShadowCopy.transform.localScale = bombDropShadow.transform.localScale * 0.1f;
		bombShadowCopy.transform.parent = transform.parent;
		bombShadowCopy.GetComponent<SpriteRenderer>().enabled = true; ;

		// Calculate time it takes for the bomb to fall down on the target position
		Vector3 targetPos = new Vector3(charPos.x, transform.position.y, spawnPos.z);
		float time = Mathf.Abs(targetPos.y - spawnPos.y) / bombDropSpeed;

		// Animate the bomb by letting it fall down and the shadow by making it bigger over time
		bombCopy.MoveTo(targetPos).Time(time).Execute();
		bombShadowCopy.ScaleTo(bombDropShadow.transform.localScale).Time(time).Execute();
		yield return new WaitForSeconds(time);

		// Destroy the bomb and its shadow
		GameObject.Destroy(bombCopy.gameObject);
		GameObject.Destroy(bombShadowCopy.gameObject);

		// Create the explosion
		GameObject explosionCopy = (GameObject)GameObject.Instantiate(explosion.gameObject);
		explosionCopy.transform.position = new Vector3(spawnPos.x, explosion.transform.position.y, explosion.transform.position.z);
		explosionCopy.transform.parent = transform.parent;

		BoxCollider2D coll2D = GetComponent<BoxCollider2D>();
		if (coll2D != null)
		{
			explosionCopy.GetComponent<FroggerExplosion>().EnforceBlastRangeHeight(coll2D);
		}

		explosionCopy.GetComponent<FroggerExplosion>().Explode();

		// Wait until the explosion is done, and destroy the explosion copy
		while (true)
		{
			if (explosionCopy.GetComponent<FroggerExplosion>().State == FroggerExplosion.ExplosionState.STOPPED)
			{
				GameObject.Destroy(explosionCopy.gameObject);
				break;
			}

			yield return new WaitForEndOfFrame();
		}
	}
}
