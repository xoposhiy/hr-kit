using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;

namespace HrKit
{
	public class ProcessMonitor
	{
		private volatile ImmutableList<Process> processes = ImmutableList<Process>.Empty;
		private readonly Action<string> log;
		private readonly long memoryLimit;
		private readonly TimeSpan timeLimit;

		public ProcessMonitor(TimeSpan timeLimit, long memoryLimit, Action<string> log)
		{
			this.timeLimit = timeLimit;
			this.memoryLimit = memoryLimit;
			this.log = log;
			CreateMonitoringThread().Start();
		}

		private Thread CreateMonitoringThread()
		{
			return new Thread(() =>
			{
				while (true)
				{
					foreach (var process in processes) Inspect(process);
					Thread.Sleep(500);
				}
				// ReSharper disable once FunctionNeverReturns
			})
			{
				IsBackground = true,
				Name = "Process monitoring"
			};
		}

		public void Register(Process process)
		{
			processes = processes.Add(process);
		}

		private void Inspect(Process process)
		{
			if (process.HasExited) processes.Remove(process);
			else
			{
				try
				{
					CheckParameter(process.TotalProcessorTime, timeLimit, process, "TimeLimit");
					CheckParameter(process.PeakWorkingSet64, memoryLimit, process, "MemoryLimit");
				}
				catch
				{
					if (process.HasExited) processes.Remove(process);
				}
			}
		}

		private void CheckParameter<T>(T param, T limit, Process process, string message) where T : IComparable<T>
		{
			if (param.CompareTo(limit) <= 0) return;
			log(string.Format(message + " {0}: {1}", process.ProcessName, param));
			process.Kill();
			processes.Remove(process);
		}
	}
}