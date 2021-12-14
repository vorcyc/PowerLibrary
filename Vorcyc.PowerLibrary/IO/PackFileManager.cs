
using System;
using System.Collections.Generic;
using System.IO;
using Vorcyc.PowerLibrary.Buffer;


namespace Vorcyc.PowerLibrary.IO
{

    /// <summary>
    /// 包文件管理器,提供一组方法以将多个文件合并成一个文件，及将一个文件展开成多个文件的方法。
    /// </summary>
    /// <remarks>
    /// <para>本类中使用.NET内建的早期版本的压缩算法，但是感觉微软官方随时在变该类的使用方法，因此导致不稳定。建议最好不要启用压缩。在未来版本中我们会用自己的压缩算法替换。</para>
    /// </remarks>
    /// <example>
    /// 以下代码演示如何使用<see cref="PackFileManager"/>。
    /// <code>
    /// Vorcyc.PowerLibrary.IO.PackFileManager pfm = new Vorcyc.PowerLibrary.IO.PackFileManager();
    /// //添加要打包的文件
    /// pfm.AddSourceFile(@"d:\vtest.avi");
    /// pfm.AddSourceFile(@"d:\1.txt");
    /// pfm.AddSourceFile(@"d:\1.bmp");
    /// //构建包文件
    /// pfm.Build(@"d:\t.pfm", false);
    ///
    /// //加载包文件
    /// pfm.LoadPackFile(@"d:\t.pfm");
    ///
    /// //若加载成功，则可以使用 CurrentPackFile 属性访问包文件
    ///
    /// //当前包文件名
    /// Console.WriteLine(pfm.CurrentPackFile.CurrentPackFilename); //  =>  d:\t.pfm
    /// //包中的文件数量
    /// Console.WriteLine(pfm.CurrentPackFile.FileCount); //    =>  3
    /// //输出包中文件的文件名
    /// foreach (var fn in pfm.CurrentPackFile.Filenames)
    ///        Console.WriteLine(fn);            
    /// /*  vtest.avi
    ///  *  1.txt
    ///  *  1.bmp
    ///  */
    ///
    /// //使用桌面作为目标文件夹
    /// var dir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    ///
    /// //使用二进制直接写文件
    /// var bytes = pfm.CurrentPackFile.GetBytes(0);
    /// System.IO.File.WriteAllBytes(System.IO.Path.Combine(dir, "video1.avi"), bytes);
    ///
    /// //输出所有文件到文件夹
    /// pfm.CurrentPackFile.OutputAllToDirectory(dir);
    /// //输出单个文件到目录
    /// pfm.CurrentPackFile.OutputOneToDirectory(0, dir);
    /// //输出单个文件到指定文件名
    /// pfm.CurrentPackFile.OutputOneToFile(0, System.IO.Path.Combine(dir, "x.avi"));
    /// </code>
    /// </example>
    public sealed class PackFileManager
    {

        private PackFile _pf;
        private List<string> _pathList = new List<string>();

        /// <summary>
        /// 添加用于生成包文件的源文件
        /// </summary>
        /// <param name="source">源文件路径</param>
        public void AddSourceFile(string source)
        {
            if (File.Exists(source))
                this._pathList.Add(source);
            else
                throw new FileNotFoundException(source);
        }

