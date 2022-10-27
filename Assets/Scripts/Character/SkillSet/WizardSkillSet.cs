using UnityEngine;

public class WizardSkillSet : CharacterSkillSet
{
    [SerializeField] private GameObject normalFireBallPrefab;
    [SerializeField] private GameObject specialFireBallPrefab;
    [SerializeField] private Sprite fireballSprite;
    [SerializeField] private int multipleFireBallCount;
    [SerializeField] private float curementPerSecond;
    [SerializeField] private float cureDuration;
    [SerializeField] private Sprite[] skillSprites;

    private void Start()
    {
        this.normalAttackImage.sprite = this.skillSprites[0];
        this.skillAImage.sprite = this.skillSprites[1];
        this.skillBImage.sprite = this.skillSprites[2];
    }

    public override bool NormalAttack()
    {
        if (!base.NormalAttack())
        {
            return false;
        }

        bool direction = Camera.main.ScreenToWorldPoint(Input.mousePosition).x >= this.transform.position.x;

        FireBall fireBall = Instantiate(original: this.normalFireBallPrefab).GetComponent<FireBall>();
        fireBall.transform.position = this.movement.root.position;
        fireBall.woundRampTexture = this.woundRampTexture;
        fireBall.GetComponent<SpriteRenderer>().sprite = this.fireballSprite;
        fireBall.FireToward(direction);

        this.movement.anim.SetBool("IsAttack", true);
        this.movement.ChangeFacing(direction);
        Scheduler.ScheduleOnce(() =>
        {
            this.movement.anim.SetBool("IsAttack", false);
            this.movement.EnsureCorrectFacing();
        }, 0.333f);
        return true;
    }

    public override bool UseSkillA()
    {
        if (!base.UseSkillA())
        {
            return false;

        }

        bool direction = Camera.main.ScreenToWorldPoint(Input.mousePosition).x >= this.transform.position.x;
        for (int i = 0; i < this.multipleFireBallCount; i++)
        {
            FireBall fireBall = Instantiate(original: this.specialFireBallPrefab).GetComponent<FireBall>();
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = -Camera.main.transform.position.z;
            fireBall.woundRampTexture = this.woundRampTexture;
            fireBall.transform.position = this.movement.root.position + new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0);
            fireBall.GetComponent<SpriteRenderer>().sprite = this.fireballSprite;
            fireBall.FireAt(Camera.main.ScreenToWorldPoint(mousePosition), i * 0.2f);
        }

        this.movement.anim.SetBool("IsAttack", true);
        this.movement.ChangeFacing(direction);
        Scheduler.ScheduleOnce(() =>
        {
            this.movement.anim.SetBool("IsAttack", false);
            this.movement.EnsureCorrectFacing();
        }, 0.333f);
        return true;
    }

    public override bool UseSkillB()
    {
        if (!base.UseSkillB())
        {
            return false;

        }
        StartCoroutine(this.state.Cure(this.curementPerSecond, this.cureDuration));
        this.effect.ShowColorTransition(Color.green, (int)this.cureDuration);
        AudioManager.Instance.PlayEffect("Heal");
        return true;

    }
}
