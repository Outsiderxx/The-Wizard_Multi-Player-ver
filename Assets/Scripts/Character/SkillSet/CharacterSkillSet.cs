using UnityEngine;
using UnityEngine.UI;

public abstract class CharacterSkillSet : MonoBehaviour
{
    [SerializeField] protected Texture2D woundRampTexture;
    [SerializeField] private float normalAttackCD = 0;
    [SerializeField] private float skillACD = 0;
    [SerializeField] private float skillBCD = 0;
    [SerializeField] protected float normalAttackMana = 0;
    [SerializeField] protected float skillAMana = 0;
    [SerializeField] protected float skillBMana = 0;
    public float leftNormalAttackCD { get; private set; } = 0;
    public float leftSkillACD { get; private set; } = 0;
    public float leftSkillBCD { get; private set; } = 0;

    protected CharacterMovementControl movement;
    protected CharacterState state;
    protected CharacterEffect effect;
    protected Image normalAttackImage;
    protected Image skillAImage;
    protected Image skillBImage;
    private Image normalAttackMask;
    private Image skillAMask;
    private Image skillBMask;
    private Text normalAttackCDText;
    private Text skillACDText;
    private Text skillBCDText;

    private void Awake()
    {
        this.movement = this.GetComponent<CharacterMovementControl>();
        this.state = this.GetComponent<CharacterState>();
        this.effect = this.GetComponent<CharacterEffect>();

        Transform skillRoot = GameObject.Find("GameCanvas/SkillSet/SkillSetFrame/Skills").transform;
        this.normalAttackImage = skillRoot.Find("NormalAttack").GetComponent<Image>();
        this.skillAImage = skillRoot.Find("SkillA").GetComponent<Image>();
        this.skillBImage = skillRoot.Find("SkillB").GetComponent<Image>();
        this.normalAttackMask = this.normalAttackImage.transform.Find("Mask").GetComponent<Image>();
        this.skillAMask = this.skillAImage.transform.Find("Mask").GetComponent<Image>();
        this.skillBMask = this.skillBImage.transform.Find("Mask").GetComponent<Image>();
        this.normalAttackCDText = this.normalAttackMask.transform.Find("CountdownTime").GetComponent<Text>();
        this.skillACDText = this.skillAMask.transform.Find("CountdownTime").GetComponent<Text>();
        this.skillBCDText = this.skillBMask.transform.Find("CountdownTime").GetComponent<Text>();
    }

    void Update()
    {
        // decrease cold down
        if (this.leftNormalAttackCD > 0)
        {
            this.leftNormalAttackCD = Mathf.Max(0, this.leftNormalAttackCD - Time.deltaTime);
            this.normalAttackMask.gameObject.SetActive(true);
            this.normalAttackMask.fillAmount = this.leftNormalAttackCD / this.normalAttackCD;
            this.normalAttackCDText.text = Mathf.CeilToInt(this.leftNormalAttackCD).ToString();
        }
        else
        {
            this.normalAttackMask.gameObject.SetActive(false);
        }

        if (this.leftSkillACD > 0)
        {
            this.leftSkillACD = Mathf.Max(0, this.leftSkillACD - Time.deltaTime);
            this.skillAMask.gameObject.SetActive(true);
            this.skillAMask.fillAmount = this.leftSkillACD / this.skillACD;
            this.skillACDText.text = Mathf.CeilToInt(this.leftSkillACD).ToString();
        }
        else
        {
            this.skillAMask.gameObject.SetActive(false);
        }

        if (this.leftSkillBCD > 0)
        {
            this.leftSkillBCD = Mathf.Max(0, this.leftSkillBCD - Time.deltaTime);
            this.skillBMask.gameObject.SetActive(true);
            this.skillBMask.fillAmount = this.leftSkillBCD / this.skillBCD;
            this.skillBCDText.text = Mathf.CeilToInt(this.leftSkillBCD).ToString();
        }
        else
        {
            this.skillBMask.gameObject.SetActive(false);
        }
    }

    public void RecoverState(SavedCharacterData data)
    {
        this.leftNormalAttackCD = data.leftNormalAttackCD;
        this.leftSkillACD = data.leftSkillACD;
        this.leftSkillBCD = data.leftSkillBCD;
    }

    public virtual bool NormalAttack()
    {
        if (this.leftNormalAttackCD > 0)
        {
            return false;
        }
        if (this.state.currentMagicPoints < this.normalAttackMana)
        {
            return false;
        }
        this.state.UseMana(this.normalAttackMana);
        this.leftNormalAttackCD = this.normalAttackCD;
        return true;
    }

    public virtual bool UseSkillA()
    {
        if (this.leftSkillACD > 0)
        {
            return false;
        }
        if (this.state.currentMagicPoints < this.skillAMana)
        {
            return false;
        }
        this.state.UseMana(this.skillAMana);
        this.leftSkillACD = this.skillACD;
        return true;
    }

    public virtual bool UseSkillB()
    {
        if (this.leftSkillBCD > 0)
        {
            return false;
        }
        if (this.state.currentMagicPoints < this.skillBMana)
        {
            return false;
        }
        this.state.UseMana(this.skillBMana);
        this.leftSkillBCD = this.skillBCD;
        return true;
    }
}
