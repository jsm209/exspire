using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private enum State
    {
        Normal,
        Rolling,
        Hurt
    }


    #region public variables
    [Header("Movement Parameters")]
    public float moveSpeed = 5f; // Speed at which to move at.
    public float rollSpeed = 250f; // Speed at which to roll at.

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
    #endregion

    void Awake()
    {
        state = State.Normal;
    }

    void Update()
    {
        switch (state)
        {
            // When we're in a normal state, we want to acknowledge inputs.
            case State.Normal:
                moveDir.x = Input.GetAxisRaw("Horizontal");
                moveDir.y = Input.GetAxisRaw("Vertical");
                if (moveDir.x != 0 || moveDir.y != 0) {
                    tempMoveDir = moveDir;
                }


                animator.SetFloat("Horizontal", moveDir.x);
                animator.SetFloat("Vertical", moveDir.y);
                animator.SetFloat("Speed", moveDir.sqrMagnitude);

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    rollDir = tempMoveDir;
                    tempRollSpeed = rollSpeed;
                    state = State.Rolling;
                }

                break;

            // When we're rolling, we do not want to take in any inputs.
            case State.Rolling:

                // Reduce the roll speed over time.
                float rollSpeedDropMultiplier = 5f;
                tempRollSpeed -= tempRollSpeed * rollSpeedDropMultiplier * Time.deltaTime;

                // If our rolling speed reduces below a threshold, break out of the roll state.
                float rollSpeedMinimum = 50f;
                if (tempRollSpeed < rollSpeedMinimum)
                {
                    state = State.Normal;
                }

                break;

        }




    }

    void FixedUpdate()
    {
        switch (state)
        {
            // If we're in a normal state, move normally.
            case State.Normal:
                rb.velocity = moveDir.normalized * moveSpeed;

                break;

            // If we're in a rolling state, the velocity is the roll.
            case State.Rolling:
                rb.velocity = rollDir * tempRollSpeed;
                break;
        }

    }
}
