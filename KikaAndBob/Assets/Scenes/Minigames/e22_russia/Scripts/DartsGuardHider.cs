using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class DartsGuardHider : DartsHider 
{
	//public string hitSoundKey;
	public string idleAnimation;
	public string hitAnimation;

	protected Vector3 showScale = Vector3.zero;
	protected Vector3 hideScale = Vector3.zero;

	protected BoneAnimation characterAnim = null;
	protected BoxCollider2D boxCollider2D = null;
	protected ILugusCoroutineHandle moveRoutine = null;

	public override void OnHit()
	{
		HitCount++;

		if (!string.IsNullOrEmpty(hitSoundKey))
		{
			LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio(hitSoundKey));
		}

		if (moveRoutine != null && moveRoutine.Running)
		{
			moveRoutine.StopRoutine();
		}

		moveRoutine = LugusCoroutines.use.StartRoutine(HideRoutine());
	}

	protected IEnumerator HideRoutine()
	{
		boxCollider2D.enabled = false;

		characterAnim.Play(hitAnimation);
		boxCollider2D.enabled = false;	// disable collider so it can't be hit, but the object is still visibile
		
		yield return new WaitForSeconds(0.25f);	// wait a little for animation to sho

		iTween.Stop(gameObject);

		yield return null;

		Hide();
	}

	public override void Show()
	{
		if (moveRoutine != null && moveRoutine.Running)
		{
			moveRoutine.StopRoutine();
		}

		moveRoutine = LugusCoroutines.use.StartRoutine(ShowRoutine());
	}

	protected IEnumerator ShowRoutine()
	{
		this.Shown = true;

		boxCollider2D.enabled = false;
		
		characterAnim.Play(idleAnimation);
		
		transform.position = hiddenPosition;
		transform.localScale = hideScale;
		
		iTween.Stop(gameObject);

		yield return null;

		Vector3[] path = new Vector3[3];
		path[0] = hiddenPosition;
		path[1] = Vector3.Lerp(hiddenPosition, shownPosition, 0.5f) + new Vector3(0, 1, 0);
		path[2] = shownPosition;
		
		gameObject.MoveTo( path ).Time (0.3f /*TODO*/).Delay(0.15f).Execute();
		gameObject.ScaleTo(showScale).Time ( 0.3f).Execute();

		yield return new WaitForSeconds(0.3f);

		boxCollider2D.enabled = true;
	}

	public override void Hide ()
	{
//		if (moveRoutine != null && moveRoutine.Running)
//			return;

		this.Shown = false;

		iTween.Stop(gameObject);

		Vector3[] path = new Vector3[3];
		path[0] = shownPosition;
		path[1] = Vector3.Lerp(shownPosition, hiddenPosition, 0.5f) + new Vector3(0, 1, 0);
		path[2] = hiddenPosition;

		boxCollider2D.enabled = false;

		gameObject.MoveTo( path ).Time ( 0.3f /*TODO*/).Execute();

		if (transform.localScale.x > hideScale.x)
		{
			gameObject.ScaleTo(hideScale).Time( 0.3f).Delay(0.15f).Execute(); 
		}
	}

	public override void Disable ()
	{
		this.Shown = false; 

		iTween.Stop(gameObject);

		transform.position = hiddenPosition; 
		transform.localScale = hideScale;
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

		showScale = transform.localScale;
	}
}
