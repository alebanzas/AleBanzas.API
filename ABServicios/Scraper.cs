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
		public HtmlNode GetNodes(Uri url)
        {
            // Create the WebRequest for the URL we are using
            var req = WebRequest.Create(url);
				
			// Get the stream from the returned web response
			var stream = new StreamReader(req.GetResponse().GetResponseStream(), Encoding.UTF7);
            
            var htmlDocument = new HtmlDocument();
            htmlDocument.Load(stream);
            return htmlDocument.DocumentNode;
        }
	}
}
