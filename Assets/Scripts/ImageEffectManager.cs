using UnityEngine;
using DG.Tweening;
using UnityStandardAssets.ImageEffects;

public class ImageEffectManager : MonoBehaviour
{
    private BlurOptimized blurEffect;
    private Grayscale grayscale;

    private void Awake()
    {
        this.blurEffect = this.GetComponent<BlurOptimized>();
        this.grayscale = this.GetComponent<Grayscale>();
    }

    public void EnableGrayScaleEffect()
    {
        this.grayscale.enabled = true;
    }

    public void ShowOpenSceneBlurTransition()
    {
        this.blurEffect.enabled = true;
        Tweener tween = DOTween.To(() => this.blurEffect.blurSize, (value) => this.blurEffect.blurSize = value, 1, 1);
        tween.OnComplete(() =>
        {
            this.blurEffect.enabled = false;
        });
    }
}
