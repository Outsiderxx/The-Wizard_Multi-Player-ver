using UnityEngine;
using Photon.Pun;

public class RingOfWizard : Item
{
    public override void Use(CharacterState player)
    {
        if (player.isRingOfWizardCollected)
        {
            return;
        }
        AudioManager.Instance.PlayEffect("Item");
        CollectionHelper.CollectCollection("Ring of Wizard");
        player.maxMagicPoints *= 1.5f;
        player.currentMagicPoints *= 1.5f;
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
