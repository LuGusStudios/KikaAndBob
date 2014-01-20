using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DartsWindow : IDartsHitable 
{
	protected Transform openItems = null;
	protected Transform closedItems = null;
	protected Transform character = null;
	protected ParticleSystem[] dustClouds = null;
	
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
		if (openItems != null)
			openItems.gameObject.SetActive(active);

		if (closedItems != null)
			closedItems.gameObject.SetActive(!active);
		
		if (character != null)
			character.gameObject.SetActive(active);

		if (dustClouds != null)
		{
			foreach(ParticleSystem ps in dustClouds)
			{
				if (active)
				{
					ps.Play();
				}
				else
				{
					if (ps.isPlaying)
						ps.Stop();
				}
			}
		}
	}

	public void SetupLocal()
	{
		openItems = transform.FindChild("Open");
		closedItems = transform.FindChild("Closed");
		character = transform.FindChild("Character");

		dustClouds = GetComponentsInChildren<ParticleSystem>();
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
