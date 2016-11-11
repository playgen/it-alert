using System.Security.Cryptography.X509Certificates;
using Engine.Core.Components;

namespace Engine.Core.Messaging
{
	public interface IMessage
	{
		IMessageHub Origin { get; }

		void SetOrigin(IMessageHub origin);

		MessageScope Scope { get; set; }
	}
}
