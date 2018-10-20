using System.Net;

namespace SeriesUI.BusinessLogic
{
    internal class WebPage
    {
        public WebPage()
        {
        }

        public WebPage(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                Url = url;
                getCode();
            }
        }

        public string Url { get; set; }
        public string PageSource { get; set; }

        public void getCode()
        {
            var html = new WebClient().DownloadString(Url);

            PageSource = html;
        }
    }
}