using UnityEngine;

//Interaction to open Metal Gate

public class GateInteraction : MonoBehaviour
{
    public Animator gateAnimator;
    public Transform player;
    public float interactionDistance = 3f;

    void Update()
    {
        if (player == null || gateAnimator == null)
            return;

        float distance = Vector3.Distance(
            player.position, transform.position
        );

        if (distance <= interactionDistance &&
            Input.GetKeyDown(KeyCode.E))
        {
            gateAnimator.SetTrigger("OpenGate");
        }
    }
}
