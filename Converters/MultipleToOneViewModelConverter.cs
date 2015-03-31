using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using ViewModelsMerger.Helpers;

namespace ViewModelsMerger.Converters
{
    public class MultipleToOneViewModelConverter : IMultiValueConverter
    {
        #region Properties

        #endregion

        #region IMultiValueConverter Implementation 
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            ViewModelBase returnValue;
            var viewModels = values.Cast<ViewModelBase>().ToList();
            MergedViewModelsTypeBuilder.ViewModelsList = viewModels;
            returnValue = MergedViewModelsTypeBuilder.CreateNewObject();
            return returnValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[3];
        }

        #endregion
    }
}
