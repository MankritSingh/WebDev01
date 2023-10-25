using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;

namespace Validation
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "http://localhost:8080/";
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();

            Console.WriteLine("Listening for HTTP POST requests...");

            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                if (request.HttpMethod == "OPTIONS")
                {
                    response.AddHeader("Access-Control-Allow-Origin", "*");
                    response.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE");
                    response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Authorization");
                    response.AddHeader("Access-Control-Allow-Credentials", "true");
                    response.StatusCode = 200;
                    response.Close();
                }
                else if (request.HttpMethod == "POST")
                {
                    //Way1
                    var header = AuthenticationHeaderValue.Parse(request.Headers["Authorization"]);
                    var credentials = header.Parameter;
                    //Way2
                    var authHeader = request.Headers["Authorization"];
                    string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();

                    Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                    string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));
                    int seperatorIndex = usernamePassword.IndexOf(':');
                    var username = usernamePassword.Substring(0, seperatorIndex);
                    var password = usernamePassword.Substring(seperatorIndex + 1);

                    bool result = IsAuthorized("db.txt", username, password);
                    Console.WriteLine(result);
                }




                //if (request.HttpMethod == "POST")
                //{
                //    using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                //    {
                //        string requestBody = reader.ReadToEnd();
                //        Console.WriteLine("Received POST data: " + requestBody);

                //        // Extract username and password from the request
                //        string[] parts = requestBody.Split('&');
                //        string username = parts[0].Split('=')[1];
                //        string password = parts[1].Split('=')[1];

                //        // Perform authorization
                //        if (IsAuthorized("db.txt", username, password))
                //        {
                //            byte[] responseBytes = Encoding.UTF8.GetBytes("Authorized");
                //            response.StatusCode = (int)HttpStatusCode.OK;
                //            response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
                //        }
                //        else
                //        {
                //            byte[] responseBytes = Encoding.UTF8.GetBytes("Unauthorized");
                //            response.StatusCode = (int)HttpStatusCode.Unauthorized;
                //            response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
                //        }
                //    }
                //}

                //response.Close();
            }
        }

        public static bool IsAuthorized(string navFileSource, string uid, string pwd)
        {
            foreach (var entry in DbFetchEntry(navFileSource))
            {
                if (uid == entry.Item1 && pwd == entry.Item2)
                {
                    return true;
                }
            }
            return false;
        }

        public static IEnumerable<Tuple<string, string>> DbFetchEntry(string navFileSource)
        {
            using (var reader = new StreamReader(navFileSource, Encoding.UTF8))
            {
                while (!reader.EndOfStream)
                {
                    var Line = reader.ReadLine().Split(':');
                    yield return Tuple.Create(Line[0], Line[1]);
                }
            }
        }
    }
}

