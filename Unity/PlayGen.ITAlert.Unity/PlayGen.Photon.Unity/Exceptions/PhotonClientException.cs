using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayGen.Photon.Unity.Exceptions
{
	public class PhotonClientException : Exception
	{
		public PhotonClientException(string message) : base(message)
		{
		}

		public PhotonClientException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
