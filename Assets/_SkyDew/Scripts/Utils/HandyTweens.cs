using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HandyTweens
{
    [Serializable]
    public class JumpToTweenInfo
    {
        public float PrepareJumpTime = .1f;
        public float PrepareJumpXScale = 1.1f;
        public float PrepareJumpYScale = .9f;
        public float PrepareJumpYMove = .3f;
        public float InJumpXScale = 1.2f;
        public float InJumpYScale = .8f;
        public Ease InJumpXEase = Ease.InSine;
        public Ease InJumpYEase = Ease.OutSine;
        public Ease InJumpZEase = Ease.Linear;
        
    }
    
    public static Tween DoJump(Transform tr, Vector3 target, float time, JumpToTweenInfo info = null)
    {
        if (info == null)
            info = new JumpToTweenInfo();
        
        var startJumpSeq = DOTween.Sequence();
        startJumpSeq.Append(tr.DOScaleY(info.PrepareJumpXScale, info.PrepareJumpTime));
        startJumpSeq.Join(tr.DOScaleX(info.PrepareJumpYScale, info.PrepareJumpTime));
        startJumpSeq.Join(tr.DOMoveY(tr.position.y - info.PrepareJumpYMove, info.PrepareJumpTime));

        var jumpScaleSeq = DOTween.Sequence();
        jumpScaleSeq.Append(tr.DOScaleY(info.InJumpYScale, time / 2f));
        jumpScaleSeq.Join(tr.DOScaleX(info.InJumpXScale, time / 2f));
        jumpScaleSeq.Append(tr.DOScale(1f, time / 2f));

        var motionSeq = DOTween.Sequence();
        motionSeq.Append(tr.DOMoveX(target.x, time).SetEase(info.InJumpXEase));
        motionSeq.Join(tr.DOMoveY(target.y, time).SetEase(info.InJumpYEase));
        motionSeq.Join(tr.DOMoveZ(target.z, time).SetEase(info.InJumpZEase));

        var seq = DOTween.Sequence();
        seq.Append(startJumpSeq);
        seq.Append(jumpScaleSeq);
        seq.Join(motionSeq);

        return seq;
    }
    
    
}