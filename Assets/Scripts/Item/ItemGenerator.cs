using UnityEngine;
using Photon.Pun;

public class ItemGenerator : MonoBehaviour
{
    [SerializeField] private Vector2[] positions;
    [SerializeField] private GameObject item;

    public void Generate()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        foreach (var position in this.positions)
        {
            Transform transform = PhotonNetwork.InstantiateRoomObject(this.item.name, position, Quaternion.identity).transform;
            transform.parent = this.transform;
            transform.localPosition = position;
        }
    }
}
