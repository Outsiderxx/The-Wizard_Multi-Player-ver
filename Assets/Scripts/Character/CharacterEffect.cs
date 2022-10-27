using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CharacterEffect : MonoBehaviour
{
    private SpriteRenderer sprite;
    private Sequence currentSequence;

    private void Awake()
    {
        this.sprite = this.transform.Find("Sprite").GetComponent<SpriteRenderer>();
    }

    public void ShowColorTransition(Color color, int times)
    {
        if (this.currentSequence != null)
        {
            this.currentSequence.Kill();
        }
        this.sprite.material.SetColor("_LtColor", color);
        this.sprite.material.SetFloat("_LtExpand", 4);
        this.sprite.material.SetFloat("_LtOuterStrength", 0);
        this.currentSequence = DOTween.Sequence();
        this.currentSequence.Append(DOTween.To(
            () => this.sprite.material.GetFloat("_LtOuterStrength"),
            (value) => this.sprite.material.SetFloat("_LtOuterStrength", value),
            1.5f, 0.5f
        ));
        this.currentSequence.Append(DOTween.To(
            () => this.sprite.material.GetFloat("_LtOuterStrength"),
            (value) => this.sprite.material.SetFloat("_LtOuterStrength", value),
            .0f, 0.5f
        ));
        this.currentSequence.SetLoops(times);
    }
}
