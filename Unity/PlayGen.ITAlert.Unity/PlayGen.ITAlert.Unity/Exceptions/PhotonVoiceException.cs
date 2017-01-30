using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
