using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using SeriesUI.Common;

namespace SeriesUI.Configuration
{
    public class ColorConfiguration : IValueConverter
    {
        public readonly Dictionary<int, Brush> colorPalette;

        private readonly IConfigurationService configurationService;

        public ColorConfiguration(IConfigurationService configurationService)
        {
            this.configurationService = configurationService;

            colorPalette = new Dictionary<int, Brush>();

            ConfigurePallette();
        }

        public ColorConfiguration()
        {
            configurationService = new ConfigurationService();

            colorPalette = new Dictionary<int, Brush>();

            ConfigurePallette();
        }

        /// <summary>
        ///     Used to translate a CompletenessState to a background in a DataGrid
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Brush result;

            if (value is CompletenessState stateValue)
                result = colorPalette[new ColorPaletteKey(stateValue, false, typeof(DataGridRow)).GetHashCode()];
            else
                result = (Brush) new BrushConverter().ConvertFrom("White");

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private void ConfigurePallette()
        {
            // Initialize the Color Palette

            // Data rows
            colorPalette[new ColorPaletteKey(CompletenessState.Complete, true, typeof(DataGridRow)).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom("White");
            colorPalette[new ColorPaletteKey(CompletenessState.Complete, false, typeof(DataGridRow)).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom("White");

            colorPalette[new ColorPaletteKey(CompletenessState.NotSubbedNl, true, typeof(DataGridRow)).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.ActiveNotSubbedNlColor);
            colorPalette[new ColorPaletteKey(CompletenessState.NotSubbedNl, false, typeof(DataGridRow)).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.InActiveNotSubbedNlColor);

            colorPalette[new ColorPaletteKey(CompletenessState.NotSubbed, true, typeof(DataGridRow)).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.ActiveNotSubbedColor);
            colorPalette[new ColorPaletteKey(CompletenessState.NotSubbed, false, typeof(DataGridRow)).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.InActiveNotSubbedColor);

            colorPalette[new ColorPaletteKey(CompletenessState.NotDownloaded, true, typeof(DataGridRow)).GetHashCode()]
                = (Brush) new BrushConverter().ConvertFrom(configurationService.ActiveNotDownloadedColor);
            colorPalette[new ColorPaletteKey(CompletenessState.NotDownloaded, false, typeof(DataGridRow)).GetHashCode()]
                = (Brush) new BrushConverter().ConvertFrom(configurationService.InActiveNotDownloadedColor);

            colorPalette[new ColorPaletteKey(CompletenessState.NotApplicable, true, typeof(DataGridRow)).GetHashCode()]
                = (Brush) new BrushConverter().ConvertFrom(configurationService.ActiveNotApplicableColor);
            colorPalette[new ColorPaletteKey(CompletenessState.NotApplicable, false, typeof(DataGridRow)).GetHashCode()]
                = (Brush) new BrushConverter().ConvertFrom(configurationService.InActiveNotApplicableColor);

            // Labels
            colorPalette[new ColorPaletteKey(CompletenessState.Complete, true, typeof(Label)).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.ActiveCompleteColor);
            colorPalette[new ColorPaletteKey(CompletenessState.Complete, false, typeof(Label)).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.InActiveCompleteColor);

            colorPalette[new ColorPaletteKey(CompletenessState.NotSubbedNl, true, typeof(Label)).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.ActiveNotSubbedNlColor);
            colorPalette[new ColorPaletteKey(CompletenessState.NotSubbedNl, false, typeof(Label)).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.InActiveNotSubbedNlColor);

            colorPalette[new ColorPaletteKey(CompletenessState.NotSubbed, true, typeof(Label)).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.ActiveNotSubbedColor);
            colorPalette[new ColorPaletteKey(CompletenessState.NotSubbed, false, typeof(Label)).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.InActiveNotSubbedColor);

            colorPalette[new ColorPaletteKey(CompletenessState.NotDownloaded, true, typeof(Label)).GetHashCode()]
                = (Brush) new BrushConverter().ConvertFrom(configurationService.ActiveNotDownloadedColor);
            colorPalette[new ColorPaletteKey(CompletenessState.NotDownloaded, false, typeof(Label)).GetHashCode()]
                = (Brush) new BrushConverter().ConvertFrom(configurationService.InActiveNotDownloadedColor);

            colorPalette[new ColorPaletteKey(CompletenessState.NotApplicable, true, typeof(Label)).GetHashCode()]
                = (Brush) new BrushConverter().ConvertFrom(configurationService.ActiveNotApplicableColor);
            colorPalette[new ColorPaletteKey(CompletenessState.NotApplicable, false, typeof(Label)).GetHashCode()]
                = (Brush) new BrushConverter().ConvertFrom(configurationService.InActiveNotApplicableColor);
        }
    }
}