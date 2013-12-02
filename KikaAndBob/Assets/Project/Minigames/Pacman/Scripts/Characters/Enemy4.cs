using UnityEngine;
using System.Collections;

public class Enemy4 : EnemyCharacter {

	protected override void SetDefaultTargetTiles()
	{
		defaultTargetTile = PacmanLevelManager.use.GetTile(0, PacmanLevelManager.use.height-1);
	}
	
	// red cat uses default targeting mechanism, always finding player directly
	
}
