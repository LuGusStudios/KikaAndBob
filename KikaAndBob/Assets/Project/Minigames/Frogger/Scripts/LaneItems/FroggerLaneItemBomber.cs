using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FroggerLaneItemBomber : FroggerCollider 
{

	public SpriteRenderer bomb = null;
	public SpriteRenderer bombDropShadow = null;
	public FroggerExplosion explosion = null;
	public float bombDropSpeed = 5f;

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{
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
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
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

		// Animate the bomb by letting it fall down
		bombCopy.gameObject.MoveTo(targetPos).Time(time).Execute();

		// Animate the shadow by letting it get bigger
		bombShadowCopy.gameObject.ScaleTo(bombDropShadow.transform.localScale).Time(time).Execute();

		yield return new WaitForSeconds(time);

		// Destroy the bomb and its shadow
		GameObject.Destroy(bombCopy.gameObject);
		GameObject.Destroy(bombShadowCopy.gameObject);

		// Create the explosion
		GameObject explosionCopy = (GameObject)GameObject.Instantiate(explosion.gameObject);
		explosionCopy.transform.position = new Vector3(spawnPos.x, explosion.transform.position.y, explosion.transform.position.z);
		explosionCopy.transform.parent = transform.parent;
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
