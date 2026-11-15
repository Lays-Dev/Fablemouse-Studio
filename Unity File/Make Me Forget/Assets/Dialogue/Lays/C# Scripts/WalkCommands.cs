using System.Collections;
using UnityEngine;
using Yarn.Unity;

public class WalkCommands : MonoBehaviour
{
    public DialogueRunner dialogueRunner;
    public Transform[] walkers;   // who shall be le walker
    public Transform[] spots;     // where they walk to
    public float speed = 2f;
    public string walkAnimParameter = "isWalking";   // the walk parameter in their Animator
    public Transform[] useAnyGround;   // walkers here follow any ground with a collider, not just terrain (JasonHospital)

    void Awake()
    {
        // this is for yarn
        dialogueRunner.AddCommandHandler<string, string>("walk", Walk);
    }

    // public so cutscenes can start a walk too
    public Coroutine Walk(string who, string where)
    {
        return StartCoroutine(WalkTo(Find(walkers, who), Find(spots, where)));
    }

    Transform Find(Transform[] list, string objectName)
    {
        foreach (Transform t in list)
        {
            if (t.name == objectName) return t;
        }

        Debug.LogError("Walk: couldn't find " + objectName);
        return null;
    }

    IEnumerator WalkTo(Transform who, Transform spot)
    {
        if (who == null || spot == null) yield break;

        // turn off physics while walking so she stops bouncing
        Rigidbody rb = who.GetComponent<Rigidbody>();
        bool wasKinematic = false;
        if (rb != null)
        {
            wasKinematic = rb.isKinematic;
            rb.isKinematic = true;
        }

        // turn them to face where they're walking
        Live2DAnimationController facing = who.GetComponent<Live2DAnimationController>();
        if (facing != null)
            facing.FaceDirection(spot.position.x < who.position.x ? -1 : 1);

        // start the walk animation
        Animator anim = who.GetComponentInChildren<Animator>();
        SetWalking(anim, true);

        // only the walkers in the Use Any Ground list use the new ground check
        bool anyGround = System.Array.IndexOf(useAnyGround, who) >= 0;

        // how high above the ground they are, so they follow the terrain
        float heightAboveGround = who.position.y - GroundHeight(who.position, who, anyGround);

        Vector2 goal = new Vector2(spot.position.x, spot.position.z);
        float timer = 0f;

        // gives up after 10 sec so the dialogue never gets stuck
        while (timer < 10f)
        {
            Vector3 pos = who.position;
            Vector2 flat = new Vector2(pos.x, pos.z);

            if (Vector2.Distance(flat, goal) < 0.05f) break;

            flat = Vector2.MoveTowards(flat, goal, speed * Time.deltaTime);

            // walk on the ground instead of levitating
            Vector3 newPos = new Vector3(flat.x, pos.y, flat.y);
            newPos.y = GroundHeight(newPos, who, anyGround) + heightAboveGround;
            who.position = newPos;

            timer += Time.deltaTime;
            yield return null;
        }

        // stop the walk animation
        SetWalking(anim, false);

        // physics back on
        if (rb != null) rb.isKinematic = wasKinematic;
    }

    float GroundHeight(Vector3 pos, Transform who, bool anyGround)
    {
        // looks straight down for any ground with a collider (paths, floors, etc.)
        if (anyGround)
        {
            RaycastHit[] hits = Physics.RaycastAll(pos + Vector3.up * 10f, Vector3.down, 50f, ~0, QueryTriggerInteraction.Ignore);
            float best = float.NegativeInfinity;

            foreach (RaycastHit hit in hits)
            {
                // skip their own body, and anything above their head
                if (hit.transform.IsChildOf(who)) continue;
                if (hit.point.y > best && hit.point.y <= pos.y + 2f) best = hit.point.y;
            }

            if (best != float.NegativeInfinity) return best;
        }

        // same as before (Lona uses this)
        Terrain terrain = Terrain.activeTerrain;
        if (terrain == null) return pos.y;

        return terrain.SampleHeight(pos) + terrain.transform.position.y;
    }

    void SetWalking(Animator anim, bool walking)
    {
        if (anim == null || string.IsNullOrEmpty(walkAnimParameter)) return;

        foreach (AnimatorControllerParameter p in anim.parameters)
        {
            if (p.name != walkAnimParameter) continue;

            // works if the parameter is a Bool or a Float (like Speed)
            if (p.type == AnimatorControllerParameterType.Bool)
                anim.SetBool(walkAnimParameter, walking);
            else if (p.type == AnimatorControllerParameterType.Float)
                anim.SetFloat(walkAnimParameter, walking ? 1f : 0f);

            return;
        }
    }
}