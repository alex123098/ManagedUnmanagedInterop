using System;
using System.Diagnostics.Contracts;
using System.IO.MemoryMappedFiles;
using System.Threading;

namespace ManUnmanInterop
{
	internal interface ICommandDispatcher : IDisposable
	{
		void InitDispatcher();
		TResult Request<TResult>(LogicCommand command) where TResult : struct;
	}

	class MmfCommandDispatcher : ICommandDispatcher
	{
		private readonly String _mmfName;
		private readonly int _mmfCapacity;
		private MemoryMappedFile _currentMmf;
		private MemoryMappedViewAccessor _mmfAccessor;
		private readonly string _writeEventName;
		private readonly string _readEventName;
		private EventWaitHandle _writeEvent;
		private EventWaitHandle _readEvent;
		private readonly int _commandOffset;
		private readonly int _resultOffset;
		private readonly int _cmdExecTimeout;

		public MmfCommandDispatcher(ISettingsProvider settingsProvider) {
			Contract.Requires<ArgumentNullException>(settingsProvider != null);
			Contract.Ensures(!String.IsNullOrEmpty(_mmfName));
			Contract.Ensures(_mmfCapacity > 0);
			Contract.Ensures(!String.IsNullOrEmpty(_writeEventName));
			Contract.Ensures(!String.IsNullOrEmpty(_readEventName));
			Contract.Ensures(_commandOffset != _resultOffset);

			_mmfName = settingsProvider.Get("MMFName");
			_mmfCapacity = settingsProvider.Get<int>("MMFCapacity");
			_writeEventName = settingsProvider.Get("writeEventName");
			_readEventName = settingsProvider.Get("readEventName");
			_commandOffset = settingsProvider.Get<int>("commandOffset");
			_resultOffset = settingsProvider.Get<int>("resultOffset");
			_cmdExecTimeout = settingsProvider.Get<int>("commandTimeout");
		}

		public void InitDispatcher() {
			Contract.Ensures(_currentMmf != null);
			Contract.Ensures(_mmfAccessor != null);
			Contract.Ensures(_writeEvent != null);
			Contract.Ensures(_readEvent != null);

			_currentMmf = MemoryMappedFile.CreateOrOpen(_mmfName, _mmfCapacity, MemoryMappedFileAccess.ReadWrite);
			_mmfAccessor = _currentMmf.CreateViewAccessor();
			_writeEvent = new EventWaitHandle(false, EventResetMode.AutoReset, _writeEventName);
			_readEvent = new EventWaitHandle(false, EventResetMode.AutoReset, _readEventName);
		}

		public TResult Request<TResult>(LogicCommand command) where TResult : struct {
			SendCommand(command);

			if (!_readEvent.WaitOne(_cmdExecTimeout)) {
				throw new CommandProcessingException("Command exectution timeout", command);
			}

			TResult result;
			_mmfAccessor.Read(_resultOffset, out result);
			return result;
		}

		private void SendCommand(LogicCommand command) {
			_mmfAccessor.Write(_commandOffset, command.ToByte());
			_writeEvent.Set();
		}

		~MmfCommandDispatcher() {
			Dispose(false);
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing) {
			if (disposing) {
				_mmfAccessor.Dispose();
				_currentMmf.Dispose();
				_writeEvent.Dispose();
				_readEvent.Dispose();
			}
		}
	}

	internal class CommandProcessingException : Exception
	{
		public CommandProcessingException(string message, LogicCommand command)
			: base(message) {
			Command = command;
		}

		public LogicCommand Command { get; private set; }
	}
}