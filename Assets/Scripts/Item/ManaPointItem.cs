using UnityEngine;

public class ManaPointItem : Item
{
    [SerializeField] private float recoverQuantity;

    public override void Use(CharacterState player)
    {
        player.RecoverMana(this.recoverQuantity);
        Destroy(this.gameObject);
    }
}
