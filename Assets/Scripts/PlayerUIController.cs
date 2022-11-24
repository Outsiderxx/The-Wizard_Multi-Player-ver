using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerUIController : MonoBehaviour
{
    // character state
    [SerializeField] private Image healthPointBar;
    [SerializeField] private Image magicPointBar;
    [SerializeField] private Image avatarImage;
    [SerializeField] private Text characterNameText;

    // skillset
    [SerializeField] private Image normalAttackImage;
    [SerializeField] private Image skillAImage;
    [SerializeField] private Image skillBImage;
    [SerializeField] private Image normalAttackMask;
    [SerializeField] private Image skillAMask;
    [SerializeField] private Image skillBMask;
    [SerializeField] private Text normalAttackCDText;
    [SerializeField] private Text skillACDText;
    [SerializeField] private Text skillBCDText;

    public void BindCharacter(CharacterState state, CharacterSkillSet skillSet)
    {
        // character state
        this.avatarImage.sprite = state.avatar;
        this.characterNameText.text = PhotonNetwork.NickName;
        state.OnHealthPointChanged += () =>
        {
            this.healthPointBar.fillAmount = state.currentHealthPoints / state.maxHealthPoints;
        };
        state.OnMagicPointChanged += () =>
        {
            this.magicPointBar.fillAmount = state.currentMagicPoints / state.maxMagicPoints;
        };

        // skillset
        this.normalAttackImage.sprite = skillSet.skillSprites[0];
        this.skillAImage.sprite = skillSet.skillSprites[1];
        this.skillBImage.sprite = skillSet.skillSprites[2];
        skillSet.onNormalAttackCDChanged += () =>
        {
            if (skillSet.leftNormalAttackCD > 0)
            {
                this.normalAttackMask.gameObject.SetActive(true);
                this.normalAttackMask.fillAmount = skillSet.leftNormalAttackCD / skillSet.normalAttackCD;
                this.normalAttackCDText.text = Mathf.CeilToInt(skillSet.leftNormalAttackCD).ToString();
            }
            else
            {
                this.normalAttackMask.gameObject.SetActive(false);
            }

        };
        skillSet.onSkillACDChanged += () =>
        {
            if (skillSet.leftSkillACD > 0)
            {
                this.skillAMask.gameObject.SetActive(true);
                this.skillAMask.fillAmount = skillSet.leftSkillACD / skillSet.skillACD;
                this.skillACDText.text = Mathf.CeilToInt(skillSet.leftSkillACD).ToString();
            }
            else
            {
                this.skillAMask.gameObject.SetActive(false);
            }
        };
        skillSet.onSkillBCDChanged += () =>
        {
            if (skillSet.leftSkillBCD > 0)
            {
                this.skillBMask.gameObject.SetActive(true);
                this.skillBMask.fillAmount = skillSet.leftSkillBCD / skillSet.skillBCD;
                this.skillBCDText.text = Mathf.CeilToInt(skillSet.leftSkillBCD).ToString();
            }
            else
            {
                this.skillBMask.gameObject.SetActive(false);
            }
        };
    }
}
