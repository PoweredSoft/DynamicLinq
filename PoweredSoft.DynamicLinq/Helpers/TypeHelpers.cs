using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using System.Reflection;

namespace PoweredSoft.DynamicLinq.Helpers
{
    public static class TypeHelpers
    {
        /*
        internal static Lazy<AssemblyName> DynamicAssemblyName = new Lazy<AssemblyName>(() => new AssemblyName("PoweredSoft.DynamicLinq.DynamicTypes"));
        internal static Lazy<AssemblyBuilder> DynamicAssembly = new Lazy<AssemblyBuilder>(() => AssemblyBuilder.DefineDynamicAssembly(DynamicAssemblyName.Value, AssemblyBuilderAccess.Run));
        internal static Lazy<ModuleBuilder> DynamicModule = new Lazy<ModuleBuilder>(() => DynamicAssembly.Value.DefineDynamicModule("PoweredSoft.DynamicLinq.DynamicTypes"));*/

        public static bool IsNullable(Type type)
        {
            if (!type.IsValueType)
                return true; // ref-type

            return Nullable.GetUnderlyingType(type) != null;
        }


     

        /*
        /// <summary>
        /// Use this to create anonymous type
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        internal static TypeInfo CreateSimpleAnonymousType(List<(Type type, string name)> fields)
        {
            // DYNAMIC TYPE CREATION
            var typeName = $"PSDLProxy_{Guid.NewGuid().ToString("N")}";
            var dynamicType = DynamicModule.Value.DefineType(typeName, TypeAttributes.Class | TypeAttributes.Public);
            fields.ForEach(field =>
            {
                CreatePropertyOnType(dynamicType, field.name, field.type);
            });
            // not needed at the end.
            // CreateConstructorWithAllPropsOnType(dynamicType, fields);
            var ret = dynamicType.CreateTypeInfo();
            return ret;
        }*/

        /* 
         * concstructor 
         * https://stackoverflow.com/questions/6879279/using-typebuilder-to-create-a-pass-through-constructor-for-the-base-class
         * works but wasn't needed at the end.
        private static void CreateConstructorWithAllPropsOnType(TypeBuilder dynamicType, List<(Type type, string name)> fields)
        {
            var ctor = dynamicType.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, fields.Select(t => t.type).ToArray());
            var parameters = fields
                .Select((field, i) =>
               {
                   return ctor.DefineParameter(i++, ParameterAttributes.None, $"{field.name}_1");
               })
               .ToList();

            var emitter = ctor.GetILGenerator();
            emitter.Emit(OpCodes.Nop);

            // Load `this` and call base constructor with arguments
            emitter.Emit(OpCodes.Ldarg_0);
            for (var i = 1; i <= parameters.Count; ++i)
            {
                emitter.Emit(OpCodes.Ldarg, i);
            }
            emitter.Emit(OpCodes.Call, ctor);
            emitter.Emit(OpCodes.Ret);
        }*/

            /*
        internal static void CreatePropertyOnType(TypeBuilder typeBuilder, string propertyName, Type propertyType)
        {
            // Generate a property called "Name"
            var field = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);
            var attributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

            // generate property
            var propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

            // Generate getter method
            var getter = typeBuilder.DefineMethod("get_" + propertyName, attributes, propertyType, Type.EmptyTypes);
            var il = getter.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);        // Push "this" on the stack
            il.Emit(OpCodes.Ldfld, field);   // Load the field "_Name"
            il.Emit(OpCodes.Ret);            // Return
            propertyBuilder.SetGetMethod(getter);

            // Generate setter method
            var setter = typeBuilder.DefineMethod("set_" + propertyName, attributes, null, new[] { propertyType });
            il = setter.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);        // Push "this" on the stack
            il.Emit(OpCodes.Ldarg_1);        // Push "value" on the stack
            il.Emit(OpCodes.Stfld, field);   // Set the field "_Name" to "value"
            il.Emit(OpCodes.Ret);            // Return
            propertyBuilder.SetSetMethod(setter);
        }
        */
    }
}
