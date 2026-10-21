using UnityEngine;
using UnityEngine.InputSystem;

public class TreeSequence : MonoBehaviour
{
    [Header("Branch")]
    public BranchPickup branchPickup;

    [Header("Player")]
    public Transform player;
    public float hitDistance = 2f;
    public int hitsNeeded = 3;

    [Header("Jason")]
    public GameObject jason;
    public Transform jasonLandPoint;
    public float jasonFallSpeed = 8f;

    [Header("Bird")]
    public GameObject bird;
    public Transform birdFlyToPoint;
    public float birdFlySpeed = 6f;

    [Header("Blocked Area")]
    public GameObject areaBlocker;

    [Header("Tree Wiggle")]
    public bool wiggleTree = true;
    public float wiggleAmount = 0.1f;

    private int hits = 0;
    private bool birdsShown = false;
    private bool jasonFalling = false;
    private bool wasHoldingLastFrame = false;
    private float wiggleTimer = 0f;
    private Vector3 treeStartPosition;

    private void Start()
    {
        treeStartPosition = transform.position;

        // Hide Jason and the bird until the birds are fed.
        if (jason != null) jason.SetActive(false);
        if (bird != null) bird.SetActive(false);
    }

    private void Update()
    {
        // When the birds are fed, show Jason and the bird.
        // Bool is in the GreenBugDrag script.
        if (GreenBugDrag.birdsFed == true && !birdsShown)
        {
            if (jason != null) jason.SetActive(true);
            if (bird != null) bird.SetActive(true);
            birdsShown = true;
        }

        // Bird flies away.
        if (bird != null && bird.activeSelf && birdFlyToPoint != null)
        {
            bird.transform.position = Vector3.MoveTowards(
                bird.transform.position,
                birdFlyToPoint.position,
                birdFlySpeed * Time.deltaTime
            );

            // Hide the bird once it gets there.
            if (bird.transform.position == birdFlyToPoint.position)
            {
                bird.SetActive(false);
            }
        }

        // Jason falls down.
        if (jasonFalling)
        {
            jason.transform.position = Vector3.MoveTowards(
                jason.transform.position,
                jasonLandPoint.position,
                jasonFallSpeed * Time.deltaTime
            );

            // When Jason lands, unlock the area.
            if (jason.transform.position == jasonLandPoint.position)
            {
                if (areaBlocker != null) areaBlocker.SetActive(false);
                jasonFalling = false;
            }
        }

        // Tree wiggles after each hit.
        if (wiggleTimer > 0f)
        {
            wiggleTimer -= Time.deltaTime;
            float offset = Mathf.Sin(wiggleTimer * 80f) * wiggleAmount;
            transform.position = treeStartPosition + new Vector3(offset, 0f, 0f);

            if (wiggleTimer <= 0f) transform.position = treeStartPosition;
        }

        // Only allow hits if the player was already holding the branch last frame.
        // (So the E press that picks up the branch doesn't count as a hit.)
        bool holding = branchPickup != null && branchPickup.IsHoldingBranch();
        bool canHit = holding && wasHoldingLastFrame && hits < hitsNeeded;
        wasHoldingLastFrame = holding;

        if (!canHit || player == null)
            return;

        // Check whether the player is close enough to the tree.
        float distance = Vector2.Distance(player.position, transform.position);

        // Press E to hit the tree.
        if (distance <= hitDistance &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            HitTree();
        }
    }

    private void HitTree()
    {
        hits++;
        Debug.Log("Hit the tree! " + hits + "/" + hitsNeeded);

        if (wiggleTree) wiggleTimer = 0.25f;

        // Third hit makes Jason fall.
        if (hits >= hitsNeeded)
        {
            jasonFalling = true;
            Debug.Log("Jason fell!");
        }
    }
}