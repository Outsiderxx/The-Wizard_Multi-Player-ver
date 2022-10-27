using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CharacterMovementControl))]
public class KeyboardCharaterControl : MonoBehaviour
{
    private CharacterMovementControl movementControl;
    private CharacterSkillSet skillset;
    private CharacterState state;
    private bool jump;


    void Awake()
    {
        this.movementControl = this.GetComponent<CharacterMovementControl>();
        this.skillset = this.GetComponent<CharacterSkillSet>();
        this.state = this.GetComponent<CharacterState>();

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
        // Read the jump input in Update so button presses aren't missed.
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }

        // check if mouse click on UI
        if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            this.skillset.NormalAttack();
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            this.skillset.UseSkillA();
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            this.skillset.UseSkillB();
        }
        else
        {
            movementControl.Move(Input.GetAxis("Horizontal"), jump);
            jump = false;
        }
    }
}
