using System;
using System.Collections.Generic;
using SeriesUI.Common;

namespace SeriesUI.BusinessLogic
{
    [Serializable]
    public class Episode
    {
        public enum SubTitle
        {
            Nl,
            En
        }

        public Episode()
        {
            SubTitles = new Dictionary<SubTitle, bool>();
            SubTitles[SubTitle.Nl] = false;
            SubTitles[SubTitle.En] = false;
        }

        public Episode(string title) : this()
        {
            Title = title;
        }

        public CompletenessState Completeness
        {
            get
            {
                var result = CompletenessState.Complete;

                // Ignore future episodes
                if (AirDate.Date >= DateTime.Today.Date) result = CompletenessState.NotApplicable;
                else if (!Downloaded) result = CompletenessState.NotDownloaded;
                else if (!SubTitles[SubTitle.Nl] && !SubTitles[SubTitle.En])
                    result = CompletenessState.NotSubbed;
                else if (!SubTitles[SubTitle.Nl]) result = CompletenessState.NotSubbedNl;

                return result;
            }
        }

        public DateTime AirDate { get; set; }

        public string Title { get; set; }

        public bool Downloaded { get; set; }

        public Dictionary<SubTitle, bool> SubTitles { get; set; }
    }
}