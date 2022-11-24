using System;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public abstract class CharacterSkillSet : MonoBehaviour
{
    public Sprite[] skillSprites;
    [SerializeField] protected Texture2D woundRampTexture;
    public float normalAttackCD = 0;
    public float skillACD = 0;
    public float skillBCD = 0;
    [SerializeField] protected float normalAttackMana = 0;
    [SerializeField] protected float skillAMana = 0;
    [SerializeField] protected float skillBMana = 0;

    protected CharacterMovementControl movement;
    protected CharacterState state;
    protected CharacterEffect effect;
    protected PhotonView view;

    public Action onNormalAttackCDChanged;
    public Action onSkillACDChanged;
    public Action onSkillBCDChanged;

    private float _leftNormalAttackCD = 0;
    private float _leftSkillACD = 0;
    private float _leftSkillBCD = 0;

    public float leftNormalAttackCD
    {
        get
        {
            return this._leftNormalAttackCD;
        }
        private set
        {
            if (this._leftNormalAttackCD == value)
            {
                return;
            }
            this._leftNormalAttackCD = value;
            this.onNormalAttackCDChanged?.Invoke();
        }
    }
    public float leftSkillACD
    {
        get
        {
            return this._leftSkillACD;
        }
        private set
        {
            if (this._leftSkillACD == value)
            {
                return;
            }
            this._leftSkillACD = value;
            this.onSkillACDChanged?.Invoke();
        }
    }
    public float leftSkillBCD
    {
        get
        {
            return this._leftSkillBCD;
        }
        private set
        {
            if (this._leftSkillBCD == value)
            {
                return;
            }
            this._leftSkillBCD = value;
            this.onSkillBCDChanged?.Invoke();
        }
    }

    private void Awake()
    {
        this.movement = this.GetComponent<CharacterMovementControl>();
        this.state = this.GetComponent<CharacterState>();
        this.effect = this.GetComponent<CharacterEffect>();
        this.view = this.GetComponent<PhotonView>();
    }

    void Update()
    {
        if (this.leftNormalAttackCD > 0)
        {
            this.leftNormalAttackCD = Mathf.Max(0, this.leftNormalAttackCD - Time.deltaTime);
        }

        if (this.leftSkillACD > 0)
        {
            this.leftSkillACD = Mathf.Max(0, this.leftSkillACD - Time.deltaTime);
        }

        if (this.leftSkillBCD > 0)
        {
            this.leftSkillBCD = Mathf.Max(0, this.leftSkillBCD - Time.deltaTime);
        }
    }

    public void RecoverState(SavedCharacterData data)
    {
        this.leftNormalAttackCD = data.leftNormalAttackCD;
        this.leftSkillACD = data.leftSkillACD;
        this.leftSkillBCD = data.leftSkillBCD;
    }

    public virtual bool NormalAttack(bool direction)
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

    public virtual bool UseSkillA(Vector3 worldPos)
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

    public virtual bool UseSkillB(Vector3 worldPos)
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
