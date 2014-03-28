using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanBirdCage : PacmanTileItem 
{
	protected bool found = false;
	
	public override void Initialize ()
	{
		parentTile.tileType = PacmanTile.TileType.Collide;
	}
	
	public override void OnTryEnter (PacmanCharacter character)
	{
		if (found)
			return;
		
		found = true;
		
		PacmanGameManager.use.WinGame();
	}
}
