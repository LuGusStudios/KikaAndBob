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

	public enum BeesEndEffect
	{
		NONE = -1,
		SCALEFADE = 1,
		OFFSCREEN = 2,
		BACKTOHIVE = 4
	};

	public BeesEndEffect endEffect = BeesEndEffect.NONE;

	protected float speed = 1f;
	protected float aliveTime = 0f;
	protected Transform target = null;

	protected float scaleFadeFactor = 0.1f;
	protected bool scaleFadeEnabled = false;

	protected Vector3 beeHivePosition = Vector3.zero;

	private bool initialized = false;

	public void FollowTarget(Transform target, float aliveTime, float speed)
	{
		if (target == null)
		{
			return;
		}

		this.target = target;
		this.aliveTime = aliveTime;
		this.speed = speed;

		this.beeHivePosition = transform.position;

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
		float scalingTime = 0.5f;

		// Set the scale of the object to be small at first
		if (scaleFadeEnabled)
		{
			gameObject.ScaleTo(transform.localScale / scaleFadeFactor).Time(scalingTime).Execute();
			yield return new WaitForSeconds(scalingTime);
		}

		// Follow the target until the alive timer runs out
		Vector2 translation = Vector2.one;
		while (FroggerGameManager.use.GameRunning)
		{
			aliveTime -= Time.deltaTime;

			if (aliveTime < 0f)
			{
				break;
			}

			translation = new Vector2(target.position.x - transform.position.x, target.position.y - transform.position.y);
			translation = translation.normalized * speed * Time.deltaTime;

			transform.position = transform.position + new Vector3(translation.x, translation.y, 0f);
			transform.position = transform.position.z(Camera.main.transform.position.z + 1f);

			DetermineDirection(translation.x);

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

		switch (endEffect)
		{
			case BeesEndEffect.OFFSCREEN:
				// Find a position off-screen to fly off to
				translation = translation.normalized * (-20f);
				DetermineDirection(translation.x);
				gameObject.MoveTo(transform.position + new Vector3(translation.x, translation.y, 0f)).Time(1f).Execute();
				yield return new WaitForSeconds(1f);
				break;
			case BeesEndEffect.SCALEFADE:
				// Scale the bees down to let the fade away
				gameObject.ScaleTo(transform.localScale * scaleFadeFactor).Time(scalingTime).Execute();
				yield return new WaitForSeconds(scalingTime);
				break;
			case BeesEndEffect.BACKTOHIVE:
				// Let the bees fly back to the hive
				DetermineDirection(beeHivePosition.x - transform.position.x);
				gameObject.MoveTo(new Vector3(beeHivePosition.x, beeHivePosition.y, transform.position.z)).Time(1f).Execute();
				yield return new WaitForSeconds(1f);
				gameObject.ScaleTo(transform.localScale * scaleFadeFactor).Time(scalingTime).Execute();
				yield return new WaitForSeconds(scalingTime);
				break;
		}

		GameObject.Destroy(gameObject);
	}

	protected void DetermineDirection(float xTranslation)
	{
		// If the bees should go to the left, but are still facing right, or
		// if the bees should go right, but are still facing left,
		// then flip the x-scale to rotate them
		if (((xTranslation < 0f) && (transform.localScale.x > 0f))
			|| ((xTranslation > 0f) && (transform.localScale.x < 0f)))
		{
			transform.localScale = transform.localScale.x(transform.localScale.x * -1f);
		}
	}
}
