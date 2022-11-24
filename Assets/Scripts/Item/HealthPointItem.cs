using UnityEngine;
using Photon.Pun;

public class HealthPointItem : Item
{
    [SerializeField] private float recoverQuantity;

    public override void Use(CharacterState player)
    {
        AudioManager.Instance.PlayEffect("Item");
        player.Cure(this.recoverQuantity);
        player.GetComponent<CharacterEffect>().ShowColorTransition(Color.red, 1);
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
