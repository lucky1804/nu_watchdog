using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

class Program
{
    static bool exitRequested = false;

    static void Main()
    {
        // Start a background thread to listen for the ESC key press
        Thread keyListenerThread = new Thread(KeyListener);
        keyListenerThread.IsBackground = true; // Make it a background thread
        keyListenerThread.Start();

        string fileListPath = "files.txt";

        // Check if the file files.txt exists
        if (!File.Exists(fileListPath))
        {
            Console.WriteLine("error");
            System.Threading.Thread.Sleep(10000); // Wait for 10 seconds before exiting
            return;
        }

        // Read all lines from files.txt
        var lines = File.ReadAllLines(fileListPath);
        bool allFilesValid = true;

        foreach (var line in lines)
        {
            if (exitRequested) return; // Exit if ESC is pressed

            var parts = line.Split(' '); // Split into filename and MD5 hash
            if (parts.Length != 2)
            {
                Console.WriteLine("error");
                System.Threading.Thread.Sleep(10000); // Wait for 10 seconds before exiting
                return;
            }

            string filePath = parts[0];
            string expectedMd5Hash = parts[1];

            // Check if file exists
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File: " + filePath + " | Expected MD5: " + expectedMd5Hash + " | Status: File not found.");
                allFilesValid = false;
                break;
            }

            // Get MD5 hash of the file
            string actualMd5Hash = GetFileMd5(filePath);

            // Output the comparison between the expected and calculated MD5
            Console.WriteLine("File: " + filePath + " | Expected MD5: " + expectedMd5Hash + " | Calculated MD5: " + actualMd5Hash);

            // Check if the MD5 hash matches the expected one
            if (actualMd5Hash != expectedMd5Hash)
            {
                Console.WriteLine("File: " + filePath + " | MD5 mismatch! Expected: " + expectedMd5Hash + " | Calculated: " + actualMd5Hash);
                allFilesValid = false;
                break;
            }
        }

        // If all files exist and have matching MD5 hash, print success
        if (allFilesValid)
        {
            Console.WriteLine("running latest version");
            System.Threading.Thread.Sleep(60000); // Wait for 60 seconds before exiting
        }
        else
        {
            Console.WriteLine("running old version");
            System.Threading.Thread.Sleep(10000); // Wait for 10 seconds before exiting
        }
    }

    static void KeyListener()
    {
        while (!exitRequested)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(intercept: true).Key;
                if (key == ConsoleKey.Escape)
                {
                    exitRequested = true;
                    Console.WriteLine("\nExiting program...");
                    Environment.Exit(0); // Immediately exit the program
                }
            }
        }
    }

    static string GetFileMd5(string filePath)
    {
        using (var md5 = MD5.Create())
        using (var stream = File.OpenRead(filePath))
        {
            byte[] hash = md5.ComputeHash(stream);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
