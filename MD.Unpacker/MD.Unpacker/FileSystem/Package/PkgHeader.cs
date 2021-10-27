using System;

namespace MD.Unpacker
{
    class PkgHeader
    {
        public Int32 dwTableSize { get; set; }
        public Int32 dwArchiveSize { get; set; }
        public Int32 dwTotalFiles { get; set; }
    }
}
