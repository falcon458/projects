using System;
using System.Collections.Generic;

namespace SeriesUI.BusinessLogic
{
    [Serializable]
    internal class Episode
    {
        public enum SubTitle
        {
            NL,
            EN
        }

        public Episode()
        {
            SubTitles = new Dictionary<SubTitle, bool>();
            SubTitles[SubTitle.NL] = false;
            SubTitles[SubTitle.EN] = false;
        }

        public Episode(string title) : this()
        {
            Title = title;
        }

        public DateTime AirDate { get; set; }

        public string Title { get; set; }

        public bool Downloaded { get; set; }

        public Dictionary<SubTitle, bool> SubTitles { get; set; }
    }
}