using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PlayGen.ITAlert.Common;
using PlayGen.ITAlert.Configuration;
using PlayGen.ITAlert.Simulation.Tests.World;
using PlayGen.ITAlert.Simulation.Visitors.Items;
using PlayGen.ITAlert.Simulation.World;

namespace PlayGen.ITAlert.Simulation.Tests
{
	[TestFixture]
	public class RepairItemTests
	{
		[Test]
		public void TestRepairItemActivation()
		{
			var sim = new TestSimulation();
			var player = sim.CreatePlayer(new PlayerConfig());
			var subsystem = sim.CreateSubsystem(new NodeConfig(1));
			var repair = sim.CreateItem(ItemType.Repair) as Repair;

			Assert.That(repair, Is.Not.Null, "Repair item is null");
			Assert.That(subsystem.TryAddItem(repair), "Could not add repair item to subsystem");

			IItem testItem;
			Assert.That(subsystem.TryGetItem(ItemType.Repair, player, out testItem), "Could not get repair item from subsystem");
			Assert.That(repair.Id, Is.EqualTo(testItem.Id), "Repair item does not equal item returned by subsystem");

			repair.SetOwnership(player);
			Assert.That(repair.IsOwnedBy(player), "Item is not owned by player");
			Assert.That(repair.CanBeActivatedBy(player), "Player cannot activate item");

			repair.Activate();
			Assert.That(repair.IsActive, "Repair item not in active state");

			Assert.That(subsystem.Shield, Is.EqualTo(SimulationConstants.MaxShield), "Subsystem shield is not full");
			Assert.That(subsystem.Health, Is.EqualTo(SimulationConstants.MaxHealth), "Subsystem health is not full");

			subsystem.ModulateHealth(-1 * (SimulationConstants.MaxHealth + SimulationConstants.MaxShield));
			Assert.That(subsystem.Health, Is.EqualTo(0), "Subsystem health is not zero");
			Assert.That(subsystem.Shield, Is.EqualTo(0), "Subsystem shield is not zero");

			var i = 1;
			while (subsystem.IsDead == false)
			{
				repair.Tick(i);
				var total = subsystem.Health + subsystem.Shield;
				Assert.That(total, Is.EqualTo(i), $"Subsystem combined health and shield incorrect after tick {i}");
			}


		}

	}
}
