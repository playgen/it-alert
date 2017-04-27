using System;
using System.Collections.Generic;
using System.Text;
using PlayGen.ITAlert.Simulation.Modules.Resources.Components;

namespace PlayGen.ITAlert.Simulation.UI.Components.Items
{
	public static class ResourceExtensions
	{
		public static float GetUtilisation(this IResource resource)
		{
			return (float) resource.Value / resource.Maximum;
		}
	}
}
