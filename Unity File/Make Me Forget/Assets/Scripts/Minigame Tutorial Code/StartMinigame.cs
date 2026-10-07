using UnityEngine;
using UnityEngine.InputSystem;

public class StartMinigame : MonoBehaviour
{

    // This will be the interactable tree that starts the minigame
    // Start by pressing E
    // Assign the "Tutorial Minigame Canvas" in the inspector 

    [Tooltip("Assign the Tutorial Minigame Canvas here")]
    public GameObject canvas;

    private bool playerInRange;
    private InputAction interactAction;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            PlayerInput playerInput = other.GetComponentInParent<PlayerInput>();

            if (playerInput != null)
            {
                interactAction = playerInput.actions
                    .FindActionMap("Player")
                    .FindAction("Interact");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            interactAction = null;
        }
    }

    private void Update()
    {
        if (playerInRange && interactAction != null)
        {
            if (interactAction.WasPressedThisFrame())
            {
                if (TutorialMinigameScript.foundJason == false)
                {
                    canvas.SetActive(true);
                }
                else
                {
                    Debug.Log("Jason has already been found. Minigame will not start.");
                }
            }
        }
    }


}