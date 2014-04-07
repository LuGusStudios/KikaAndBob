using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class FroggerLaneItemConditionalWin : FroggerLaneItem 
{
	public KikaAndBob.CommodityType pickupType = KikaAndBob.CommodityType.NONE;
	public KikaAndBob.ScreenAnchor messageAnchor = KikaAndBob.ScreenAnchor.NONE;
	public float messageHideTimer = 5f;

	public BoneAnimation boneAnimation = null;
	public string idleAnimationName = "";
	public string acceptAnimationName = "";
	public string deniedAnimationName = "";

	public enum AnimationState
	{
		NONE = -1,
		IDLE = 1,
		ACCEPTING = 2,
		DENYING = 4
	};

	public List<FroggerRequiredPickup> RequiredPickups
	{
		get
		{
			if (requiredPickups == null)
			{
				FindRequiredPickups();
			}

			return requiredPickups;
		}
	}

	protected AnimationState state = AnimationState.NONE;
	protected List<FroggerRequiredPickup> requiredPickups = null;

	public override void SetUpLocal()
	{
		base.SetUpLocal();

		state = AnimationState.IDLE;
	}
	
	public override void SetupGlobal()
	{
		base.SetupGlobal();

		// Find all of the required pickups in the scene
		if (requiredPickups == null)
		{
			FindRequiredPickups();
		}

		// Find the bone animation, and start the idle animation
		if (boneAnimation == null)
		{
			boneAnimation = transform.FindChild("Bone Animation").GetComponent<BoneAnimation>();
			if (boneAnimation == null)
			{
				Debug.LogError("Could not find the bone animation!");
			}
			else
			{
				if (boneAnimation.AnimationClipExists(idleAnimationName))
				{
					boneAnimation.Play(idleAnimationName, PlayMode.StopAll);
				}
			}
		}
	}

	private void Awake()
	{
		SetUpLocal();
	}

	private void Start () 
	{
		SetupGlobal();
	}

	protected void FindRequiredPickups()
	{
		//GameObject.FindObjectOfType(typeof(FroggerRequiredPickup))
		FroggerRequiredPickup[] pickups = GameObject.FindObjectsOfType<FroggerRequiredPickup>();

		Debug.Log("Length of found pickups: " + pickups.Length);

		requiredPickups = new List<FroggerRequiredPickup>();

		foreach (FroggerRequiredPickup pickup in pickups)
		{
			if (!requiredPickups.Contains(pickup))
			{
				requiredPickups.Add(pickup);
			}

			Debug.Log("Found pickup at position: " + pickup.transform.position.x + ", " + pickup.transform.position.y);
		}
	}

	protected override void EnterSurfaceEffect(FroggerCharacter character)
	{
		bool gameWon = true;
		foreach (FroggerRequiredPickup pickUp in requiredPickups)
		{
			if (!pickUp.PickedUp)
			{
				gameWon = false;
				break;
			}
		}

		if (gameWon)
		{
			character.characterAnimator.PlayAnimation(character.characterAnimator.idleUp);

			StartCoroutine(AcceptAnimationRoutine());

			FroggerGameManager.use.WinGame();
		}
		else
		{
			UnityEngine.Sprite icon = HUDManager.use.GetElementForCommodity(pickupType).icon.sprite;
			string message = LugusResources.use.Localized.GetText(Application.loadedLevelName + ".conditionalwin.pickups");

			if (messageAnchor != KikaAndBob.ScreenAnchor.NONE)
			{
				DialogueManager.use.CreateBox(KikaAndBob.ScreenAnchor.Top, message, icon).Show(messageHideTimer);
			}
			else
			{
				DialogueManager.use.CreateBox(transform, message, icon).Show(messageHideTimer);
			}
			

			StartCoroutine(DeniedAnimationRoutine());
		}
	}

	private IEnumerator AcceptAnimationRoutine()
	{
		if (!((state != AnimationState.IDLE) || (state != AnimationState.NONE)))
		{
			yield break;
		}

		if ((boneAnimation != null) && boneAnimation.AnimationClipExists(acceptAnimationName))
		{
			state = AnimationState.ACCEPTING;

			float length = boneAnimation.animation.GetClip(acceptAnimationName).length * 0.1f;
			boneAnimation.Play(acceptAnimationName, PlayMode.StopAll);

			yield return new WaitForSeconds(length);

			if (boneAnimation.AnimationClipExists(idleAnimationName))
			{
				boneAnimation.Play(idleAnimationName, PlayMode.StopAll);
			}
			else
			{
				boneAnimation.Stop();
			}

			state = AnimationState.IDLE;
		}
	}

	private IEnumerator DeniedAnimationRoutine()
	{
		if (!((state != AnimationState.IDLE) || (state != AnimationState.NONE)))
		{
			yield break;
		}

		if ((boneAnimation != null) && boneAnimation.AnimationClipExists(deniedAnimationName))
		{
			state = AnimationState.DENYING;

			float length = boneAnimation.animation.GetClip(deniedAnimationName).length * 0.1f;
			boneAnimation.Play(deniedAnimationName, PlayMode.StopAll);

			yield return new WaitForSeconds(length);

			if (boneAnimation.AnimationClipExists(idleAnimationName))
			{
				boneAnimation.Play(idleAnimationName, PlayMode.StopAll);
			}
			else
			{
				boneAnimation.Stop();
			}

			state = AnimationState.IDLE;
		}
	}
}
