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
        public static List<Tuple<PropertyInfo,object>> Properties { get; private set; }
        public static List<List<Tuple<PropertyInfo, object>>> RedundantProperties { get; private set; }

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
            Properties = new List<Tuple<PropertyInfo, object>>();
            var mergedProperties = new List<List<Tuple<PropertyInfo, object>>>();
            RedundantProperties = new List<List<Tuple<PropertyInfo, object>>>();

            ViewModelsList.ForEach(viewModel =>
            {
                var type = viewModel.GetType();
                var properties = type.GetProperties().ToList();
                var viewModelProperties = properties.Select(property => new Tuple<PropertyInfo, object>(property, viewModel)).ToList();
                mergedProperties.Add(viewModelProperties);
            });

            var propertiesComparer = new PropertyComparer();
            mergedProperties.ForEach(viewModelPropertis => Properties.AddRange(viewModelPropertis));

            mergedProperties.ForEach(viewModelPropertis => viewModelPropertis.ForEach(property =>
            {
                var redundant = Properties.FindAll(p => propertiesComparer.Equals(p, property)).ToList();
                if (redundant.Count > 1)
                {
                    Properties.RemoveAll(redundant.Contains);
                    RedundantProperties.Add(redundant);
                }
            }));


            foreach (var property in Properties)
            {
                CreateProperty(typeBuilder, property.Item1.Name, property.Item1.PropertyType, null);
            }
            foreach (var list in RedundantProperties)
            {
                CreateProperty(typeBuilder, list[0].Item1.Name, typeof(List<Object>), null);
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

        private static void AssignPropertyValues(ViewModelBase mergedViewModel)
        {
            //For each non redundante property, find the corresponding one inside the ViewModels, and assign to it its value
            var propertyComparer = new PropertyInfoComparer();
            
            var mergedProperties = mergedViewModel.GetType()
                .GetProperties()
                .Intersect(Properties.Select(property=>property.Item1), propertyComparer);

            var mergedRedundantProperties = (mergedViewModel.GetType()
                .GetProperties().Except(mergedProperties))
                .Where(mergedProperty => mergedProperty.PropertyType.Name == "List`1")
                .ToList();

            foreach (var propertyInfo in mergedProperties)
            {
                //For each ViewModel
                foreach (var viewModel in ViewModelsList)
                {
                    //Find the orresponding one
                    var query = (from property in viewModel.GetType().GetProperties()
                                 where property.Name == propertyInfo.Name
                                 select property).FirstOrDefault();
                    //Assign value
                    //Exception here , don't have setvalue method !!
                    if (query != null) propertyInfo.SetValue(mergedViewModel, query.GetValue(viewModel, null), null);
                }
            }

            var redundant = RedundantProperties.Select(red => red.Select(tuple => tuple.Item1.GetValue(tuple.Item2)).ToList()).ToList();
            //For each list that contain duplicates from viewmodels
            for (var i = 0; i< mergedRedundantProperties.Count(); i++)
            {
                //Give me the ViewModels that contain the properties of that list
                //var query = (from viewModel in ViewModelsList
                //             where viewModel.GetType().GetProperties().Any(info => info.Name == list.Name)
                //             select viewModel).ToList().Select(viewModel=>viewModel.);

                mergedRedundantProperties[i].SetValue(mergedViewModel, redundant[i]); 
            }
        }
        #endregion






    }
}
