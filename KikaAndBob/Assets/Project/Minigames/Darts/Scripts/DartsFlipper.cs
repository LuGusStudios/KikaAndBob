using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DartsFlipper : IDartsHitable 
{
	public Vector3 hiddenPosition = Vector3.zero;
	public Vector3 shownPosition = Vector3.zero;
	
	public override void OnHit()
	{
		HitCount++;
		Hide();
		
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
		gameObject.ScaleTo( originalScale ).Time ( 0.1f /*TODO*/).Execute();
	}
	
	public override void Hide()
	{
		this.Shown = false;
		gameObject.ScaleTo( originalScale.x( originalScale.x / 10.0f)  ).Time ( 0.1f /*TODO*/).Execute();
	}

	public Vector3 originalScale = Vector3.one;
	
	public void SetupLocal()
	{
		originalScale = this.transform.localScale;
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
