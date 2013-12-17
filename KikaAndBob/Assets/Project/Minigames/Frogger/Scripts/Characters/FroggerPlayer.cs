using UnityEngine;
using System.Collections;

public class FroggerPlayer : FroggerCharacter {

	protected override void UpdatePosition ()
	{
		// it might make more sense to update the camera after moving, but that can have weird effects in combination with ClampToScreen when restarting a level
		FroggerCameraController.use.UpdateCameraFollow(this);

		if (!movingToLane && FroggerGameManager.use.gameRunning)
		{
			if (LugusInput.use.Key(KeyCode.UpArrow))
			{
				MoveToLane(FroggerLaneManager.use.GetLaneAbove(currentLane));
			}
			else if (LugusInput.use.Key(KeyCode.DownArrow))
			{
				MoveToLane(FroggerLaneManager.use.GetLaneBelow(currentLane));
			}
			else if (LugusInput.use.Key(KeyCode.LeftArrow))
			{
				MoveSideways(false);
			}
			else if (LugusInput.use.Key(KeyCode.RightArrow))
			{
				MoveSideways(true);
			}
			else
			{
				PlayAnimation("Idle");
			}
			ClampToScreen();
		}
	} 
}
