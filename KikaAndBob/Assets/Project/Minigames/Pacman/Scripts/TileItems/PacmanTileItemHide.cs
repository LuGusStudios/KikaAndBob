using UnityEngine;
using System.Collections;

public class PacmanTileItemHide : PacmanTileItem
{
    
    protected Transform particleTransform;
    protected Transform hideTransform;
    public string enterSoundKey = "ShrubberyDive01";
    public override void Initialize()
    {
        parentTile.tileType = PacmanTile.TileType.Hide;
        Transform[] childrenTransform = GetComponentsInChildren<Transform>();
        foreach (Transform child in childrenTransform)
        {
            if (child.GetComponent<ParticleSystem>())
            {
                particleTransform = child;
            }
            else if ( child != transform)
            {
                hideTransform = child;
            }
        }
        hideTransform.gameObject.SetActive(false);
        //default tile placer will put it at depth 1 but for hide tiles we want it in front of the player
        transform.localPosition = parentTile.location.v3().z(-1);
    }
    public override void OnEnter(PacmanCharacter character)
    {
        particleTransform.particleSystem.Play();
        hideTransform.gameObject.SetActive(true);
       // gameObject.ScaleTo(Vector3.one * 0.5f).Time(0.5f).EaseType(iTween.EaseType.easeInOutBounce).Execute();
        if (!string.IsNullOrEmpty(enterSoundKey))
        {
            LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio(enterSoundKey));
        }
        iTween.PunchScale(gameObject, Vector3.one * 0.4f, 0.3f);
    }
    public override void OnLeave(PacmanCharacter character)
    {
        hideTransform.gameObject.SetActive(false);
    }
}
