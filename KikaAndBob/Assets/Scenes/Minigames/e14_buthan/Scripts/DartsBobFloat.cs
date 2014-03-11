using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class DartsBobFloat : DartsToggle 
{
	protected float wanderRadius = 5.0f;

	public BoneAnimation[] boneAnimations = null;
	protected ILugusCoroutineHandle blinkRoutine = null;
	protected Vector3 originalPosition = Vector3.zero;
	protected float minReachedDistance = 0.05f;
	protected float speed = 1.0f;
	protected Transform shadow;
	protected Vector3 shadowScale = Vector3.zero;
	protected float maxShadowDistance = 5.0f;
	protected float shadowDistanceOffset = 0.0f;

	public override void OnHit ()
	{
		base.OnHit ();
		if (blinkRoutine != null && blinkRoutine.Running)
		{
			blinkRoutine.StopRoutine();
		}

		blinkRoutine = LugusCoroutines.use.StartRoutine(SmoothMovesUtil.Blink(boneAnimations, Color.red, 1.0f, 5));
	}

	public override void Show ()
	{
		base.Show();
		shadow.gameObject.SetActive(true);
	}

	public override void Hide ()
	{
		// WE DO NOT ALLOW BOB TO HIDE - he stays on screen until he's disabled by entering a new level
	}

	public override void Disable ()
	{
		SetupGlobal();	// we need to call this once, so the shadow can be found (see why this is not parented under Update)

		base.Hide();	// do standard hide for immediate disable (on game start)

		shadow.gameObject.SetActive(false);
	}

	public override void SetupGlobal()
	{
		base.SetupGlobal();

		if (boneAnimations == null || boneAnimations.Length <= 0)
			boneAnimations = GetComponentsInChildren<BoneAnimation>(true);
		if (boneAnimations == null || boneAnimations.Length <= 0)
			Debug.LogError("DartsBobFloat: Missing Bob bone animations...");

		if (shadow == null)
		{
			shadow = transform.FindChildRecursively("Shadow");
		}
		if (shadow == null)
		{
			Debug.LogError("DartsBobFloat: Missing shadow.");
		}

		// we UNparent the shadow - this way we can move it separately from the Bob animation (which is moved with Itween)
		// we keep it parented until this point, for neatness and to be able to easily get a reference to it below
		if (shadow != null)
		{
			shadow.parent = this.transform.parent;
			shadowScale = shadow.transform.localScale;
			shadowDistanceOffset = Vector2.Distance(transform.position.v2(), shadow.position.v2() );
		}

		originalPosition = transform.position;


		LugusCoroutines.use.StartRoutine(FloatRoutine());
	}

	private void Update () 
	{
		shadow.position = shadow.position.x(this.transform.position.x);

		float distancePercentage = 	(Vector2.Distance(transform.position.v2(), shadow.position.v2()) - shadowDistanceOffset) / maxShadowDistance;

		shadow.localScale = Vector3.Lerp(shadowScale, shadowScale * 0.5f, distancePercentage);
		
				
	}

	protected IEnumerator FloatRoutine()
	{
		int maxIterations = 20;

		while(true)
		{
			Vector3 targetLocation = Vector3.zero;
			float duration = 0.0f;
			float distance = 0.0f;
			int iterationCounter = 0;
			bool goodLocationFound = false;

			while (!goodLocationFound && iterationCounter < maxIterations)
			{
				iterationCounter++;

				targetLocation = originalPosition + (Random.insideUnitCircle * wanderRadius).v3();
				distance = Vector2.Distance(targetLocation.v2(), transform.position.v2());
				duration = distance / speed;

				if (distance < 2.0f)
					continue;
			
				if (shadow.transform.position.y > targetLocation.y)
					continue;

				goodLocationFound = true;
			}

			gameObject.MoveTo(targetLocation).Time(duration).EaseType(iTween.EaseType.linear).Execute();

	
			yield return new WaitForSeconds(duration);

			iTween.Stop(gameObject);	// this just makes sure there isn't any itweens still running

			yield return null;
		}
	}
}
