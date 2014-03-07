using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DartsToggle : IDartsHitable 
{

	public override void OnHit()
	{
		HitCount++;
		Hide();
	}
	
	public override void Show()
	{
		this.Shown = true;
		SetTogglePartsActive(true);
	}
	
	public override void Hide()
	{
		this.Shown = false;
		SetTogglePartsActive(false);
	}
	
	protected void SetTogglePartsActive(bool active)
	{
		foreach(Transform t in transform)
		{
			t.gameObject.SetActive(active);
		}
	}
	
	public virtual void SetupLocal()
	{
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
	
	private void Update () 
	{
	}
}
