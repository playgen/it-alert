using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zenject;

namespace Engine.Entities
{
	public class EntityFactory : IFactory<string, Entity>
	{
		private DiContainer _container;

		public EntityFactory(DiContainer container)
		{
			_container = container;
		}

		public Entity Create(string param)
		{
			throw new NotImplementedException();
		}

		//public Entity Create(string param)
		//{
		//	return _container.
		//}
	}
}
