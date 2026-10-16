using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class BirdManager : MonoBehaviour
{

// SFX

    public string hungryBirds = "event:/SFX/Mechanics/Fish eggs/Hungry FB";

    private EventInstance birdSound;


    void Awake()
    {
        birdSound = RuntimeManager.CreateInstance(hungryBirds);
    }

    void OnEnable()
    {
        birdSound.start();
        Debug.Log("Hungry Bird Fish sound started.");

    }

    void OnDisable()
    {
        birdSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        Debug.Log("Hungry Bird Fish sound stopped!");
    }

    void OnDestroy()
    {
        birdSound.release();
    }
}
