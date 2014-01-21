using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DartsEagle : IDartsHitable 
{
	public string hitSoundKey = "";
	protected Vector3 originalPosition = Vector3.one;
	protected ILugusCoroutineHandle moveRoutine = null;
	protected float moveTime = 3.0f;
	protected ParticleSystem feathers = null;
	protected Animator anim = null;

	public override void OnHit()
	{
		anim.speed = 4;
		HitCount++;
		feathers.Play();
		this.GetComponent<BoxCollider2D>().enabled = false; // we only want this to be hit once in a row, but we also don't want to use Hide because then it will immediately be able to respawn
		LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio(hitSoundKey));
		LugusCoroutines.use.StartRoutine(CrashRoutine());
	}
	
	public override void Show()
	{
		anim.speed = 1;
		this.GetComponent<BoxCollider2D>().enabled = true;
		transform.position = originalPosition;
		this.Shown = true;
		moveRoutine = LugusCoroutines.use.StartRoutine(MoveRoutine());
	}

	protected IEnumerator MoveRoutine()
	{
		gameObject.MoveTo(originalPosition + new Vector3(20.0f, 0, 0)).Time(moveTime).Execute();
		yield return new WaitForSeconds(moveTime);
		Hide();
	}

	protected IEnumerator CrashRoutine()
	{
		iTween.Stop(gameObject);
		if (moveRoutine != null && moveRoutine.Running)
		{
			moveRoutine.StopRoutine();
		}

		gameObject.MoveTo(originalPosition + new Vector3(20.0f, -5.0f, 0)).Time(moveTime * 0.5f).Execute();

		yield return new WaitForSeconds(moveTime * 0.5f);

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

		feathers = GetComponentInChildren<ParticleSystem>();
		if (feathers == null)
		{
			Debug.LogError(name + " did not find a feather particle system for this eagle .");
		}

		anim = GetComponent<Animator>();
		if (anim == null)
		{
			Debug.LogError(name + " did not find an animator for this eagle.");
		}
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
