using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using SeriesUI.Common;

namespace SeriesUI.Configuration
{
    public class ColorPaletteKey
    {
        public ColorPaletteKey(CompletenessState completeness, bool active)
        {
            Completeness = completeness;
            Active = active;
        }

        private CompletenessState Completeness { get; }

        private bool Active { get; }

        // [!] The default GetHashCode does not work as intended
        public override int GetHashCode()
        {
            return Completeness.GetHashCode() * 100 + Active.GetHashCode();
        }
    }

    //public class Pipo2 : IValueConverter
    //{

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return (Brush) new BrushConverter().ConvertFrom("FF6DF4AA");
    //    }
    //}
}