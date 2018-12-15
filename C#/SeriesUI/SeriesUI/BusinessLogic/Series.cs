﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using SeriesUI.Common;

namespace SeriesUI.BusinessLogic
{
    [Serializable]
    internal class Series
    {
        public Series()
        {
            Seasons = new List<Season>();
        }

        public string Name { get; set; }

        public string WebSite { get; set; }
        public string LocalUrl { get; set; }

        public List<Season> Seasons { get; set; }

        public CompletenessState Completeness
        {
            get
            {
                // Select the worst completeness
                var seriesCompleteness = Seasons.Where(c => c.Completeness > CompletenessState.NotApplicable)
                    .Select(x => x.Completeness).Max();

                return seriesCompleteness;
            }
        }

        public void GetDataFromWebsite()
        {
            GetSeasons();
            GetEpisodes();
        }

        private void GetSeasons()
        {
            var webPage = new WebPage($"{WebSite}{LocalUrl}/1");

            // Extract the seasons
            var regex = new Regex($@"href=""{LocalUrl}/(\d+)");
            var matchlist = (from Match m in regex.Matches($"{webPage.PageSource}") select m.Groups[1]).ToList();

            // Store the season numbers
            foreach (var match in (from Match m in regex.Matches($"{webPage.PageSource}") select m.Groups[1]).ToList())
                if (int.TryParse(match.Value, out var intValue))
                    Seasons.Add(new Season(intValue));
                else
                    Common.Common.Log($"ERROR: Non-numeric match for season: {match.Value}");
        }

        private void GetEpisodes()
        {
            var webPage = new WebPage();

            for (var i = 1; i <= Seasons.Count; i++)
            {
                // Retrieve page source
                webPage.Url = $"{WebSite}{LocalUrl}/{i}";
                webPage.getCode();

                // Extract the episodes
                var regex = new Regex(
                    $@"href=.*>((S0*?{i}E\d+).*)</a>\s*\n\s*</td>\s*\n\s*<td>\s*\n\s*(.*)\s*\n\s*</td>");
                var matchResult = regex.Matches(webPage.PageSource);
                var episodeNames = (from Match m1 in matchResult select m1.Groups[1]).ToList();
                var episodeNumbers = (from Match m1 in matchResult select m1.Groups[2]).ToList();
                var episodeDates = (from Match m1 in matchResult select m1.Groups[3]).ToList();

                // Verify number of episodes and dates
                if (episodeDates.Count != episodeNames.Count || episodeDates.Count == 0)
                    Common.Common.Log(
                        $"ERROR: Unequal or zero number of episodes ({episodeNames.Count}) and dates ({episodeDates.Count}) for \"{Name}\", season {i}");
                else
                    for (var j = 0; j < episodeNames.Count; j++)
                    {
                        // Translate HTML characters
                        var episode = new Episode(Name + " - " + WebUtility.HtmlDecode(episodeNames[j].ToString()));

                        if (DateTime.TryParseExact(episodeDates[j].ToString(), "dd/MM/yyyy",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out var airDate))
                            episode.AirDate = airDate;
                        else
                            episode.AirDate =
                                DateTime.ParseExact("31/12/2099", "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        episode.Number = episodeNumbers[j].ToString();

                        Seasons[i - 1].Episodes.Add(episode);
                    }
            }
        }
    }
}