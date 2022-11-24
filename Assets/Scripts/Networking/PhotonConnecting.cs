using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonConnecting : MonoBehaviourPunCallbacks
{
    public static PhotonConnecting Instance { get; private set; }

    [SerializeField] private string gameVersion = "0.1.0";
    [SerializeField] private EnterStringDialog enterNickNameDialog;
    [SerializeField] private Page waitForConnectingPage;

    private void Awake()
    {
        if (PhotonConnecting.Instance != null)
        {
            Destroy(this.gameObject);
        }
        PhotonConnecting.Instance = this;
    }

    private async void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            return;
        }
        string nickname = PlayerPrefs.GetString("Nickname");
        if (string.IsNullOrEmpty(nickname))
        {
            this.enterNickNameDialog.Open();
            await this.enterNickNameDialog.WaitForAction();
            nickname = this.enterNickNameDialog.result;
            this.enterNickNameDialog.Close();
            PlayerPrefs.SetString("Nickname", nickname);
            PlayerPrefs.Save();
        }
        this.waitForConnectingPage.Open();
        this.ConnectToPhoton(nickname);
    }

    private void ConnectToPhoton(string nickName)
    {
        PhotonNetwork.GameVersion = this.gameVersion;
        PhotonNetwork.SendRate = 20;
        PhotonNetwork.SerializationRate = 10;
        // PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NickName = nickName;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        print($"We are connect to: {PhotonNetwork.CloudRegion} server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print($"Disconnect reason: {cause.ToString()}");
    }

    public override void OnJoinedLobby()
    {
        this.waitForConnectingPage.Close();
    }
}
