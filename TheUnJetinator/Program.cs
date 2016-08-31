using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheUnJetinator
{
    class Program
    {
        private const string JETBRAINS_HEADER = "// Decompiled with JetBrains decompiler";
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("The UnJetinator requires an argument to function!");
            }

            UnJetinate(new DirectoryInfo(args[args.Length - 1]));
        }
        
        private static void UnJetinate(DirectoryInfo root)
        {
            Console.WriteLine("UnJetinating {0}", root);
            
            foreach (var file in root.EnumerateFiles())
            {
                if (file.Extension == ".cs")
                {
                    UnJetinate(file);
                }
            }

            foreach (var directory in root.EnumerateDirectories())
            {
                UnJetinate(directory);
            }

            Console.WriteLine("The UnJetinator just unjetinated {0}", root);
        }

        private static void UnJetinate(FileInfo file)
        {
            string content = null;
            using (StreamReader reader = new StreamReader(file.OpenRead()))
            {
                content = reader.ReadToEnd();
            }

            // only unjetinate, if it is jetinated
            if (content.StartsWith(JETBRAINS_HEADER))
            {
                // kill the beast... for glory!
                using (StreamWriter writer = new StreamWriter(new FileStream(file.FullName, FileMode.Create)))
                {
                    writer.Write(string.Join("\n", content.Split('\n').Skip(6).ToArray()));
                }
            }
        }
    }
}
