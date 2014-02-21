using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class DartsWindow : IDartsHitable 
{
	protected Transform openItems = null;
	protected Transform closedItems = null;
	protected Transform character = null;
	protected ParticleSystem[] dustClouds = null;
	protected BoneAnimation characterAnim = null;
	
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
		if (openItems == null)
			openItems = transform.FindChild("Open");
		if (openItems == null)
			Debug.LogError("DartsWindow: Missing open version transform.");

		if (closedItems == null)
			closedItems = transform.FindChild("Closed");
		if (closedItems == null)
			Debug.LogError("DartsWindow: Missing closed version transform.");

		if (character == null)
			character = transform.FindChild("Character");
		if (character == null)
			Debug.LogError("DartsWindow: Missing character transform.");

		if (characterAnim == null)
			characterAnim = character.GetComponent<BoneAnimation>();
		if (characterAnim == null)
			Debug.LogError("DartsWindow: Missing character animation transform.");

		if (dustClouds == null)
			dustClouds = GetComponentsInChildren<ParticleSystem>();
		if (dustClouds == null)
			Debug.LogError("DartsWindow: Missing dust cloud particles");
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
