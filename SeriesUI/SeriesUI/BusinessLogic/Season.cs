using System;
using System.Collections.Generic;
using System.Linq;
using SeriesUI.Common;

namespace SeriesUI.BusinessLogic
{
    [Serializable]
    public class Season
    {
        public Season(int sequence)
        {
            Sequence = sequence;
            Episodes = new List<Episode>();
        }

        public int Sequence { get; }

        public List<Episode> Episodes { get; set; }

        public CompletenessState Completeness
        {
            get
            {
                // Select the worst completeness
                var seasonCompleteness = Episodes.Where(c => c.Completeness > CompletenessState.NotApplicable)
                    .Select(x => x.Completeness).Max();

                return seasonCompleteness;
            }
        }

        private CompletenessState Max(CompletenessState arg1, CompletenessState arg2)
        {
            var result = (CompletenessState) Math.Max((int) arg1, (int) arg2);

            return result;
        }

        public void SetAllSubs(Episode.SubTitle sub)
        {
            foreach (var episode in Episodes)
                episode.SubTitles[sub] = true;
        }

        public void SetAllEpisodesDownloaded()
        {
            foreach (var episode in Episodes)
                episode.Downloaded = true;
        }

        public void ToggleAllSubs(Episode.SubTitle subTitle)
        {
            // [!] Somehow, using:
            // c.SubTitles[subTitle] = !Episodes[0].SubTitles[subTitle];
            // results in all subs having a value unequal to the first

            if (Episodes.Count > 0)
            {
                var checkedValue = !Episodes[0].SubTitles[subTitle];

                Episodes.ToList().ForEach(c => c.SubTitles[subTitle] = checkedValue);
            }
        }

        public void ToggleDownloaded()
        {
            if (Episodes.Count > 0)
            {
                var checkedValue = !Episodes[0].Downloaded;

                Episodes.ToList().ForEach(c => c.Downloaded = checkedValue);
            }
        }
    }
}