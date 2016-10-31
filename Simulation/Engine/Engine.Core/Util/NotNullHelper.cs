using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayGen.Engine.Util
{
	public static class NotNullHelper
	{
		public static void ArgumentNotNull(object obj, string membername)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(membername);
			}
		}
	}
}
