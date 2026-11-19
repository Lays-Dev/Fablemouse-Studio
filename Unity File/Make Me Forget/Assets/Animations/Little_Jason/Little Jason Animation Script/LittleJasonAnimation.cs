using UnityEngine;
using System.Collections;
using TMPro;
using Yarn.Unity;
using FMODUnity;

public class LittleJasonAnimation : MonoBehaviour
{
    public Animator animator;
    [SerializeField] private float facingScale = 3f;
 [SerializeField]   private bool flipArt = false; // if he walks backwards

 [SerializeField] private bool walkWhenMoved =true;
    [SerializeField] private float walkThreshold = 0.05f;

 public DialogueRunner dialogueRunner;
    public TMP_Text  characterNameText;
public string[] jasonNames = { "Jason","Little Jason", "Lil Jason" };

     public EventReference talkSound;
public TMP_Text dialogueText;
     public int lettersPerNote = 2;
public int[] noteScale = { 0, 2, 4,7, 9, 12 };


    private int currentDirection = 1;
 private Vector3 lastPosition;
    private bool forcedWalking = false;
    private bool isCounting=false;
 private bool isFalling = false;
     private bool wasTalking = false;
    private int lastVisibleLetters = 0;
    private int lettersSinceNote= 0;
    private int lastNote = -1;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if(animator == null)
        {
            Debug.LogError("Animator is missing.", this);
        }

        lastPosition = transform.position;
    }


    private void Update()
    {
        if (animator == null) return;

        UpdateTalking();
          UpdateWalking();
    }

    private void UpdateWalking()
    {
        float moveX = Time.deltaTime > 0f ? (transform.position.x - lastPosition.x) / Time.deltaTime : 0f;
        lastPosition = transform.position;

        // counting and falling win over walking
        if (isCounting || isFalling)
        {
            animator.SetFloat("Speed", 0f);
            return;
        }

        bool movingByItself = walkWhenMoved && Mathf.Abs(moveX) > walkThreshold;
        animator.SetFloat("Speed",(forcedWalking || movingByItself) ? 1f : 0f);

        if (movingByItself) {
            FaceDirection(moveX < 0 ? -1 : 1);
        }
    }

    [YarnCommand("jason_walk")]
    public void SetWalking(bool walking)
    {
        forcedWalking = walking;
    }

    [YarnCommand("jason_count")]
    public void SetCounting(bool counting)
    {
        isCounting = counting;
        animator.SetBool("IsCounting",counting);
    }
    public void CountFor(float seconds)
    {
        StartCoroutine(CountForRoutine(seconds));
    }

    private IEnumerator CountForRoutine(float seconds)
    {
        SetCounting(true);
        yield return new WaitForSeconds(seconds);
            SetCounting(false);
    }

    [YarnCommand("jason_fall")]
    public void SetFalling(bool falling)
    {
        isFalling = falling;
        animator.SetBool("IsFalling", falling);
    }

    private void UpdateTalking()
    {
        if (dialogueRunner == null || characterNameText == null) return;

        bool talking = dialogueRunner.IsDialogueRunning
            && characterNameText.gameObject.activeInHierarchy
              && System.Array.IndexOf(jasonNames, characterNameText.text) >= 0;

        if (talking != wasTalking)
        {
            animator.SetBool("IsTalking", talking);
            wasTalking = talking;
        }

        UpdateTalkSound(talking);
    }
    private void UpdateTalkSound(bool talking)
    {
        if (dialogueText == null || talkSound.IsNull) return;

        int visible = Mathf.Min(dialogueText.maxVisibleCharacters, dialogueText.textInfo.characterCount);

        // new line started
        if (visible < lastVisibleLetters)
        {
            lettersSinceNote = lettersPerNote - 1;
        }

        if (talking)
        {
            bool playedThisFrame = false;

            for (int i = lastVisibleLetters; i < visible; i++)
            {
                char letter = dialogueText.textInfo.characterInfo[i].character;
                if (!char.IsLetterOrDigit(letter)) continue;

            lettersSinceNote++;

              // one note per frame so skipping doesnt blast notes
                if (lettersSinceNote >= lettersPerNote && !playedThisFrame)
                {
                    PlayNote();
                    lettersSinceNote = 0;
                     playedThisFrame = true;
                }
            }
        }

        lastVisibleLetters = visible;
    }

    private void PlayNote()
    {
        FMOD.Studio.EventInstance note = RuntimeManager.CreateInstance(talkSound);

        if (noteScale != null && noteScale.Length > 0)
        {
            int pick = Random.Range(0, noteScale.Length);
            if (noteScale.Length > 1 && pick == lastNote)
            {
                pick = (pick + 1) % noteScale.Length;
            }
            lastNote = pick;

            note.setPitch(Mathf.Pow(2f, noteScale[pick] / 12f));
        }

        note.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
        note.start();
        note.release();
    }

    [YarnCommand("jason_face")]
    public void FaceDirection(int direction)
    {
        if (direction == 0 || direction == currentDirection) return;

        currentDirection = direction;
        UpdateFacingDirection();
    }
    private void UpdateFacingDirection()
    {
        int artFlip = flipArt ? -1 : 1;

        transform.localScale = new Vector3(
            facingScale * currentDirection * artFlip,
            transform.localScale.y,
                transform.localScale.z
        );
    }
}