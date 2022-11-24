using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

[RequireComponent(typeof(CharacterMovementControl))]
public class KeyboardCharaterControl : MonoBehaviour
{
    private CharacterMovementControl movementControl;
    private CharacterSkillSet skillset;
    private CharacterState state;
    private PhotonView view;
    private bool jump;


    void Awake()
    {
        this.movementControl = this.GetComponent<CharacterMovementControl>();
        this.skillset = this.GetComponent<CharacterSkillSet>();
        this.state = this.GetComponent<CharacterState>();
        this.view = this.GetComponent<PhotonView>();

        this.state.OnDead += () =>
        {
            this.enabled = false;
        };
        this.state.OnHurt += () =>
        {
            this.enabled = false;
            Scheduler.ScheduleOnce(() =>
            {
                if (this.state.isAlive)
                {
                    this.enabled = true;
                }
            }, 0.3f);
        };
    }

    void Update()
    {
        if (!view.IsMine)
        {
            return;
        }
        // Read the jump input in Update so button presses aren't missed.
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }

        // check if mouse click on UI
        if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            bool direction = Camera.main.ScreenToWorldPoint(Input.mousePosition).x >= this.transform.position.x;
            this.view.RPC("RPC_NormalAttack", RpcTarget.All, direction);
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = -Camera.main.transform.position.z;
            this.view.RPC("RPC_UseSkillA", RpcTarget.All, Camera.main.ScreenToWorldPoint(mousePosition));
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = -Camera.main.transform.position.z;
            this.view.RPC("RPC_UseSkillB", RpcTarget.All, Camera.main.ScreenToWorldPoint(mousePosition));
        }
        else
        {
            this.view.RPC("RPC_Move", RpcTarget.All, Input.GetAxis("Horizontal"), jump);
            jump = false;
        }
    }

    [PunRPC]
    private void RPC_NormalAttack(bool direction)
    {
        this.skillset.NormalAttack(direction);
    }

    [PunRPC]
    private void RPC_UseSkillA(Vector3 worldPos)
    {
        this.skillset.UseSkillA(worldPos);
    }

    [PunRPC]
    private void RPC_UseSkillB(Vector3 worldPos)
    {
        this.skillset.UseSkillB(worldPos);
    }

    [PunRPC]
    private void RPC_Move(float movement, bool jump)
    {
        this.movementControl.Move(movement, jump);
    }
}
