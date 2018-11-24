using System.Diagnostics;

namespace BLL_Infrastructure
{
    public static class CommonProcesses
    {
        public static void StartWord(string fileName)
        {

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "WINWORD.EXE";
            startInfo.Arguments = fileName;
            Process.Start(startInfo);

        }
    }
}
