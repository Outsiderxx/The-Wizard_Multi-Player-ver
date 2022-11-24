using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomPagePlayerInfo : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text playerName;
    [SerializeField] private Color normalNameColor;
    [SerializeField] private Color roomMasterNameColor;
    [SerializeField] private GameObject characterOne;
    [SerializeField] private GameObject characterTwo;
    [SerializeField] private Button changeButton;
    [SerializeField] private ChooseCharacterDialog chooseCharacterDialog;

    private int _chooseCharacterID = -1;
    private Player player;

    public int chooseCharacterID
    {
        get
        {
            return this._chooseCharacterID;
        }
        private set
        {
            if (this._chooseCharacterID == value)
            {
                return;
            }
            this._chooseCharacterID = value;
            this.characterOne.SetActive(value == 0);
            this.characterTwo.SetActive(value == 1);
            ExitGames.Client.Photon.Hashtable myCustomProperties = this.player.CustomProperties;
            myCustomProperties["chooseCharacterIndex"] = value;
            this.player.SetCustomProperties(myCustomProperties);
        }
    }

    public void InitPlayerInfo(Player player)
    {
        this.player = player;
        this.playerName.text = player.NickName;
        this.playerName.color = player.IsMasterClient ? this.roomMasterNameColor : this.normalNameColor;
        this.changeButton.interactable = player.IsLocal;
        if (player.CustomProperties["chooseCharacterIndex"] != null)
        {
            this.chooseCharacterID = (int)player.CustomProperties["chooseCharacterIndex"];
        }
        else
        {
            this.chooseCharacterID = 0;
        }
        this.playerName.gameObject.SetActive(true);
        this.changeButton.gameObject.SetActive(true);
    }

    public void ResetPlayerInfo()
    {
        this.playerName.gameObject.SetActive(false);
        this.changeButton.gameObject.SetActive(false);
        this.characterOne.SetActive(value: false);
        this.characterTwo.SetActive(false);
        this._chooseCharacterID = -1;
    }

    // pun callback
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (targetPlayer != this.player)
        {
            return;
        }
        this.chooseCharacterID = (int)changedProps["chooseCharacterIndex"];
    }

    // button callback
    public async void OnChangeButtonClick()
    {
        this.chooseCharacterDialog.Open();
        await this.chooseCharacterDialog.WaitForAction();
        this.chooseCharacterID = this.chooseCharacterDialog.chooseCharaterID;
    }
}
