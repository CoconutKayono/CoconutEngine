using System;
using UnityEngine;

namespace GameLogic
{
    public class ErrorLogger : IDisposable
    {
        private readonly UIModule _uiModule;
        
        public ErrorLogger(UIModule uiModule)
        {
            _uiModule = uiModule;
            Application.logMessageReceived += LogHandler;
        }

        public void Dispose()
        {
            Application.logMessageReceived -= LogHandler;
        }

        private void LogHandler(string condition, string stacktrace, LogType type)
        {
            if (type == LogType.Exception)
            {
                string des = $"�ͻ��˱���, \n#����#��---{condition} \n#λ��#��---{stacktrace}";
                _uiModule.ShowUIAsync<LogUI>(des);
            }
        }
    }
}