using System;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Train
{
    public class TrainLogger : ILogHandler
    {
        private readonly string _logFile;
    
        public TrainLogger(string logFile)
        {
            _logFile = logFile;
        }
    
        public void LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            using (var w = File.AppendText(_logFile))
                w.WriteLine(args[0]);
        }

        public void LogException(Exception exception, Object context)
        {

        }
    
    
    }
}