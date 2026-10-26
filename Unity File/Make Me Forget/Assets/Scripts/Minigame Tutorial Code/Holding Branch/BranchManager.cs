using UnityEngine;

public class BranchManager : MonoBehaviour
{
    public GameObject branchImage;

    private bool branchMoved = false;

    public bool HasBranchMoved => branchMoved;

    public void RestoreBranchMoved(bool wasMoved)
    {
        // Restore whether the branch has already fallen.
        branchMoved = wasMoved;
    }
    private void Update()
    {
        // Check whether the birds have been fed.
        // Bool is in the GreenBugDrag script.
        if (GreenBugDrag.birdsFed == true && !branchMoved)
        {
            // Show the branch.
            branchImage.SetActive(true);

            // Get the branch's current position.
            Vector3 position = branchImage.transform.position;

            // Move the branch down by 4 on the Y axis.
            position.y -= 4f;

            // Apply the new position.
            branchImage.transform.position = position;

            // Make sure this only happens once.
            branchMoved = true;
        }
    }
}

