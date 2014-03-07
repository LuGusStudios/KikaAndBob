using UnityEngine;
using System.Collections;

public class PacmanTileItemTeleport : PacmanTileItem 
{
    public PacmanTileItem linkedTile; 
    public override void Initialize()
    {
        //parentTile.tileType = PacmanTile.TileType.Open;
        //goes through every tile item in the level
        if (linkedTile == null)
        {
            foreach (PacmanTileItem tileItem in PacmanLevelManager.use.tileItemScripts)
            {
                if (tileItem.uniqueId == linkedId)
                {
                    linkedTile = tileItem;
                    break;
                }

            }
            if (linkedTile == null)
            {
                Debug.LogError("tile item with unique id " + linkedId + " not found");
                return;
            }
        }
    }

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void OnEnter(PacmanCharacter character)
    {
        if (!character.teleportUsed)
        {
            character.transform.localPosition = linkedTile.parentTile.location.v3();
            character.currentTile = linkedTile.parentTile;
            character.teleportUsed = true;
        }  
    }

    public override void OnLeave(PacmanCharacter character)
    {
        character.teleportUsed = false;
    }
}
