using System.Diagnostics;
using System.Text;

namespace Core.Git
{
    public class Git
    {
        public static string RunGitCommand(string command, string args, string workingDirectory)
        {
            StringBuilder sb = new();

            using var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = $"{command} {args}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    WorkingDirectory = workingDirectory,
                }
            };
            proc.Start();

            while (!proc.StandardOutput.EndOfStream)
            {
                sb.Append($"{proc.StandardOutput.ReadLine()}");
            }

            proc.WaitForExit();
            return sb.ToString();
        }
    }
}
