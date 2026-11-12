using System.Collections;
using UnityEngine;
using Yarn.Unity;

public class WalkCommands : MonoBehaviour
{
    public DialogueRunner dialogueRunner;
    public Transform[] walkers;   // who shall be le walker
    public Transform[] spots;     // where they walk to
    public float speed = 2f;

    void Awake()
    {
        // this is for yarn
        dialogueRunner.AddCommandHandler<string, string>("walk", Walk);
    }

    Coroutine Walk(string who, string where)
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

        // found this part online to keep their height? idk
        Vector2 goal = new Vector2(spot.position.x, spot.position.z);
        float timer = 0f;

        // gives up after 10 sec so the dialogue never gets stuck
        while (timer < 10f)
        {
            Vector3 pos = who.position;
            Vector2 flat = new Vector2(pos.x, pos.z);

            if (Vector2.Distance(flat, goal) < 0.05f) break;

            flat = Vector2.MoveTowards(flat, goal, speed * Time.deltaTime);
            who.position = new Vector3(flat.x, pos.y, flat.y);

            timer += Time.deltaTime;
            yield return null;
        }

        // physics back on
        if (rb != null) rb.isKinematic = wasKinematic;
    }
}