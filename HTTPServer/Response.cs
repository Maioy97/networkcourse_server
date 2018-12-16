using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            string statusLine = GetStatusLine(code);
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            headerLines.Add("Content-Type: " + contentType);
            headerLines.Add("Content-Length: " + content.Length);
            headerLines.Add("Date: " + DateTime.Now);
            if (code == StatusCode.Redirect)
                headerLines.Add("Redirect: " + redirectoinPath);

            // TODO: Create the request string
            responseString = statusLine;
            foreach (string header in headerLines)
            {
                responseString += header + "\r\n";
            }
            responseString += "\r\n" + content;
        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string message = "ok";
            if (code == StatusCode.BadRequest)
                message = "Bad Request";
            else if (code == StatusCode.InternalServerError)
                message = "Internal Server Error";
            else if (code == StatusCode.NotFound)
                message = "Not Found";
            else if (code == StatusCode.Redirect)
                message = "Redirect";

            string statusLine = "HTTP/1.1 " + code + ' ' + message + "\r\n";
            return statusLine;
        }
    }
}
