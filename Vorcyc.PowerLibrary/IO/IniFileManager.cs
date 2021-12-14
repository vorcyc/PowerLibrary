
/*[Config]
 * IpAddress = 0.0.0.0
 * Port = 30000
 */

namespace Vorcyc.PowerLibrary.IO
{
    using System.Collections.Generic;
    using System.IO;

    
    [System.Obsolete("Use Projection class instead.")]
    public sealed class IniFileManager
    {

        public sealed class IniBlock
        {
            private Dictionary<string, string> _fields =
                new Dictionary<string, string>();

            public Dictionary<string, string> Fields
            {
                get { return _fields; }
            }
        }



        private Dictionary<string, IniBlock> _blocks = new Dictionary<string, IniBlock>();


        public void Load(string filename, System.Text.Encoding encoding)
        {
            var reader = new StreamReader(filename, encoding);

            bool IsBlock(string text)
            {
                return (text.StartsWith("[") && text.EndsWith("]"));
            }

            bool IsField(string text)
            {
                if (string.IsNullOrEmpty(text)) return false;
                return (!text.StartsWith("[") & (text.Contains("=")));
            }

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (IsBlock(line))
                {
                    var blockName = line.Substring(1, line.Length - 2);
                    var block = new IniBlock();

                    line = reader.ReadLine();

                    while (IsField(line))
                    {
                        var kvp = SpilitField(line);
                        block.Fields.Add(kvp.Key, kvp.Value);

                        line = reader.ReadLine();
                    }

                    line = reader.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        var lastField = SpilitField(line);
                        block.Fields.Add(lastField.Key, lastField.Value);
                    }

                    _blocks.Add(blockName, block);
                }
            }
        }

        /// <summary>
        /// 帮助函数，分离Field的键值部分
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private KeyValuePair<string, string> SpilitField(string text)
        {
            var sa = text.Split('=');
            return new KeyValuePair<string, string>(sa[0], sa[1]);
        }


        public void Save(string filename)
        {
            using (var sw = new StreamWriter(filename))
            {
                foreach (var kvp in _blocks)
                {
                    sw.WriteLine($"[{kvp.Key}]");
                    foreach (var b in kvp.Value.Fields)
                    {
                        sw.WriteLine($"{b.Key}={b.Value}");
                    }
                }
            }

        }



        public IDictionary<string, IniBlock> Blocks
        {
            get { return _blocks; }
        }



    }
}
