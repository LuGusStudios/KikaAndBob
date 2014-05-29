using UnityEngine;
using System.Collections;

public class FroggerPlayer : FroggerCharacter {
	
	protected bool headingUp = true;
	protected DirectionPad directionPad = null;

	protected void Start()
	{
		SetUpGlobal ();
	}

	public virtual void SetUpGlobal ()
	{
		if (directionPad == null)
		{
			directionPad = (DirectionPad) FindObjectOfType(typeof(DirectionPad));
		}

		if (directionPad == null)
		{
			Debug.LogWarning("FroggerPlayer: No direction pad found. Continuing without.");
		}

	}

	protected override void UpdatePosition ()
	{
		// it might make more sense to update the camera after moving, but that can have weird effects in combination with ClampToScreen when restarting a level
		FroggerCameraController.use.UpdateCameraFollow(this);

		if (!movingToLane && FroggerGameManager.use.gameRunning)
		{
			if (LugusInput.use.Key(KeyCode.UpArrow) || (directionPad != null && directionPad.IsInDirection(Joystick.JoystickDirection.Up) ))
			{
				MoveToLane(FroggerLaneManager.use.GetLaneAbove(currentLane));
				headingUp = true;
			}
			else if (LugusInput.use.Key(KeyCode.DownArrow) || (directionPad != null && directionPad.IsInDirection(Joystick.JoystickDirection.Down) ))
			{
				MoveToLane(FroggerLaneManager.use.GetLaneBelow(currentLane));
				headingUp = false;
			}
			else if (LugusInput.use.Key(KeyCode.LeftArrow) || (directionPad != null && directionPad.IsInDirection(Joystick.JoystickDirection.Left) ))
			{
				MoveSideways(false);
			}
			else if (LugusInput.use.Key(KeyCode.RightArrow) || (directionPad != null && directionPad.IsInDirection(Joystick.JoystickDirection.Right) ))
			{
				MoveSideways(true);
			}
			else
			{
				if (!headingUp)
					characterAnimator.PlayAnimation(characterAnimator.idleDown);
				else
					characterAnimator.PlayAnimation(characterAnimator.idleUp);
			}

			ClampToScreen();
		}
	} 
}
