using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DartsTumbleweed : IDartsHitable 
{
	public Vector3 move = new Vector3(25.0f, 0, 0);
	public string hitSoundKey = "";
	protected Vector3 originalPosition = Vector3.one;
	protected ILugusCoroutineHandle moveRoutine = null;
	public float moveTime = 3.0f;
	protected ParticleSystem hitParticles = null;
	protected BoxCollider2D boxCollider2D = null;
	
	public override void OnHit()
	{
		renderer.enabled = false;
		HitCount++;
		boxCollider2D.enabled = false; // we only want this to be hit once in a row, but we also don't want to use Hide because then it will immediately be able to respawn

		if (!string.IsNullOrEmpty(hitSoundKey))
			LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio(hitSoundKey));

		LugusCoroutines.use.StartRoutine(HitRoutine());
	}
	
	public override void Show()
	{
		renderer.enabled = true;
		boxCollider2D.enabled = true;
		transform.position = originalPosition;
		this.Shown = true;
		moveRoutine = LugusCoroutines.use.StartRoutine(MoveRoutine());
	}
	
	protected IEnumerator MoveRoutine()
	{
		float rotate = 3.0f;

		if (move.x > 0)
			rotate *= -1;

		// using old fashioned iTween here because Itweener doesn't have Rotate methods yet and they'd take quite a while to implement just for this
		iTween.RotateBy(gameObject, iTween.Hash(
			"amount", new Vector3(0, 0, rotate),
			"time", moveTime,
			"easetype", iTween.EaseType.linear));

		gameObject.MoveTo(originalPosition + move).Time(moveTime).Execute();
		yield return new WaitForSeconds(moveTime);
		Hide();
	}
	
	protected IEnumerator HitRoutine()
	{
		if (hitParticles != null)
			hitParticles.Play();

		iTween.Stop(gameObject);

		if (moveRoutine != null && moveRoutine.Running)
			moveRoutine.StopRoutine();

		yield return new WaitForSeconds(1.0f);
		
		Hide();
	}
	
	public override void Hide()
	{
		transform.position = originalPosition;
		this.Shown = false;
		iTween.Stop(gameObject);
		
		if (moveRoutine != null && moveRoutine.Running)
		{
			moveRoutine.StopRoutine();
		}
	}
	
	
	public void SetupLocal()
	{
		originalPosition = transform.position;

		if (hitParticles == null)
			hitParticles = GetComponentInChildren<ParticleSystem>();
		if (hitParticles == null)
			Debug.LogError(name + " did not find a hit particle system for this object .");

		if (boxCollider2D == null)
			boxCollider2D = GetComponent<BoxCollider2D>();
		if (boxCollider2D == null)
			Debug.LogError(name + " did not find a BoxCollider2D.");
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
