using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;
    private int _playerCount = 0;

    private int playerCount
    {
        get
        {
            return this._playerCount;
        }
        set
        {
            if (this._playerCount == value)
            {
                return;
            }
            this._playerCount = value;
            print($"player count {value}");
            if (value == this.levelManager.alivePlayerCount)
            {
                this.levelManager.GoToNextStage();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<CharacterState>())
        {
            playerCount++;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponentInParent<CharacterState>())
        {
            playerCount--;
        }
    }
}
