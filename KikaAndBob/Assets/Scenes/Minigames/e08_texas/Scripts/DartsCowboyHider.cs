using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class DartsCowboyHider : DartsHider 
{
	public string hitSoundKey;
	public string idleAnimation;
	public string hitAnimation;

	protected BoneAnimation characterAnim = null;
	protected BoxCollider2D boxCollider2D = null;

	public override void OnHit()
	{
		HitCount++;

		if (!string.IsNullOrEmpty(hitSoundKey))
		{
			LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio(hitSoundKey));
		}

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
		characterAnim.Play(idleAnimation);
		gameObject.MoveTo( shownPosition ).Time ( 0.1f /*TODO*/).Execute();
	}

	public override void SetupLocal()
	{
		base.SetupLocal();

		if (characterAnim == null)
			characterAnim = GetComponentInChildren<BoneAnimation>();
		if (characterAnim == null)
			Debug.LogError("DartsWindow: Missing character animation transform.");
		
		if (boxCollider2D == null)
			boxCollider2D = GetComponent<BoxCollider2D>();
		if (boxCollider2D == null)
			Debug.LogError(name + " did not find a BoxCollider2D.");
	}
}
