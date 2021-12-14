
namespace Vorcyc.PowerLibrary.IO
{
    using System;
    using System.Reflection;

    internal abstract class ProjectionMemberInfo
    {
        internal Type TargetType;
        internal int Order;
    }

    internal class ProjectionFieldInfo : ProjectionMemberInfo
    {
        internal FieldInfo FieldInfo;
    }

    internal class ProjectionPropertyInfo : ProjectionMemberInfo
    {
        internal PropertyInfo PropertyInfo;
    }


    /// <summary>
    /// 标记一个类型的可读、可写字段，以使用<see cref="Projection"/>。
    /// </summary>
    /// <remarks>
    /// 字段类型只能是.NET 的基元类型。
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ProjectionMemberAttribute : Attribute
    {

        private int _order;

        /// <summary>
        /// 标记一个成员的可读、可写字段，以使用<see cref="Projection"/>。
        /// </summary>
        public ProjectionMemberAttribute()
        {
            _order = int.MaxValue;
        }


        /// <summary>
        /// 标记一个成员的可读、可写字段，以使用<see cref="Projection"/>。
        /// </summary>
        /// <param name="order">顺序</param>
        public ProjectionMemberAttribute(int order)
        {
            _order = order;
        }


        ///// <summary>
        ///// 标记一个成员字段（可读、可写），并按特定类型来读取（类型必须是.NET的基元类型，该类型也是实际用于存储的类型）。
        ///// </summary>
        ///// <param name="order">顺序</param>
        ///// <param name="readAs">指定以某种类型读取</param>
        //public HeadFieldAttribute(int order, Type readAs = null)
        //{
        //    _order = order;
        //    _readAs = readAs;
        //    _length = null;
        //}


        /// <summary>
        /// 顺序
        /// </summary>
        public int Order => _order;

    }
}
