using System;
using System.IO;
using System.Collections.Generic;

namespace MD.Unpacker
{
    class PkgUnpack
    {
        static List<PkgEntry> m_EntryTable = new List<PkgEntry>();

        public static void iDoIt(String m_Archive, String m_DstFolder)
        {
            PkgHashList.iLoadProject();
            using (FileStream TPkgStream = File.OpenRead(m_Archive))
            {
                var lpHeader = TPkgStream.ReadBytes(12);
                var m_Header = new PkgHeader();

                using (var THeaderReader = new MemoryStream(lpHeader))
                {
                    m_Header.dwTableSize = THeaderReader.ReadInt32();
                    m_Header.dwArchiveSize = THeaderReader.ReadInt32();
                    m_Header.dwTotalFiles = THeaderReader.ReadInt32();

                    if (m_Header.dwTableSize + m_Header.dwArchiveSize + 4 != TPkgStream.Length)
                    {
                        Utils.iSetError("[ERROR]: Invalid PKG archive file");
                        return;
                    }

                    THeaderReader.Dispose();
                }

                m_EntryTable.Clear();
                var lpTable = TPkgStream.ReadBytes(m_Header.dwTableSize);
                using (var TEntryReader = new MemoryStream(lpTable))
                {
                    for (Int32 i = 0; i < m_Header.dwTotalFiles; i++)
                    {
                        UInt64 dwHash = TEntryReader.ReadUInt64();
                        UInt32 dwOffset = TEntryReader.ReadUInt32();
                        UInt32 dwSize = TEntryReader.ReadUInt32();

                        var TEntry = new PkgEntry
                        {
                            dwHash = dwHash,
                            dwOffset = dwOffset,
                            dwSize = dwSize,
                        };

                        m_EntryTable.Add(TEntry);
                    }

                    TEntryReader.Dispose();
                }

                foreach (var m_Entry in m_EntryTable)
                {
                    String m_FileName = PkgHashList.iGetNameFromHashList(m_Entry.dwHash);
                    String m_FullPath = m_DstFolder + m_FileName;

                    Utils.iSetInfo("[UNPACKING]: " + m_FileName);
                    Utils.iCreateDirectory(m_FullPath);

                    TPkgStream.Seek(m_Entry.dwOffset,  SeekOrigin.Begin);
                    var lpBuffer = TPkgStream.ReadBytes((Int32)m_Entry.dwSize - (Int32)m_Entry.dwOffset);

                    File.WriteAllBytes(m_FullPath, lpBuffer);
                }
            }
        }
    }
}
