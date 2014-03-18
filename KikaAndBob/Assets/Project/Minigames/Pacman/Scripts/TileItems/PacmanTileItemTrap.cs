using UnityEngine;
using System.Collections;

public class PacmanTileItemTrap : PacmanTileItem
{
    public bool TrapUsed = false;
    protected bool UpgradeUsed = false;
    public override void Initialize()
    {
        parentTile.tileType = PacmanTile.TileType.Lethal;
    }

    public override void OnEnter(PacmanCharacter character)
    {
        if (!TrapUsed && character is PacmanEnemyCharacter)
        {
            TrapUsed = true;
            parentTile.tileType = PacmanTile.TileType.Upgrade;
        }
        else
        {
            if (!UpgradeUsed && (parentTile.tileType == PacmanTile.TileType.Upgrade))
            {
                UpgradeUsed = true;
                this.gameObject.SetActive(false);
            }
        }
        
    }
}
