using System;

namespace MD.Unpacker
{
    class PkgEntry
    {
        public UInt64 dwHash { get; set; }
        public UInt32 dwOffset { get; set; }
        public UInt32 dwSize { get; set; }
    }
}
