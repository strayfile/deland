using System;
using System.Collections.ObjectModel;

namespace LandscapeDesignServer
{
    class LogMessages
    {
        private static LogMessages _instance;
        private static readonly object _sync = new Object();
        private LogMessages()
        {
            Logs = new ObservableCollection<string>();
        }
        public void Add(string log)
        {
            lock (Logs)
            {
                if (Logs.Count == 100)
                    Logs.RemoveAt(0);
                Logs.Add($"{DateTime.Now}\t{log}");
            }
        }
        public void Clear()
        {
            lock (Logs)
                Logs.Clear();
        }
        public ObservableCollection<string> Logs { get; private set; }
        public static LogMessages GetInstance()
        {
            if (_instance == null)
            {
                lock (_sync)
                {
                    if (_instance == null)
                        _instance = new LogMessages();
                }
            }
            return _instance;
        }
    }
}
