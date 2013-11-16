using System;
using System.Net;
using System.IO;
using System.Text;
using HtmlAgilityPack;

namespace ABServicios
{
	/// <summary>
	/// This class opens a website and scraps the contents
	/// </summary>
	public class Scraper
    {
        private readonly Encoding _encoding;
        private readonly string _referer;
        
        public Scraper(){}

        public Scraper(Encoding encoding)
        {
            _encoding = encoding;
        }

        public Scraper(Encoding encoding, string referer)
        {
            _encoding = encoding;
            _referer = referer;
        }

	    public HtmlNode GetNodes(Uri url)
        {
            // Create the WebRequest for the URL we are using
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            if(!string.IsNullOrWhiteSpace(_referer))
            {
                req.Referer = _referer;
            }

            // Get the stream from the returned web response
            var stream = _encoding != null ? new StreamReader(req.GetResponse().GetResponseStream(), _encoding) : new StreamReader(req.GetResponse().GetResponseStream());
            
            var htmlDocument = new HtmlDocument();
            htmlDocument.Load(stream);
            return htmlDocument.DocumentNode;
        }
	}
}
