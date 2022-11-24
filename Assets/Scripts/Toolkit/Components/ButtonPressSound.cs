using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonPressSound : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool playOnMouseUp = true;
    public bool playOnMouseDown = false;
    public string clipName;

    private Button button;

    private void Awake()
    {
        this.button = this.GetComponent<Button>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!button.interactable)
        {
            return;
        }
        if (this.playOnMouseDown)
        {
            AudioManager.Instance.PlayEffect(this.clipName);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!button.interactable)
        {
            return;
        }
        if (this.playOnMouseUp)
        {
            AudioManager.Instance.PlayEffect(this.clipName);
        }
    }
}
