using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Configuration;
using Engine.Entities;

namespace PlayGen.ITAlert.Simulation.Scenario.Evaluators.Filters
{
	public interface IEntityFilter : IDisposable
	{
		bool Evaluate(Entity entity);
	}

	public interface IEntityFilter<in TECS, in TConfiguration> : IEntityFilter
		where TECS : class, IECS
		where TConfiguration : ECSConfiguration
	{
		void Initialize(TECS ecs, TConfiguration configuration);
	}
}
