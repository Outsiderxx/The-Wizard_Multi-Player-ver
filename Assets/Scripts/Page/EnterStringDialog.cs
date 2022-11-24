using UnityEngine;
using UnityEngine.UI;

public class EnterStringDialog : Dialog
{
    [SerializeField] private InputField input;
    [SerializeField] private Button confirmButton;

    public string result
    {
        get
        {
            return this.input.text;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        this.input.onValueChanged.AddListener((value) =>
        {
            this.confirmButton.interactable = !string.IsNullOrEmpty(value);
        });
    }

    public void Clear()
    {
        this.input.text = "";
    }
}
