﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using OfflineMediaV3.Common.Enums.View;

namespace OfflineMediaV3.DisplayHelper.Converter.SpecificConverter
{
    public class DisplayStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            DisplayState ds = (DisplayState)value;
            if (ds.ToString() == (string)parameter)
                return Application.Current.Resources["AkzentBackgroundThemeBrush"];
            return Application.Current.Resources["BackgroundThemeBrush"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
