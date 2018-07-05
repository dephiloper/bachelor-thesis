using System.IO;
using Agent.Data;
using UnityEngine;
using Action = Agent.Data.Action;

public class RecordManager
{
    private readonly StreamWriter _writer;

    public RecordManager(string path)
    {
        _writer = File.AppendText(path);
    }

    ~RecordManager()
    {
        _writer.Close();
    }

    public void SaveDecision(Percept percept, Action action)
    {
        var json = JsonUtility.ToJson(percept);
        _writer.WriteLine(json);
        json = JsonUtility.ToJson(action);
        _writer.WriteLine(json);
    }
}
