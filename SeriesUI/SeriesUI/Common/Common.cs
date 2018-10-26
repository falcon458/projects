using System;

namespace SeriesUI.Common
{
    public enum CompletenessState
    {
        NotApplicable,
        Complete,
        NotSubbedNl,
        NotSubbed,
        NotDownloaded
    }

    public enum SubTitle
    {
        Nl,
        En
    }

    public static class Common
    {
        public static void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}