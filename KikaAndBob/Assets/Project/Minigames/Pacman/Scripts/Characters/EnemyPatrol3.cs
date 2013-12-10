using UnityEngine;
using System.Collections;

public class EnemyPatrol3 : EnemyPatrol {

	protected override void SetDefaultTargetTiles ()
	{
		patrolPath.Add(PacmanLevelManager.use.GetTile(11,11));
		patrolPath.Add(PacmanLevelManager.use.GetTile(11,1));
		patrolPath.Add(PacmanLevelManager.use.GetTile(1,1));
		patrolPath.Add(PacmanLevelManager.use.GetTile(1,11));
	
		defaultTargetTile = patrolPath[0];
	}
}
