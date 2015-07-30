using Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public class ConsoleService
    {
        private const int _maxBufferLength = 8192;

        private Dictionary<Guid, ProcessInfo> _outputBuffers = new Dictionary<Guid, ProcessInfo>();

        private Dictionary<Guid, StringBuilder> _buffers = new Dictionary<Guid, StringBuilder>();

        private Dictionary<Guid, Process> _processes = new Dictionary<Guid, Process>();

        private Dictionary<Guid, BackgroundAction[]> _backgroundAction = new Dictionary<Guid, BackgroundAction[]>();

        private BackgroundAction StartBufferThread(Guid id, StreamReader reader)
        {
            var bufferSize = 8192;
            var buffer = new char[bufferSize];

            var action = new BackgroundAction(() =>
            {
                var bytesRead = reader.Read(buffer, 0, bufferSize);

                lock (_buffers)
                {
                    StringBuilder sb;

                    if (_buffers.TryGetValue(id, out sb))
                    {
                        sb.Append(new string(buffer, 0, bytesRead));
                    }
                }
            }) { Interval = 100 };

            action.StartAsync();

            return action;
        }

        private void PerformProcessAction(Action a)
        {
            lock (_processes)
            {
                lock (_buffers)
                {
                    a();
                }
            }
        }

        private void CreateProcess(Guid id)
        {
            var p = new Process()
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo("cmd")
                {
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                }
            };            

            PerformProcessAction(() =>
            {
                _buffers.Add(id, new StringBuilder());
                _processes.Add(id, p);
                p.Start();
                _backgroundAction.Add(id, new[] 
                {
                    StartBufferThread(id, p.StandardOutput),
                    StartBufferThread(id, p.StandardError),
                });
            });
        }

        public Guid Execute(string command)
        {
            var guid = Guid.NewGuid();

            DataReceivedEventHandler handler = (o, e) =>
            {
                if (e.Data != null)
                {
                    lock (_outputBuffers) _outputBuffers[guid].Output.AppendLine(e.Data);
                }
            };

            var p = new Process()
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo("cmd", "/c " + command)
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                }
            };

            p.OutputDataReceived += handler;
            p.ErrorDataReceived += handler;

            lock (_outputBuffers)
            {
                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();

                _outputBuffers.Add(guid, new ProcessInfo());
                _outputBuffers[guid].Process = p;
            }

            return guid;
        }

        public string GetOutput(Guid processGuid)
        {
            ProcessInfo processInfo;
            bool hasExited;

            lock (_outputBuffers)
            {
                if (!_outputBuffers.TryGetValue(processGuid, out processInfo))
                {
                    return null;
                }

                hasExited = processInfo.Process.HasExited;
                //}

                //lock (_outputBuffers)
                //{
                var s = processInfo.Output.ToString();
                processInfo.Output.Clear();

                if (s.Length > _maxBufferLength)
                {
                    processInfo.Output.Append(s.Substring(_maxBufferLength));

                    return s.Remove(_maxBufferLength);
                }

                if (hasExited && _outputBuffers[processGuid].Output.Length == 0)
                {
                    _outputBuffers.Remove(processGuid);
                }

                return s;
            }
        }

        public RemoteProcess[] GetProcesses()
        {
            return Process
                .GetProcesses()
                .Select(x => new RemoteProcess() { Id = x.Id, Name = x.ProcessName })
                .ToArray();
        }

        public bool KillProcess(int id)
        {
            Process p;

            try
            {
                p = Process.GetProcessById(id);
            }
            catch
            {
                return false;
            }

            try
            {
                p.Kill();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool KillProcessesByName(string name)
        {
            var processKilled = false;

            foreach (var p in Process.GetProcessesByName(name))
            {
                try
                {
                    p.Kill();

                    processKilled = true;
                }
                catch { }
            }

            return processKilled;
        }

        public Guid Register()
        {
            var g = Guid.NewGuid();

            CreateProcess(g);

            return g;
        }

        public void Unregister(Guid id)
        {
            BackgroundAction[] actions = null;

            PerformProcessAction(() =>
            {
                Process p;

                if (_processes.TryGetValue(id, out p))
                {
                    p.Kill();

                    _processes.Remove(id);
                    _buffers.Remove(id);
                    actions = _backgroundAction[id];
                    _backgroundAction.Remove(id);
                }
            });

            foreach (var b in actions)
            {
                b.Stop(false);
            }
        }

        public string ReadBuffer(Guid id)
        {
            lock (_buffers)
            {
                var sb = _buffers[id];

                if (sb.Length == 0)
                {
                    return null;
                }
                else
                {
                    var s = sb.ToString();
                    sb.Clear();
                    return s;
                }
            }
        }

        public void WriteBuffer(Guid id, string buffer)
        {
            lock (_processes)
            {
                var p = _processes[id];
                p.StandardInput.Write(buffer);
                p.StandardInput.Flush();
            }
        }
    }
}
