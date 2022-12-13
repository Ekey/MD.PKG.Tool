using System;
using System.IO;
using System.Collections.Generic;

namespace MD.Unpacker
{
    class PkgUnpack
    {
        private static List<PkgEntry> m_EntryTable = new List<PkgEntry>();

        public static void iDoIt(String m_Archive, String m_DstFolder)
        {
            PkgHashList.iLoadProject();
            using (FileStream TPkgStream = File.OpenRead(m_Archive))
            {
                var m_Header = new PkgHeader();

                m_Header.dwTableSize = TPkgStream.ReadInt32();
                m_Header.dwArchiveSize = TPkgStream.ReadInt32();
                m_Header.dwTotalFiles = TPkgStream.ReadInt32();

                if (m_Header.dwTableSize + m_Header.dwArchiveSize + 4 != TPkgStream.Length)
                {
                    Utils.iSetError("[ERROR]: Invalid PKG archive file");
                    return;
                }

                m_EntryTable.Clear();
                for (Int32 i = 0; i < m_Header.dwTotalFiles; i++)
                {
                    var m_Entry = new PkgEntry();

                    m_Entry.dwHash = TPkgStream.ReadUInt64();
                    m_Entry.dwOffset = TPkgStream.ReadUInt32();
                    m_Entry.dwSize = TPkgStream.ReadUInt32();

                    m_EntryTable.Add(m_Entry);
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
