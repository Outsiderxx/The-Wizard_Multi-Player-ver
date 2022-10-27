using UnityEngine;
using UnityEngine.UI;

public class SettingPage : Page
{
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider effectVolumeSlider;

    protected override void Awake()
    {
        base.Awake();
        this.musicVolumeSlider.onValueChanged.AddListener((value) =>
        {
            AudioManager.Instance.MusicVolume = value;
        });
        this.effectVolumeSlider.onValueChanged.AddListener((value) =>
        {
            AudioManager.Instance.EffectVolume = value;
        });
    }

    private void Start()
    {
        this.musicVolumeSlider.value = AudioManager.Instance.MusicVolume;
        this.effectVolumeSlider.value = AudioManager.Instance.EffectVolume;
    }
}
