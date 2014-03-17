using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DartsCockroach : IDartsHitable 
{
	public Vector3 targetLocation = Vector3.zero;
	public float moveTime = 3.0f;


	protected ParticleSystem hitParticles = null;
	protected Vector3 originalLocalPosition = Vector3.zero;

	public virtual void SetupLocal()
	{
		if (hitParticles == null)
			hitParticles = GetComponentInChildren<ParticleSystem>();

		if (hitParticles == null)
			Debug.LogError("DartsCockroach: Missing hit particles!");

		originalLocalPosition = transform.localPosition;
	}
	
	public virtual void SetupGlobal()
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


	public override void OnHit ()
	{
		LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio("BugSquash01"));

		iTween.Stop(gameObject);
		
		// instantiate it instead of playing the child particle - so we don't have to wait with turning off the children
		ParticleSystem spawnedParticles = (ParticleSystem) Instantiate(hitParticles);
		spawnedParticles.transform.position = hitParticles.transform.position;
		spawnedParticles.Play();
		Destroy(spawnedParticles, 2.0f);

		HitCount++;

		iTween.Stop(gameObject);
		this.Shown = false;
		SetTogglePartsActive(false);
	}

	public override void Show ()
	{
		this.Shown = true;
		SetTogglePartsActive(true);

		LugusCoroutines.use.StartRoutine(MoveRoutine());
	}

	protected IEnumerator MoveRoutine()
	{
		iTween.RotateBy(gameObject, iTween.Hash(
			"z", 0.02f,
			"time", moveTime / 50.0f,	// eyeball value
			"looptype", iTween.LoopType.pingPong,
			"easetype", iTween.EaseType.linear
			));

		transform.localPosition = originalLocalPosition;
		gameObject.MoveTo(targetLocation).Time(moveTime).IsLocal(true).Execute();
		
		yield return new WaitForSeconds(moveTime);

		this.Shown = false;
	}


	public override void Hide ()
	{
		// doesn't autohide
	}

	protected void SetTogglePartsActive(bool active)
	{
		foreach(Transform t in transform)
		{
			t.gameObject.SetActive(active);
		}
	}
}
