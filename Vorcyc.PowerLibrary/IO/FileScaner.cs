using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using Path = System.IO.Path;

namespace Vorcyc.PowerLibrary.IO
{
    public class FileScaner
    {
        private Thread _workThread;
        private string[] _exts;
        private SynchronizationContext _syncCtxt = SynchronizationContext.Current;

        private bool _isScanning;

        /// <summary>
        /// 是否是全局扫描
        /// </summary>
        public bool IsGlobalScanning { get; set; }

        private string _searchPath;

        /// <summary>
        /// 非全局扫描的搜索路径
        /// </summary>
        /// <exception cref="DirectoryNotFoundException"/>
        public string SearchPath { 
            get {  return _searchPath;  } 
            set
            {
                if (value.Length <= 3)
                    throw new DirectoryNotFoundException("不存在这样的文件夹");
                if (Directory.Exists(value))
                    _searchPath = value;
                else
                    throw new DirectoryNotFoundException("不存在这样的文件夹");
            }
        }



        /// <summary>
        /// 开始全局异步扫描
        /// </summary>
        /// <param name="syncResult">扫描结果的存储</param>
        /// <param name="exts">小写扩展名如: ".mp3",".wav"</param>
        public void BeginScan(List<string> syncResult, params string[] exts)
        {
            if (!_isScanning)
            {
                _exts = exts;
                _isScanning = true;
                //IsGlonalScanning = true;
                _workThread = new Thread(new ParameterizedThreadStart(Scan));
                _workThread.Start(syncResult);

                //Console.WriteLine("开始扫描文件");
            }
        }

        /// <summary>
        /// 停止异步扫描
        /// </summary>
        public void EndScan()
        {
            if (_workThread != null)
            {
                _workThread.Abort();
                _workThread = null;
            }
            _isScanning = false;
        }

        private void Scan(object r)
        {
            List<string> result = (List<string>)r;
            if (IsGlobalScanning)
            {
                foreach (var folder in GetAllRootFolders())
                    AddFile(folder, result);
            }
            else
            {
                if (string.IsNullOrEmpty(SearchPath))
                    throw new Exception ("非全局扫描需要先设置 SearchPath 属性");
                AddFile(_searchPath, result);
            }

            //扫描完成
            if (ScanCompleted != null)
            {
                if (_syncCtxt != null)
                    _syncCtxt.Send(ScanCompleted,null );
                else
                    ScanCompleted(EventArgs.Empty);
            }
            _isScanning = false;
        }//Scan method over


        private void AddFile(string folderPath, List<string> r)
        {
            try
            {

                foreach (var f in Directory.EnumerateFiles(folderPath, "*", SearchOption.AllDirectories))
                {
                    if (_exts.Contains(System.IO.Path.GetExtension(f).ToLower()))
                    {
                        r.Add(f);

                        if (FileFound != null)
                        {
                            if (_syncCtxt != null)
                                _syncCtxt.Send(FileFound,  new FileFoundEventArgs(f));
                            else
                                FileFound(new FileFoundEventArgs(f));
                        }
                    }
                }
            }
            catch (Exception)
            {

            }

        }

        //得到可用逻辑盘中的根目录的文件夹
        private string[] GetAllRootFolders()
        {
            List<string> folders = new List<string>();
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    foreach (var folder in Directory.GetDirectories(drive.Name))
                    {
                        if ((new DirectoryInfo(folder).Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)//只扫描非隐藏文件夹
                        {
                            folders.Add(folder);
                        }
                    }
                }
            }
            return folders.ToArray();
        }

        /// <summary>
        /// 找到匹配扩展名的文件时引发
        /// </summary>
        public event SendOrPostCallback FileFound;
        /// <summary>
        /// 异步扫描结束时引发
        /// </summary>
        public event SendOrPostCallback ScanCompleted;

        /// <summary>
        /// 文件找事件的参数
        /// </summary>
        public class FileFoundEventArgs : EventArgs
        {
            private string _file;

            public FileFoundEventArgs(string file)
            {
                _file = file;
            }

            public string File { get { return _file; } }
        }
    }
}
