
using System;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// 本命名空间提供一些可以简化IO相关操作的功能
/// </summary>
namespace Vorcyc.PowerLibrary.IO
{

    /// <summary>
    /// 并合文件管理器,使用<see cref="PackFileManager"/>作为替代。
    /// Use <see cref=" PackFileManager"/> as replacement.
    /// </summary>
    [Obsolete]
    public class CoalescentFileManager
    {

        private CoalescentFile _cf;
        //private bool _coaFileLoaded;
        private List<string> _fullFilenameList = new List<string>();


        public void AddSourceFile(string source)
        {
            if (File.Exists(source))
                this._fullFilenameList.Add(source);
            else
                throw new FileNotFoundException(source);
        }

        public void LoadCoalescentFile(string coalescentFile)
        {
            if (!File.Exists(coalescentFile))
            {
                throw new FileNotFoundException(coalescentFile);
            }
            using (FileStream fs = new FileStream(coalescentFile, FileMode.Open, FileAccess.Read))
            {
                BinaryReader br = new BinaryReader(fs);
                if (br.ReadString() != "CoalescentFile")
                {
                    throw new InvalidCoalescentFileException("该文件不是有效的合并文件。");
                }
            }
            this._cf = new CoalescentFile(coalescentFile);
            //this._coaFileLoaded = true;
        }




        /// <summary>
        /// 写并合文件
        /// </summary>
        /// <param name="filename"></param>
        public void WriteTo(string filename)
        {
            WriteTo(filename, false);
        }

        /// <summary>
        /// 写并合文件，是否应用压缩（压缩开关无效）
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="useDeflate"></param>
        public void WriteTo(string filename, bool useDeflate)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite))
            {
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write("CoalescentFile");
                bw.Write(useDeflate);
                bw.Write(this._fullFilenameList.Count);
                foreach (string f in this._fullFilenameList)
                {
                    FileInfo fi = new FileInfo(f);
                    bw.Write(fi.Length);
                    fi = null;
                }
                foreach (string f in this._fullFilenameList)
                {
                    bw.Write(System.IO.Path.GetFileName(f));
                }
                foreach (string f in this._fullFilenameList)
                {
                    bw.Write(File.ReadAllBytes(f));
                    bw.Flush();
                }
            }
        }

        // Properties
        public CoalescentFile CurrentCoalescentFile
        {
            get
            {
                return this._cf;
            }
        }

        /// <summary>
        /// 文件部分
        /// </summary>
        public class CoalescentFile
        {

            private string _cf;
            private long _contentStartPos;
            private int _fileCount;
            private List<long> _fileLengthList = new List<long>();
            private bool _isZip;
            private List<string> _shortNameList = new List<string>();


            public CoalescentFile(string cf)
            {
                this._cf = cf;
                using (FileStream fs = new FileStream(this._cf, FileMode.Open, FileAccess.Read))
                {
                    BinaryReader br = new BinaryReader(fs);
                    br.ReadString();
                    this._isZip = br.ReadBoolean();
                    this._fileCount = br.ReadInt32();

                    for (int i = 1; i <= _fileCount; i++)
                    {
                        this._fileLengthList.Add(br.ReadInt64());
                    }

                    for (int i = 1; i <= _fileCount; i++)
                    {
                        this._shortNameList.Add(br.ReadString());
                    }
                    this._contentStartPos = fs.Position;
                    br.Close();
                }
            }


            public MemoryStream GetStream(int index)
            {
                return new MemoryStream(GetBytes(index));
            }

            public byte[] GetBytes(int index)
            {
                long startPos = this._contentStartPos;

                for (int i = 0; i < index; i++)
                {
                    startPos += this._fileLengthList[i];
                }

                using (var fs = new FileStream(_cf, FileMode.Open, FileAccess.Read))
                {
                    fs.Position = (int)startPos;
                    using (var br = new BinaryReader(fs))
                    {
                        return br.ReadBytes((int)_fileLengthList[index]);
                    }
                }
            }

            public void OutputAllToDirectory(string dir)
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                using (FileStream fs = new FileStream(this._cf, FileMode.Open, FileAccess.Read))
                {
                    BinaryReader br = new BinaryReader(fs);
                    fs.Position = this._contentStartPos;

                    for (int i = 0; i < _fileCount; i++)
                    {
                        byte[] buffer = br.ReadBytes((int)this._fileLengthList[i]);
                        File.WriteAllBytes(dir + this._shortNameList[i], buffer);
                    }
                    br.Close();
                }
            }

            /// <summary>
            /// 以新名字输出某个内容到文件
            /// </summary>
            /// <param name="index"></param>
            /// <param name="file"></param>
            public void OutputOneToFile(int index, string file)
            {
                using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write))
                {
                    var buffer = GetBytes(index);
                    BinaryWriter bw = new BinaryWriter(fs);
                    bw.Write(buffer);
                    bw.Close();
                }
            }

            /// <summary>
            /// 用原始文件名输出某个内容到指定文件夹
            /// </summary>
            /// <param name="index"></param>
            /// <param name="dir"></param>
            public void OutputOneToDirectory(int index, string dir)
            {
                string name = _shortNameList[index];
                using (FileStream fs = new FileStream(System.IO.Path.Combine(dir, name), FileMode.Create, FileAccess.Write))
                {
                    var buffer = GetBytes(index);
                    BinaryWriter bw = new BinaryWriter(fs);
                    bw.Write(buffer);
                    bw.Close();
                }
            }

            #region 属性

            public string CurrentFile
            {
                get
                {
                    return this._cf;
                }
            }

            public int FileCount
            {
                get
                {
                    return this._fileCount;
                }
            }

            public bool IsDeflate
            {
                get
                {
                    return this._isZip;
                }
            }

            public string[] NameList
            {
                get
                {
                    return this._shortNameList.ToArray();
                }
            }
            #endregion

        }//CoalescentFile over




        public class InvalidCoalescentFileException : Exception
        {
            public InvalidCoalescentFileException(string text)
                : base(text)
            {
            }
        }




    }
}
