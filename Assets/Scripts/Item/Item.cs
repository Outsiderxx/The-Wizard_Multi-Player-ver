using UnityEngine;
using Photon.Pun;

public abstract class Item : MonoBehaviour
{
    protected PhotonView view;

    public abstract void Use(CharacterState player);

    private void Awake()
    {
        this.view = this.GetComponent<PhotonView>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.GetComponentInParent<CharacterState>())
        {
            return;
        }
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonView view = other.GetComponentInParent<PhotonView>();
            if (view)
            {
                this.view.RPC("RPC_Use", RpcTarget.All, view.ViewID);
            }
        }
    }
}
