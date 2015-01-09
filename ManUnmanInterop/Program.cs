using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ManUnmanInterop
{
	class Program
	{
		private static Lazy<LogicCommand[]> _logicCommandValues = new Lazy<LogicCommand[]>(
			() => (LogicCommand[] )Enum.GetValues(typeof (LogicCommand)));

		static void Main() {

			var settingsProvider = new AppSettingsProvider();
			using (var commandDispatcher = new MmfCommandDispatcher(settingsProvider))
			using (var logicDispatcher = new LogicDispatcher(commandDispatcher, settingsProvider)) {
				logicDispatcher.Init();

				WriteAvailableCommands();
				ConsoleKeyInfo keyInfo;
				do {
					keyInfo = Console.ReadKey();
					Console.WriteLine();
					LogicCommand command;

					if (keyInfo.Key == ConsoleKey.I) {
						WriteAvailableCommands();
						continue;
					}
					if (TryGetCommand(keyInfo, out command)) {
						try {
							Console.WriteLine("Logic returns: {0}", logicDispatcher.RequestCommand<bool>(command));
						} catch (CommandProcessingException ex) {
							Console.WriteLine("Command processing exception {0} for command {1}", ex, ex.Command);
						}
					}
				} while (keyInfo.Key != ConsoleKey.Escape);
			}
		}

		[SuppressMessage("Microsoft.Contracts", "Enum-2-0")]
		[SuppressMessage("Microsoft.Contracts", "Enum-19-0")]
		private static bool TryGetCommand(ConsoleKeyInfo keyInfo, out LogicCommand command) {
			command = default (LogicCommand);
			try {
				var byteVal = (byte) Char.GetNumericValue(keyInfo.KeyChar);
				command = (LogicCommand) byteVal;
				if (command > _logicCommandValues.Value.Last() || command < _logicCommandValues.Value.First()) {
					return false;
				}
			} catch (Exception) {
				return false;
			}
			return true;
		}

		private static void WriteAvailableCommands() {
			Console.WriteLine("Available commands: ");
			Array.ForEach(
				_logicCommandValues.Value, 
				value => Console.WriteLine("{0} - {1}", (byte)value, value));

			Console.WriteLine("Press Esc to exit");
			Console.WriteLine("Press i for this message");
		}
	}
}
