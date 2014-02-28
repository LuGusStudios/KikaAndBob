using UnityEngine;
using System.Collections;

public class PacmanTileItemHide : PacmanTileItem 
{
    public override void Initialize()
    {
        parentTile.tileType = PacmanTile.TileType.Hide;
    }
    public override void OnEnter(PacmanCharacter character)
    {
        
    }
	
}
