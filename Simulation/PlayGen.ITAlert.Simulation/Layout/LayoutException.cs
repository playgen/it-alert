using System;

namespace PlayGen.ITAlert.Simulation.Layout
{
	public class LayoutException : Exception
	{
		public LayoutException()
		{
		}

		public LayoutException(string message) : base(message)
		{
		}

		public LayoutException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
