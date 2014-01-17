using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DartsBull : IDartsHitable 
{
	public string hitSoundKey = "CowMoo";
	protected Vector3 originalPosition = Vector3.one;
	protected ILugusCoroutineHandle moveRoutine = null;
	protected float moveTime = 3.0f;
	protected ParticleSystem dust = null;
	
	public override void OnHit()
	{
		HitCount++;
		dust.Play();
		this.GetComponent<BoxCollider2D>().enabled = false; // we only want this to be hit once in a row, but we also don't want to use Hide because then it will immediately be able to respawn
		LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio(hitSoundKey));
	}
	
	public override void Show()
	{
		this.GetComponent<BoxCollider2D>().enabled = true;
		transform.position = originalPosition;
		this.Shown = true;
		moveRoutine = LugusCoroutines.use.StartRoutine(MoveRoutine());
	}
	
	protected IEnumerator MoveRoutine()
	{
		gameObject.MoveTo(originalPosition + new Vector3(-30.0f, 0, 0)).Time(moveTime).Execute();
		yield return new WaitForSeconds(moveTime);
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
		
		dust = GetComponentInChildren<ParticleSystem>();
		if (dust == null)
		{
			Debug.LogError(name + " did not find a dust particle system for this bull .");
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
