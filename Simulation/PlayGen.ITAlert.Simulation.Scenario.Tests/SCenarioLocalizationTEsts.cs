using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PlayGen.ITAlert.Simulation.Scenario.Localization;

namespace PlayGen.ITAlert.Simulation.Scenario.Tests
{
	[TestFixture]
	public class SCenarioLocalizationTests
	{
		[Test]
		public void TestLoadFromResource()
		{
			var localizationDict = LocalizationHelper.GetLocalizationFromEmbeddedResource();

			Assert.That(localizationDict, Is.Not.Null);
		}


		[Test]
		public void TestLoadFromResourceWithPrefix()
		{
			var localizationDict = LocalizationHelper.GetLocalizationFromEmbeddedResource("Tutorial1");

			Assert.That(localizationDict, Is.Not.Null);
		}

	}
}
