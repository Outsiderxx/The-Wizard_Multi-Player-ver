using UnityEngine;

public class HealthPointItem : Item
{
    [SerializeField] private float recoverQuantity;

    public override void Use(CharacterState player)
    {
        player.Cure(this.recoverQuantity);
        Destroy(this.gameObject);
    }
}
