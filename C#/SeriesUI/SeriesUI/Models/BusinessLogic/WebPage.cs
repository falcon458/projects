using System.Net;
using System.Text;
using SeriesUI.Interfaces;

namespace SeriesUI.BusinessLogic
{
    internal class WebPage : IWebPage
    {
        public WebPage(string url)
        {
            if (!string.IsNullOrEmpty(url)) Url = url;
        }

        public string Url { get; }
        public string PageSource { get; set; }

        #region IWebPage members

        public string GetPageSource()
        {
            var webClient = new WebClient {Encoding = Encoding.UTF8};
            var html = webClient.DownloadString(Url);

            return html;

            /* For reference: HttpClient is the new standard, but for us much slower
            var client = new HttpClient();
            var response = client.GetAsync(Url).Result; // Calling .Result makes it synchronous, making "await" unneeded
            var content = response.Content;

            var result = content.ReadAsStringAsync().Result;

            if (result != null) PageSource = result;
            */
        }

        #endregion
    }
}