using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GetFile
{
    /// <summary>
    /// get 的摘要描述
    /// </summary>
    public class get : IHttpHandler
    {
        private static readonly int BUFFER_SIZE = 4 * 1024 * 1024;

        public void ProcessRequest(HttpContext context)
        {
            //context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");
            string FILE_QUERY_VAR = "f";
            string FILEDIR_QUERY_VAR = "s";
            string QueryFileName = context.Request[FILE_QUERY_VAR];
            string QueryFileDirectory = context.Request[FILEDIR_QUERY_VAR].Substring(0, 6);  //只取前六碼，不確定是否會有被攻擊的問題
            string FILE_GET_CONTENT_TYPE = "application/octet-stream";
            string FILES_PATH = @"d:\Source\tester0724\tester0724\Uploads\" + QueryFileDirectory;
            string FilesPath = FilesPath = FILES_PATH;

            string FullFileName = null;
            string ShortFileName = null;

            //if (!String.IsNullOrEmpty(QueryFileName))
            if (QueryFileName != null) // param specified, but maybe in wrong format (empty). else user will download json with listed files
            {
                ShortFileName = HttpUtility.UrlDecode(QueryFileName);
                FullFileName = String.Format(@"{0}\{1}", FilesPath, ShortFileName);

                //檔名長度為0或檔案不存在，則回傳404錯誤
                if (QueryFileName.Trim().Length == 0 || !File.Exists(FullFileName))
                {
                    context.Response.StatusCode = 404;
                    context.Response.StatusDescription = "File not found";

                    context.Response.End();
                    return;
                }

                //解決IE下載中文檔名變亂碼問題
                string FileNameWithoutExtension = Path.GetFileNameWithoutExtension(ShortFileName);
                if (context.Request.Browser.Browser == "IE")
                {
                    FileNameWithoutExtension = HttpUtility.UrlPathEncode(FileNameWithoutExtension);
                }

                context.Response.ContentType = FILE_GET_CONTENT_TYPE;                   // http://www.digiblog.de/2011/04/android-and-the-download-file-headers/ :)
                context.Response.AddHeader("Content-Disposition", String.Format("attachment; filename={0}{1}", FileNameWithoutExtension, Path.GetExtension(ShortFileName).ToUpper()));

                using (FileStream FileReader = new FileStream(FullFileName, FileMode.Open, FileAccess.Read))
                {
                    FromStreamToStream(FileReader, context.Response.OutputStream);

                    context.Response.OutputStream.Close();
                }

                context.Response.End();
            }
            else
            {
                //未提供參數，則回傳404錯誤
                context.Response.StatusCode = 404;
                context.Response.StatusDescription = "File not found";
            }

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private void FromStreamToStream(Stream source, Stream destination)
        {
            int BufferSize = source.Length >= BUFFER_SIZE ? BUFFER_SIZE : (int)source.Length;
            long BytesLeft = source.Length;

            byte[] Buffer = new byte[BufferSize];

            int BytesRead = 0;

            while (BytesLeft > 0)
            {
                BytesRead = source.Read(Buffer, 0, BytesLeft > BufferSize ? BufferSize : (int)BytesLeft);

                destination.Write(Buffer, 0, BytesRead);

                BytesLeft -= BytesRead;
            }
        }


    }
}