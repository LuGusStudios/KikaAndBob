using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DartsFlipper : IDartsHitable 
{
	public Vector3 hiddenPosition = Vector3.zero;
	public Vector3 shownPosition = Vector3.zero;

	public string hitSoundKey = "Poof01";

	protected SpriteRenderer offVersion = null;
	protected SpriteRenderer onVersion = null;
	
	public override void OnHit()
	{
		HitCount++;
		Hide();

		if (!string.IsNullOrEmpty(hitSoundKey))
		{
			LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio(hitSoundKey));
		}
		
		//LugusCoroutines.use.StartRoutine( HitRoutine() );
	}
	
	/*
	protected IEnumerator HitRoutine()
	{
		Vector3 originalPos = this.transform.position;
		
		this.transform.position = LugusUtil.OFFSCREEN;
		
		yield return new WaitForSeconds(1.0f);
		
		this.transform.position = originalPos;
	}
	*/
	
	public override void Show()
	{
		onVersion.enabled = true;
		offVersion.enabled = false;

		this.Shown = true;
		//gameObject.ScaleTo( originalScale ).Time ( 0.1f /*TODO*/).Execute();
	}
	
	public override void Hide()
	{
		onVersion.enabled = false;
		offVersion.enabled = true;

		this.Shown = false;
		//gameObject.ScaleTo( originalScale.x( originalScale.x / 10.0f)  ).Time ( 0.1f /*TODO*/).Execute();
	}

	public Vector3 originalScale = Vector3.one;
	
	public void SetupLocal()
	{
		originalScale = this.transform.localScale;

		if (offVersion == null)
		{
			Transform t = transform.FindChild("OffVersion");

			if (t != null)
				offVersion = t.GetComponent<SpriteRenderer>();
		}

		if (offVersion == null)
			Debug.Log("DartsFlipper: Missing Off version.");

		if (onVersion == null)
		{
			onVersion = GetComponent<SpriteRenderer>();
		}

		if (onVersion == null)
		{
			Debug.Log("DartsFlipper: Missing On version.");
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
