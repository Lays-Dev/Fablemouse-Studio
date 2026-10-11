using UnityEngine;
using UnityEngine.InputSystem;

public class Live2DAnimationController : MonoBehaviour
{
    public Animator animator;

    private PlayerInput playerInput;
    private InputAction moveAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
    }

    private void Update()
    {
        float horizontalInput = moveAction.ReadValue<Vector2>().x;

        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
    }
}