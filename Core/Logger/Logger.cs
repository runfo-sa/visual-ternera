﻿using System.IO;

namespace Core.Logger
{
    public class Logger
    {
        public static string Log(string content)
        {
            DateTime date = DateTime.Now;

            var commonpath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var path = Path.Combine(commonpath, "Visual Ternera");
            var file = Path.Combine(path, date.ToString("yyyy_MM_dd") + ".log");
            Directory.CreateDirectory(path);

            string separator = new('-', 128);
            File.AppendAllText(file,
                $"{separator}{Environment.NewLine}Error - {date:HH::mm:ss}{Environment.NewLine}{separator}{Environment.NewLine}{content}{Environment.NewLine}");

            return file;
        }
    }
}
