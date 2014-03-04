using UnityEngine;
using System.Collections;

public class PacmanTileItemTeleport : PacmanTileItem
{
    protected PacmanTileItem linkedTile;
    public override void Initialize()
    {
        //parentTile.tileType = PacmanTile.TileType.Open;
        //goes through every tile item in the level
        
    }

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void OnTryEnter(PacmanCharacter character)
    {
        base.OnTryEnter(character);
        character.alreadyTeleported = false;
    }

    public override void OnEnter(PacmanCharacter character)
    {
        if (!character.alreadyTeleported)
        {
            if (linkedTile == null)
            {
                foreach (PacmanTileItem tileItem in PacmanLevelManager.use.tileItemScripts)
                {
                    Debug.Log("unique id " +  tileItem.uniqueId);
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
  
            character.transform.localPosition = linkedTile.parentTile.location.v3();
            character.currentTile = linkedTile.parentTile;
            character.alreadyTeleported = true;
        }
       
    }
}
