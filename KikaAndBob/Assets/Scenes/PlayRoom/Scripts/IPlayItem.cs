using UnityEngine;
using System.Collections;

public class IPlayItem : MonoBehaviour {
	
	public string catAnimationPath = "DOWN/@Front_Attack";
	public bool avoidWhenWalking = false;
	public float scale = 1f;
	public string useSoundKey = "";
	public bool loopUseSound = false;
	
	protected Transform actionPoint = null;
	protected ILugusAudioTrack useSoundTrack = null;
	protected LugusAudioTrackSettings useSoundTrackSettings = null;

	public void SetupLocal()
	{
		if (actionPoint == null)
			actionPoint = transform.FindChild("ActionPoint");

		if (useSoundTrackSettings == null)
		{
			useSoundTrackSettings = new LugusAudioTrackSettings().Loop(loopUseSound);
		}
	}

	protected void Awake()
	{
		SetupLocal();
	}

	public virtual void Activate(PlayCatController cat)
	{
		Debug.Log("PlayItemTest: Activated " + this.name);

		cat.catAnimation.PlayAnimation(catAnimationPath);

		if (!string.IsNullOrEmpty(useSoundKey))
		{
			useSoundTrack = LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio(useSoundKey), false, useSoundTrackSettings);
			useSoundTrack.Claim();
		}
	}

	public virtual void Deactivate(PlayCatController cat)
	{
		if (useSoundTrack != null)
		{
			useSoundTrack.Stop();
			useSoundTrack.Release();
		}
	}

	public Transform GetActionPoint()
	{
		if (actionPoint != null)
		{
			return actionPoint;
		}
		else
		{
			return this.transform;
		}
	}	
}
