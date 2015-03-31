using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Windows;
using GalaSoft.MvvmLight;

namespace ViewModelsMerger.Helpers
{
    public static class MergedViewModelsTypeBuilder
    {
        #region Properties

        public static List<ViewModelBase> ViewModelsList { get; set; }
        public static List<PropertyInfo> Properties { get; private set; }

        #endregion

        public static ViewModelBase CreateNewObject()
        {
            //Create the target type 
            var targetType = CompileResultType();
            //Instantiate it !
            var targetObject = (ViewModelBase)Activator.CreateInstance(targetType);
            //Assign values to its properties !
            AssignPropertyValues(targetObject);

            return targetObject;
        }

        #region Methods
        private static Type CompileResultType()
        {
            //The Object that helps build the target type
            TypeBuilder typeBuilder = GetTypeBuilder();

            //The Object that helps build the Constructor
            ConstructorBuilder constructor = typeBuilder.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);


            //Pull properties from the input ViewModels and inject them in the target type
            Properties = new List<PropertyInfo>();

            foreach (var viewModelBase in ViewModelsList)
            {
                var type = viewModelBase.GetType();
                var props = type.GetProperties().ToList();
                Properties.AddRange(props);

                foreach (var property in props)
                {
                    CreateProperty(typeBuilder, property.Name, property.PropertyType, property.GetValue(viewModelBase, null));
                }
            }
            Type objectType = typeBuilder.CreateType();
            return objectType;
        }

        private static TypeBuilder GetTypeBuilder()
        {
            var typeSignature = "MergedViewModels";
            var an = Application.ResourceAssembly.GetName();
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            TypeBuilder typeBuilder = moduleBuilder.DefineType(typeSignature
                                , TypeAttributes.Public |
                                TypeAttributes.Class |
                                TypeAttributes.AutoClass |
                                TypeAttributes.AnsiClass |
                                TypeAttributes.BeforeFieldInit |
                                TypeAttributes.AutoLayout
                                , typeof(ViewModelBase));
            return typeBuilder;
        }

        private static void CreateProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType, object propertyValue)
        {
            //Creating the private fields
            FieldBuilder fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

            //Create the Public Property
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

            //Creating the Getters
            MethodBuilder getPropMthdBldr =
                typeBuilder.DefineMethod("Get" + propertyName,
                  MethodAttributes.Public |
                  MethodAttributes.SpecialName |
                  MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);

            //Creating the Setters
            MethodBuilder setPropMthdBldr =
                typeBuilder.DefineMethod("set_" + propertyName,
                  MethodAttributes.Public |
                  MethodAttributes.SpecialName |
                  MethodAttributes.HideBySig,
                  null, new[] { propertyType });

            //Generating IL
            ILGenerator getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

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

        private static void AssignPropertyValues(ViewModelBase obj)
        {
            var prop = obj.GetType().GetProperties();
            foreach (var propertyInfo in prop)
            {
                foreach (var viewModel in ViewModelsList)
                {
                    var query = (from property in viewModel.GetType().GetProperties()
                                 where property.Name == propertyInfo.Name
                                 select property).FirstOrDefault();
                    if (query != null) propertyInfo.SetValue(obj, query.GetValue(viewModel, null), null);
                }
            }
        }
        #endregion






    }
}
