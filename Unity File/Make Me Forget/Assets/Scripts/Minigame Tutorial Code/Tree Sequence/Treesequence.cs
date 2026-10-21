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

        // jason n the bird r hiding until the birbs get fed hehe
        if (jason != null) jason.SetActive(false);
        if (bird != null) bird.SetActive(false);
    }

    private void Update()
    {
        // birbs got fed!! show jason n the bird
        // (birdsFed lives in GreenBugDrag btw)
        if (GreenBugDrag.birdsFed == true && !birdsShown)
        {
            if (jason != null) jason.SetActive(true);
            if (bird != null) bird.SetActive(true);
            birdsShown = true;
        }

        // bird said bye and dipped
        if (bird != null && bird.activeSelf && birdFlyToPoint != null)
        {
            bird.transform.position = Vector3.MoveTowards(
                bird.transform.position,
                birdFlyToPoint.position,
                birdFlySpeed * Time.deltaTime
            );

            // poof, bird's gone
            if (bird.transform.position == birdFlyToPoint.position)
            {
                bird.SetActive(false);
            }
        }

        // dude falls down lmao
        if (jasonFalling)
        {
            jason.transform.position = Vector3.MoveTowards(
                jason.transform.position,
                jasonLandPoint.position,
                jasonFallSpeed * Time.deltaTime
            );

            // jason hit the ground, area's unlocked yayy
            if (jason.transform.position == jasonLandPoint.position)
            {
                if (areaBlocker != null) areaBlocker.SetActive(false);
                jasonFalling = false;
            }
        }

        // tree goes wiggle wiggle
        if (wiggleTimer > 0f)
        {
            wiggleTimer -= Time.deltaTime;
            float offset = Mathf.Sin(wiggleTimer * 80f) * wiggleAmount;
            transform.position = treeStartPosition + new Vector3(offset, 0f, 0f);

            if (wiggleTimer <= 0f) transform.position = treeStartPosition;
        }

        // lona needs le branch first
        // (the E press that picks it up doesn't count as a bonk, sneaky)
        bool holding = branchPickup != null && branchPickup.IsHoldingBranch();
        bool canHit = holding && wasHoldingLastFrame && hits < hitsNeeded;
        wasHoldingLastFrame = holding;

        if (!canHit || player == null)
            return;

        // is lona close enough to the tree
        float distance = Vector2.Distance(player.position, transform.position);

        // press E to bonk
        if (distance <= hitDistance &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            HitTree();
        }
    }

    // bonk
    private void HitTree()
    {
        hits++;
        Debug.Log("bonk! " + hits + "/" + hitsNeeded);

        if (wiggleTree) wiggleTimer = 0.25f;

        // third bonk, jason's going down
        if (hits >= hitsNeeded)
        {
            jasonFalling = true;
            Debug.Log("jason fell lmao");
        }
    }
}