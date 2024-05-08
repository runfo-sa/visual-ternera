using System.Diagnostics;
using System.Text;

namespace Core
{
    public class GitTag(string Tag, string Date, string Message)
    {
        public string Tag { get; set; } = Tag;
        public string Date { get; set; } = Date;
        public string Message { get; set; } = Message;

        public static GitTag Parse(string input)
        {
            string[] inputs = input.Split('|', StringSplitOptions.TrimEntries);

            return new GitTag(inputs[0], inputs[1], inputs[2]);
        }

        public override String ToString()
        {
            return $"{Tag} :: {Date} :: {Message}";
        }

        public static string RunGitCommand(string command, string args, string workingDirectory)
        {
            StringBuilder sb = new();

            var proc = new Process
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
