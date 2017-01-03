using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;

namespace Engine.Messaging
{
	public interface IMessageHub : ISubject<IMessage>
	{
	}
}
