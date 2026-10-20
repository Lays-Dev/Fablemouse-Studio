using System.Collections;
using UnityEngine;
using UnityEngine.Events;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class TreeSequence : MonoBehaviour
{
    // which mini game sets this off
    [Header("Mini Game")]
    public MiniGamePopup popup;
    public int triggerPartIndex = 1;

    // lona stuff
    [Header("Lona")]
    public Transform lona;
    public Transform lonaHand;
    public Animator lonaAnimator;

    // the branch
    [Header("Branch")]
    public Transform branch;
    public Transform branchLandPoint;
    public Vector3 holdOffset = Vector3.zero;
    public float holdRotationZ = -30f;
    public int heldSortingOrder = 10;

    // the dude in the tree
    [Header("Jason")]
    public Transform jason;
    public Transform jasonLandPoint;
    public Animator jasonAnimator;

    // the bird that knocks the branch down
    [Header("Bird")]
    public Transform bird;
    public Transform birdFlyToPoint;
    public Animator birdAnimator;
    public float birdFlySpeed = 6f;
    public float branchDelay = 0f;
    public bool hideBirdWhenGone = true;
    public bool hideBranchUntilFall = true;
    public bool hideBirdUntilStart = false;
    public bool hideJasonUntilStart = false;

    // buttons and numbers
    [Header("Rules")]
    public KeyCode pickUpKey = KeyCode.E;
    public KeyCode hitKey = KeyCode.E;
    public float pickUpRange = 1.5f;
    public float hitRange = 2f;
    public int hitsNeeded = 3;

    // how fast stuff falls (bigger = faster)
    [Header("Falling")]
    public float branchFallSpeed = 25f;
    public float branchSpinSpeed = 360f;
    public float jasonFallSpeed = 25f;

    // tree wiggle settings (untick to turn it off)
    [Header("Tree Wiggle")]
    public bool wiggleTree = true;
    public float wiggleTime = 0.25f;
    public float wiggleAmount = 0.08f;
    public float wiggleSpeed = 80f;

    // extra stuff for sounds and dialogue
    [Header("Events")]
    public UnityEvent onBirdFlewAway;
    public UnityEvent onBranchFell;
    public UnityEvent onBranchPickedUp;
    public UnityEvent onTreeHit;
    public UnityEvent onJasonFell;

    enum State { Waiting, BranchFalling, BranchOnGround, Holding, Busy, Done }
    State state = State.Waiting;
    int hits;
    Vector3 treeStartPos;

    // branch is invisible at the start (if you want)
    void Start()
    {
        treeStartPos = transform.position;
        if (branch && hideBranchUntilFall) branch.gameObject.SetActive(false);
        if (bird && hideBirdUntilStart) bird.gameObject.SetActive(false);
        if (jason && hideJasonUntilStart) jason.gameObject.SetActive(false);

        // wait for the right mini game to be beaten
        if (popup) popup.onCompleted.AddListener(OnMiniGameDone);
    }

    void OnDestroy()
    {
        if (popup) popup.onCompleted.RemoveListener(OnMiniGameDone);
    }

    void OnMiniGameDone()
    {
        if (popup.CurrentPart == triggerPartIndex) StartSequence();
    }

    // mini game 2 is done, let's go
    [ContextMenu("Start Sequence")]
    public void StartSequence()
    {
        if (state != State.Waiting) return;
        state = State.BranchFalling;

        // surprise, they were here all along
        if (jason) jason.gameObject.SetActive(true);
        if (bird) bird.gameObject.SetActive(true);

        if (bird) StartCoroutine(BirdFlyAway());
        StartCoroutine(BranchFall());
    }

    // birb yeets
    IEnumerator BirdFlyAway()
    {
        if (birdAnimator) birdAnimator.SetTrigger("Fly");

        // face the way it's flying
        var sr = bird.GetComponent<SpriteRenderer>();
        if (sr && birdFlyToPoint) sr.flipX = birdFlyToPoint.position.x < bird.position.x;

        // flap flap flap
        float speed = 0f;
        while (birdFlyToPoint && Vector2.Distance(bird.position, birdFlyToPoint.position) > 0.05f)
        {
            speed = Mathf.MoveTowards(speed, birdFlySpeed, birdFlySpeed * 2f * Time.deltaTime);
            bird.position = Vector3.MoveTowards(bird.position, birdFlyToPoint.position, speed * Time.deltaTime);
            yield return null;
        }

        // bye bird
        if (hideBirdWhenGone) bird.gameObject.SetActive(false);
        onBirdFlewAway.Invoke();
    }

    // checking for button presses
    void Update()
    {
        // you pick up le branch
        if (state == State.BranchOnGround && Pressed(pickUpKey) && Near(branch, pickUpRange))
        {
            PickUpBranch();
            return;
        }

        // bonk the tree
        if (state == State.Holding && Pressed(hitKey) && Near(transform, hitRange))
        {
            HitTree();
        }
    }

    // works w the old AND new input system
    bool Pressed(KeyCode key)
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && System.Enum.TryParse(key.ToString(), out Key k)
            && Keyboard.current[k].wasPressedThisFrame) return true;
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(key)) return true;
#endif
        return false;
    }

    // is lona close enough
    bool Near(Transform t, float range)
    {
        Vector2 a = lona.position, b = t.position;
        return Vector2.Distance(a, b) <= range;
    }

    // branch falls from the tree
    IEnumerator BranchFall()
    {
        // give the bird a sec to take off
        if (branchDelay > 0f) yield return new WaitForSeconds(branchDelay);
        branch.gameObject.SetActive(true);
        yield return Drop(branch, branchLandPoint.position, branchFallSpeed, branchSpinSpeed);
        state = State.BranchOnGround;
        onBranchFell.Invoke();
    }

    // branch goes in lona's hand
    void PickUpBranch()
    {
        var rb2d = branch.GetComponent<Rigidbody2D>(); if (rb2d) rb2d.simulated = false;
        var rb = branch.GetComponent<Rigidbody>(); if (rb) rb.isKinematic = true;

        branch.SetParent(lonaHand, worldPositionStays: false);
        branch.localPosition = holdOffset;
        branch.localRotation = Quaternion.Euler(0, 0, holdRotationZ);

        var sr = branch.GetComponent<SpriteRenderer>();
        if (sr) sr.sortingOrder = heldSortingOrder;

        state = State.Holding;
        onBranchPickedUp.Invoke();
    }

    // bonk
    void HitTree()
    {
        hits++;
        if (lonaAnimator) lonaAnimator.SetTrigger("Swing");
        onTreeHit.Invoke();
        if (wiggleTree) StartCoroutine(Shake());

        // third bonk, dude is going down
        if (hits >= hitsNeeded) StartCoroutine(JasonFall());
    }

    // tree goes wiggle
    IEnumerator Shake()
    {
        float t = 0f;
        while (t < wiggleTime)
        {
            t += Time.deltaTime;
            transform.position = treeStartPos + new Vector3(Mathf.Sin(t * wiggleSpeed) * wiggleAmount, 0, 0);
            yield return null;
        }
        transform.position = treeStartPos;
    }

    // dude falls down
    IEnumerator JasonFall()
    {
        state = State.Busy;
        yield return new WaitForSeconds(0.3f);
        if (jasonAnimator) jasonAnimator.SetTrigger("Fall");
        yield return Drop(jason, jasonLandPoint.position, jasonFallSpeed, 0f);
        state = State.Done;
        onJasonFell.Invoke();
    }

    // gravity but homemade
    IEnumerator Drop(Transform obj, Vector3 target, float gravity, float spinSpeed)
    {
        bool spin = spinSpeed != 0f;
        float speed = 0f;
        Vector3 start = obj.position;
        float totalDrop = Mathf.Max(0.01f, start.y - target.y);

        while (obj.position.y > target.y)
        {
            speed += gravity * Time.deltaTime;
            Vector3 p = obj.position;
            p.y -= speed * Time.deltaTime;
            float progress = Mathf.Clamp01((start.y - p.y) / totalDrop);
            p.x = Mathf.Lerp(start.x, target.x, progress);
            obj.position = p;
            if (spin) obj.Rotate(0, 0, spinSpeed * Time.deltaTime);
            yield return null;
        }

        // splat
        obj.position = target;
        if (spin) obj.rotation = Quaternion.Euler(0, 0, 90f);
    }

    // circles in the editor so you can see the ranges
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hitRange);
        if (branchLandPoint) { Gizmos.color = Color.green; Gizmos.DrawWireSphere(branchLandPoint.position, pickUpRange); }
        if (bird && birdFlyToPoint) { Gizmos.color = Color.cyan; Gizmos.DrawLine(bird.position, birdFlyToPoint.position); }
    }
}