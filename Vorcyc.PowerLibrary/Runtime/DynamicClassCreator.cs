using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

/// <summary>
/// 提供运行时功能扩展
/// </summary>
namespace Vorcyc.PowerLibrary.Runtime
{

    //https://stackoverflow.com/questions/3862226/how-to-dynamically-create-a-class

    /// <summary>
    /// 提供工具，使用属性描述序列以动态创建简单类，该类只有可读写的公共属性。
    /// </summary>
    /// <example>
    /// 以下代码演示如何使用该类：
    /// <code>
    /// //创建属性描述集
    /// var propertyDescriptions = new List&lt;PropertyDescription&gt;()
    /// {
    ///     new PropertyDescription ("Name",typeof(string)),//表示创建一个名为 "Name"，类型为string的公共实例属性
    ///     new PropertyDescription ("Age",typeof(int)) //表示创建一个名为 "Age" ，类型为 int 的公共实例属性
    /// };
    ///
    /// //使用属性描述集来创建新类，由于是动态类型，因此可以设置其属性。
    /// //注意：若使用的属性不在类中则会抛出异常。
    /// var newClass = DynamicClassCreator.CreateNewClass(propertyDescriptions);
    /// newClass.Age = 5;
    /// newClass.Name = "cyclone_dll";
    ///
    /// Console.WriteLine(newClass.Age);    =>  5
    /// Console.WriteLine(newClass.Name);   =>  cyclone_dll
    ///
    /// </code>
    /// </example>
    public static class DynamicClassCreator
    {
        /// <summary>
        /// 属性描述
        /// </summary>
        public class PropertyDescription
        {
            /// <summary>
            /// 创建属性描述示例
            /// </summary>
            /// <param name="name">属性名称</param>
            /// <param name="propertyType">属性类型</param>
            public PropertyDescription(string name, Type propertyType)
            {
                this.Name = name;
                this.PropertyType = propertyType;
            }

            /// <summary>
            /// 属性名
            /// </summary>
            public string Name {
                get; set;
            }
            /// <summary>
            /// 属性类型
            /// </summary>
            public Type PropertyType {
                get; set;
            }
        }

        /// <summary>
        /// 动态创建一个类，并返回其类型。
        /// </summary>
        /// <param name="propDescriptions"></param>
        /// <returns>返回所创建类的类型<see cref="Type"/></returns>
        public static Type CreateNewClassType(IEnumerable<PropertyDescription> propDescriptions)
        {
            return CustomTypeBuilder.CompileResultType(propDescriptions);
        }

        /// <summary>
        /// 动态创建一个新类的类型，然后创建并返回其实例（动态类型）。
        /// </summary>
        /// <param name="propDescriptions"></param>
        /// <returns>返回所创建类的实例</returns>
        public static dynamic CreateNewClass(IEnumerable<PropertyDescription> propDescriptions)
        {
            var t = CustomTypeBuilder.CompileResultType(propDescriptions);
            return Activator.CreateInstance(t);
        }

    }


    internal static class CustomTypeBuilder
    {
        public static Type CompileResultType(IEnumerable<DynamicClassCreator.PropertyDescription> propDescs)
        {
            TypeBuilder tb = GetTypeBuilder();
            ConstructorBuilder constructor = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);


            // NOTE: assuming your list contains Field objects with fields FieldName(string) and FieldType(Type)
            foreach (var field in propDescs)
                CreateProperty(tb, field.Name, field.PropertyType);

            Type objectType = tb.CreateType();
            return objectType;
        }

        private static TypeBuilder GetTypeBuilder()
        {
            var typeSignature = "Vorcyc.MyDynamicType";
            var an = new AssemblyName(typeSignature);

            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            TypeBuilder tb = moduleBuilder.DefineType(typeSignature,
                    TypeAttributes.Public |
                    TypeAttributes.Class |
                    TypeAttributes.AutoClass |
                    TypeAttributes.AnsiClass |
                    TypeAttributes.BeforeFieldInit |
                    TypeAttributes.AutoLayout,
                    null);
            return tb;
        }

        private static void CreateProperty(TypeBuilder tb, string propertyName, Type propertyType)
        {
            FieldBuilder fieldBuilder = tb.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

            PropertyBuilder propertyBuilder = tb.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
            MethodBuilder getPropMthdBldr = tb.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
            ILGenerator getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            MethodBuilder setPropMthdBldr =
                tb.DefineMethod("set_" + propertyName,
                  MethodAttributes.Public |
                  MethodAttributes.SpecialName |
                  MethodAttributes.HideBySig,
                  null, new[] { propertyType });

            ILGenerator setIl = setPropMthdBldr.GetILGenerator();
            Label modifyProperty = setIl.DefineLabel();
            Label exitSet = setIl.DefineLabel();

            setIl.MarkLabel(modifyProperty);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);

            setIl.Emit(OpCodes.Nop);
            setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }
    }
}
