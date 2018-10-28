using System.Net;
using System.Text;

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
            var webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;
            var html = webClient.DownloadString(Url);

            PageSource = html;
        }
    }
}