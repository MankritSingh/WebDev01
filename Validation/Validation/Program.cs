using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;

namespace Validation
{
    public class Program
    {
        static void Main(string[] args)
        {
            string url = "http://localhost:8080/";
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            //string fileSource = @"C:\Users\QP-202204\Desktop\Mankritold\WebDev01Githubrepo\WebDev01\Validation\db.txt";
            string fileSource = @"..\..\..\..\db.txt";
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

                    //bool result = IsAuthorized(fileSource, username, password);
                    //Console.WriteLine(result);


                    response.AddHeader("Access-Control-Allow-Origin", "*");
                    response.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE");
                    response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Authorization");
                    response.AddHeader("Access-Control-Allow-Credentials", "true");
                    if (IsAuthorized(fileSource, username, password))
                    {
                        string message = "Successfully Validated";
                        byte[] buffer = Encoding.UTF8.GetBytes(message);

                        response.StatusCode = (int)HttpStatusCode.OK;
                        response.ContentLength64 = buffer.Length;

                        using (var output = response.OutputStream)
                        {
                            output.Write(buffer, 0, buffer.Length);
                        }
                    }
                    else {
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    }                   
                    response.Headers.Add("Content-Type", "application/json");                    
                    response.Close();
                }
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

