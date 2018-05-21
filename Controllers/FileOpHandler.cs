using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FileService.Controllers
{
    public class FileOpHandler
    {
        private string default_dir = "d://";

        public FileOpHandler(FileOpInfo config)
        {
            if (!string.IsNullOrEmpty(config.Path))
                default_dir = config.Path;
        }

        public bool uploadFileBytes(string type, string version, string content) 
        {
            string filepath = getFilePath(type, version);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(content);
            using (FileStream fs = new FileStream(filepath, FileMode.Append))
            {
                fs.Write(bytes, 0, bytes.Length);
            }
            return true;
        }


        public string downloadFileBytes(string type, string version)
        {
            string filepath = getFilePath(type, version);
            byte[] bytes = null;
            using (FileStream fs = new FileStream(filepath, FileMode.Open))
            {
                int len = (int)fs.Length;
                bytes = new byte[len];
                fs.Read(bytes, 0, len);
            }
            return System.Text.Encoding.UTF8.GetString(bytes);
        }


        public string getMaxVersionByType(string type)
        {
            string maxVersion = "0";
            string dir = getDir();
            var files = Directory.GetFiles(dir);
            foreach (var file in files)
            {
                if (file.Contains(type))
                {
                    int st = file.LastIndexOf("_");
                    int ed = file.LastIndexOf(".");
                    string newVersion = file.Substring(st + 1, ed - st - 1);
                    if ( double.Parse(maxVersion) < double.Parse(newVersion) )
                    {
                        maxVersion = newVersion;
                    }
                }
            }
            return maxVersion;
        }

        public string getFilePath(string type, string version)
        {
            return getDir() + "//" + type + "_" + version + ".bin";
        }
        private string getDir()
        {
            return default_dir;
        }     
    }
}
