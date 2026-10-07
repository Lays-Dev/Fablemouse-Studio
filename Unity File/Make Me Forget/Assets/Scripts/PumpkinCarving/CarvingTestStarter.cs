using UnityEngine;

// TESTTTTTT OOOOONLY: 1 = Shave, 2 = Cut, Enter = finish, R = restart
public class CarvingTestStarter : MonoBehaviour
{
    public PumpkinCarvingMinigame minigame;

    float timeLeft;
    bool hasResult;
    CarvingResult result;

    void Start()
    {
        minigame.onTimerTick.AddListener(t => timeLeft = t);
        minigame.onFinished.AddListener(r => { result = r; hasResult = true; });
        Restart();
    }

    void Restart()
    {
        hasResult = false;
        minigame.StartMinigame(0);
    }

    void Update()
    {
#if ENABLE_INPUT_SYSTEM
        var kb = UnityEngine.InputSystem.Keyboard.current;
        if (kb == null) return;
        if (kb.enterKey.wasPressedThisFrame) minigame.Finish();
        if (kb.rKey.wasPressedThisFrame) Restart();
        if (kb.digit1Key.wasPressedThisFrame) minigame.carver.UseShave();
        if (kb.digit2Key.wasPressedThisFrame) minigame.carver.UseCut();
#else
        if (Input.GetKeyDown(KeyCode.Return)) minigame.Finish();
        if (Input.GetKeyDown(KeyCode.R)) Restart();
        if (Input.GetKeyDown(KeyCode.Alpha1)) minigame.carver.UseShave();
        if (Input.GetKeyDown(KeyCode.Alpha2)) minigame.carver.UseCut();
#endif
    }

    void OnGUI()
    {
        var style = new GUIStyle(GUI.skin.label) { fontSize = 28 };
        if (minigame.Running)
        {
            var live = minigame.Preview();
            GUI.Label(new Rect(20, 20, 600, 40), $"Time: {timeLeft:0.0}s   Accuracy: {live.Percent}%", style);
            GUI.Label(new Rect(20, 60, 1000, 40), $"Tool: {minigame.carver.Tool}   (1 = Shave, 2 = Cut, Enter = done, R = restart)", style);
        }
        else if (hasResult)
        {
            GUI.Label(new Rect(20, 20, 800, 40),
                $"Final: {result.Percent}%  ({new string('*', result.stars)}{new string('-', 3 - result.stars)})", style);
            GUI.Label(new Rect(20, 60, 800, 40),
                $"Coverage {result.coverage:P0}   Precision {result.precision:P0}   R = try again", style);
        }
    }
}