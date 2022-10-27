using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Monster : MonoBehaviour
{
    [SerializeField] private Slider healthPointBar;
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private GameObject healthPointItem;
    [SerializeField] private GameObject ManaPointItem;
    [SerializeField] private bool _isFacingRight = false;
    [SerializeField] private float maxHealthPoints = 0;
    [SerializeField] private float detectPlayerRadius = 0;
    [SerializeField] private float detectGroundDistance = 0;
    [SerializeField] private float detectWallDistance = 0;
    [SerializeField] private float attackDistance = 0;
    [SerializeField] private float moveSpeed = 0;
    [SerializeField] private float damage = 0;
    [SerializeField] private float attackCD = 0;
    [SerializeField] private float attackForce = 0;


    public Transform foot { get; private set; }
    public Transform root { get; private set; }
    public SpriteRenderer sprite { get; private set; }
    private Rigidbody2D body;
    private float _currentHealthPoints = 0;
    private float currentAttackCD = 0;
    private bool isRecovered = false;

    public bool isAlive
    {
        get
        {
            return this.currentHealthPoints != 0;
        }
    }

    public float currentHealthPoints
    {
        get
        {
            return this._currentHealthPoints;
        }
        private set
        {
            if (this._currentHealthPoints == value)
            {
                return;
            }
            this._currentHealthPoints = Mathf.Max(0, value);
            this.OnHealthPointsChanged();
        }
    }

    public bool isFacingRight
    {
        get
        {
            return this._isFacingRight;
        }
        set
        {
            if (this._isFacingRight == value)
            {
                return;
            }
            this._isFacingRight = value;
            this.Flip();
        }
    }

    private void Awake()
    {
        this.body = this.GetComponent<Rigidbody2D>();
        this.sprite = this.transform.Find("Sprite").GetComponent<SpriteRenderer>();
        this.root = this.sprite.transform.Find("Root");
        this.foot = this.sprite.transform.Find("Foot");
    }

    private void Start()
    {
        if (!this.isRecovered)
        {
            this.currentHealthPoints = this.maxHealthPoints;
        }
        this.OnHealthPointsChanged();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.currentAttackCD > 0)
        {
            this.currentAttackCD = Mathf.Max(0, this.currentAttackCD - Time.deltaTime);
        }

        // detect player
        CharacterMovementControl player = Physics2D.OverlapCircle(this.transform.position, this.detectPlayerRadius, this.whatIsPlayer)?.GetComponentInParent<CharacterMovementControl>();
        if (player != null && !player.GetComponent<CharacterState>().isAlive)
        {
            player = null;
        }
        if (player)
        {
            // change facing direction
            this.isFacingRight = player.root.position.x > this.root.position.x;
            // calculate distance
            float distance = Vector2.Distance(player.root.position, this.root.position);
            if (distance <= this.attackDistance)
            {
                this.body.velocity = Vector2.zero;
                this.Attack(player.GetComponent<CharacterState>());
                return;
            }
        }

        RaycastHit2D hitGround = Physics2D.Raycast(this.foot.position, this.isFacingRight ? Vector2.right : Vector2.left, this.detectGroundDistance, this.whatIsGround);
        RaycastHit2D hitWall = Physics2D.Raycast(this.root.position, this.isFacingRight ? Vector2.right : Vector2.left, this.detectWallDistance, this.whatIsGround);
        // check if can move forward
        if (!hitGround || hitWall)
        {
            if (!player)
            {
                this.isFacingRight = !this.isFacingRight;
            }
            else
            {
                return;
            }
        }

        this.body.velocity = (this.isFacingRight ? Vector2.right : Vector2.left) * this.moveSpeed;
    }

    public void RecoverState(float currentHealthPoints)
    {
        this.isRecovered = true;
        this._currentHealthPoints = currentHealthPoints;
    }

    public void Hurt(float damage)
    {
        this.currentHealthPoints -= damage;
        this.sprite.color = Color.red;
        this.sprite.DOColor(Color.white, 0.3f).Play();
    }

    protected void Attack(CharacterState player)
    {
        if (this.currentAttackCD != 0)
        {
            return;
        }
        this.currentAttackCD = this.attackCD;
        player.Hurt(this.damage);
        Vector2 attackDirection = this.isFacingRight ? Vector2.right : Vector2.left;
        attackDirection.y += 0.5f;
        player.GetComponent<Rigidbody2D>().AddForce(attackDirection * this.attackForce);
    }

    private void OnHealthPointsChanged()
    {
        this.healthPointBar.value = this.currentHealthPoints / this.maxHealthPoints;
        if (this.currentHealthPoints == 0)
        {
            this.OnDead();
        }
    }

    private void Flip()
    {
        Vector3 newScale = this.sprite.transform.localScale;
        newScale.x *= -1;
        this.sprite.transform.localScale = newScale;
    }

    private void OnDead()
    {
        this.enabled = false;
        this.healthPointBar.gameObject.SetActive(false);
        this.body.bodyType = RigidbodyType2D.Kinematic;
        this.GetComponentInChildren<Collider2D>().enabled = false;
        AudioManager.Instance.PlayEffect("Monster_Dead");
        bool hasDroppedItem = false;
        if (Random.Range(0, 10) >= 7)
        {
            hasDroppedItem = true;
            Instantiate(this.healthPointItem, this.transform.position, Quaternion.identity);
        }
        if (!hasDroppedItem && Random.Range(0, 10) >= 7)
        {
            Instantiate(this.ManaPointItem, this.transform.position, Quaternion.identity);
        }
        Sequence sequence = DOTween.Sequence();
        sequence.Append(DOTween.To(() => this.sprite.material.GetFloat("_DissolvePos"), (value) => this.sprite.material.SetFloat("_DissolvePos", value), 1f, 0.5f));
        sequence.AppendCallback(() =>
        {
            Destroy(this.gameObject);
        });
    }
}
