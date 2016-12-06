﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Entities;
using NUnit.Framework;

namespace Engine.Core.Tests
{
	[TestFixture]
    public class EntityRegistryTests
    {

	    [Test]
	    public void TestSimpleEntityPooling()
	    {
		    var registry = new EntityRegistry();

			Assert.That(registry.LastEntityId, Is.EqualTo(0));

		    Entity first = null;

		    var i = 0;
		    var n = 0;
		    while (i++ < 10)
		    {
				var second = first;

				first = registry.CreateEntity();
				n++;
				
			    if (second != null)
				{
					Assert.That(registry.Entities.Count, Is.EqualTo(2));
				    second.Dispose();
					Assert.That(registry.Entities.Count, Is.EqualTo(1));
					Assert.That(registry.EntityPool.Count, Is.EqualTo(1));
					Assert.That(registry.EntityPool.Count + registry.Entities.Count, Is.EqualTo(2));
				}

				Assert.That(first.Id, Is.EqualTo(registry.LastEntityId));
				Assert.That(n, Is.EqualTo(registry.LastEntityId));
			}
		}

	    [Test]
	    public void TestEntityReset()
	    {
			var registry = new EntityRegistry();

			Assert.That(registry.LastEntityId, Is.EqualTo(0));

			var entity = registry.CreateEntity();

			entity.Components.Add(new );
		}
	}
}
