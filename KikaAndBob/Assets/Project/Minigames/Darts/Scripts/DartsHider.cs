using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DartsHider : IDartsHitable 
{
	public Vector3 hiddenPosition = Vector3.zero;
	public Vector3 shownPosition = Vector3.zero;
	public string hitSoundKey = "";	// not setting a default value here so other instances of this script won't suddenly behave differently

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
		this.Shown = true;
		gameObject.MoveTo( shownPosition ).Time ( 0.1f /*TODO*/).Execute();
	}

	public override void Hide()
	{
		this.Shown = false;
		gameObject.MoveTo( hiddenPosition ).Time ( 0.1f /*TODO*/).Execute();
	}

	public virtual void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public virtual void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
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

	public override void Disable ()
	{
		gameObject.transform.position = hiddenPosition;
	}
}
