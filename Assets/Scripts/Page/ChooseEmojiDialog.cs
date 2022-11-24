using UnityEngine;
using UnityEngine.UI;

public class ChooseEmojiDialog : Dialog
{
    public Sprite[] emojis;
    [SerializeField] private Transform emojiRoot;
    [SerializeField] private GameObject emojiButtonPrefab;

    public int chooseEmojiID { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        foreach (Sprite emoji in this.emojis)
        {
            Button emojiButton = Instantiate(this.emojiButtonPrefab, this.emojiRoot).GetComponent<Button>();
            emojiButton.GetComponent<Image>().sprite = emoji;
            emojiButton.onClick.AddListener(() =>
            {
                this.ChooseEmoji(emojiButton.transform.GetSiblingIndex());
            });
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            this.Open();
        }
    }

    public void ChooseEmoji(int characterID)
    {
        this.chooseEmojiID = characterID;
        this.Confirm();
        this.Close();
    }
}
