using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CharacterEmoji : MonoBehaviour
{
    [SerializeField] private float showEmojiDuration = 3;
    [SerializeField] private Image emojiImage;

    private ChooseEmojiDialog emojiPanel;
    private PhotonView view;

    private float _showEmojiLeftTime = 0;

    private float showEmojiLeftTime
    {
        get
        {
            return this._showEmojiLeftTime;
        }
        set
        {
            if (this._showEmojiLeftTime == value)
            {
                return;
            }
            this._showEmojiLeftTime = value;
            this.emojiImage.gameObject.SetActive(value > 0);
        }
    }

    private void Awake()
    {
        this.view = this.GetComponent<PhotonView>();
        this.emojiPanel = GameObject.Find("GameCanvas/EmojiPanel").GetComponent<ChooseEmojiDialog>();
        this.emojiPanel.OnConfirm.AddListener(() =>
        {
            if (this.view.IsMine)
            {
                this.showEmoji(this.emojiPanel.chooseEmojiID);
            }
        });
    }
    private void Update()
    {
        if (this.showEmojiLeftTime > 0)
        {
            this.showEmojiLeftTime = Mathf.Max(0, this.showEmojiLeftTime - Time.deltaTime);
        }
    }

    public void showEmoji(int emojiIndex)
    {
        this.view.RPC("RPC_ShowEmoji", RpcTarget.All, emojiIndex);
    }

    [PunRPC]
    private void RPC_ShowEmoji(int emojiIndex)
    {
        this.showEmojiLeftTime = this.showEmojiDuration;
        this.emojiImage.sprite = this.emojiPanel.emojis[emojiIndex];
    }
}
