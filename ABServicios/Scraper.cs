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
        
        public Scraper(){}

	    public Scraper(Encoding encoding)
        {
            _encoding = encoding;
        }

	    public HtmlNode GetNodes(Uri url)
        {
            // Create the WebRequest for the URL we are using
            var req = WebRequest.Create(url);

            // Get the stream from the returned web response
            var stream = _encoding != null ? new StreamReader(req.GetResponse().GetResponseStream(), _encoding) : new StreamReader(req.GetResponse().GetResponseStream());
            
            var htmlDocument = new HtmlDocument();
            htmlDocument.Load(stream);
            return htmlDocument.DocumentNode;
        }
	}
}
