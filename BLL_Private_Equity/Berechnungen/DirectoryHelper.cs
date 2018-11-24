using BLL_DataAccess;
using System;
using System.IO;

namespace BLL_Private_Equity.Berechnungen
{
    public static class DirectoryHelper
    {
        private static string rootDirectory = ComboboxLists.GetApplicationInformation("RootDirectory");
        public static void CheckDirectory(string directoryName)
        {
            DirectoryInfo directory = new DirectoryInfo(System.IO.Path.Combine(rootDirectory, directoryName));
            if (directory.Exists) return;

            // create Directories
            string[] levels = directoryName.Split('/');
            string root = rootDirectory;

            foreach (string level in levels)
            {
                root = Path.Combine(root, level);
                try
                {
                    Directory.CreateDirectory(root);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static void CopyFile(string sourceFile, string destination, bool overwrite)
        {
            destination = Path.Combine(rootDirectory, destination);
            try
            {
                File.Copy(sourceFile, destination, (bool)overwrite);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static void DeleteFile(string fileName)
        {
            try
            {
                File.Delete(fileName);
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }

        public static void MoveFile(string sourceFile, string destination, bool overwrite)
        {

            FileInfo fileInfo = new FileInfo(destination);
            if (fileInfo.Exists == true && overwrite == false)
            {
                throw new Exception("eine Datei mit dem gleichen Namen existiert bereits");
            }

            destination = Path.Combine(rootDirectory, destination);
            try
            {
                File.Move(sourceFile, destination);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static FileInfo GetTextFileName(int peFundId)
        {
            string fileName = Path.Combine(rootDirectory, "TextFiles");
            fileName = Path.Combine(fileName, $"{peFundId:D6}");
            FileInfo fileInfo = new FileInfo(fileName);
            return fileInfo;
        }

        public static DirectoryInfo GetWordTemplateDirectory()
        {
            DirectoryInfo info = new DirectoryInfo(Path.Combine(rootDirectory, "WordTemplates"));
            return info;
        }

        public static string FindInvestorCashFlow(int investorId, int cashFlowNumber)
        {
            string name = rootDirectory;
            name = Path.Combine(name, "Investors");
            name = Path.Combine(name, $"{investorId:d6}");
            name = Path.Combine(name, $"CashFlows");
            CheckDirectory(name);
            name = Path.Combine(name, $"{cashFlowNumber:d6}.docx");
            
            return name;
        }
        public static string FindFundCashFlow(int fundId, int cashFlowNumber)
        {
            string name = rootDirectory;
            name = Path.Combine(name, "Funds");
            name = Path.Combine(name, $"{fundId:d6}");
            name = Path.Combine(name, $"CashFlows");
            CheckDirectory(name);
            name = Path.Combine(name, $"{cashFlowNumber:d6}.docx");
            return name;
        }
        public static string GetHqtTelefon(string extension)
        {
            return Properties.Settings.Default.HqtTelefon + extension;
        }
        public static string GetHqtTelefonInternational(string extension)
        {
            return Properties.Settings.Default.HqtTelefonInternational + extension;
        }
    }
}
