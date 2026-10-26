
using System;
using System.Collections.Generic;

[Serializable]
public class SaveEntry
{
    public string id;
    public string json;
}

[Serializable]
public class GameSaveData
{
    public string sceneName;
    public List<SaveEntry> objects = new List<SaveEntry>();
}

