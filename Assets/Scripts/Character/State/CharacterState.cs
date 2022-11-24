using UnityEngine;
using System;
using System.Collections;
using Photon.Realtime;
using Photon.Pun;

public class CharacterState : MonoBehaviour, IPunInstantiateMagicCallback
{
    public Sprite avatar;
    public float maxHealthPoints;
    public float maxMagicPoints;
    [SerializeField] private float recoverManaPerSecond;
    [SerializeField] private string hurtClipName;

    private CharacterMovementControl movement;
    private CharacterEffect effect;

    private Player playerData;
    private float _currentHealthPoints = 100;
    private float _currentMagicPoints = 100;
    private bool isRecovered = false;

    public Action OnDead;
    public Action OnHurt;
    public Action OnHealthPointChanged;
    public Action OnMagicPointChanged;

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
        set
        {
            if (this._currentHealthPoints == Mathf.Clamp(value, 0, this.maxHealthPoints))
            {
                return;
            }
            this._currentHealthPoints = Mathf.Clamp(value, 0, this.maxHealthPoints);
            this.OnHealthPointChanged?.Invoke();
            if (this._currentHealthPoints == 0)
            {
                this.enabled = false;
                this.OnDead.Invoke();
            }
        }
    }

    public float currentMagicPoints
    {
        get
        {
            return this._currentMagicPoints;
        }
        set
        {
            if (this._currentMagicPoints == Mathf.Clamp(value, 0, this.maxHealthPoints))
            {
                return;
            }
            this._currentMagicPoints = Mathf.Clamp(value, 0, this.maxMagicPoints);
            this.OnMagicPointChanged?.Invoke();
        }
    }

    public bool isRingOfEternityCollected
    {
        get
        {
            return (bool)this.playerData.CustomProperties["Ring of Eternity"];
        }
    }

    public bool isRingOfWizardCollected
    {
        get
        {
            return (bool)this.playerData.CustomProperties["Ring of Wizard"];
        }
    }

    public bool isRingOfStrengthCollected
    {
        get
        {
            return (bool)this.playerData.CustomProperties["Ring of Strength"];
        }
    }

    private void Awake()
    {
        this.playerData = this.GetComponent<PhotonView>().Owner;
        this.movement = this.GetComponent<CharacterMovementControl>();
        this.effect = this.GetComponent<CharacterEffect>();
    }

    private void Start()
    {
        this.maxHealthPoints *= this.isRingOfEternityCollected ? 1.5f : 1;
        this.maxMagicPoints *= this.isRingOfWizardCollected ? 1.5f : 1;
        if (!this.isRecovered)
        {
            this._currentHealthPoints = this.maxHealthPoints;
            this._currentMagicPoints = this.maxMagicPoints;
        }
    }

    private void Update()
    {
        this.RecoverMana(this.recoverManaPerSecond * Time.deltaTime);
    }

    public void RecoverState(SavedCharacterData data)
    {
        this.isRecovered = true;
        this._currentHealthPoints = data.currentHealthPoints;
        this._currentMagicPoints = data.currentMagicPoints;
    }

    public void Hurt(float damage)
    {
        this.currentHealthPoints -= damage;
        AudioManager.Instance.PlayEffect(this.hurtClipName);
        Debug.LogError($"Left hp: {this.currentHealthPoints}");
        this.OnHurt.Invoke();
    }

    public void Cure(float curement)
    {
        this.currentHealthPoints += curement;
    }

    public IEnumerator Cure(float curement, float duration)
    {

        float passTime = 0;
        while (passTime < duration)
        {
            if (!this.isAlive)
            {
                break;
            }
            this.currentHealthPoints += curement * Time.deltaTime;
            passTime += Time.deltaTime;
            yield return null;
        }
    }

    public void UseMana(float magicPoints)
    {
        this.currentMagicPoints -= magicPoints;
    }

    public void RecoverMana(float magicPoints)
    {
        this.currentMagicPoints += magicPoints;
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (info.Sender != PhotonNetwork.LocalPlayer)
        {
            LevelManager.Instance.BindOtherCharacter(this);
        }
    }
}
