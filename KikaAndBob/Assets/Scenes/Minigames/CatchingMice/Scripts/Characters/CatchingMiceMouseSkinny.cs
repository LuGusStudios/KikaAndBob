using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceMouseSkinny : CatchingMiceCharacterMouse 
{
    public delegate void OnAttack();
    public event OnAttack onAttack;
    
    public override void GetTarget()
    {
        if (CatchingMiceLevelManager.use.TrapTiles.Count <= 0)
        {
            //no traps left, check for cheese
            CatchingMiceLogVisualizer.use.Log("No traps has been found, checking for cheese.");
            base.GetTarget();
            return;
        }

        //search for traps first before checking for cheese
        List<CatchingMiceTile> tiles = new List<CatchingMiceTile>(CatchingMiceLevelManager.use.TrapTiles);
        targetWaypoint = GetTargetWaypoint(tiles);
            
        if (targetWaypoint != null)
        {
                
            //CatchingMiceLogVisualizer.use.LogError("Getting new trap " + CatchingMiceLevelManager.use.trapTiles.Count);
            CalculateTarget(targetWaypoint); 
        }
        else
        {
            CatchingMiceLogVisualizer.use.LogError("No target found");
            //try go for cheese instead
            base.GetTarget();
        }
        
        
    }
    
	public override void DoCurrentTileBehaviour(int pathIndex)
    {
		// For the skinny mouse, when a trap is present, start attacking the trap,
		// else, do the default behavior for a mouse: attacking cheese, if present
        if (((currentTile.tileType & CatchingMiceTile.TileType.Trap) == CatchingMiceTile.TileType.Trap) && (pathIndex == 0))
        {
            // Begin attacking the trap
            StartCoroutine(AttackTrap());
        }
		else
		{
			base.DoCurrentTileBehaviour(pathIndex);
		}
    }
    
	public IEnumerator AttackTrap()
    {
        attacking = true;
        if (onAttack != null)
		{
            onAttack();
		}

        CatchingMiceTile trapTile = currentTile;
        while ((health > 0)
			&& (trapTile.trap != null)
			&& (trapTile.trap.Health > 0))
        {
            //CatchingMiceLogVisualizer.use.Log(currentTile.trapObject.Stacks);
            trapTile.trap.Health -= damage;

            yield return new WaitForSeconds(attackInterval);
        }

        attacking = false;

		// Get the next target when the trap has been destroyed
        GetTarget();
    }
    
	protected override void OnEnable()
    {
        base.OnEnable();
        CatchingMiceLevelManager.use.TrapRemoved += TargetRemoved;

    }
    
	protected override void OnDisable()
    {
        //base.OnDisable();
        //CatchingMiceLevelManager.use.TrapRemoved -= TargetRemoved;
    }
    
	public override void DieRoutine()
    {
        base.DieRoutine();
        CatchingMiceLevelManager.use.TrapRemoved -= TargetRemoved;
    }
}
