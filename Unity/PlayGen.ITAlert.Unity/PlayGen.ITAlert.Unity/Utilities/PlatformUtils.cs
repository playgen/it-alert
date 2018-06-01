using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.Utilities
{
    public static class PlatformUtils
    {
        public static bool IsMobile => Application.isMobilePlatform;
    }
}
