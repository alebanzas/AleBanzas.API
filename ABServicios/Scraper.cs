using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Web;
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
	        return GetNodes(url, HttpMethod.Get, null);
	    }

        public HtmlNode GetNodes(Uri url, HttpMethod method, Dictionary<string, object> postParameters)
        {
            // Create the WebRequest for the URL we are using
            var req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = method.Method;

            if (HttpMethod.Post.Equals(method))
            {
                if (postParameters != null)
                {
                    var postData = new StringBuilder();
                    foreach (var postParameter in postParameters)
                    {
                        postData.Append(string.Format("{0}={1}&", postParameter.Key, HttpUtility.UrlEncode(postParameter.Value.ToString())));
                    }

                    byte[] postBytes = _encoding.GetBytes(postData.ToString());

                    req.ContentType = "application/x-www-form-urlencoded";
                    req.ContentLength = postBytes.Length;

                    Stream postStream = req.GetRequestStream();
                    postStream.Write(postBytes, 0, postBytes.Length);
                    postStream.Flush();
                    postStream.Close();
                }
            }

            if (!string.IsNullOrWhiteSpace(_referer))
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
