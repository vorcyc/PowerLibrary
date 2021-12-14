using System;
using System.Collections.Generic;
using System.Text;

namespace Vorcyc.PowerLibrary.IO
{
    /// <summary>
    /// Represents a path of the file system which can be a file or a directory.
    /// </summary>
    public sealed class Path
    {

        private string _path;

        public Path(string path) => _path = path;


        public static implicit operator string(Path p) => p._path;

        public static implicit operator Path(string path) => new Path(path);


        public static Path operator /(Path left, string right)
        {
            if (left._path.EndsWith('/'))
                return left._path + right;
            else
                return left._path + '/' + right;
        }


        public static Path operator /(Path left, Path right)
        {
            if (left._path.EndsWith('/'))
                return left._path + right._path;
            else
                return left._path + '/' + right._path;
        }


        public static Path operator /(string left, Path right)
        {
            if (left.EndsWith('/'))
                return left + right._path;
            else
                return left + '/' + right._path;
        }


        public static Path operator +(Path left, string right) => left / right;
        public static Path operator +(string left, Path right) => left / right;
        public static Path operator +(Path left, Path right) => left / right;


        /// <summary>
        /// Gets if the content of the path exists as a file.
        /// </summary>
        public bool FileExists => System.IO.File.Exists(this);

        /// <summary>
        /// Gets if the content of the path exists as a Directory.
        /// </summary>
        public bool DirectoryExists => System.IO.Directory.Exists(this);

    }
}
