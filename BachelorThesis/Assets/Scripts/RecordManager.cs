using System.IO;
using AgentData;
using AgentData.Base;
using Newtonsoft.Json;

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

    public void SaveDecision(IPercept percept, IAction action)
    {
        var json = JsonConvert.SerializeObject(percept.ToDoubleArray());
        _writer.WriteLine(json);
        json = JsonConvert.SerializeObject(action.Raw);
        _writer.WriteLine(json);
    }
}