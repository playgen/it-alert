using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;

namespace Engine.Core.Messaging
{
	public interface IMessageHub : ISubject<IMessage>
	{
	}
}
