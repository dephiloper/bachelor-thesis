using System;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Train
{
    public class TrainLogger : ILogHandler
    {
        private readonly StreamWriter _writer;
    
        public TrainLogger(string logFile)
        {
            _writer = File.AppendText(logFile);
        }

        ~TrainLogger() 
        {
            _writer.Close();
        }
        public void LogFormat(LogType logType, Object context, string format, params object[] args) => 
            _writer.WriteLine(args[0]);

        public void LogException(Exception exception, Object context)
        {

        }
    }
}