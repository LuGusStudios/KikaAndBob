using UnityEngine;
using System.Collections;

public class PacmanTileItemDoorGuard : PacmanTileItem
{
    protected bool opened = false;
    public override void Initialize()
    {
        parentTile.tileType = PacmanTile.TileType.Collide;
    }

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void OnTryEnter(PacmanCharacter character) 
    {
        if (!opened && character.id == linkedId)// && PacmanPickups.use.GetPickupAmount(keyID) >= 1)
        {
            opened = true;
            //PacmanPickups.use.ModifyPickupAmount(keyID, -1);
            parentTile.tileType = PacmanTile.TileType.Open;
            this.gameObject.SetActive(false);
        }
    }
}