        /// <summary>
        /// 装载一个包文件，通过<see cref="CurrentPackFile"/>访问。
        /// </summary>
        /// <param name="path">包文件路径</param>
        public void LoadPackFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }
            if (_pf != null)
            {
                _pf.Close();
                _pf = null;
            }
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            if (br.ReadString() != "PackFile")
            {
                throw new InvalidPackFileException("该文件不是有效的包文件");
            }
            this._pf = new PackFile(fs, br);
        }


        /// <summary>
        /// 生成包文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="compress"></param>
        public void Build(string path, bool compress = false)
        {
            List<byte[]> commpressedFiles = null;

            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write("PackFile");//写标识
                bw.Write(this._pathList.Count);//写文件总数
                bw.Write(compress);//写是否启用压缩
                                   
                //先写文件名，因为这样好找字符串的边界
                foreach (string f in this._pathList)
                {
                    bw.Write(System.IO.Path.GetFileName(f));
                }

                //再写文件长度，因为长度固定为64位整数
                if (compress)
                {
                    commpressedFiles = new List<byte[]>();

                    foreach (string f in this._pathList)
                    {
                        var commpressedBuffer = Compression.CompressFileToBytes(f);

                        commpressedFiles.Add(commpressedBuffer);
                        //写压缩后的文件长度，记得一定要转成long，否则就会以INT32写，读的时候就不对了
                        bw.Write((long)commpressedBuffer.Length);
                    }
                }
                else
                {
                    foreach (string f in this._pathList)
                    {
                        FileInfo fi = new FileInfo(f);
                        bw.Write(fi.Length);
                        fi = null;
                    }
                }


                //写文件内容
                int index = 0;
                foreach (string f in this._pathList)
                {
                    if (compress)
                        bw.Write(commpressedFiles[index]);
                    else
                        bw.Write(File.ReadAllBytes(f));

                    bw.Flush();

                    index++;
                }
            }
        }

        /// <summary>
        /// 通过<see cref="LoadPackFile(string)"/>装载的包文件。
        /// </summary>
        public PackFile CurrentPackFile => this._pf;


        /// <summary>
        /// 文件部分
        /// </summary>
        public sealed class PackFile
        {
            //不像并合文件，包文件是持续占用文件的
            private FileStream _sourceFile;
            private BinaryReader _br;
            private long _contentStartPos;

            private int _fileCount;
            private bool _isCompressed = false;

            private List<long> _fileLengthList = new List<long>();
            private List<string> _shortNameList = new List<string>();


            internal PackFile(FileStream srcFile, BinaryReader br)
            {
                this._sourceFile = srcFile;
                _br = br;
                this._fileCount = _br.ReadInt32();//取文件数
                this._isCompressed = _br.ReadBoolean();//读是否启用压缩

                for (int i = 1; i <= _fileCount; i++)
                {
                    this._shortNameList.Add(_br.ReadString());
                }

                //读每个文件长度
                for (int i = 1; i <= _fileCount; i++)
                {
                    this._fileLengthList.Add(_br.ReadInt64());
                }

         
                this._contentStartPos = _sourceFile.Position;//设置实体文件内容的起始位置

            }

            /// <summary>
            /// 以索引为下标，以内存流的形式返回包文件中某个文件。
            /// </summary>
            /// <param name="index">包文件中的文件索引</param>
            /// <returns></returns>
            /// <exception cref="IndexOutOfRangeException"/>
            public MemoryStream GetStream(int index)
            {
                if (index < 0 || index > _fileCount - 1) throw new IndexOutOfRangeException(nameof(index));
                return new MemoryStream(GetBytes(index));
            }

            /// <summary>
            /// 以索引为下标，以字节的形式返回包文件中某个文件。
            /// </summary>
            /// <param name="index">包文件中的文件索引</param>
            /// <returns></returns>
            /// <exception cref="IndexOutOfRangeException"/>
            public byte[] GetBytes(int index)
            {
                if (index < 0 || index > _fileCount - 1) throw new IndexOutOfRangeException(nameof(index));

                long startPos = this._contentStartPos;

                for (int i = 0; i < index; i++)
                {
                    startPos += this._fileLengthList[i];
                }

                _sourceFile.Position = startPos;

                var buffer = _br.ReadBytes((int)_fileLengthList[index]);
                //if (_isCompressed)
                //    return Compression.UncompressBytesToBytes(buffer);
                //else
                //    return buffer;

                return _isCompressed ? Compression.DecompressBytesToBytes(buffer) : buffer;
            }

            /// <summary>
            /// 将包中的所有文件导出到指定目录
            /// </summary>
            /// <param name="dir">输出目录</param>
            public void OutputAllToDirectory(string dir)
            {
                var finnalDir = dir;
                if (!finnalDir.EndsWith("\\"))
                    finnalDir += "\\";

                if (!Directory.Exists(finnalDir))
                {
                    Directory.CreateDirectory(finnalDir);
                }

                for (int i = 0; i < _fileCount; i++)
                {
                    File.WriteAllBytes(finnalDir + this._shortNameList[i], GetBytes(i));
                }
            }

            /// <summary>
            /// 以新名字输出某个内容到文件
            /// </summary>
            /// <param name="index"></param>
            /// <param name="file"></param>
            public void OutputOneToFile(int index, string file)
            {
                File.WriteAllBytes(file, GetBytes(index));
            }

            /// <summary>
            /// 用原始文件名输出某个内容到指定文件夹
            /// </summary>
            /// <param name="index"></param>
            /// <param name="dir"></param>
            public void OutputOneToDirectory(int index, string dir)
            {
                string name = _shortNameList[index];
                File.WriteAllBytes(System.IO.Path.Combine(dir, name), GetBytes(index));
            }


            internal void Close()
            {
                if (_sourceFile != null)
                {
                    _br.Close();
                    _br = null;
                    _sourceFile.Close();
                    _sourceFile = null;
                }
            }

            #region 属性


            /// <summary>
            /// 当前包文件的文件名
            /// </summary>
            public string CurrentPackFilename => this._sourceFile.Name;

            /// <summary>
            /// 包中的文件数
            /// </summary>
            public int FileCount => this._fileCount;


            /// <summary>
            /// 包中所有文件的文件名
            /// </summary>
            public string[] Filenames => this._shortNameList.ToArray();

            /// <summary>
            /// 是否启用压缩
            /// </summary>
            public bool IsCompressed => this._isCompressed;

            #endregion


        }//CoalescentFile over



        /// <summary>
        /// 无效包文件异常
        /// </summary>
        public class InvalidPackFileException : Exception
        {
            public InvalidPackFileException(string text)
                : base(text)
            {
            }
        }




    }
}
