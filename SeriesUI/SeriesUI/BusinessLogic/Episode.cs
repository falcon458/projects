using System;
using System.Collections.Generic;

namespace SeriesUI.BusinessLogic
{
    [Serializable]
    internal class Episode
    {
        public Episode()
        {
            SubTitles = new List<string>();
        }

        public Episode(string title)
        {
            Title = title;
        }

        public DateTime AirDate { get; set; }

        public string Title { get; set; }

        public bool Downloaded { get; set; }

        public List<string> SubTitles { get; set; }
    }
}