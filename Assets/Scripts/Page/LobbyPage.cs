using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyPage : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform roomListRoot;
    [SerializeField] private GameObject roomListItemPrefab;
    [SerializeField] private Page roomPage;
    [SerializeField] private EnterStringDialog enterPasswordDialog;
    [SerializeField] private InputField createRoomName;
    [SerializeField] private InputField createRoomPassword;
    [SerializeField] private Text failedReason;

    private Page page;

    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

    private void Awake()
    {
        this.page = this.GetComponent<Page>();
    }

    private void UpdateRoomList()
    {
        if (this.cachedRoomList.Count > this.roomListRoot.childCount)
        {
            for (int i = 0; i < this.cachedRoomList.Count - this.roomListRoot.childCount; i++)
            {
                Button button = Instantiate(this.roomListItemPrefab, this.roomListRoot).GetComponent<Button>();
                button.onClick.AddListener(async () =>
                {
                    RoomInfo roomInfo = this.cachedRoomList[button.transform.Find("RoomName").GetComponent<Text>().text];
                    if ((string)roomInfo.CustomProperties["password"] != "")
                    {
                        this.enterPasswordDialog.Open();
                        await this.enterPasswordDialog.WaitForAction();
                        string enterPassword = this.enterPasswordDialog.result;
                        this.enterPasswordDialog.Clear();
                        this.enterPasswordDialog.Close();
                        if (enterPassword != (string)roomInfo.CustomProperties["password"])
                        {
                            this.failedReason.text = "Wrong password";
                            return;
                        }
                    }
                    PhotonNetwork.JoinRoom(roomInfo.Name);
                });
            }
        }

        int index = 0;
        foreach (Transform transform in this.roomListRoot)
        {
            if (index >= this.cachedRoomList.Count)
            {
                transform.gameObject.SetActive(false);
            }
            else
            {
                transform.gameObject.SetActive(true);
                transform.Find("RoomName").GetComponent<Text>().text = this.cachedRoomList.ToList()[index].Key;
            }
            index++;
        }
    }

    // pun callback
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            if (info.RemovedFromList)
            {
                cachedRoomList.Remove(info.Name);
            }
            else
            {
                cachedRoomList[info.Name] = info;
            }
        }
        this.UpdateRoomList();
    }

    public override void OnJoinedLobby()
    {
        this.cachedRoomList.Clear();
        this.UpdateRoomList();
    }

    public override void OnJoinedRoom()
    {
        this.failedReason.text = "";
        this.createRoomName.text = "";
        this.createRoomPassword.text = "";
        this.page.Close();
        this.roomPage.Open();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        this.failedReason.text = message;
        this.createRoomName.text = "";
        this.createRoomPassword.text = "";
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        this.failedReason.text = message;
    }

    // button callback
    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(this.createRoomName.text))
        {
            this.failedReason.text = "Room name can't be empty";
            return;
        }
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        roomOptions.EmptyRoomTtl = 0;
        roomOptions.PlayerTtl = 0;
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties["password"] = this.createRoomPassword.text;
        roomOptions.CustomRoomProperties = customProperties;
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "password" };
        PhotonNetwork.CreateRoom(this.createRoomName.text, roomOptions, TypedLobby.Default);
    }
}
