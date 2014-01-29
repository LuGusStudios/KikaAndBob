using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class DartsBarrel : IDartsHitable 
{
	public string hitSoundKey = "";
	public string idleAnim = "";
	public string showAnim = "";

	protected ILugusCoroutineHandle peekRoutine = null;
	protected float moveTime = 3.0f;
	protected ParticleSystem feathers = null;
	protected BoneAnimation boneAnimation = null;
	
	public override void OnHit()
	{
		HitCount++;
		LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio(hitSoundKey));
		Hide();
	}
	
	public override void Show()
	{
		print ("--------------------------------------------------------------------");
		this.Shown = true;
		this.renderer.enabled = true;

		boneAnimation.Play(showAnim);

		if (peekRoutine != null && peekRoutine.Running)
		{
			peekRoutine.StopRoutine();
		}

		peekRoutine = LugusCoroutines.use.StartRoutine(PeekabooRoutine());
	}

	// basically this one will be shown for as long as its 'peek' animation calls for
	// unless it's set to hidden by the functionality group having a shorter autohide time than the animation's lengh
	protected IEnumerator PeekabooRoutine()
	{
		while (boneAnimation != null && boneAnimation.IsPlaying(showAnim))
		{
			yield return new WaitForEndOfFrame();
		}

		Hide();
	}
	
	public override void Hide()
	{
		boneAnimation.Play(idleAnim);

		if (peekRoutine != null && peekRoutine.Running)
		{
			peekRoutine.StopRoutine();
		}
		
		this.Shown = false;
	}

	public override void Disable ()
	{
		Hide();
		this.renderer.enabled = false;
	}

	public override void Enable ()
	{
		this.renderer.enabled = true;
		boneAnimation.Play(idleAnim);
	}
	
	
	public void SetupLocal()
	{
		boneAnimation = GetComponent<BoneAnimation>();
		if (boneAnimation == null)
		{
			Debug.LogError(name + " did not find a bone animation for this barrel.");
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
