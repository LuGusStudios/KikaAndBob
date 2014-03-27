using UnityEngine;
using System.Collections;

public class PacmanTileItemTeleport : PacmanTileItem 
{
	public string enterSoundKey = "Ladder01";
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

    public override void OnEnter(PacmanCharacter character)
    {
        if (!character.teleportUsed)
        {
			character.teleportUsed = true;
			LugusCoroutines.use.StartRoutine(TunnelRoutine(character));
        }  
    }

	protected IEnumerator TunnelRoutine(PacmanCharacter character)
	{
		PacmanGameManager.use.gameRunning = false;

		LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio(enterSoundKey));

		PacmanScreenFader.use.FadeInAndOut(1.0f);

		yield return new WaitForSeconds(0.5f);


		character.transform.localPosition = linkedTile.parentTile.location.v3();
		character.currentTile = linkedTile.parentTile;

		PacmanGameManager.use.gameRunning = true;

		yield break;
	}

    public override void OnLeave(PacmanCharacter character)
    {
        character.teleportUsed = false;
    }
}
