using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    public float movementSpeed = 2.0f;
    public float sprintSpeed = 5.0f;
    public float rotationSpeed = 0.2f;
    public float animationBlendSpeed = 0.2f;
    public float jumpSpeed = 7.0f;

    CharacterController controller;
    Animator animator;
    Camera characterCamera;
    float rotationAngle = 0.0f;
    float targetAnimationSpeed = 0.0f;
    bool isSprint = false;

    float speedY = 0.0f;
    float gravity = -9.81f;
    public bool isJumping = false;
    public bool isActiveMove = false;
    public bool isAttaking = false;
    public bool isDeath = false;

    public CharacterController Controller
    {
        get { return controller = controller ?? GetComponent<CharacterController>(); }
    }
    public Camera CharacterCamera
    {
        get { return characterCamera = characterCamera ?? FindObjectOfType<Camera>(); }
    }
    public Animator CharacterAnimator
    {
        get { return animator = animator ?? FindObjectOfType<Animator>(); }
    }

    private void Start()
    {
        CharacterAnimator.Play("Spawn");
    }
    private void Update()
    {
        Debug.Log(Controller.isGrounded);
        MoveAndRotation();
        Jump();
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(isJumping)
            {
                return;
            }
            isAttaking = true;
            isActiveMove = false;
            CharacterAnimator.SetTrigger("Death");
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            isActiveMove = false;
            isAttaking = true;
            CharacterAnimator.SetTrigger("Spawn");
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(isAttaking)
            {
                return;
            }
            int numberAttack = Random.Range(0, 3);
            CharacterAnimator.SetInteger("RandAttack", numberAttack);
        }

    }

    public void MoveAndRotation()
    {
        if(isActiveMove)
        {
            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");

            isSprint = Input.GetKey(KeyCode.LeftShift);
            Vector3 movement = new Vector3(horizontal, 0.0f, vertical);
            Vector3 rotatedMovement = Quaternion.Euler(0.0f, CharacterCamera.transform.rotation.eulerAngles.y, 0.0f) * movement.normalized;
            Vector3 verticalMovement = Vector3.up * speedY;

            float currentSpeed = isSprint ? sprintSpeed : movementSpeed;
            Controller.Move((verticalMovement + rotatedMovement * currentSpeed) * Time.deltaTime);

            if (rotatedMovement.sqrMagnitude > 0.0f)
            {
                rotationAngle = Mathf.Atan2(rotatedMovement.x, rotatedMovement.z) * Mathf.Rad2Deg;
                targetAnimationSpeed = isSprint ? 1.0f : 0.5f;
            }
            else
            {
                targetAnimationSpeed = 0.0f;
            }
            CharacterAnimator.SetFloat("Speed", Mathf.Lerp(CharacterAnimator.GetFloat("Speed"), targetAnimationSpeed, animationBlendSpeed));
            Quaternion currentRotation = Controller.transform.rotation;
            Quaternion targetRotation = Quaternion.Euler(0.0f, rotationAngle, 0.0f);
            Controller.transform.rotation = Quaternion.Lerp(currentRotation, targetRotation, rotationSpeed);
        }
    }
    public void Jump()
    {

        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            isAttaking = false;
            isJumping = true;
            CharacterAnimator.SetTrigger("Jump");
            speedY += jumpSpeed;
        }
        if (!Controller.isGrounded)
        {
            speedY += gravity * Time.deltaTime;
        }
        else if (speedY < 0.0f)
        {
            speedY = 0.0f;
        }
        CharacterAnimator.SetFloat("SpeedY", speedY / jumpSpeed);
        if (isJumping && speedY < 0.0f)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 1f, LayerMask.GetMask("Default")))
            {
                isAttaking = false;
                isJumping = false;
                CharacterAnimator.SetTrigger("Land");
            }
        }
    }
    public void MeleeAttackStart()
    {
        isActiveMove = false;
        isAttaking = true;
        Debug.Log("MeleeAttackStart");
    }    
    public void MeleeAttackEnd()
    {
        CanMove();
        isAttaking = false;
        Debug.Log("MeleeAttackEnd");
    }
    public void CanMove()
    {
        isActiveMove = true;
        isAttaking = false;
    }

}
