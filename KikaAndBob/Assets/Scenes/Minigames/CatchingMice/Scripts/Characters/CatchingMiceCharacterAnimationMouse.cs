using UnityEngine;
using System.Collections;

public class CatchingMiceCharacterAnimationMouse : CatchingMiceCharacterAnimation
{
    public override void OnHit()
    {
        if (currentAnimationClip != characterNameAnimation + _backAnimationClip + eatingAnimationClip)
        {
            //CatchingMiceLogVisualizer.use.LogError("Loading Eating Animation Clip");
            PlayAnimation("UP/" + characterNameAnimation + _backAnimationClip + eatingAnimationClip);
            _currentMovementQuadrant = KikaAndBob.CMMovementQuadrant.NONE;
        }
    }
    public virtual void OnGetHit()
    {
        LugusCoroutines.use.StartRoutine(SmoothMovesUtil.Blink(animationContainers, Color.red, 1f, 3));
    }
    public override void PlayAnimation(string animationPath, bool moveRight = true)
    {
        string correctedAnimationPath = animationPath.Replace("LEFT", "RIGHT");
        base.PlayAnimation(correctedAnimationPath, !moveRight);
    }
    protected override void SetCharacter()
    {
        if (character == null)
        {
            character = transform.GetComponent<CatchingMiceCharacterMouse>();
        }

        if (character == null)
        {
            CatchingMiceLogVisualizer.use.LogError(name + " : no character found!");
        }
        else
        {
            character.onJump += OnJump;
            character.onHit += OnHit;
            ((CatchingMiceCharacterMouse)character).onGetHit += OnGetHit;
        }
    }
}
