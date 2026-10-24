using UnityEngine;
using UnityEngine.InputSystem;

public class BranchPickup : MonoBehaviour
{
    [Header("Branch")]
    public GameObject branchObject;

    [Header("Player")]
    public GameObject heldBranch;
    public Transform player;

    [Header("Pickup Settings")]
    public float pickupDistance = 3f;

    private bool canPickUp = false;
    private bool isHoldingBranch = false;

    private void Update()
    {
        // Only pick up the branch when it is available.
        if (branchObject == null || !branchObject.activeInHierarchy)
            return;

        // Check whether the player is close enough.
        if (player != null)
        {
            float distance = Vector2.Distance(
                player.transform.position,
                branchObject.transform.position
            );

            canPickUp = distance <= pickupDistance;
        }

        // Press E to pick up the branch.
        if (canPickUp &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            PickUpBranch();
        }
    }

    private void PickUpBranch()
    {
        // Hide the branch from the ground.
        branchObject.SetActive(false);

        // Show the branch the player is holding.
        if (heldBranch != null)
        {
            heldBranch.SetActive(true);
        }

        isHoldingBranch = true;
        canPickUp = false;

        Debug.Log("Player picked up the branch!");
    }

    public bool IsHoldingBranch()
    {
        return isHoldingBranch;
    }
}
