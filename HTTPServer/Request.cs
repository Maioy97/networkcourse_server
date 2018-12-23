using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string requestString;
        RequestMethod method;
        public string relativeURI;
        HTTPVersion httpVersion;
        string[] requestLines;  
        string[] contentLines;
        // atri value
        Dictionary<string, string> headerLines = new Dictionary<string, string>();

        //
        string[] splitRequestString;
        int index_splitRequestString;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            //throw new NotImplementedException();

            //TODO: parse the receivedRequest using the \r\n delimeter   
            splitRequestString = Regex.Split(requestString, "\r\n");
   
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (splitRequestString.Count() >= 3)
            {
                // Parse Request line
               bool x = this.ParseRequestLine();
                // Validate blank line exists
               bool y = this.ValidateBlankLine();
                // Load header lines into HeaderLines dictionary
               bool z = this.LoadHeaderLines();
               return x && y && z;
            }
            else
                return false;
        }

        private bool ParseRequestLine()
        {
            //throw new NotImplementedException();
            string [] req_line;
            string[] uri;
            req_line = splitRequestString[0].Split(' ');

            if (req_line[0].ToLower() == "get")
                this.method = RequestMethod.GET;
            else if (req_line[0].ToLower() == "head")
                this.method = RequestMethod.HEAD;
            else if (req_line[0].ToLower() == "post")
                this.method = RequestMethod.POST;
            else
                return false;

            uri = req_line[1].Split('/');
            this.relativeURI = uri[1];

            if (ValidateIsURI(req_line[1]) != true)
                return false;

            if (req_line[2].ToLower() == "http/0.9")
                this.httpVersion = HTTPVersion.HTTP09;
            else if (req_line[2].ToLower() == "http/1.0")
                this.httpVersion = HTTPVersion.HTTP10;
            else if (req_line[2].ToLower() == "http/1.1")
                this.httpVersion = HTTPVersion.HTTP11;
            else
                return false;

            return true;
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            //throw new NotImplementedException();
            string[] substrings;
            index_splitRequestString = 1;
            this.headerLines= new Dictionary<string, string>();
            for (; splitRequestString[index_splitRequestString] != ""; index_splitRequestString++)
            {
                if (index_splitRequestString == 1 && this.httpVersion == HTTPVersion.HTTP11)
                {
                    int x=splitRequestString[1].Length;
                    string firstheaderline = splitRequestString[1].Substring(5, splitRequestString[1].Length-5);
                    substrings = firstheaderline.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    substrings = splitRequestString[index_splitRequestString].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                }
                //this.requestLines[index_splitRequestString - 1] = splitRequestString[index_splitRequestString];
                headerLines.Add(substrings[0] , substrings[1]);
            }
            if (index_splitRequestString == 1 && this.httpVersion == HTTPVersion.HTTP10)
                return true;
            else if (index_splitRequestString > 1)
            {
                index_splitRequestString++;
                return true;
            }
            else
                return false;
        }

        private bool ValidateBlankLine()
        {
            //throw new NotImplementedException();
            return(this.splitRequestString.Contains(""));            
        }

        // not exist in temp
        /*
        private bool ValidatecontentLines()
        {
            for (int i = 0; index_splitRequestString < splitRequestString.Count(); i++)
            {
                this.contentLines[i] = splitRequestString[index_splitRequestString];
                index_splitRequestString++;
            }
            if (contentLines.Count() != 0 && this.method == RequestMethod.POST)
                return false;
            else
                return true;
        }*/

    }
}
