using UnityEngine;
using DG.Tweening;
using System.Collections;


public class CharacterMovementControl : MonoBehaviour
{
    public bool facingRight = true;
    [SerializeField] private float maxSpeed = 10;
    [SerializeField] private float jumpForce = 400;
    [SerializeField] private float groundedRadius = 0.1f;
    [SerializeField] private bool airControl = false;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private AudioSource moveAudioSource;


    public Transform root { get; private set; }
    public Animator anim { get; private set; }
    private Transform foot;
    private SpriteRenderer sprite;
    private Rigidbody2D body;
    private CharacterState state;
    private bool grounded = false;

    private Vector3 correctLocalScale;
    private bool correctFacing;
    private bool jumpStart = false;

    private void Awake()
    {
        this.body = this.GetComponent<Rigidbody2D>();
        this.state = this.GetComponent<CharacterState>();
        this.sprite = this.transform.Find("Sprite").GetComponent<SpriteRenderer>();
        this.foot = this.sprite.transform.Find("Foot");
        this.root = this.sprite.transform.Find("Root");
        this.anim = this.sprite.GetComponent<Animator>();

        this.state.OnDead += () =>
        {
            this.enabled = false;
            this.anim.SetBool("IsDead", true);
            this.body.velocity = Vector2.zero;
            this.body.bodyType = RigidbodyType2D.Kinematic;
            this.moveAudioSource.Stop();
        };
        this.state.OnHurt += () =>
        {
            this.enabled = false;
            Scheduler.ScheduleOnce(() =>
            {
                if (this.state.isAlive)
                {
                    this.enabled = true;
                }
            }, 0.3f);

            this.sprite.material.color = Color.red;
            this.sprite.material.DOColor(Color.white, 0.3f).Play();
            this.anim.SetBool("IsHurt", true);
            Scheduler.ScheduleOnce(() => this.anim.SetBool("IsHurt", false), 0.3f);

        };
    }

    private void Start()
    {
        this.correctLocalScale = this.sprite.transform.localScale;
        this.correctFacing = this.facingRight;
    }

    private void Update()
    {
        grounded = Physics2D.OverlapCircle(foot.position, groundedRadius, whatIsGround);
        anim.SetBool("IsGrounded", grounded);

        if (this.grounded && !this.jumpStart)
        {
            this.body.drag = 2;
        }
    }

    public void Move(float move, bool jump)
    {
        if (!this.enabled)
        {
            return;
        }

        if ((grounded || airControl))
        {

            this.anim.SetFloat("Speed", Mathf.Abs(move));

            if (move != 0)
            {
                this.body.velocity = new Vector2(move * maxSpeed, this.body.velocity.y);
            }

            if (move > 0 && !facingRight)
            {
                Flip();
            }
            else if (move < 0 && facingRight)
            {
                Flip();
            }
        }

        if (grounded && jump)
        {
            AudioManager.Instance.PlayEffect("Jump");
            anim.SetBool("IsGrounded", false);
            this.body.drag = 0f;
            this.jumpStart = true;
            Scheduler.ScheduleOnce(() => this.jumpStart = false, 0.3f);
            this.body.AddForce(new Vector2(0f, jumpForce));
        }

        if (this.grounded && move != 0)
        {
            if (!this.moveAudioSource.isPlaying)
            {
                this.moveAudioSource.Play();
            }
        }
        else if (this.moveAudioSource.isPlaying)
        {
            this.moveAudioSource.Stop();
        }
    }

    private void Flip()
    {
        this.correctLocalScale.x *= -1;
        this.correctFacing = !this.correctFacing;
        if (this.anim.GetBool("IsAttack"))
        {
            return;
        }
        this.sprite.transform.localScale = this.correctLocalScale;
        facingRight = this.correctFacing;
    }

    public void EnsureCorrectFacing()
    {
        this.sprite.transform.localScale = this.correctLocalScale;
    }

    public void ChangeFacing(bool facingRight)
    {
        if (this.facingRight != facingRight)
        {
            Vector3 newScale = this.sprite.transform.localScale;
            newScale.x *= -1;
            this.sprite.transform.localScale = newScale;
        }
    }
}
