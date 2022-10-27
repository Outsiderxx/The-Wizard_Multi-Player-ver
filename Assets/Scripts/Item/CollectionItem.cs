using UnityEngine;

public class CollectionItem : Item
{
    [SerializeField] private string _collectionName;

    public string collectionName
    {
        get
        {
            return this._collectionName;
        }
    }

    private void Awake()
    {
        if (CollectionHelper.IsCollectionCollected(this.collectionName))
        {
            Destroy(this.gameObject);
        }
    }

    public override void Use(CharacterState player)
    {
        player.AcquireCollection(this);
        Destroy(this.gameObject);
    }
}
