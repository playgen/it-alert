﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayGen.ITAlert.Unity
{
    public static class Version
    {
        public const int Major = 1;

        public const int Minor = 0;

        public const int Build = 2;

        public new static string ToString()
        {
            return $"{Major}.{Minor}.{Build}";
        }
    }
}
