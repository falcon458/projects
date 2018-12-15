﻿namespace SeriesUI.Configuration
{
    public interface IConfigurationService
    {
        string InActiveCompleteColor { get; }
        string ActiveCompleteColor { get; }

        string InActiveNotSubbedNlColor { get; }
        string ActiveNotSubbedNlColor { get; }

        string InActiveNotSubbedColor { get; }
        string ActiveNotSubbedColor { get; }

        string InActiveNotDownloadedColor { get; }
        string ActiveNotDownloadedColor { get; }

        string InActiveNotApplicableColor { get; }
        string ActiveNotApplicableColor { get; }

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