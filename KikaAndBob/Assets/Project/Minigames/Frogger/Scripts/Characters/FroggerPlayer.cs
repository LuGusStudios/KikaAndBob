using UnityEngine;
using System.Collections;

public class FroggerPlayer : FroggerCharacter {

	protected override void UpdatePosition ()
	{
		if (!movingToLane && FroggerGameManager.use.gameRunning)
		{
			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				MoveToLane(FroggerLaneManager.use.GetLaneAbove(currentLane));
			}
			else if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				MoveToLane(FroggerLaneManager.use.GetLaneBelow(currentLane));
			}
			else if (Input.GetKey(KeyCode.LeftArrow))
			{
				MoveSideways(false);
			}
			else if (Input.GetKey(KeyCode.RightArrow))
			{
				MoveSideways(true);
			}
			ClampToScreen();
		}

		//FroggerCameraController.use.UpdateCameraFollow(this);
	} 
}
