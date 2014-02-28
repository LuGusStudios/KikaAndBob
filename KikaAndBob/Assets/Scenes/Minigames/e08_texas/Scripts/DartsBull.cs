using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class DartsBull : IDartsHitable 
{
	public Vector3 firstMove = new Vector3(-10.0f, 0, 0);
	public Vector3 secondMove = new Vector3(-30.0f, 0, 0);

	public string hitSoundKey = "CowMoo";
	public string idleAnimation;
	public string runAnimation;
	
	protected Vector3 originalPosition = Vector3.one;
	protected ILugusCoroutineHandle moveRoutine = null;
	protected float moveTime = 3.0f;
	protected ParticleSystem dust = null;
	protected BoxCollider2D boxCollider2D = null;
	protected BoneAnimation characterBoneAnimation = null;
	protected bool stampeding = false;
	protected int hitStreak = 0;
	protected bool offScreen = true;
	
	public override void OnHit()
	{
		if (hitStreak < 5)
		{
			hitStreak++;
			HitCount++;
		}

		if (stampeding == false)
		{
			stampeding = true;

			dust.Play();
			//boxCollider2D.enabled = false; // we only want this to be hit once in a row, but we also don't want to use Hide because then it will immediately be able to respawn

			if (!string.IsNullOrEmpty(hitSoundKey))
				LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio(hitSoundKey));

			if (moveRoutine != null && moveRoutine.Running)
				moveRoutine.StopRoutine();

			moveRoutine = LugusCoroutines.use.StartRoutine(StampedeRoutine());
		}
	}
	
	public override void Show()
	{
		stampeding = false;
		offScreen = true;
		hitStreak = 0;

		if (secondMove.x < 0)
		{
			transform.localScale = transform.localScale.x(-Mathf.Abs(transform.localScale.x));
		}
		else
		{
			transform.localScale = transform.localScale.x(Mathf.Abs(transform.localScale.x));
		}

		transform.position = originalPosition;
		this.Shown = true;

		if (moveRoutine != null && moveRoutine.Running)
			moveRoutine.StopRoutine();

		moveRoutine = LugusCoroutines.use.StartRoutine(WalkInRoutine());
	}

	protected IEnumerator WalkInRoutine()
	{	
		Debug.LogWarning("Bull moving in.");

		characterBoneAnimation[runAnimation].speed = 1.0f;
		characterBoneAnimation.Play(runAnimation);
	
		dust.Stop();
		dust.Clear();
		boxCollider2D.enabled = false;

		gameObject.MoveTo(originalPosition + firstMove).Time(moveTime).Execute();

		yield return new WaitForSeconds(moveTime);


		Debug.LogWarning("Bull moving out.");
		
		boxCollider2D.enabled = true;
		characterBoneAnimation.CrossFade(idleAnimation, 1.0f, PlayMode.StopSameLayer);
	}

	protected IEnumerator StampedeRoutine()
	{
		offScreen = false;
		
		characterBoneAnimation[runAnimation].speed = 2.0f;
		characterBoneAnimation.Play(runAnimation);
		dust.Play();

		gameObject.MoveTo(originalPosition + secondMove).Time(moveTime).Execute();

		yield return new WaitForSeconds(moveTime);

		offScreen = true;

		transform.position = originalPosition;
		this.Shown = false;
		stampeding = false;
	}

	protected IEnumerator RunBackRoutine()
	{
		offScreen = false;

		transform.localScale = transform.localScale.x(-transform.localScale.x);	// turn bull around

		characterBoneAnimation[runAnimation].speed = 1.0f;
		characterBoneAnimation.Play(runAnimation);
		gameObject.MoveTo(originalPosition).Time(moveTime).Execute();

		yield return new WaitForSeconds(moveTime);

		offScreen = true;
		
		transform.position = originalPosition;
		this.Shown = false;
	}

	public override void Hide()
	{	
		if (!stampeding) // if the bull is already running offscreen, don't allow autohide					
		{
			iTween.Stop(gameObject);

			if (moveRoutine != null && moveRoutine.Running)
				moveRoutine.StopRoutine();

			// if this is being autohidden and stampede was never started, let bull run back

			// this check will prevent the RunBackRoutine from running when everything is being disabled by the functionality group on Start()
			if (Vector3.Distance(transform.position, originalPosition) > 0.1f)
			{
				moveRoutine = LugusCoroutines.use.StartRoutine(RunBackRoutine());
			}
			else
			{
				transform.position = originalPosition;
				this.Shown = false;
			}
		}
	}

	public override int GetScore ()
	{
		return group.score * hitStreak;
	}
	
	public void SetupLocal()
	{
		originalPosition = transform.position;

		if (dust == null)
			dust = GetComponentInChildren<ParticleSystem>();
		if (dust == null)
			Debug.LogError(name + " did not find a dust particle system for this bull .");

		if (boxCollider2D == null)
			boxCollider2D = GetComponent<BoxCollider2D>();
		if (boxCollider2D == null)
			Debug.LogError(name + " did not find a BoxCollider2D.");

		if (characterBoneAnimation == null)
			characterBoneAnimation = GetComponentInChildren<BoneAnimation>();
		if (characterBoneAnimation == null)
			Debug.LogError("DartsWindow: Missing character animation transform.");
	}
	
	
	public void SetupGlobal()
	{
		
	}
	
	protected void Awake()
	{
		SetupLocal();
	}
	
	protected void Start () 
	{
		SetupGlobal();
	}
	
	protected void Update () 
	{
		
	}
}
