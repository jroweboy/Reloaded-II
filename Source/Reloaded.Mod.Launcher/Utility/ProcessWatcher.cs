﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PropertyChanged;
using Reloaded.WPF.MVVM;

namespace Reloaded.Mod.Launcher.Utility 
{
    public class ProcessWatcher : ObservableObject, IDisposable
    {
        private const string WmiProcessidName = "ProcessID";
        private static object _lock = new object();

        /* Fired when the available mods collection changes. */
        public event NotifyCollectionChangedEventHandler ProcessesChanged = (sender, args) => { };

        public ObservableCollection<Process> Processes
        {
            get => _processes;
            set
            {
                value.CollectionChanged += ProcessesChanged;
                _processes = value;

                RaisePropertyChangedEvent(nameof(Processes));
                ProcessesChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        private ObservableCollection<Process> _processes;
        private ManagementEventWatcher _startWatcher;
        private ManagementEventWatcher _stopWatcher;

        public ProcessWatcher()
        {
            // Populate bindings.
            Processes = new ObservableCollection<Process>();
            PopulateProcesses();
            _startWatcher = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
            _stopWatcher = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace"));
            _startWatcher.EventArrived += ApplicationLaunched;
            _stopWatcher.EventArrived += ApplicationExited;
            _startWatcher.Start();
            _stopWatcher.Start();
        }

        ~ProcessWatcher()
        {
            Dispose();
        }

        public void Dispose()
        {
            _startWatcher?.Dispose();
            _stopWatcher?.Dispose();
            GC.SuppressFinalize(this);
        }

        private void PopulateProcesses()
        {
            foreach (Process process in Process.GetProcesses())
                AddProcess(process);
        }

        private void AddProcess(Process process)
        {
            Application.Current.Dispatcher.Invoke(() => {
                lock (_lock)
                {
                    Processes.Add(process);
                    ProcessesChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            });
        }


        private void ApplicationLaunched(object sender, EventArrivedEventArgs e)
        {
            ActionWrappers.TryCatch(() =>
            {
                var processId = Convert.ToInt32(e.NewEvent.Properties[WmiProcessidName].Value);
                AddProcess(Process.GetProcessById(processId));
            });
        }

        private void ApplicationExited(object sender, EventArrivedEventArgs e)
        {
            lock (_lock)
            {
                int processId = Convert.ToInt32(e.NewEvent.Properties[WmiProcessidName].Value);
                var processes = Processes.Where(x => x.Id == processId).ToList();
                foreach (var process in processes)
                    Processes.Remove(process);

                ProcessesChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }
    }
}
