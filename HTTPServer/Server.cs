using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;
        int maxconnection;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket
            this.serverSocket=new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint IpE = new IPEndPoint(IPAddress.Parse("127.0.0.1"), portNumber);
            serverSocket.Bind(IpE);
            maxconnection = 100;
        }

        public void StartServer()
        {  
            // TODO: Listen to connections, with large backlog.
            serverSocket.Listen(maxconnection);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket clientSocket = serverSocket.Accept();
                Console.WriteLine("New client accepted: {0}", clientSocket.RemoteEndPoint);

                Thread thread = new Thread(new ParameterizedThreadStart(this.HandleConnection));
                thread.Start(clientSocket);
              
            }
        }

        public void HandleConnection(object obj)
        {

            // TODO: Create client socket 
            Socket clientsoc = (Socket)obj;
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            clientsoc.ReceiveTimeout = 0;
            int receivedLength = 0;
            byte[] message = new byte[1024];
            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {                    
                    // TODO: Receive request
                    receivedLength = clientsoc.Receive(message);
                    // TODO: break the while loop if receivedLen==0
                    if (receivedLength == 0) { break; }
                    // TODO: Create a Request object using received request string
                    Request req = new Request(Encoding.ASCII.GetString(message, 0, receivedLength));
                    //Console.WriteLine(Encoding.ASCII.GetString(message, 0, receivedLength));
                    // TODO: Call HandleRequest Method that returns the response
                    Response res = this.HandleRequest(req);
                    // TODO: Send Response back to client
                    message = Encoding.ASCII.GetBytes(res.ResponseString);
                    //Console.WriteLine(Encoding.ASCII.GetString(message, 0, receivedLength));
                    clientsoc.Send(message);
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }
            // TODO: close client socket
            clientsoc.Close();
        }

        Response HandleRequest(Request request)
        {
            //throw new NotImplementedException();
            string content ;
            string contentpath;
            string redirectionpath="";
            StatusCode stC ;
            try
            {
                //TODO: map the relativeURI in request to get the physical path of the resource.
                contentpath = Configuration.RootPath + "\\" + request.relativeURI;
                bool proper_request = request.ParseRequest();
                //TODO: check for bad request 
                if (!proper_request)
                {
                    stC = StatusCode.BadRequest;
                    contentpath = Configuration.RootPath + '\\' + Configuration.NotFoundDefaultPageName;

                }

                //TODO: check for redirect
                else if (Configuration.RedirectionRules.ContainsKey(request.relativeURI))
                {
                    stC = StatusCode.Redirect;
                    redirectionpath = Configuration.RedirectionRules[request.relativeURI];
                    contentpath = Configuration.RootPath + "\\" + redirectionpath;
                }
                //TODO: check file exists
                else if (File.Exists(contentpath))
                {
                    // Create OK response
                    stC = StatusCode.OK;
                }
                else
                {
                    stC = StatusCode.NotFound;
                    contentpath = Configuration.RootPath + '\\' + Configuration.NotFoundDefaultPageName;
                }
                //TODO: read the physical file
                content = File.ReadAllText(contentpath);
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                // TODO: in case of exception, return Internal Server Error. 
                Logger.LogException(ex);
                stC = StatusCode.InternalServerError;
                contentpath = Configuration.RootPath + '\\' + Configuration.InternalErrorDefaultPageName;
                content = File.ReadAllText(contentpath);
            }
            return new Response(stC, "html", content, redirectionpath);
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            
            return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            
            // else read file and return its content
            return string.Empty;
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                // then fill Configuration.RedirectionRules dictionary   aboutus.html,aboutus2.html
                string[] redirectionline = File.ReadAllLines(filePath);
                foreach (string line in redirectionline)
                {
                    string[] uri = line.Split(',');
                    Configuration.RedirectionRules = new Dictionary<string, string>() {{ uri[0], uri[1] }};
                }
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
