using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CharacterUI : MonoBehaviour
{
    [SerializeField] private Slider healthPointBar;
    [SerializeField] private Slider magicPointBar;
    [SerializeField] private Text characterNameText;

    private CharacterState state;

    private void Awake()
    {
        this.state = this.GetComponent<CharacterState>();
        this.state.OnHealthPointChanged += () =>
        {
            this.healthPointBar.value = this.state.currentHealthPoints / this.state.maxHealthPoints;
        };
        this.state.OnMagicPointChanged += () =>
        {
            this.magicPointBar.value = this.state.currentMagicPoints / this.state.maxMagicPoints;
        };
    }

    void Start()
    {
        this.characterNameText.text = this.state.GetComponent<PhotonView>().Owner.NickName;
    }
}
