using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayGen.ITAlert.Simulation
{
    public static class Version
    {
        public const int Major = 1;

        public const int Minor = 0;

        public const int Build = 1;

        public new static string ToString()
        {
            return $"{Major}.{Minor}.{Build}";
        }
    }
}