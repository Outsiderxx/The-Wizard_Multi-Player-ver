using UnityEngine;
using Photon.Pun;

public class ManaPointItem : Item
{
    [SerializeField] private float recoverQuantity;

    public override void Use(CharacterState player)
    {
        AudioManager.Instance.PlayEffect("Item");
        player.RecoverMana(this.recoverQuantity);
        player.GetComponent<CharacterEffect>().ShowColorTransition(Color.cyan, 1);
        if (this.view.IsMine)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }

    [PunRPC]
    private void RPC_Use(int viewID)
    {
        this.Use(PhotonView.Find(viewID).GetComponent<CharacterState>());
    }
}
