using UnityEngine;
using Photon.Pun;

public class RingOfStrength : Item
{
    public override void Use(CharacterState player)
    {
        if (player.isRingOfStrengthCollected)
        {
            return;
        }
        AudioManager.Instance.PlayEffect("Item");
        CollectionHelper.CollectCollection("Ring of Strength");
        player.GetComponent<CharacterEffect>().ShowColorTransition(new Color(0.84f, 0.6f, 0.035f), 3);
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
