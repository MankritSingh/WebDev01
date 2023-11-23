using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;

namespace UserValidation
{
    public static class UserValidationsWithDb
    {
        static  void Main(string[] args)
        {
        
        }
        public static IEnumerable<string[]> DbFetchEntry(string navFileSource)
        {
            using (var reader = new StreamReader(navFileSource, Encoding.UTF8))
            {
                while (!reader.EndOfStream)
                {
                    var Line = reader.ReadLine().Split(':');
                    yield return Line;
                }
            }
        }

        public static bool IsUserPresent(string navFileSource, string username)
        {
            foreach (var entry in DbFetchEntry(navFileSource))
            {
                if (username == entry[0]) return true;
            }
            return false;

        }
        public static void AddUser(string navFileSource, string username, byte[] passwordHash, byte[] passwordSalt)
        {
            if (IsUserPresent(navFileSource, username)) return;
            string userPasswordHash = Encoding.UTF8.GetString(passwordHash);
            string userPasswordSalt = Encoding.UTF8.GetString(passwordSalt);
            string userInfoString = username + ":" + userPasswordHash + ":" + userPasswordSalt;
            File.AppendAllText(navFileSource, userInfoString + Environment.NewLine);

        }
        public static bool IsAuthorized(string navFileSource, string uid, string pwd)
        {
            foreach (var entry in DbFetchEntry(navFileSource))
            {
                if (uid == entry[0] && pwd == entry[0])
                {
                    return true;
                }
            }
            return false;
        }
    }
}


