using Camurphy.CopyWithMd5.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Camurphy.CopyWithMd5
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] files = Directory.GetFiles(Settings.Default.SourceDirectory);

            foreach (string file in files)
            {
                string filename = Path.GetFileName(file);

                if (Path.GetExtension(file).ToLower() == ".md5")
                {
                    continue;
                }

                string fileHash = null;

                using (MD5 md5 = MD5.Create())
                {
                    try
                    {
                        using (Stream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.None))
                        {
                            fileHash = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                        }
                    }
                    catch (IOException)
                    {
                        continue;
                    }
                }

                if (fileHash == null)
                {
                    continue;
                }

                using (StreamWriter md5File = new StreamWriter(file + ".md5", false))
                {
                    md5File.Write(fileHash);
                }

                try
                {
                    File.Move(file + ".md5", Settings.Default.DestinationDirectory + Path.GetFileName(file) + ".md5");
                    File.Move(file, Settings.Default.DestinationDirectory + Path.GetFileName(file));
                }
                catch (IOException)
                {
                    continue;
                }
            }
        }
    }
}