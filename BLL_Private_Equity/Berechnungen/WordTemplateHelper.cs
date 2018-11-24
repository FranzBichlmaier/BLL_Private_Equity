using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace BLL_Private_Equity.Berechnungen
{
    public static class WordTemplateHelper
    {

        private static List<string> templates = new List<string>();
        private static FileInfo[] files;

       
      
        public static List<string> GetWordTemplates()
        {
            DirectoryHelper.CheckDirectory("WordTemplates");

            DirectoryInfo directoryInfo = DirectoryHelper.GetWordTemplateDirectory();

            files = directoryInfo.GetFiles();
            templates = new List<string>();
            for (int i = 0; i < directoryInfo.GetFiles().Length; i++)
            {
                FileInfo info = files[i];
                templates.Add(info.Name);
            }
            return templates;
        }

        public static FileInfo GetWordFileInfo(string name)
        {
            int pos = templates.FindIndex(x => x == name);
            if (pos < 0) return null;
            return files[pos];
        }
    }
}
