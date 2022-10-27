using UnityEngine;
using UnityEngine.UI;

public class CollectionPage : Page
{
    [SerializeField] private Text[] collectionCollectedStatusTexts;

    protected override void Awake()
    {
        base.Awake();
        this.OnOpen += this.UpdateCollectionState;
    }

    private void UpdateCollectionState()
    {
        for (int i = 0; i < CollectionHelper.collectionNames.Length; i++)
        {
            bool isCollected = CollectionHelper.IsCollectionCollected(CollectionHelper.collectionNames[i]);
            this.collectionCollectedStatusTexts[i].text = isCollected ? "COLLECTED" : "UNCOLLECTED";
            this.collectionCollectedStatusTexts[i].color = isCollected ? Color.red : new Color(0.55f, 0.55f, 0.55f);
        }
    }
}
