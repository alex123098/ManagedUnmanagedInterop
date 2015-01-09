using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace ManUnmanInterop
{
	internal interface ILogicDispatcher : IDisposable
	{
		void Init();

		TResponse RequestCommand<TResponse>(LogicCommand command) where TResponse : struct;

		Task<TResponse> RequestCommandAsync<TResponse>(LogicCommand command) where TResponse : struct;
	}

	internal class LogicDispatcher : ILogicDispatcher
	{
		private readonly ICommandDispatcher _dispatcher;
		private readonly ISettingsProvider _settingsProvider;
		private Process _logicProcess;
		private EventWaitHandle _exitEvent;

		public LogicDispatcher(ICommandDispatcher dispatcher, ISettingsProvider settingsProvider) {
			Contract.Requires<ArgumentNullException>(dispatcher != null);
			Contract.Requires<ArgumentNullException>(settingsProvider != null);
			
			_dispatcher = dispatcher;
			_settingsProvider = settingsProvider;
			
		}

		public void Init() {
			Contract.Ensures(_logicProcess != null);
			Contract.Ensures(_exitEvent != null);
			
			_logicProcess = Process.Start(_settingsProvider.Get("logicAppPath"));
			_exitEvent = new EventWaitHandle(false, EventResetMode.ManualReset, _settingsProvider.Get("exitEventName"));
			_dispatcher.InitDispatcher();
		}

		public TResponse RequestCommand<TResponse>(LogicCommand command) 
			where TResponse : struct {
			
			return _dispatcher.Request<TResponse>(command);
		}

		public Task<TResponse> RequestCommandAsync<TResponse>(LogicCommand command)
			where TResponse : struct {
			
			return Task.Run(() => _dispatcher.Request<TResponse>(command));
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				_dispatcher.Dispose();
				_exitEvent.Set();
				if (!_logicProcess.WaitForExit(1000)) {
					_logicProcess.Kill();
				}
				_logicProcess.Dispose();
				_exitEvent.Dispose();
			}
		}

		~LogicDispatcher() {
			Dispose(false);
		}
	}
}
