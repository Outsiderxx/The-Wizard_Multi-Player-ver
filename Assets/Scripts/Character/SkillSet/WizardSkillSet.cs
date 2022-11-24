using UnityEngine;

public class WizardSkillSet : CharacterSkillSet
{
    [SerializeField] private GameObject normalFireBallPrefab;
    [SerializeField] private GameObject specialFireBallPrefab;
    [SerializeField] private Sprite fireballSprite;
    [SerializeField] private int multipleFireBallCount;
    [SerializeField] private float curementPerSecond;
    [SerializeField] private float cureDuration;
    [SerializeField] private LayerMask whatIsCharacter;

    public override bool NormalAttack(bool direction)
    {
        if (!base.NormalAttack(direction))
        {
            return false;
        }

        FireBall fireBall = Instantiate(original: this.normalFireBallPrefab).GetComponent<FireBall>();
        fireBall.damage *= this.state.isRingOfStrengthCollected ? 1.5f : 1;
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

    public override bool UseSkillA(Vector3 worldPos)
    {
        if (!base.UseSkillA(worldPos))
        {
            return false;
        }

        bool direction = worldPos.x >= this.transform.position.x;
        for (int i = 0; i < this.multipleFireBallCount; i++)
        {
            FireBall fireBall = Instantiate(original: this.specialFireBallPrefab).GetComponent<FireBall>();
            fireBall.damage *= this.state.isRingOfStrengthCollected ? 1.5f : 1;
            fireBall.woundRampTexture = this.woundRampTexture;
            fireBall.transform.position = this.movement.root.position + new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0);
            fireBall.GetComponent<SpriteRenderer>().sprite = this.fireballSprite;
            fireBall.FireAt(worldPos, i * 0.2f);
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

    public override bool UseSkillB(Vector3 worldPos)
    {
        CharacterState player = Physics2D.OverlapCircle(worldPos, 2, this.whatIsCharacter)?.GetComponentInParent<CharacterState>();
        if (!player)
        {
            return false;
        }
        if (!base.UseSkillB(worldPos))
        {
            return false;
        }
        StartCoroutine(player.Cure(this.curementPerSecond, this.cureDuration));
        player.GetComponent<CharacterEffect>().ShowColorTransition(Color.green, (int)this.cureDuration);
        AudioManager.Instance.PlayEffect("Heal");
        return true;
    }
}
