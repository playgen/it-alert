using System.Collections.Generic;

namespace PlayGen.ITAlert.Simulation.Components.Items
{
	public class Scanner : IItemType
	{
	}

	public class Repair : IItemType
	{
	}

	public class GarbageDisposal : IItemType
	{
	}

	public class Analyser : IItemType
	{
	}

	public class Tracer : IItemType
	{
	}

	public class Antivirus : IItemType
	{
		public int TargetGenome { get; set; }
	}

	public class Capture : IItemType
	{
		public int CapturedGenome { get; set; }

	}

	public class Data : IItemType
	{
	}

	public class Transfer : IItemType
	{
		
	}
	
}
