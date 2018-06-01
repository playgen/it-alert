using System;

namespace PlayGen.ITAlert.Unity.Exceptions
{
	public class PhotonDisconnectionException : Exception
	{
		public PhotonDisconnectionException()
		{
		}

		public PhotonDisconnectionException(string message) : base(message)
		{
		}

		public PhotonDisconnectionException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
