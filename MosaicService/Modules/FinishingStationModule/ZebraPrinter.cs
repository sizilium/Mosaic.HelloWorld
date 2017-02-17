using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace MosaicSample.FinishingStationModule
{
    //
    // Exposes methods that render an image using an existing Zebra printer.
    //
    public class ZebraPrinter
    {
        // Printer's IP address.
        private readonly string _address;
        private readonly int _portNbr;


        public ZebraPrinter(string address, int port)
        {
            if (String.IsNullOrEmpty(address))
            {
                throw new ArgumentNullException("address");
            }

            _address = address;
            _portNbr = port;
        }


        //
        // Creates an image from a ZPL file.
        //
        public Image RenderFile(string filename)
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                return Render(reader.ReadToEnd());
            }
        }

        //
        // Creates and image from a string containing ZPL code.
        //
        public Image Render(string zpl)
        {
            var imageName = CreateNewImage(zpl);

            var response = GetImage(imageName);

            using (var stream = new MemoryStream(response))
            {
                return Image.FromStream(stream);
            }
        }

        public void PrintLabel(string zplString)
        {
            try
            {
                // Open connection
                using (System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient())
                {
                    client.Connect(_address, _portNbr);

                    // Write ZPL String to connection
                    using (System.IO.StreamWriter writer = new System.IO.StreamWriter(client.GetStream()))
                    {
                        writer.Write(zplString);
                        writer.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                // Catch Exception
            }
        }


        private string CreateNewImage(string zpl)
        {
            string url = String.Format("http://{0}/zpl", _address);

            string parameters = String.Format(
                "data={0}&dev=R&oname=UNKNOWN&otype=ZPL&prev=Preview Label&pw=",
                HttpUtility.UrlEncode(zpl));

            // Post to the printer.
            string response = HttpPost(url, parameters);

            // The response contains something like:
            // <IMG SRC="png?prev=Y&dev=R&oname=__TMP039&otype=PNG" ALT="R:__TMP039.PNG" WIDTH="100%" BORDER="2">
            int imageNameBegin = 7 + response.IndexOf("ALT=\"R:", StringComparison.OrdinalIgnoreCase);

            return response.Substring(
                imageNameBegin,
                response.IndexOf(".PNG\"", imageNameBegin, StringComparison.OrdinalIgnoreCase) - imageNameBegin);
        }


        private byte[] GetImage(string imageName)
        {
            string url = String.Format("http://{0}/png?prev=Y&dev=R&oname={1}&otype=PNG", _address, imageName);

            using (WebClient client = new WebClient())
            {
                return client.DownloadData(url);
            }
        }


        private string HttpPost(string url, string parameters)
        {
            WebRequest request = WebRequest.Create(url);
            request.Proxy = new WebProxy();
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";

            byte[] bytes = Encoding.ASCII.GetBytes(parameters);
            request.ContentLength = bytes.Length;

            Stream stream = request.GetRequestStream();
            stream.Write(bytes, 0, bytes.Length);
            stream.Close();

            WebResponse response = request.GetResponse();
            if (response == null)
            {
                return null;
            }

            StreamReader reader = new StreamReader(response.GetResponseStream());
            return reader.ReadToEnd().Trim();
        }
    }
}