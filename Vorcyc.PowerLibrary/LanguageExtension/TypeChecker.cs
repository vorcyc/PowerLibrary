using System;

namespace Vorcyc.PowerLibrary.LanguageExtension
{

    /// <summary>
    /// 类型检查器扩展
    /// </summary>
    public static class TypeChecker
    {


        /// <summary>
        /// 获得一个值以说明实例对象是否是枚举类型。
        /// </summary>
        /// <param name="obj">实例对象</param>
        /// <returns></returns>
        public static bool IsEnum(this object obj) => obj is Enum;



        /// <summary>
        /// 获得一个值以说明实例对象是否是结构体类型。
        /// </summary>
        /// <param name="obj">实例对象</param>
        /// <returns></returns>
        public static bool IsStructure(this object obj)
            => (obj is ValueType) && !(obj is Enum);



    }
}
