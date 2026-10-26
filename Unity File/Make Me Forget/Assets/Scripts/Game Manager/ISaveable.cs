public interface ISaveable
{
    // A unique identifier for this saveable object.
    string SaveId { get; }

    // Returns the object's state as JSON.
    string CaptureState();

    // Restores the object using previously saved JSON.
    void RestoreState(string json);
}
