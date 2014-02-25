using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class DartsCowboyWindow : IDartsHitable 
{
	public string idleAnimation;
	public string hitAnimation;

	protected Transform openItems = null;
	protected Transform closedItems = null;
	protected Transform character = null;
	protected ParticleSystem[] dustClouds = null;
	protected BoneAnimation characterAnim = null;
	protected BoxCollider2D boxCollider2D = null;
	
	public override void OnHit()
	{
		HitCount++;
		LugusCoroutines.use.StartRoutine(HideRoutine());
	}

	protected IEnumerator HideRoutine()
	{
		characterAnim.Play(hitAnimation);
		boxCollider2D.enabled = false;	// disable collider so it can't be hit, but the object is still visibile

		yield return new WaitForSeconds(0.25f);

		Hide();
	}
	
	public override void Show()
	{
		this.Shown = true;
		boxCollider2D.enabled = true;
		SetTogglePartsActive(true);
		characterAnim.Play(idleAnimation);
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

		if (boxCollider2D == null)
			boxCollider2D = GetComponent<BoxCollider2D>();
		if (boxCollider2D == null)
			Debug.LogError(name + " did not find a BoxCollider2D.");
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
