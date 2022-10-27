using UnityEngine;
using DG.Tweening;

public class FireBall : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 0;
    [SerializeField] private float maxMoveDistance = 100;
    [SerializeField] private float damage = 0;

    public Texture2D woundRampTexture;
    private Sequence sequence;
    private float moveDistance = 0;

    private bool isCollided = false;

    private void Awake()
    {
        this.damage *= CollectionHelper.IsCollectionCollected("Ring of Strength") ? 1.5f : 1;
    }

    private void Update()
    {
        if (this.moveDistance >= this.maxMoveDistance)
        {
            Destroy(this.gameObject);
            return;
        }
        this.transform.Translate((Vector2.right * this.moveSpeed * Time.deltaTime));
        this.moveDistance += this.moveSpeed * Time.deltaTime;
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        if (this.isCollided)
        {
            return;
        }
        this.isCollided = true;
        Monster monster = other.transform.GetComponentInParent<Monster>();
        if (monster != null && monster.isAlive)
        {
            monster.sprite.material.SetTexture("_DissolveEdgeRampTex", this.woundRampTexture);
            monster.Hurt(this.damage);
        }
        this.sequence.Kill();
        Destroy(this.gameObject);
    }

    public void FireToward(bool isForward)
    {
        this.enabled = true;
        this.transform.right = isForward ? Vector2.right : Vector2.left;
        AudioManager.Instance.PlayEffect("FireBall02");
    }

    public void FireAt(Vector3 destination, float delay)
    {
        Vector3 direction = (destination - this.transform.position).normalized;
        float originScale = this.transform.localScale.x;
        this.transform.right = direction;
        this.transform.localScale = Vector3.zero;
        this.sequence = DOTween.Sequence();
        this.sequence.AppendInterval(delay);
        this.sequence.Append(this.transform.DOScale(originScale, 0.2f));
        this.sequence.Append(this.transform.DOLocalMove(-direction, 0.3f).SetRelative());
        this.sequence.AppendCallback(() =>
        {
            this.enabled = true;
            AudioManager.Instance.PlayEffect("FireBall01");
        });
    }
}
