using System;
using System.Collections.Generic;
using System.Globalization;
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
            var result = (Brush) new BrushConverter().ConvertFrom("White");

            if (value is CompletenessState stateValue)
                return colorPalette[new ColorPaletteKey(stateValue, false).GetHashCode()];

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private void ConfigurePallette()
        {
            // Initialize the Color Palette
            colorPalette[new ColorPaletteKey(CompletenessState.Complete, true).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.ActiveCompleteColor);
            colorPalette[new ColorPaletteKey(CompletenessState.Complete, false).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.InActiveCompleteColor);

            colorPalette[new ColorPaletteKey(CompletenessState.NotSubbedNl, true).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.ActiveNotSubbedNlColor);
            colorPalette[new ColorPaletteKey(CompletenessState.NotSubbedNl, false).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.InActiveNotSubbedNlColor);

            colorPalette[new ColorPaletteKey(CompletenessState.NotSubbed, true).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.ActiveNotSubbedColor);
            colorPalette[new ColorPaletteKey(CompletenessState.NotSubbed, false).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.InActiveNotSubbedColor);

            colorPalette[new ColorPaletteKey(CompletenessState.NotDownloaded, true).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.ActiveNotDownloadedColor);
            colorPalette[new ColorPaletteKey(CompletenessState.NotDownloaded, false).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.InActiveNotDownloadedColor);

            colorPalette[new ColorPaletteKey(CompletenessState.NotApplicable, true).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.ActiveNotApplicableColor);
            colorPalette[new ColorPaletteKey(CompletenessState.NotApplicable, false).GetHashCode()] =
                (Brush) new BrushConverter().ConvertFrom(configurationService.InActiveNotApplicableColor);
        }
    }
}