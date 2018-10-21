namespace SeriesUI.Configuration
{
    internal interface IConfigurationService
    {
        string LabelInActiveColor { get; }

        string LabelActiveColor { get; }

        int LabelWidth { get; }

        int LabelHeight { get; }

        int LabelSpacing { get; }

        SeriesConfigCollection SeriesConfigCollection { get; }

        SeriesWebSite SeriesWebSite { get; }

        SeriesPlaceHolder SeriesPlaceHolder { get; }

        SaveFile SaveFile { get; }

        SeriesWebSiteLocation SeriesWebSiteLocation { get; }
    }
}