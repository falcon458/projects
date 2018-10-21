using System;
using System.Collections.Generic;

namespace SeriesUI.BusinessLogic
{
    [Serializable]
    internal class Season
    {
        public Season(int sequence)
        {
            Sequence = sequence;
            Episodes = new List<Episode>();
        }

        public int Sequence { get; }

        public List<Episode> Episodes { get; set; }

        public void SetAllSubs(Episode.SubTitle sub)
        {
            foreach (var episode in Episodes)
                episode.SubTitles[sub] = true;
        }
    }
}