using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Engine.Newtonsoft.Json.Tests
{
	[TestFixture]
	public class NewtonsoftActivatorTests
	{
		public class ActivatorTestClass
		{
			public ActivatorTestClass()
			{
			}
		}


		[Test]
		public void Test()
		{
			const string json = "{}";

			var test = JsonConvert.DeserializeObject<ActivatorTestClass>(json);




		}



	}
}
