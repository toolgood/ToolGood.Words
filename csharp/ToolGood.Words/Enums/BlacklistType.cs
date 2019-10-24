using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words
{
    [Flags]
    public enum BlacklistType : int
    {
        None,
        Level01 = 1,
        Level02 = 2,
        Level03 = 4,
        Level04 = 8,
        Level05 = 16,
        Level06 = 32,
        Level07 = 64,
        Level08 = 128,
        Level09 = 256,
        Level10 = 512,
        Level11 = 1024,
        Level12 = 2048,
        Level13 = 4096,
        Level14 = 8192,
        Level15 = 16384,
        Level16 = 32768,
        Level17 = 65536,
        Level18 = 131072,
        Level19 = 262144,
        Level20 = 524288,
        Level21 = 1048576,
        Level22 = 2097152,
        Level23 = 4194304,
        Level24 = 8388608,
        Level25 = 16777216,
        Level26 = 33554432,
        Level27 = 67108864,
        Level28 = 134217728,
        Level29 = 268435456,
        Level30 = 536870912,
        Level31 = 1073741824,
        All = int.MaxValue
    }
}
