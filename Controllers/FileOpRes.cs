using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileService.Controllers
{
    public class FileOpRes
    {
        // 错误代码
        public string Code { get; }

        // 错误描述
        public string ErrMsg { get; }

        // 实体数据
        public Object Data { get; set; }


        public FileOpRes(string code, string errMsg, Object data)
        {
            Code = code;
            ErrMsg = errMsg;
            Data = data;
        }
    }
}
