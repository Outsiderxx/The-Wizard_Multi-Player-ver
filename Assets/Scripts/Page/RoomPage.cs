using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomPage : MonoBehaviourPunCallbacks
{
    [SerializeField] private Page lobbyPage;
    [SerializeField] private Text roomName;
    [SerializeField] private RoomPagePlayerInfo playerOne;
    [SerializeField] private RoomPagePlayerInfo playerTwo;
    [SerializeField] private Button switchButon;
    [SerializeField] private Button startButon;

    private Page page;
    private PhotonView view;

    private void Awake()
    {
        this.page = this.GetComponent<Page>();
        this.view = this.GetComponent<PhotonView>();
    }

    private void updatePlayerInfos()
    {
        this.playerOne.ResetPlayerInfo();
        this.playerTwo.ResetPlayerInfo();
        foreach (KeyValuePair<int, Player> item in PhotonNetwork.CurrentRoom.Players)
        {
            int playerIndex = item.Key;
            Player player = item.Value;
            RoomPagePlayerInfo playerInfo = player.IsMasterClient ? this.playerOne : this.playerTwo;
            print($"playerIndex: {playerIndex}, playerName: {player.NickName}, isMe: {player.IsLocal}");
            playerInfo.InitPlayerInfo(player);
        }
        this.switchButon.interactable = PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2;
        this.startButon.interactable = PhotonNetwork.IsMasterClient;
        // this.startButon.interactable = PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2;
    }

    // pun callbacks
    public override void OnJoinedRoom()
    {
        this.roomName.text = $"Room: {PhotonNetwork.CurrentRoom.Name}";
        this.updatePlayerInfos();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        this.updatePlayerInfos();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        this.updatePlayerInfos();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        this.updatePlayerInfos();
    }

    // button callback
    public void ExitRoom()
    {
        PhotonNetwork.LeaveRoom(false);
        this.lobbyPage.Open();
        this.page.Close();
    }

    public void StartGame()
    {
        this.view.RPC("RPC_StartGame", RpcTarget.All);
    }

    public void SwitchRoomMaster()
    {
        Player player = PhotonNetwork.PlayerListOthers[0];
        PhotonNetwork.SetMasterClient(player);
    }

    [PunRPC]
    private void RPC_StartGame()
    {
        GameManager.Instance.StartNewGame();
    }
}
