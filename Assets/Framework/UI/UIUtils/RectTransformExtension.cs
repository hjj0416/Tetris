using DG.Tweening;
using System;
using UnityEngine;

public static class RectTransformExtension
{
    public static Tweener DoUILocalMove(this RectTransform target, Vector3 endValue,float duration, bool snapping = false)
    {
        return TweenSettingsExtensions.SetTarget<Tweener>(TweenSettingsExtensions.SetOptions(DOTween.To(delegate { return target.anchoredPosition3D; },delegate(Vector3 x) { target.anchoredPosition3D = x; },endValue, duration),snapping),target);
    }   
}

