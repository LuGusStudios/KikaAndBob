using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FroggerFreezingPlayer : FroggerPlayer 
{
	public float preFreezeTime = 5f;
	public string freezingAnimationFront = "";
	public string freezingAnimationBack = "";

	protected float freezeTimer = 0f;

	protected override void UpdatePosition()
	{
		// If there is input, then pass it down, and reset the timer.
		// If there wasn't any input for some time, then let the character start to freeze.
		// If the freezing animation is complete (played once), then the game is lost

		FroggerCameraController.use.UpdateCameraFollow(this);

		if (!movingToLane && FroggerGameManager.use.gameRunning)
		{
			if (LugusInput.use.Key(KeyCode.UpArrow)
				|| LugusInput.use.Key(KeyCode.DownArrow)
				|| LugusInput.use.Key(KeyCode.LeftArrow)
				|| LugusInput.use.Key(KeyCode.RightArrow))
			{
				base.UpdatePosition();
				freezeTimer = preFreezeTime;
			}
			else if (freezeTimer > 0)
			{
				base.UpdatePosition();
			}
			else
			{

				if (!headingUp)
				{
					characterAnimator.PlayAnimation(freezingAnimationFront);
				}
				else
				{
					characterAnimator.PlayAnimation(freezingAnimationBack);
				}

				if ((characterAnimator.currentAnimationContainer != null)
					&& (!characterAnimator.currentAnimationContainer.animation.isPlaying))
				{
					FroggerGameManager.use.LoseGame();
					freezeTimer = preFreezeTime;

				}
			}
		}
	}

	public override void SetUpLocal()
	{
		base.SetUpLocal();
		freezeTimer = preFreezeTime;
	}
	
	protected void Awake()
	{
		SetUpLocal();
	}

	protected void Update()
	{
		if (FroggerGameManager.use.gameRunning)
		{
			freezeTimer -= Time.deltaTime;
		}

		base.Update();
	}
}
