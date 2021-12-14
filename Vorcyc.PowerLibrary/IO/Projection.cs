
namespace Vorcyc.PowerLibrary.IO
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using static ProjectionRuntimeHelper;


    /// <summary>
    /// “投影”提供一个新的持久化机制。用于将内存实体与磁盘文件进行方便的互映射。
    /// </summary>
    /// <remarks>
    /// <para>读取或赋值都需要是实例成员字段或属性，它们的访问修饰符可以是任意访问类别，且它们必须是可读可写的。</para>
    /// <para>顺序是提供灵活定义存储方式的途径，顺序值不一定要连续；它按升序排列。</para>
    /// <para>类型的属性或字段的类型只能是以下类型：<em>sbyte、byte、short、ushort、int、uint、long、ulong、char、float、double、decimal、bool、string和DateTime</em></para>
    /// <para>读取或写时，同一个类型不能重复读或写。</para>
    /// </remarks>
    /// <example>
    /// 以下代码定义两个实体类，其中的属性和字段均作为“投影成员”：
    /// <code>
    /// public class NewFileHead
    /// {
    ///     [ProjectionMember(3)]
    ///     public double LsbVal { get; set;}
    ///
    ///     [ProjectionMember(0)]
    ///     internal int SampleCount;
    ///
    ///     [ProjectionMember(1)]
    ///     public float Frequency;
    ///
    ///     [ProjectionMember(2, 5)]
    ///     public string Name;
    ///
    ///     [ProjectionMember(100, 15)]
    ///     public string Version {get; set;}
    ///
    /// }
    ///
    /// class MainWindowStatus
    /// {
    ///    [ProjectionMember(0)]
    ///    public double Width;
    ///
    ///    [ProjectionMember(1)]
    ///    public double Height;
    ///
    ///    [ProjectionMember(2)]
    ///    public int X { get; set; }
    ///
    ///    [ProjectionMember(3)]
    ///    public int Y { get; set; }
    /// }
    /// 
    /// </code>
    /// 以下代码创建上一步定义的两个类的实例，并为其属性和字段赋值：
    /// <code>
    /// var h = new NewFileHead()
    /// {
    ///    Frequency = 3.14f,
    ///    SampleCount = 1000,
    ///    LsbVal = 7,
    ///    Name = "dll_cyclone",
    ///    Version = "11.22",
    /// };
    ///
    /// var mws = new MainWindowStatus()
    /// {
    ///    Width = 800,
    ///    Height = 500,
    ///    X = 50,
    ///    Y = 100
    /// };
    /// </code>
    /// 在桌面创建INI文件，并按文顺序写实例：
    /// <code>
    /// var iniFile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "1.txt");
    /// Projection.Write(iniFile, ProjectionFormat.IniText, mws, h);
    /// </code>
    /// <em>得到如下形式的文件：</em>
    /// <code>
    /// <em>
    /// [MainWindowStatus]
    /// Width=800
    /// Height=500
    /// X=50
    /// Y=100
    /// [NewFileHead]
    /// SampleCount=1000
    /// Frequency=3.14
    /// Name=cyclone_dll
    /// LsbVal = 7
    /// Version=11.22
    /// </em>
    /// </code>
    /// 读取上一步创建的文件：
    /// <code>
    ///  //读取并填充指定单个实例
    ///  var mws2 = new MainWindowStatus();
    ///  Projection.Read(iniFile, ProjectionFormat.IniText, mws2);
    ///  
    /// //读取并填充多个实例，可以以任意顺序读取
    /// var h2 = new NewFileHead();
    /// var mws3 = new MainWindowStatus();
    /// Projection.Read(iniFile, ProjectionFormat.IniText, h2, mws3);
    /// 
    /// //另一种语法支持，以类型参数来读取，不需要创建实例。
    /// var(h3, mws4) = Projection.Read&lt;NewFileHead, MainWindowStatus&gt;(iniFile, ProjectionFormat.IniText);
    /// 
    /// </code>
    /// <strong>使用<see cref="ProjectionFormat.Binary"/>和是使用<see cref="ProjectionFormat.IniText"/>时的用法相同，只是存储格式不同。</strong>
    /// </example>
    public static class Projection
    {

        #region core

        private static void SetFieldOrPropValue(ProjectionMemberInfo fp, object obj, dynamic value)
        {
            if (fp is ProjectionFieldInfo f)
                f.FieldInfo.SetValue(obj, value);
            else if (fp is ProjectionPropertyInfo p)
                p.PropertyInfo.SetValue(obj, value);
        }

        //[Obsolete]
        //private static void ReadToObject_Binary_Old(BinaryReader reader, object obj)
        //{
        //    var fieldsAndProps = GetFieldsAndProps(obj);

        //    foreach (var fp in fieldsAndProps)
        //    {

        //        if (fp.TargetType == typeof(string))
        //        {
        //            if (!fp.Length.HasValue)
        //                throw new InvalidDataException("String type must has a length.");

        //            var size = fp.Length.Value * 2;

        //            var buffer = reader.ReadBytes(size);

        //            var value = ConvertBytes(buffer, fp.TargetType);

        //            SetFieldOrPropValue(fp, obj, value);

        //        }
        //        else
        //        {
        //            var size = SizeOf(fp.TargetType);

        //            var buffer = reader.ReadBytes(size);

        //            var value = ConvertBytes(buffer, fp.TargetType);

        //            SetFieldOrPropValue(fp, obj, value);
        //        }
        //    }
        //}


        private static void ReadToObject_Binary(BinaryReader reader, object obj)
        {
            var fieldsAndProps = GetFieldsAndProps(obj);

            var fourcc = ProjectionRuntimeHelper.GetFourCC('v', 'p', 'b', 'k');

            var targetBlockName = obj.GetType().Name;

            //未对齐的搜索方式
            //-3可以到尾巴，理论上应该-4，不知道咋说
            for (long pos = 0; pos < reader.BaseStream.Length - 3; pos++) {
                reader.BaseStream.Position = pos;

                if (reader.ReadInt32() == fourcc) {
                    //Console.WriteLine("pos" + pos);

                    var currentBlockName = reader.ReadString();
                    if (currentBlockName == targetBlockName) {
                        foreach (var fp in fieldsAndProps) {
                            var value = ReadFromBinaryReader(reader, fp.TargetType);

                            SetFieldOrPropValue(fp, obj, value);
                        }

                    }
                }
            }

        }

        //[Obsolete]
        //private static TEntity ReadToEntity_Binary_Old<TEntity>(BinaryReader reader)
        //{
        //    var resultObj = Activator.CreateInstance<TEntity>();
        //    ReadToObject_Binary_Old(reader, resultObj);
        //    return resultObj;
        //}



        private static TEntity ReadToEntity_Binary<TEntity>(BinaryReader reader)
        {
            var resultObj = Activator.CreateInstance<TEntity>();
            ReadToObject_Binary(reader, resultObj);
            return resultObj;
        }



        //按类名找块
        //每次都从文件头找，这样就可以取消严格按顺序的限制
        private static IDictionary<string, string> GetBlockItems_Text(StreamReader reader, string blockName)
        {
            var result = new Dictionary<string, string>();

            reader.BaseStream.Position = 0;

            var match = $"[{blockName}]";

            while (!reader.EndOfStream) {
                var line = reader.ReadLine();
                if (line == match) {
                    while (true) {
                        var next = reader.ReadLine();

                        //若是最后一行
                        if (string.IsNullOrEmpty(next) || string.IsNullOrWhiteSpace(next))
                            break;

                        if (next.StartsWith("["))
                            break;

                        if (next.StartsWith(";"))//未测试
                            break;

                        //Console.WriteLine(next);
                        var array = next.Split('=');
                        result.Add(array[0], array[1]);
                    }
                }
            }

            return result;
        }


        private static void ReadToObject_Text(StreamReader reader, object obj)
        {
            var fieldsAndProps = GetFieldsAndProps(obj);

            var blockItems = GetBlockItems_Text(reader, obj.GetType().Name);

            foreach (var fp in fieldsAndProps) {
                if (fp is ProjectionPropertyInfo pi)
                    pi.PropertyInfo.SetValue(obj, ConvertText(blockItems[pi.PropertyInfo.Name], pi.PropertyInfo.PropertyType));
                else if (fp is ProjectionFieldInfo fi)
                    fi.FieldInfo.SetValue(obj, ConvertText(blockItems[fi.FieldInfo.Name], fi.FieldInfo.FieldType));
            }
        }

        private static TEntity ReadToEntity_Text<TEntity>(StreamReader reader)
        {
            var resultObj = Activator.CreateInstance<TEntity>();
            ReadToObject_Text(reader, resultObj);
            return resultObj;
        }

        #endregion 

        /// <summary>
        /// 读取指定文件，并按顺序将文件中的数据读取到指定的一或多个类实例。
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="format">读取格式</param>
        /// <param name="objects">待写入属性和字段值的实例集</param>
        public static void Read(string filename, ProjectionFormat format, params object[] objects)
        {
            using (var fs = new FileStream(filename, FileMode.Open)) {

                if (format == ProjectionFormat.Binary) {
                    var reader = new BinaryReader(fs);

                    foreach (var obj in objects) {
                        ReadToObject_Binary(reader, obj);
                    }
                }
                else if (format == ProjectionFormat.IniText) {
                    var reader = new StreamReader(fs);

                    foreach (var obj in objects) {
                        ReadToObject_Text(reader, obj);
                    }
                }
            }

        }

        /// <summary>
        /// 读取指定文件，创建并填充指定类型的单个实例。
        /// </summary>
        /// <typeparam name="TEntity">实体类型，约束为：类、具有默认构造函数</typeparam>
        /// <param name="filename">文件名</param>
        /// <param name="format">读取格式</param>
        /// <returns>指定类型的实例</returns>
        public static TEntity Read<TEntity>(string filename, ProjectionFormat format)
            where TEntity : class, new()
        {
            using (var fs = new FileStream(filename, FileMode.Open)) {
                if (format == ProjectionFormat.Binary) {
                    var reader = new BinaryReader(fs);
                    return ReadToEntity_Binary<TEntity>(reader);
                }
                else if (format == ProjectionFormat.IniText) {
                    var reader = new StreamReader(fs);
                    return ReadToEntity_Text<TEntity>(reader);
                }
            }
            return null;
        }

        /// <summary>
        /// 读取指定文件，创建并填充指定多个类型的多个实例。
        /// </summary>
        /// <typeparam name="TEntity1">实体类型，约束为：类、具有默认构造函数</typeparam>
        /// <typeparam name="TEntity2">实体类型，约束为：类、具有默认构造函数</typeparam>
        /// <param name="filename">文件名</param>
        /// <param name="format">读取格式</param>
        /// <returns>多个指定类型的实例的<see cref="System.ValueTuple{T1,T2}"/></returns>
        public static ValueTuple<TEntity1, TEntity2>
            Read<TEntity1, TEntity2>(string filename, ProjectionFormat format)
            where TEntity1 : class, new()
            where TEntity2 : class, new()

        {
            using (var fs = new FileStream(filename, FileMode.Open)) {
                if (format == ProjectionFormat.Binary) {
                    var reader = new BinaryReader(fs);
                    return (ReadToEntity_Binary<TEntity1>(reader), ReadToEntity_Binary<TEntity2>(reader));
                }
                else if (format == ProjectionFormat.IniText) {
                    var reader = new StreamReader(fs);
                    return (ReadToEntity_Text<TEntity1>(reader), ReadToEntity_Text<TEntity2>(reader));
                }
            }
            return (null, null);
        }

        /// <summary>
        /// 读取指定文件，创建并填充指定多个类型的多个实例。
        /// </summary>
        /// <typeparam name="TEntity1">实体类型，约束为：类、具有默认构造函数</typeparam>
        /// <typeparam name="TEntity2">实体类型，约束为：类、具有默认构造函数</typeparam>
        /// <typeparam name="TEntity3">实体类型，约束为：类、具有默认构造函数</typeparam>
        /// <param name="filename">文件名</param>
        /// <param name="format">读取格式</param>
        /// <returns>多个指定类型的实例的<see cref="System.ValueTuple{T1,T2,T3}"/></returns>
        public static ValueTuple<TEntity1, TEntity2, TEntity3>
            Read<TEntity1, TEntity2, TEntity3>(string filename, ProjectionFormat format)
            where TEntity1 : class, new()
            where TEntity2 : class, new()
            where TEntity3 : class, new()

        {
            using (var fs = new FileStream(filename, FileMode.Open)) {
                if (format == ProjectionFormat.Binary) {
                    var reader = new BinaryReader(fs);
                    return
                        (
                        ReadToEntity_Binary<TEntity1>(reader),
                        ReadToEntity_Binary<TEntity2>(reader),
                        ReadToEntity_Binary<TEntity3>(reader)
                        );
                }
                else if (format == ProjectionFormat.IniText) {
                    var reader = new StreamReader(fs);
                    return
                        (
                        ReadToEntity_Text<TEntity1>(reader),
                        ReadToEntity_Text<TEntity2>(reader),
                        ReadToEntity_Text<TEntity3>(reader)
                        );
                }
            }
            return (null, null, null);
        }


        /// <summary>
        /// 读取指定文件，创建并填充指定多个类型的多个实例。
        /// </summary>
        /// <typeparam name="TEntity1">实体类型，约束为：类、具有默认构造函数</typeparam>
        /// <typeparam name="TEntity2">实体类型，约束为：类、具有默认构造函数</typeparam>
        /// <typeparam name="TEntity3">实体类型，约束为：类、具有默认构造函数</typeparam>
        /// <typeparam name="TEntity4">实体类型，约束为：类、具有默认构造函数</typeparam>
        /// <param name="filename">文件名</param>
        /// <param name="format">读取格式</param>
        /// <returns>多个指定类型的实例的<see cref="System.ValueTuple{T1,T2,T3,T4}"/></returns>
        public static ValueTuple<TEntity1, TEntity2, TEntity3, TEntity4>
            Read<TEntity1, TEntity2, TEntity3, TEntity4>(string filename, ProjectionFormat format)
            where TEntity1 : class, new()
            where TEntity2 : class, new()
            where TEntity3 : class, new()
            where TEntity4 : class, new()
        {
            using (var fs = new FileStream(filename, FileMode.Open)) {
                if (format == ProjectionFormat.Binary) {
                    var reader = new BinaryReader(fs);
                    return
                        (
                        ReadToEntity_Binary<TEntity1>(reader),
                        ReadToEntity_Binary<TEntity2>(reader),
                        ReadToEntity_Binary<TEntity3>(reader),
                        ReadToEntity_Binary<TEntity4>(reader)
                        );
                }
                else if (format == ProjectionFormat.IniText) {
                    var reader = new StreamReader(fs);
                    return
                        (
                        ReadToEntity_Text<TEntity1>(reader),
                        ReadToEntity_Text<TEntity2>(reader),
                        ReadToEntity_Text<TEntity3>(reader),
                        ReadToEntity_Text<TEntity4>(reader)
                        );
                }
            }
            return (null, null, null, null);
        }


        /// <summary>
        /// 读取指定文件，创建并填充指定多个类型的多个实例。
        /// </summary>
        /// <typeparam name="TEntity1">实体类型，约束为：类、具有默认构造函数</typeparam>
        /// <typeparam name="TEntity2">实体类型，约束为：类、具有默认构造函数</typeparam>
        /// <typeparam name="TEntity3">实体类型，约束为：类、具有默认构造函数</typeparam>
        /// <typeparam name="TEntity4">实体类型，约束为：类、具有默认构造函数</typeparam>
        /// <typeparam name="TEntity5">实体类型，约束为：类、具有默认构造函数</typeparam>
        /// <param name="filename">文件名</param>
        /// <param name="format">读取格式</param>
        /// <returns>多个指定类型的实例的<see cref="System.ValueTuple{T1,T2,T3,T4,T5}"/></returns>
        public static ValueTuple<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5>
            Read<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5>(string filename, ProjectionFormat format)
            where TEntity1 : class, new()
            where TEntity2 : class, new()
            where TEntity3 : class, new()
            where TEntity4 : class, new()
            where TEntity5 : class, new()
        {
            using (var fs = new FileStream(filename, FileMode.Open)) {
                if (format == ProjectionFormat.Binary) {
                    var reader = new BinaryReader(fs);
                    return
                        (
                        ReadToEntity_Binary<TEntity1>(reader),
                        ReadToEntity_Binary<TEntity2>(reader),
                        ReadToEntity_Binary<TEntity3>(reader),
                        ReadToEntity_Binary<TEntity4>(reader),
                        ReadToEntity_Binary<TEntity5>(reader)
                        );
                }
                else if (format == ProjectionFormat.IniText) {
                    var reader = new StreamReader(fs);
                    return
                        (
                        ReadToEntity_Text<TEntity1>(reader),
                        ReadToEntity_Text<TEntity2>(reader),
                        ReadToEntity_Text<TEntity3>(reader),
                        ReadToEntity_Text<TEntity4>(reader),
                        ReadToEntity_Text<TEntity5>(reader)
                        );
                }
            }
            return (null, null, null, null, null);
        }

        /// <summary>
        /// 读取指定文件，创建并填充指定多个类型的多个实例。
        /// </summary>
        /// <typeparam name="TEntity1">实体类型，约束为：类、具有默认构造函数</typeparam>
        /// <typeparam name="TEntity2">实体类型，约束为：类、具有默认构造函数</typeparam>
        /// <typeparam name="TEntity3">实体类型，约束为：类、具有默认构造函数</typeparam>
        /// <typeparam name="TEntity4">实体类型，约束为：类、具有默认构造函数</typeparam>
        /// <typeparam name="TEntity5">实体类型，约束为：类、具有默认构造函数</typeparam>
        /// <typeparam name="TEntity6">实体类型，约束为：类、具有默认构造函数</typeparam>
        /// <param name="filename">文件名</param>
        /// <param name="format">读取格式</param>
        /// <returns>多个指定类型的实例的<see cref="System.ValueTuple{T1,T2,T3,T4,T5,T6}"/></returns>
        public static ValueTuple<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6>
            Read<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6>(string filename, ProjectionFormat format)
            where TEntity1 : class, new()
            where TEntity2 : class, new()
            where TEntity3 : class, new()
            where TEntity4 : class, new()
            where TEntity5 : class, new()
            where TEntity6 : class, new()
        {
            using (var fs = new FileStream(filename, FileMode.Open)) {
                if (format == ProjectionFormat.Binary) {
                    var reader = new BinaryReader(fs);
                    return
                        (
                        ReadToEntity_Binary<TEntity1>(reader),
                        ReadToEntity_Binary<TEntity2>(reader),
                        ReadToEntity_Binary<TEntity3>(reader),
                        ReadToEntity_Binary<TEntity4>(reader),
                        ReadToEntity_Binary<TEntity5>(reader),
                        ReadToEntity_Binary<TEntity6>(reader)
                        );
                }
                else if (format == ProjectionFormat.IniText) {
                    var reader = new StreamReader(fs);
                    return
                        (
                        ReadToEntity_Text<TEntity1>(reader),
                        ReadToEntity_Text<TEntity2>(reader),
                        ReadToEntity_Text<TEntity3>(reader),
                        ReadToEntity_Text<TEntity4>(reader),
                        ReadToEntity_Text<TEntity5>(reader),
                        ReadToEntity_Text<TEntity6>(reader)
                        );
                }
            }
            return (null, null, null, null, null, null);
        }


        /// <summary>
        /// 读取指定文件，创建并填充指定多个类型的多个实例。
        /// </summary>
        /// <typeparam name="TEntity1">实体类型，约束为：类、具有默认构造函数</typeparam>
        /// <typeparam name="TEntity2">实体类型，约束为：类、具有默认构造函数</typeparam>
        /// <typeparam name="TEntity3">实体类型，约束为：类、具有默认构造函数</typeparam>
        /// <typeparam name="TEntity4">实体类型，约束为：类、具有默认构造函数</typeparam>
        /// <typeparam name="TEntity5">实体类型，约束为：类、具有默认构造函数</typeparam>
        /// <typeparam name="TEntity6">实体类型，约束为：类、具有默认构造函数</typeparam>
        /// <typeparam name="TEntity7">实体类型，约束为：类、具有默认构造函数</typeparam>
        /// <param name="filename">文件名</param>
        /// <param name="format">读取格式</param>
        /// <returns>多个指定类型的实例的<see cref="System.ValueTuple{T1,T2,T3,T4,T5,T6,T7}"/></returns>
        public static ValueTuple<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7>
            Read<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7>(string filename, ProjectionFormat format)
            where TEntity1 : class, new()
            where TEntity2 : class, new()
            where TEntity3 : class, new()
            where TEntity4 : class, new()
            where TEntity5 : class, new()
            where TEntity6 : class, new()
            where TEntity7 : class, new()
        {
            using (var fs = new FileStream(filename, FileMode.Open)) {
                if (format == ProjectionFormat.Binary) {
                    var reader = new BinaryReader(fs);
                    return
                        (
                        ReadToEntity_Binary<TEntity1>(reader),
                        ReadToEntity_Binary<TEntity2>(reader),
                        ReadToEntity_Binary<TEntity3>(reader),
                        ReadToEntity_Binary<TEntity4>(reader),
                        ReadToEntity_Binary<TEntity5>(reader),
                        ReadToEntity_Binary<TEntity6>(reader),
                        ReadToEntity_Binary<TEntity7>(reader)
                        );
                }
                else if (format == ProjectionFormat.IniText) {
                    var reader = new StreamReader(fs);
                    return
                        (
                        ReadToEntity_Text<TEntity1>(reader),
                        ReadToEntity_Text<TEntity2>(reader),
                        ReadToEntity_Text<TEntity3>(reader),
                        ReadToEntity_Text<TEntity4>(reader),
                        ReadToEntity_Text<TEntity5>(reader),
                        ReadToEntity_Text<TEntity6>(reader),
                        ReadToEntity_Text<TEntity7>(reader)
                        );
                }
            }
            return (null, null, null, null, null, null, null);
        }




        /// <summary>
        /// 将多个实例的指定属性和字段以指定格式写入文件。
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="format">格式</param>
        /// <param name="objects">多个对象实例</param>
        public static void Write(string filename, ProjectionFormat format, params object[] objects)
        {
            if (format == ProjectionFormat.Binary) {
                var fourcc = ProjectionRuntimeHelper.GetFourCC('v', 'p', 'b', 'k');

                using (var fs = new FileStream(filename, FileMode.Create)) {
                    var writer = new BinaryWriter(fs);

                    foreach (var obj in objects) {
                        writer.Write(fourcc);

                        writer.Write(obj.GetType().Name);

                        var fieldsAndProps = GetFieldsAndProps(obj);

                        foreach (var f in fieldsAndProps) {
                            WriteField_Binary(writer, obj, f);
                        }
                    }
                }
            }
            else if (format == ProjectionFormat.IniText)
            {
                using (var fs = new FileStream(filename, FileMode.Create)) {
                    var writer = new StreamWriter(fs);

                    foreach (var obj in objects) {
                        var fieldsAndProps = GetFieldsAndProps(obj);

                        writer.WriteLine($"[{obj.GetType().Name}]");

                        foreach (var fp in fieldsAndProps) {
                            if (fp is ProjectionFieldInfo fi) {
                                writer.WriteLine($"{fi.FieldInfo.Name}={fi.FieldInfo.GetValue(obj)}");
                            }
                            else if (fp is ProjectionPropertyInfo pi) {
                                writer.WriteLine($"{pi.PropertyInfo.Name}={pi.PropertyInfo.GetValue(obj)}");
                            }
                        }
                    }

                    writer.Flush();
                }
            }
        }




    }



}
