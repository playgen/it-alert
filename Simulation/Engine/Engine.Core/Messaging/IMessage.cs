using System.Security.Cryptography.X509Certificates;
using Engine.Core.Components;

namespace Engine.Core.Messaging
{
	public interface IMessage
	{
		IMessageHub Origin { get; set; }

		MessageScope Scope { get; }
	}
}
