using System;
using System.Collections.Generic;
using System.Linq;

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

        public int Completeness
        {
            get
            {
                var result = (int) CompletenessStates.Complete;
                var i = -1;
                while (result < (int) CompletenessStates.NotDownloaded && i < Episodes.Count - 1)
                {
                    i++;

                    if (Episodes[i].AirDate.Date >= DateTime.Today.Date) continue;

                    if (!Episodes[i].Downloaded)
                        result = Math.Max(result, (int) CompletenessStates.NotDownloaded);
                    else if (!Episodes[i].SubTitles[Episode.SubTitle.NL] && !Episodes[i].SubTitles[Episode.SubTitle.EN])
                        result = Math.Max(result, (int) CompletenessStates.NotSubbed);
                    else if (!Episodes[i].SubTitles[Episode.SubTitle.NL])
                        result = Math.Max(result, (int) CompletenessStates.NotSubbedNl);
                }

                return result;
            }
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

        private enum CompletenessStates
        {
            Complete,
            NotSubbedNl,
            NotSubbed,
            NotDownloaded
        }
    }
}