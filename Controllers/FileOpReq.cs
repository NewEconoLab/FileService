using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileService.Controllers
{
    public class FileOpReq
    {
        // 文件类型
        public string type { get; set; }

        // 文件版本
        public string version { get; set; }

        // 文件内容
        public string content { get; set; }
    }
}
