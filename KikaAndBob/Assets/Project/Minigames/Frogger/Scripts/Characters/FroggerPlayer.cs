using UnityEngine;
using System.Collections;

public class FroggerPlayer : FroggerCharacter {
	
	protected bool headingUp = true;
	protected Joystick joystick = null;

	protected void Start()
	{
		SetUpGlobal ();
	}

	public virtual void SetUpGlobal ()
	{
		if (joystick == null)
		{
			joystick = (Joystick) FindObjectOfType(typeof(Joystick));
		}

		if (joystick == null)
		{
			Debug.LogWarning("FroggerPlayer: No joystick found. Continuing without.");
		}

	}

	protected override void UpdatePosition ()
	{
		// it might make more sense to update the camera after moving, but that can have weird effects in combination with ClampToScreen when restarting a level
		FroggerCameraController.use.UpdateCameraFollow(this);

		if (!movingToLane && FroggerGameManager.use.gameRunning)
		{
			if (LugusInput.use.Key(KeyCode.UpArrow) || (joystick != null && joystick.IsInDirection(Joystick.JoystickDirection.Up) ))
			{
				MoveToLane(FroggerLaneManager.use.GetLaneAbove(currentLane));
				headingUp = true;
			}
			else if (LugusInput.use.Key(KeyCode.DownArrow) || (joystick != null && joystick.IsInDirection(Joystick.JoystickDirection.Down) ))
			{
				MoveToLane(FroggerLaneManager.use.GetLaneBelow(currentLane));
				headingUp = false;
			}
			else if (LugusInput.use.Key(KeyCode.LeftArrow) || (joystick != null && joystick.IsInDirection(Joystick.JoystickDirection.Left) ))
			{
				MoveSideways(false);
			}
			else if (LugusInput.use.Key(KeyCode.RightArrow) || (joystick != null && joystick.IsInDirection(Joystick.JoystickDirection.Right) ))
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
