using System;
using System.Linq;
using System.Reflection;

namespace Vorcyc.PowerLibrary.Runtime
{


    /// <summary>
    /// 对象工具
    /// </summary>
    public static class ObjectHelper
    {

        #region ShadowCopyPropertyValues

        /// <summary>
        /// 从源对象中将具有同名称的属性的值拷贝到目标对象。
        /// <para>在很多场景下，这可以极大减少手动编写代码过程。</para>
        /// </summary> 
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="excludes">排除</param>
        /// <remarks>
        /// <para>本方法只对实体的公共实例属性有效</para>
        /// <para>源对象的属性需要是可读的，目标对象的属性需要是可写的</para>
        /// </remarks>
        /// <example>
        /// 以下代码用于创建一个源对象：
        /// <code>
        /// public class SourceObj
        /// {
        ///    public string StrValue { get; set; }
        ///  
        ///    public string hahahah { get; set; }
        /// 
        ///    public string hahahah2 { get; set; }
        /// }
        /// </code>
        /// 创建目标对象：
        /// <code>
        /// public class TargetObj
        /// {
        ///    private int _intValue;
        ///    public int IntValue => _intValue;
        ///
        ///    public string StrValue {get; set;}
        ///
        ///    public string hahahah {get; set;}
        ///
        ///    public string hahahah2 {get; set;}
        /// }
        /// </code>
        /// 拷贝对象值，并排除名字为 hahahah2 的属性。注意，当目标对象的属性不是可写的时，无法赋值。
        /// <code>
        /// var src = new SourceObj { StrValue = "xxxx", hahahah = "rrrrr", hahahah2 = "22" };
        /// var target = new TargetObj() { StrValue = "123123" };
        ///
        /// Vorcyc.PowerLibrary.Runtime.ObjectHelper.ShadowCopyPropertyValues(src, target,"hahahah2");
        ///
        /// Console.WriteLine(target.StrValue); => xxxx
        /// Console.WriteLine(target.IntValue); => 0
        /// Console.WriteLine(target.hahahah);  => rrrrr
        /// Console.WriteLine(target.hahahah2); => (null)
        /// </code>
        /// </example>
        public static void ShadowCopyPropertyValues(object source, object target, params string[] excludes)
        {
            Type sourceType = source.GetType();
            Type targetType = target.GetType();

            var sourceProperties = sourceType.GetProperties(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.GetProperty);

            var targetProperties = targetType.GetProperties(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.SetProperty);

            foreach (var targetProperty in targetProperties) {
                //排除
                if (excludes != null) {
                    if (excludes.Contains(targetProperty.Name)) continue;
                }

                var sourceProperty = FindProperty(sourceProperties,
                    targetProperty.Name, targetProperty.PropertyType);

                if (sourceProperty != null) {
                    targetProperty.SetValue(target,
                        sourceProperty.GetValue(source, null),
                        null);

                }
            }
        }


        private static System.Reflection.PropertyInfo FindProperty(
            System.Reflection.PropertyInfo[] properties,
            string propertyName,
            Type propertyType)
        {
            foreach (var property in properties)
                if (property.Name == propertyName && property.PropertyType == propertyType)
                    return property;
            return null;
        }


        #endregion

        /// <summary>
        /// 返回某类型是否包含指定名称的属性，若有则导出属性信息
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="bindingAttr">绑定标识</param>
        /// <param name="propertyInfo">若该类型存在指定属性，则返回属性信息，否则返回null(Nothing in VB)</param>
        /// <returns></returns>
        public static bool IsContainsProperty(Type type, string propertyName, BindingFlags bindingAttr, out PropertyInfo propertyInfo)
        {
            var props = type.GetProperties(bindingAttr);
            foreach (var p in props) {
                if (p.Name == propertyName) {
                    propertyInfo = p;
                    return true;
                }
            }
            propertyInfo = null;
            return false;
        }



    }
}
