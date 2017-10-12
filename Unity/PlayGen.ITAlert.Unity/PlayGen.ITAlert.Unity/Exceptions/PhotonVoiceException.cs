using System;

namespace PlayGen.ITAlert.Unity.Exceptions
{
	public class PhotonVoiceException : Exception
	{
		public PhotonVoiceException()
		{
		}

		public PhotonVoiceException(string message) : base(message)
		{
		}

		public PhotonVoiceException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
