using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FileService.Controllers
{
    /// <summary>
    /// 文件操作控制器
    /// 
    ///  文件上传、文件下载、获取最大版本号
    /// 
    /// 
    /// </summary>
    [Route("api/s9670/v0")]
    public class FileOpController : Controller
    {
        private FileOpInfo config;
        // 文件操作处理器
        private FileOpHandler handler = null;// new FileOpHandler();


        public FileOpController(IOptions<FileOpInfo> setting)
        {
            config = setting.Value;
            handler = new FileOpHandler(config);
        }

        /// <summary>
        /// 上传文件
        /// 
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("upload")]
        public FileOpRes uploadFileBytes([FromBody] FileOpReq req)
        {

            string type = req.type;
            string version = req.version;
            string content = req.content;

            // 参数检查 
            if (string.IsNullOrEmpty(type)) return newFailedRes(RespCode.FILE_OP_SERVICE_IllegalParameter, "参数不能为空(type)");
            if (string.IsNullOrEmpty(version)) return newFailedRes(RespCode.FILE_OP_SERVICE_IllegalParameter, "参数不能为空(version)");
            if (string.IsNullOrEmpty(content)) return newFailedRes(RespCode.FILE_OP_SERVICE_IllegalParameter, "参数不能为空(content)");

            // 上传数据
            bool flag = handler.uploadFileBytes(type, version, content);

            // 返回
            return newSuccessRes(new FileSave(flag));

        }


        /// <summary>
        /// 下载文件
        /// 
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("download")]
        public FileOpRes downloadFileBytes([FromBody] FileOpReq req)
        {
            string type = req.type;
            string version = req.version;

            // 参数检查
            if (string.IsNullOrEmpty(type)) return newFailedRes(RespCode.FILE_OP_SERVICE_IllegalParameter, "参数不能为空(type)");
            if (string.IsNullOrEmpty(version)) return newFailedRes(RespCode.FILE_OP_SERVICE_IllegalParameter, "参数不能为空(version)");

            // 下载数据
            string content = handler.downloadFileBytes(type, version);

            // 返回
            return newSuccessRes(new FileContent(content));
        }


        /// <summary>
        /// 获取最大版本
        /// 
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("version")]
        public FileOpRes getMaxVersionByType([FromBody] FileOpReq req)
        {
            string type = req.type;

            // 参数检查
            if (string.IsNullOrEmpty(type)) return newFailedRes(RespCode.FILE_OP_SERVICE_IllegalParameter, "参数不能为空(type)");

            // 下载数据
            string version = handler.getMaxVersionByType(type);

            // 返回
            return newSuccessRes(new FileVersion(version));
        }


        // 测试POST请求参数
        [HttpPost("info")]
        public string getReqInfo([FromBody] FileOpReq req)
        {
            return "FileOpController.req(POST).parameter:" + ",type=" + req.type + ",version=" + req.version + ",content=" + req.content + ",random:" + new Random().Next(0, 9);
        }

        // 测试服务
        [HttpGet("test")]
        public string test()
        {
            return "welcome to FileController~~~" + new Random().Next(0, 9);
        }

        private FileOpRes newFailedRes(String code, String errMsg)
        {
            return new FileOpRes(code, errMsg, "");
        }
        private FileOpRes newSuccessRes(Object data)
        {
            return new FileOpRes(RespCode.SUCCESS, "", data);
        }

        [HttpPost("fileupload")]
        public async Task<FileOpRes> Upload(string version, string type, IFormFile uploadfile)
        {
            // 参数检查 
            if (string.IsNullOrEmpty(type)) return newFailedRes(RespCode.FILE_OP_SERVICE_IllegalParameter, "参数不能为空(type)");
            if (string.IsNullOrEmpty(version)) return newFailedRes(RespCode.FILE_OP_SERVICE_IllegalParameter, "参数不能为空(version)");

            var filePath = handler.getFilePath(type, version);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await uploadfile.CopyToAsync(stream);
            }
            return newSuccessRes(new FileSave(true));
        }

    }
    public class FileParam
    {
        public string type { get; }
        public string version { get; }
    }

    internal class FileSave
    {
        public bool Success { get; set; }

        public FileSave(bool success)
        {
            Success = success;
        }
    }
    internal class FileContent
    {
        public string Content { get; set; }

        public FileContent(string content)
        {
            Content = content;
        }
    }
    internal class FileVersion
    {
        public string Version { get; set; }

        public FileVersion(string version)
        {
            Version = version;
        }
    }

    internal class RespCode
    {
        public static string SUCCESS = "000000";
        public static string FILE_OP_SERVICE_IllegalParameter = "100001";
        public static string FILE_OP_SERVICE_NotSupportContentType = "100002";
    }
}
