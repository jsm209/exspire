using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private enum State
    {
        Normal,
        Rolling,
        Hurt,
        Disabled,
        Dead
    }


    #region public variables
    public Camera cam; // Camera to use in order to reference click positions
    public SpriteRenderer spriteRenderer; // Sprite Renderer

    private float moveSpeed = 0f; // Speed at which to move at.
    private float rollSpeed = 0f; // Speed at which zto roll at.

    [Header("Other")]
    public Rigidbody2D rb;
    public Animator animator;
    #endregion

    #region private variables
    private Vector3 moveDir; // Direction to move.
    private Vector3 rollDir; // Direction to roll.
    private Vector3 tempMoveDir; // Last known move direction. Used for direction of stationary rolls.
    private float tempRollSpeed; // Current rolling speed.
    private State state; // Current player state.
    private Rigidbody2D body; // Will refer to "this" object's rigidbody
    private Vector2 mousePos; // Current mouse position.
    private float disabledTimeLeft; // How much time left before we can leave the disabled state.
    #endregion

    void Awake()
    {
        state = State.Normal;
        body = GetComponent<Rigidbody2D>();
        moveSpeed = GameObject.FindWithTag("Player").GetComponent<Player>().moveSpeed;
        rollSpeed = GameObject.FindWithTag("Player").GetComponent<Player>().rollSpeed;
        spriteRenderer = this.GetComponent<SpriteRenderer>();
    }


    void Update()
    {
        // Flips the character depending on movement.
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            spriteRenderer.flipX = false; // face right
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            spriteRenderer.flipX = true; // face left
        }

        switch (state)
        {
            // When we're in a normal state, we want to acknowledge inputs.
            case State.Normal:
                moveDir.x = Input.GetAxisRaw("Horizontal");
                moveDir.y = Input.GetAxisRaw("Vertical");
                if (moveDir.x != 0 || moveDir.y != 0)
                {
                    tempMoveDir = moveDir;
                }


                animator.SetFloat("Horizontal", moveDir.x);
                animator.SetFloat("Vertical", moveDir.y);
                animator.SetFloat("Speed", moveDir.sqrMagnitude);

                if (Input.GetKeyDown(KeyCode.Mouse1) && tryToSpendKindles(1))
                {
                    //rollDir = tempMoveDir;
                    mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 lookDirection = (mousePos - body.position);
                    lookDirection.Normalize();

                    rollDir = lookDirection;
                    tempRollSpeed = rollSpeed;
                    state = State.Rolling;


                    // Checks abilities
                    int[] inventory = this.GetComponent<Player>().inventory;

                    // FAULTY BOOSTER ABILITY
                    if (inventory[8] > 0)
                    {
                        for (int i = 0; i < inventory[8]; i++)
                        {
                            this.GetComponentInChildren<PlayerAbilities>().ShootRandomDirection();
                        }
                    }

                    // FORTRESS BELT ABILITY
                    if (inventory[9] > 0)
                    {
                        this.GetComponentInChildren<PlayerAbilities>().Shield();
                    }
                }



                break;

            // When we're rolling, we do not want to take in any inputs.
            case State.Rolling:

                // Player is invulnerable while rolling
                Physics2D.IgnoreLayerCollision(9, 10, true); // Ignore enemy projectiles
                Physics2D.IgnoreLayerCollision(9, 11, true); // Ignore enemies

                this.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);

                // Reduce the roll speed over time.
                float rollSpeedDropMultiplier = 5f;
                tempRollSpeed -= tempRollSpeed * rollSpeedDropMultiplier * Time.deltaTime;

                // If our rolling speed reduces below a threshold, break out of the roll state.
                float rollSpeedMinimum = 50f;
                if (tempRollSpeed < rollSpeedMinimum)
                {
                    // Player is vulnerable while normal
                    Physics2D.IgnoreLayerCollision(9, 10, false); // Ignore enemy projectiles
                    Physics2D.IgnoreLayerCollision(9, 11, false); // Ignore enemies

                    this.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                    state = State.Normal;
                }

                break;

            // When we're disabled, we want to stop all movvement input but still take collision input.
            case State.Disabled:

                // If the player is disabled for longer than 600 seconds or 10 minutes
                // assume the player is dead, and don't subtract time.
                if (disabledTimeLeft < 600f) {
                    disabledTimeLeft -= Time.deltaTime;
                }
                
                if (disabledTimeLeft <= 0.0f)
                {
                    state = State.Normal;
                }

                break;

            // When we're dead, we want to ignore all input and collisions.
            case State.Dead:

                Physics2D.IgnoreLayerCollision(9, 10, true); // Ignore enemy projectiles
                Physics2D.IgnoreLayerCollision(9, 11, true); // Ignore enemies

                break;

        }




    }

    void FixedUpdate()
    {
        switch (state)
        {
            // If we're in a normal state, move normally.
            case State.Normal:
                rb.AddForce(moveDir.normalized * moveSpeed);
                break;

            // If we're in a rolling state, the velocity is the roll.
            case State.Rolling:
                rb.velocity = rollDir * tempRollSpeed;
                break;

            // If we're disabled or dead, don't move.
            case State.Disabled:
            case State.Dead:
                rb.velocity = Vector2.zero;
                animator.SetFloat("Horizontal", 0f);
                animator.SetFloat("Vertical", 0f);
                animator.SetFloat("Speed", 0f);
                break;
        }

    }

    // Will make the player disabled for the given amount of seconds.
    // Disabled means they cannot move.
    // if disabled time is over 600f, it will disable the player indefinitely.
    public void disable(float seconds)
    {
        state = State.Disabled;
        disabledTimeLeft = seconds;
    }

    public void setMoveSpeed(float newSpeed)
    {
        this.moveSpeed = newSpeed;
    }

    public void setRollSpeed(float newSpeed)
    {
        this.rollSpeed = newSpeed;
    }

    void resetState()
    {
        state = State.Normal;
    }


    // Will check if the player has enough kindles to perform the ability
    // and if so, will subtract it and return true. Else false.
    bool tryToSpendKindles(int n)
    {
        if (GetComponent<Player>().kindles >= n)
        {
            GetComponent<Player>().addKindles(-n);
            return true;
        }
        return false;
    }

    public string getState()
    {
        switch (state)
        {
            case State.Normal:
                return "Normal";
                break;

            case State.Rolling:
                return "Rolling";
                break;

            case State.Disabled:
                return "Disabled";
                break;

            case State.Dead:
                return "Dead";
                break;

        }

        return null;
    }

    public void setState(string updatedState) {
        switch (updatedState.ToUpper()) {
            case "NORMAL":
                state = State.Normal;
                break;
            case "ROLLING":
                state = State.Rolling;
                break;
            case "DISABLED":
                state = State.Disabled;
                break;
            case "DEAD":
                state = State.Dead;
                break;
        }
    }
}
