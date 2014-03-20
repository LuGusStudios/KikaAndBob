using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class FroggerBees : MonoBehaviour 
{
	public bool Initialized
	{
		get
		{
			return initialized;
		}
	}

	protected float speed = 1f;
	protected float aliveTime = 0f;
	protected Transform target = null;

	protected float scaleFadeFactor = 0.1f;
	protected bool scaleFadeEnabled = false;

	private bool initialized = false;

	public void FollowTarget(Transform target, float aliveTime, float speed)
	{
		this.target = target;
		this.aliveTime = aliveTime;
		this.speed = speed;

		StartCoroutine(FollowTargetRoutine());
	}

	public void ScaleFade()
	{
		scaleFadeEnabled = true;
		transform.localScale = transform.localScale * scaleFadeFactor;
	}

	protected void Start()
	{
		initialized = true;
	}

	private IEnumerator FollowTargetRoutine()
	{
		if (target == null)
		{
			yield break;
		}

		float scalingTime = 0.5f;

		// Set the scale of the object to be small at first
		if (scaleFadeEnabled)
		{
			gameObject.ScaleTo(transform.localScale / scaleFadeFactor).Time(scalingTime).Execute();
			yield return new WaitForSeconds(scalingTime);
		}

		// Follow the target until the alive timer runs out
		while (FroggerGameManager.use.GameRunning)
		{
			aliveTime -= Time.deltaTime;

			if (aliveTime < 0f)
			{
				break;
			}

			Vector2 translation = new Vector2(target.position.x - transform.position.x, target.position.y - transform.position.y);
			translation = translation.normalized * speed * Time.deltaTime;

			transform.position = transform.position + new Vector3(translation.x, translation.y, 0f);
			transform.position = transform.position.z(Camera.main.transform.position.z + 1f);

			// If the bees should go to the left, but are still facing right, or
			// if the bees should go right, but are still facing left,
			// then flip the x-scale to rotate them
			if (((translation.x < 0f) && (transform.localScale.x > 0f))
				|| ((translation.x > 0f) && (transform.localScale.x < 0f)))
			{
				transform.localScale = transform.localScale.x(transform.localScale.x * -1f);
			}

			// Check whether the player is within reach
			BoxCollider2D box2D = GetComponent<BoxCollider2D>();
			Bounds bounds = new Bounds(new Vector3(transform.position.x + box2D.center.x, transform.position.y + box2D.center.y, transform.position.z),
				new Vector3(box2D.size.x, box2D.size.y, float.PositiveInfinity));
			
			if (bounds.Contains(new Vector3(target.position.x, target.position.y, 0f)))
			{
				FroggerCharacter player = target.GetComponent<FroggerCharacter>();
				if (player != null)
				{
					player.Blink(Color.red, 1f, 3);
					FroggerGameManager.use.LoseGame();
					player.DoHitAnimation();
				}
			}

			yield return new WaitForEndOfFrame();
		}

		// When the timer runs out, scale back down and destroy the game object
		if (scaleFadeEnabled)
		{
			gameObject.ScaleTo(transform.localScale * scaleFadeFactor).Time(scalingTime).Execute();
			yield return new WaitForSeconds(scalingTime);
		}

		GameObject.Destroy(gameObject);
	}
}
