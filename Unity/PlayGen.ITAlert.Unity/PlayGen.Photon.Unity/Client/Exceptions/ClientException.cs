using System;

namespace PlayGen.Photon.Unity.Client.Exceptions
{
    public abstract class ClientException : Exception
    {
        protected ClientException(string message) : base(message)
        {
        }
    }
}
