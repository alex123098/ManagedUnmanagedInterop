namespace ManUnmanInterop
{
	public enum LogicCommand : byte
	{
		Start = 1,
		Process = 2,
		End = 3
	}

	public static class LogicCommandExtensions
	{
		public static byte ToByte(this LogicCommand command) {
			return (byte) command;
		}
	}
}