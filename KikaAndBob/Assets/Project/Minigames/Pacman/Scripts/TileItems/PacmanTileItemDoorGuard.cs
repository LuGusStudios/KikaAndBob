using UnityEngine;
using System.Collections;

public class PacmanTileItemDoorGuard : PacmanTileItem
{
    protected bool opened = false;
    public override void Initialize()
    {
        parentTile.tileType = PacmanTile.TileType.Collide;
    }

    public override void OnTryEnter(PacmanCharacter character)
    {
        if (!opened && !string.IsNullOrEmpty(linkedId) && character.id == linkedId)// && PacmanPickups.use.GetPickupAmount(keyID) >= 1)
        {
            opened = true;
            //PacmanPickups.use.ModifyPickupAmount(keyID, -1);
            parentTile.tileType = PacmanTile.TileType.Open;
            transform.GetComponent<SpriteRenderer>().enabled = false;
            //this.gameObject.SetActive(false);
			LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio("GateOpen01"));
        }
    }
}
