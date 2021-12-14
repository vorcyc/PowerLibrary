
namespace Vorcyc.PowerLibrary.IO
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;
    using System.Linq;
    using System.IO;

    internal static class ProjectionRuntimeHelper
    {

        public static IEnumerable<ProjectionMemberInfo> GetFieldsAndProps(object obj)
        {
            var type = obj.GetType();
            //all instance fields
            var fields = type.GetFields(
                            BindingFlags.Public |
                            BindingFlags.NonPublic |
                            BindingFlags.Instance |
                            BindingFlags.GetField |
                            BindingFlags.SetField);


            var temp = new List<ProjectionMemberInfo>();
            foreach (var f in fields) {
                if (f.IsDefined(typeof(ProjectionMemberAttribute))) {
                    //取Attr
                    var fieldAttr = f.GetCustomAttribute<ProjectionMemberAttribute>(false);

                    var fInfo = new ProjectionFieldInfo
                    {
                        FieldInfo = f,
                        Order = fieldAttr.Order,
                        TargetType = f.FieldType,
                        //Length = fieldAttr.Length,
                    };

                    temp.Add(fInfo);
                }
            }



            var props = type.GetProperties(
                            BindingFlags.Public |
                            BindingFlags.NonPublic |
                            BindingFlags.Instance |
                            BindingFlags.GetProperty |
                            BindingFlags.SetProperty);

            foreach (var p in props) {
                if (p.IsDefined(typeof(ProjectionMemberAttribute))) {
                    var attr = p.GetCustomAttribute<ProjectionMemberAttribute>();

                    var info = new ProjectionPropertyInfo
                    {
                        PropertyInfo = p,
                        Order = attr.Order,
                        TargetType = p.PropertyType,
                        //Length = attr.Length,
                    };

                    temp.Add(info);
                }
            }

            return temp.OrderBy(f => f.Order);

        }



        //public static dynamic ConvertBytes(byte[] buffer, Type targetType)
        //{
        //    dynamic value = null;

        //    if (targetType == typeof(byte)) {
        //        value = buffer[0];
        //    }
        //    else if (targetType == typeof(short)) {
        //        value = BitConverter.ToInt16(buffer, 0);
        //    }
        //    else if (targetType == typeof(int)) {
        //        value = BitConverter.ToInt32(buffer, 0);
        //    }
        //    else if (targetType == typeof(long)) {
        //        value = BitConverter.ToInt64(buffer, 0);
        //    }
        //    else if (targetType == typeof(float)) {
        //        value = BitConverter.ToSingle(buffer, 0);
        //    }
        //    else if (targetType == typeof(double)) {
        //        value = BitConverter.ToDouble(buffer, 0);
        //    }
        //    else if (targetType == typeof(ushort)) {
        //        value = BitConverter.ToUInt16(buffer, 0);
        //    }
        //    else if (targetType == typeof(uint)) {
        //        value = BitConverter.ToUInt32(buffer, 0);
        //    }
        //    else if (targetType == typeof(ulong)) {
        //        value = BitConverter.ToUInt64(buffer, 0);
        //    }
        //    else if (targetType == typeof(string)) {
        //        value = Encoding.Unicode.GetString(buffer);
        //    }

        //    //.,......
        //    //...
        //    //.
        //    //.
        //    return value;
        //}


        public static dynamic ReadFromBinaryReader(BinaryReader reader, Type targetType)
        {
            if (targetType == typeof(byte)) {
                return reader.ReadByte();
            }
            else if (targetType == typeof(short)) {
                return reader.ReadInt16();
            }
            else if (targetType == typeof(int)) {
                return reader.ReadInt32();
            }
            else if (targetType == typeof(long)) {
                return reader.ReadInt64();
            }
            else if (targetType == typeof(float)) {
                return reader.ReadSingle();
            }
            else if (targetType == typeof(double)) {
                return reader.ReadDouble();
            }
            else if (targetType == typeof(ushort)) {
                return reader.ReadUInt16();
            }
            else if (targetType == typeof(uint)) {
                return reader.ReadUInt32();
            }
            else if (targetType == typeof(ulong)) {
                return reader.ReadUInt64();
            }
            else if (targetType == typeof(string)) {
                return reader.ReadString();
            }
            else if (targetType == typeof(decimal)) {
                return reader.ReadDecimal();
            }
            else if (targetType == typeof(DateTime)) {
                return DateTime.Parse(reader.ReadString());
            }

            return null;
        }


        public static dynamic ConvertText(string text, Type targetType)
        {
            dynamic value = null;

            if (targetType == typeof(byte)) {
                value = Convert.ToByte(text);
            }
            else if (targetType == typeof(short)) {
                value = Convert.ToInt16(text);
            }
            else if (targetType == typeof(int)) {
                value = Convert.ToInt32(text);
            }
            else if (targetType == typeof(long)) {
                value = Convert.ToInt64(text);
            }
            else if (targetType == typeof(float)) {
                value = Convert.ToSingle(text);
            }
            else if (targetType == typeof(double)) {
                value = Convert.ToDouble(text);
            }
            else if (targetType == typeof(ushort)) {
                value = Convert.ToUInt16(text);
            }
            else if (targetType == typeof(uint)) {
                value = Convert.ToUInt32(text);
            }
            else if (targetType == typeof(ulong)) {
                value = Convert.ToUInt64(text);
            }
            else if (targetType == typeof(string)) {
                value = text;//Encoding.Unicode.GetString(buffer);
            }
            else if (targetType == typeof(decimal)) {
                value = Convert.ToDecimal(text);
            }
            else if (targetType == typeof(DateTime)) {
                value = DateTime.Parse(text);
            }

            return value;
        }


        public static int GetFourCC(char ch0, char ch1, char ch2, char ch3)
        {
            int num = 0;
            num |= ch0;
            num |= ch1 << 8;
            num |= ch2 << 16;
            return (num | (ch3 << 24));
        }




        internal static void WriteField_Binary(BinaryWriter writer, object obj, ProjectionMemberInfo info)
        {
            if (info.TargetType == typeof(byte)) {
                if (info is ProjectionFieldInfo fi) {
                    var value = (byte)fi.FieldInfo.GetValue(obj);
                    writer.Write(value);
                }
                else if (info is ProjectionPropertyInfo pi) {
                    var value = (byte)pi.PropertyInfo.GetValue(obj);
                    writer.Write(value);
                }
            }
            else if (info.TargetType == typeof(short)) {
                if (info is ProjectionFieldInfo fi) {
                    var value = (short)fi.FieldInfo.GetValue(obj);
                    writer.Write(value);
                }
                else if (info is ProjectionPropertyInfo pi) {
                    var value = (short)pi.PropertyInfo.GetValue(obj);
                    writer.Write(value);
                }
            }
            else if (info.TargetType == typeof(int)) {
                if (info is ProjectionFieldInfo fi) {
                    var value = (int)fi.FieldInfo.GetValue(obj);
                    writer.Write(value);
                }
                else if (info is ProjectionPropertyInfo pi) {
                    var value = (int)pi.PropertyInfo.GetValue(obj);
                    writer.Write(value);
                }
            }
            else if (info.TargetType == typeof(long)) {
                if (info is ProjectionFieldInfo fi) {
                    var value = (long)fi.FieldInfo.GetValue(obj);
                    writer.Write(value);
                }
                else if (info is ProjectionPropertyInfo pi) {
                    var value = (long)pi.PropertyInfo.GetValue(obj);
                    writer.Write(value);
                }
            }
            else if (info.TargetType == typeof(float)) {
                if (info is ProjectionFieldInfo fi) {
                    var value = (float)fi.FieldInfo.GetValue(obj);
                    writer.Write(value);
                }
                else if (info is ProjectionPropertyInfo pi) {
                    var value = (float)pi.PropertyInfo.GetValue(obj);
                    writer.Write(value);
                }
            }
            else if (info.TargetType == typeof(double)) {
                if (info is ProjectionFieldInfo fi) {
                    var value = (double)fi.FieldInfo.GetValue(obj);
                    writer.Write(value);
                }
                else if (info is ProjectionPropertyInfo pi) {
                    var value = (double)pi.PropertyInfo.GetValue(obj);
                    writer.Write(value);
                }
            }
            else if (info.TargetType == typeof(ushort)) {
                if (info is ProjectionFieldInfo fi) {
                    var value = (ushort)fi.FieldInfo.GetValue(obj);
                    writer.Write(value);
                }
                else if (info is ProjectionPropertyInfo pi) {
                    var value = (ushort)pi.PropertyInfo.GetValue(obj);
                    writer.Write(value);
                }
            }
            else if (info.TargetType == typeof(uint)) {
                if (info is ProjectionFieldInfo fi) {
                    var value = (uint)fi.FieldInfo.GetValue(obj);
                    writer.Write(value);
                }
                else if (info is ProjectionPropertyInfo pi) {
                    var value = (uint)pi.PropertyInfo.GetValue(obj);
                    writer.Write(value);
                }
            }
            else if (info.TargetType == typeof(ulong)) {
                if (info is ProjectionFieldInfo fi) {
                    var value = (ulong)fi.FieldInfo.GetValue(obj);
                    writer.Write(value);
                }
                else if (info is ProjectionPropertyInfo pi) {
                    var value = (ulong)pi.PropertyInfo.GetValue(obj);
                    writer.Write(value);
                }
            }
            else if (info.TargetType == typeof(string)) {
                string value = string.Empty;
                if (info is ProjectionFieldInfo fi) {
                    value = (string)fi.FieldInfo.GetValue(obj);
                }
                else if (info is ProjectionPropertyInfo pi) {
                    value = (string)pi.PropertyInfo.GetValue(obj);
                }

                //老模式的
                //var strBuffer = System.Text.Encoding.Unicode.GetBytes(value);
                ////因为是utf16，所以3个字符是6个字节，也就是有几个字符，字节数要对应×2
                ////那么不够的话要补0，超出的话就截断
                //var buffer = new byte[info.Length.Value * 2];
                //var minLen = Math.Min(info.Length.Value * 2, strBuffer.Length);
                //System.Buffer.BlockCopy(strBuffer, 0, buffer, 0, minLen);
                //writer.Write(buffer);

                //新模式的不限制长度，直接写即可
                writer.Write(value);
            }
        }

    }
}
