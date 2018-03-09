using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace masschat
{
    class Program
    {

        static void Main(string[] args)
        {

            Console.WriteLine($"MASS CHAT");
            Console.WriteLine($"Server: {Configuration.Instance.Server}:{Configuration.Instance.Port}");
            Console.WriteLine($"Nick: {Configuration.Instance.Nick}");


            Console.ReadLine();

        }

    }
}
