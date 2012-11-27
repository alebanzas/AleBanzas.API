using System;
using System.Net;
using System.IO;

namespace ABServicios
{
	/// <summary>
	/// This class opens a website and scraps the contents
	/// for weather information
	/// </summary>
	public class Scraper
	{
		// Stores a copy of the streamed site
		private string m_strSite;
        
		/// <summary>
		/// This method attempts to open the requested URI and read
		/// the returned stream into a string for storage and later
		/// processing
		/// </summary>
		public bool OpenSite(Uri url)
		{
			try
			{
				// Create the WebRequest for the URL we are using
                WebRequest req = WebRequest.Create(url);
				
				// Get the stream from the returned web response
				var stream = new StreamReader(req.GetResponse().GetResponseStream());
			
				var sb = new System.Text.StringBuilder();
				string strLine;
				// Read the stream a line at a time and place each one
				// into the stringbuilder
				while( (strLine = stream.ReadLine()) != null )
				{
					// Ignore blank lines
					if(strLine.Length > 0 )
						sb.Append(strLine);
				}
				// Finished with the stream so close it now
				stream.Close();

				// Cache the streamed site now so it can be used
				// without reconnecting later
				m_strSite = sb.ToString();

				return true;
			}
			catch(Exception e)
			{
				// Handle the error in some fashion
				return false;
			}
		}
		
		///<summary>
		/// Return a string containing the HTML for current
		/// weather conditions
		///</summary>
		public string GetResponse()
		{
			return m_strSite;
		}

        public StreamReader GetStream(Uri url)
        {
            // Create the WebRequest for the URL we are using
            var req = WebRequest.Create(url);
				
			// Get the stream from the returned web response
			var stream = new StreamReader(req.GetResponse().GetResponseStream());

            return stream;
        }
	}
}
