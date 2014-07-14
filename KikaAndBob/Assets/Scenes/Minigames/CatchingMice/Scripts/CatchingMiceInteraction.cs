using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Kasper: This script used to handle both the player's input with regard to path drawing and with regard to interacting with traps etc.
// This made it a big and unwieldy file that was hard to extend or alter. Everything with regard to path drawing has been re-implemented in CatchingMiceInputManager.
// (Ideally, a name such as CatchingMicePathInput would have been better, but too late now.)
// The remainder of the object interactions is here.
public class CatchingMiceInteraction : LugusSingletonRuntime<CatchingMiceInteraction>
{
	protected CatchingMiceTile _previousTile = null;
	protected CatchingMiceTile lastAddedTile = null;
	protected float _timer = 0.0f;
	
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		CheckPlayerObjectInteraction();
	}
	
	public void CheckPlayerObjectInteraction()
	{
		// This will check whether the user wants to interact
		// with a trap or other world object, either to
		// reactivate it, or to do something else with it

		// This is done by casting a ray and checking for traps
		// When a trap is found, the player characters are searched
		// for and if they are in allowed ranged, the interaction can be executed

		if (!LugusInput.use.down)
		{
			return;
		}

		Transform hit = LugusInput.use.RayCastFromMouse(LugusCamera.game);

		if (hit == null)
		{
			return;
		}

		CheckPlayerTrapInteraction(hit);
		CheckPlayerObstacleInteraction(hit);
	}

	protected void CheckPlayerTrapInteraction(Transform hit)
	{
		CatchingMiceTrap trap = null;

		if (hit.parent != null)
			trap = hit.parent.GetComponent<CatchingMiceTrap>();

		if (trap == null)
		{
			return;
		}

		// Go over the characters and check if the object is in range

		// This seems very superfluous...
//		List<CatchingMiceCharacterPlayer> characters = new List<CatchingMiceCharacterPlayer>(CatchingMiceLevelManager.use.Players);


		foreach (CatchingMiceCharacterPlayer character in CatchingMiceLevelManager.use.Players)
		{
			CatchingMiceTile[] tilesAround = CatchingMiceLevelManager.use.GetTileAround(character.currentTile);
			foreach (CatchingMiceTile tile in tilesAround)
			{
				if (tile == null)
				{
					continue;
				}

				if (tile.trap != null)
				{
					// When the trap and player character are in range of each other
					// Interact with it, and return
					if (tile.trap == trap)
					{
						trap.PlayerInteraction();
					}
				}
			}
		}
	}

	protected void CheckPlayerObstacleInteraction(Transform hit)
	{
		CatchingMiceObstacle obstacle = hit.parent.GetComponent<CatchingMiceObstacle>();
		if (obstacle == null)
		{
			return;
		}

		// Go over the characters and check if the object is in range
		List<CatchingMiceCharacterPlayer> characters = new List<CatchingMiceCharacterPlayer>(CatchingMiceLevelManager.use.Players);
		foreach (CatchingMiceCharacterPlayer character in characters)
		{
			CatchingMiceTile[] tilesAround = CatchingMiceLevelManager.use.GetTileAround(character.currentTile);
			foreach (CatchingMiceTile tile in tilesAround)
			{
				if (tile == null)
				{
					continue;
				}

				if ((tile.obstacle != null) && (tile.obstacle == obstacle))
				{
					CatchingMiceCage cage = obstacle as CatchingMiceCage;
					if ((cage != null) && (cage.capturedPlayers != null))
					{
						cage.PlayerInteraction();
					}
				}
			}
		}
	}
}

