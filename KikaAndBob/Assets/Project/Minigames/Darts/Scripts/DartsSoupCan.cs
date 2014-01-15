using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class DartsSoupCan : IDartsHitable 
{
	public string hitAnimation = "Can_Closed";
	public string hitSoundKey = "";
	public Vector3 hitMove = new Vector3(1, 0.3f, 0);
	public float airborneTime = 1.0f;

	protected BoneAnimation boneAnimation = null;
	protected bool flying = false;
	protected Vector3 originalPosition = Vector3.zero;
	protected ILugusCoroutineHandle routineHandle = null;
	protected int hitStreak = 0;

	public override void OnHit()
	{
		// like anyone is ever gonna be this badass...
		if (hitStreak < 6)
			hitStreak++;

		HitCount++;
		LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio(hitSoundKey));

		if (!flying)
			routineHandle = LugusCoroutines.use.StartRoutine(AnimationRoutine());
	}

	protected IEnumerator AnimationRoutine()
	{
		flying = true;

		this.renderer.enabled = false;
		boneAnimation.gameObject.SetActive(true);
		boneAnimation.Play(hitAnimation);


		Vector3 beginning = transform.localPosition;
		this.gameObject.MoveTo(beginning + hitMove).Time(airborneTime * 0.5f).IsLocal(true).MoveToPath(false).EaseType(iTween.EaseType.easeOutQuad).Execute();

		yield return new WaitForSeconds(airborneTime * 0.5f);


		this.gameObject.MoveTo(beginning + new Vector3(0, -1, 0)).Time(airborneTime * 0.5f).IsLocal(true).MoveToPath(false).EaseType(iTween.EaseType.easeInQuad).Execute();

		yield return new WaitForSeconds(airborneTime * 0.5f);


		Hide();
		flying = false;

		this.gameObject.transform.localPosition = beginning;

		boneAnimation.gameObject.SetActive(false);
	}
	
	public override void Show()
	{
		this.Shown = true;
		this.renderer.enabled = true;
	}
	
	public override void Hide()
	{
		hitStreak = 0;

		this.Shown = false;
		flying = false;

		this.renderer.enabled = false;
		boneAnimation.gameObject.SetActive(false);

		transform.position = originalPosition;
		iTween.Stop(gameObject);
		if (routineHandle != null && routineHandle.Running)
		{
			routineHandle.StopRoutine();
		}
	}
	

	public void SetupLocal()
	{
		boneAnimation = GetComponentInChildren<BoneAnimation>();
		if (boneAnimation != null)
		{
			boneAnimation.gameObject.SetActive(false);
		}
		else
		{
			Debug.Log("No bone animation parented to this game object found.");
		}

		originalPosition = transform.position;
	}

	public override int GetScore ()
	{
		return score * hitStreak;
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
