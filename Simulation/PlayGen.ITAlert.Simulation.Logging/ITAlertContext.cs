using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Engine.Logging.Database;
using PlayGen.ITAlert.Simulation.Logging.Models;

namespace PlayGen.ITAlert.Simulation.Logging
{
	// ReSharper disable once InconsistentNaming
	public class ITAlertLoggingContext : EventLogContext
	{
		public DbSet<PlayerFeedback> PlayerFeedback { get; set; }

		public ITAlertLoggingContext(string nameOrConnectionString) : base(nameOrConnectionString)
		{
		}

		public ITAlertLoggingContext(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection,
			contextOwnsConnection)
		{
		}

		protected override void OnModelCreating(DbModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<PlayerFeedback>()
				.ToTable("PlayerFeedback");

			builder.Entity<PlayerFeedback>()
				.HasKey(pf => pf.Id);

			builder.Entity<PlayerFeedback>()
				.HasRequired(pf => pf.Player);

			builder.Entity<PlayerFeedback>()
				.HasRequired(pf => pf.RankedPlayer);
		}
	}
}
