using UnityEngine;
using System.Collections;

public class CatchingMiceAnimationMouseSkinny : CatchingMiceCharacterAnimationMouse 
{
    public string attackAnimationClip = "_Attack";
    
	public override void OnHit()
    {
        if (currentAnimationClip != characterNameAnimation + _sideAnimationClip + eatingAnimationClip)
        {
            //CatchingMiceLogVisualizer.use.LogError("Loading Idle Animation Clip");
            PlayAnimation("RIGHT/" + characterNameAnimation + _sideAnimationClip + eatingAnimationClip);
            _currentMovementQuadrant = KikaAndBob.CMMovementQuadrant.NONE;
        }
    }
    
	protected void OnAttack()
    {
        if (currentAnimationClip != characterNameAnimation + _sideAnimationClip + attackAnimationClip)
        {
            //CatchingMiceLogVisualizer.use.LogError("Loading attack Animation Clip");
            PlayAnimation("RIGHT/" + characterNameAnimation + _sideAnimationClip + attackAnimationClip);
            _currentMovementQuadrant = KikaAndBob.CMMovementQuadrant.NONE;
        }
    }
    
	protected override void SetCharacter()
    {
        if (character == null)
        {
            character = transform.GetComponent<CatchingMiceMouseSkinny>();
        }

        if (character == null)
        {
            CatchingMiceLogVisualizer.use.LogError(name + " : no character found!");
        }
        else
        {
            character.onJump += OnJump;
            character.onHit += OnHit;
            ((CatchingMiceMouseSkinny)character).onGetHit += OnGetHit;
            ((CatchingMiceMouseSkinny)character).onAttack += OnAttack;
        }
    }
    
}
