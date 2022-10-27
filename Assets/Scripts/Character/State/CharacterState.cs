using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharacterState : MonoBehaviour
{
    [SerializeField] private Sprite avatar;
    [SerializeField] private string characterName;
    [SerializeField] private float maxHealthPoints;
    [SerializeField] private float maxMagicPoints;
    [SerializeField] private float recoverManaPerSecond;
    [SerializeField] private string hurtClipName;

    private CharacterMovementControl movement;
    private CharacterEffect effect;
    private CharacterEventHandler eventHander;
    private Image healthPointBar;
    private Image magicPointBar;
    private Image avatarImage;
    private Text characterNameText;
    private float _currentHealthPoints = 100;
    private float _currentMagicPoints = 100;
    private bool isRecovered = false;

    public System.Action OnDead;
    public System.Action OnHurt;

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
            if (this._currentHealthPoints == Mathf.Clamp(value, 0, this.maxHealthPoints))
            {
                return;
            }
            this._currentHealthPoints = Mathf.Clamp(value, 0, this.maxHealthPoints);
            this.OnHealthPointChanged();
        }
    }

    public float currentMagicPoints
    {
        get
        {
            return this._currentMagicPoints;
        }
        private set
        {
            if (this._currentMagicPoints == Mathf.Clamp(value, 0, this.maxHealthPoints))
            {
                return;
            }
            this._currentMagicPoints = Mathf.Clamp(value, 0, this.maxMagicPoints);
            this.OnMagicPointChanged();
        }
    }

    private void Awake()
    {
        this.movement = this.GetComponent<CharacterMovementControl>();
        this.effect = this.GetComponent<CharacterEffect>();
        this.eventHander = this.GetComponentInChildren<CharacterEventHandler>();
        this.healthPointBar = GameObject.Find("GameCanvas/PlayerInfo/Bars/Healthbar").GetComponent<Image>();
        this.magicPointBar = GameObject.Find("GameCanvas/PlayerInfo/Bars/Manabar").GetComponent<Image>();
        this.avatarImage = GameObject.Find("GameCanvas/PlayerInfo/Image Wrap/UserPic").GetComponent<Image>();
        this.characterNameText = GameObject.Find("GameCanvas/PlayerInfo/Name").GetComponent<Text>();

        this.eventHander.OnObtainItem += this.OnObtainItem;
        this.eventHander.OnTriggerPortal += () =>
        {
            GameManager.Instance.GoToNextStage();
        };
    }

    private void Start()
    {
        this.maxHealthPoints *= CollectionHelper.IsCollectionCollected("Ring of Eternity") ? 1.5f : 1;
        this.maxMagicPoints *= CollectionHelper.IsCollectionCollected("Ring of Wizard") ? 1.5f : 1;
        this.avatarImage.sprite = this.avatar;
        this.characterNameText.text = this.characterName;
        if (!this.isRecovered)
        {
            this._currentHealthPoints = this.maxHealthPoints;
            this._currentMagicPoints = this.maxMagicPoints;
        }
        this.OnHealthPointChanged();
        this.OnMagicPointChanged();
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

    private void OnHealthPointChanged()
    {
        this.healthPointBar.fillAmount = this.currentHealthPoints / this.maxHealthPoints;
        if (this.currentHealthPoints == 0)
        {
            this.enabled = false;
            GameManager.Instance.ShowGameResult(false);
            this.OnDead.Invoke();
        }
    }

    private void OnMagicPointChanged()
    {
        this.magicPointBar.fillAmount = this.currentMagicPoints / this.maxMagicPoints;
    }

    private void OnObtainItem(Item item)
    {
        if (item == null)
        {
            Debug.LogError("GameObject doesn't have Item Component");
            return;
        }
        AudioManager.Instance.PlayEffect("Item");
        item.Use(this);
        if (item.GetType() == typeof(HealthPointItem))
        {
            this.effect.ShowColorTransition(Color.red, 1);
        }
        else if (item.GetType() == typeof(ManaPointItem))
        {
            this.effect.ShowColorTransition(Color.cyan, 1);
        }
        else
        {
            this.effect.ShowColorTransition(new Color(0.84f, 0.6f, 0.035f), 3);
        }
    }

    public void AcquireCollection(CollectionItem collection)
    {
        CollectionHelper.CollectCollection(collection.collectionName);
        if (collection.collectionName == "Ring of Eternity")
        {
            this.maxHealthPoints *= 1.5f;
            this.currentHealthPoints *= 1.5f;
        }
        else if (collection.collectionName == "Ring of Wizard")
        {
            this.maxMagicPoints *= 1.5f;
            this.currentMagicPoints *= 1.5f;
        }
    }
}
