<%@ Page ContentType="application/json" %>

<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Runtime.Serialization.Json" %>
<%@ Import Namespace="System.Runtime.Serialization" %>
<%@ Import Namespace="System.Runtime.Serialization.Json" %>
<%@ Import Namespace="System.Linq" %>

<script language="C#" runat="server">    

    //  Sergey Dobychin 
    //  dobychin@gmail.com
    //  May, 2014

    // don't forget about IIS site settings 
    //<system.web>
    //    <httpRuntime executionTimeout="9999999" maxRequestLength="2097151" />
    //</system.web>
    //<system.webServer>
    //    <security>
    //        <requestFiltering>
    //            <requestLimits maxAllowedContentLength="2147483648" />
    //        </requestFiltering>
    //    </security>
    //</system.webServer>    

    //private static readonly FilesDisposition FILES_DISPOSITION = FilesDisposition.ServerRoot;
	private static readonly FilesDisposition FILES_DISPOSITION = FilesDisposition.Absolute;
    private string FILES_PATH;
    private string AccountEncoded;  //編碼過後的目錄名稱
    
    private static readonly string FILE_QUERY_VAR = "file";
    private static readonly string FILE_GET_CONTENT_TYPE = "application/octet-stream";
    
    private static readonly int ATTEMPTS_TO_WRITE = 3;
    private static readonly int ATTEMPT_WAIT = 100; //msec

    private static readonly int BUFFER_SIZE = 4 * 1024 * 1024;
    
    private enum FilesDisposition
    {
        ServerRoot,
        HandlerRoot,
        Absolute
    }
        
    private static class HttpMethods
    {
        public static readonly string GET = "GET";
        public static readonly string POST = "POST";
        public static readonly string DELETE = "DELETE";
    }

    [DataContract]
    private class FileResponse
    {
        [DataMember]
        public string name;
        [DataMember]
        public long size;
        [DataMember]
        public string type;
        [DataMember]
        public string url;
        [DataMember]
        public string error;
        [DataMember]
        public string deleteUrl;
        [DataMember]
        public string deleteType;
    }
    
    [DataContract]
    private class UploaderResponse
    {
        [DataMember]
        public FileResponse[] files;

        public UploaderResponse(FileResponse[] fileResponses)
        {
            files = fileResponses;
        }
    }
    
    private string CreateFileUrl(string fileName, FilesDisposition filesDisposition)
    {
        switch (filesDisposition)
        {
            case FilesDisposition.ServerRoot:
                // 1. files directory lies in root directory catalog WRONG
                return String.Format("{0}{1}/{2}", Request.Url.GetLeftPart(UriPartial.Authority),
                    FILES_PATH, Path.GetFileName(fileName));

            case FilesDisposition.HandlerRoot:
                // 2. files directory lays in current page catalog WRONG
                return String.Format("{0}{1}{2}/{3}", Request.Url.GetLeftPart(UriPartial.Authority), 
                    Path.GetDirectoryName(Request.CurrentExecutionFilePath).Replace(@"\", @"/"), FILES_PATH, Path.GetFileName(fileName));

            case FilesDisposition.Absolute:
                // 3. files directory lays anywhere YEAH
                return String.Format("{0}?{1}={2}", Request.Url.AbsoluteUri, FILE_QUERY_VAR, HttpUtility.UrlEncode(Path.GetFileName(fileName)));
            default:
                return String.Empty;
        }
    }
    
    private FileResponse CreateFileResponse(string fileName, long size, string error)
    {
        return new FileResponse()
        {
            name = Path.GetFileName(fileName),
            size = size,
            type = String.Empty,
            url = CreateFileUrl(fileName, FILES_DISPOSITION),
			//url = @"http://ed.chu.edu.tw/staff/GetFile.ashx?s=" + AccountEncoded + "&f=" + HttpUtility.UrlEncode(Path.GetFileName(fileName)),
            error = error,
            deleteUrl = CreateFileUrl(fileName, FilesDisposition.Absolute),
			deleteType = HttpMethods.POST
            //deleteType = HttpMethods.DELETE
        };
    }

    private void SerializeUploaderResponse(List<FileResponse> fileResponses)
    {
        DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(UploaderResponse));
        
        Serializer.WriteObject(Response.OutputStream, new UploaderResponse(fileResponses.ToArray()));        
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
    
    protected void Page_Load(object sender, EventArgs e)
    {
      
        //將帳號編碼過: 帳號094169 => 目錄zqvytq         
        string Account = HttpContext.Current.User.Identity.Name;
        AccountEncoded = "";
        string EncodeString = "zyxwvutsrq";     // 編碼規則: 0=>z 1=y 2=x ..... 9=q
        for (int i=0;i<Account.Length;i++)
        {
            AccountEncoded = AccountEncoded + EncodeString.Substring( Convert.ToInt16(Account.Substring(i,1)) ,1);
        }
        FILES_PATH = @"d:\Source\tester0724\tester0724\Uploads\";  // +AccountEncoded;
        
        string FilesPath;

        switch (FILES_DISPOSITION)
        {
            case FilesDisposition.ServerRoot:
                FilesPath = Server.MapPath(FILES_PATH);
                break;
            case FilesDisposition.HandlerRoot:
                FilesPath = Server.MapPath(Path.GetDirectoryName(Request.CurrentExecutionFilePath) + FILES_PATH);
                break;
            case FilesDisposition.Absolute:
                FilesPath = FILES_PATH;
                break;
            default:
                Response.StatusCode = 500;
                Response.StatusDescription = "Configuration error (FILES_DISPOSITION)";
                return;
        }   

        // prepare directory
        if (!Directory.Exists(FilesPath))
        {
            Directory.CreateDirectory(FilesPath);
        }


        string QueryFileName = Request[FILE_QUERY_VAR];  //從POST資料取出上傳的檔案名稱
        string FullFileName = null;     //用來放完整路徑
        string ShortFileName = null;  //存放檔名

        //if (!String.IsNullOrEmpty(QueryFileName))
        // 判斷上傳的檔名變數是否有內容 
        if (QueryFileName != null) // param specified, but maybe in wrong format (empty). else user will download json with listed files
        {
            ShortFileName = HttpUtility.UrlDecode(QueryFileName);     //取出檔名
            FullFileName = String.Format(@"{0}\{1}", FilesPath, ShortFileName);   // 結合完整路徑與檔名~~成為完整PATH

            if (QueryFileName.Trim().Length == 0 || !File.Exists(FullFileName))  //判斷檔案是否存在
            {
                Response.StatusCode = 404;
                Response.StatusDescription = "File not found";

                Response.End();
                return;
            }
        }       
        
        if (Request.HttpMethod.ToUpper() == HttpMethods.GET)   // ---- GET 的處理片段  -----------------------------------
        {           
            if (FullFileName != null)
            {
                Response.ContentType = FILE_GET_CONTENT_TYPE;                   // http://www.digiblog.de/2011/04/android-and-the-download-file-headers/ :)
                Response.AddHeader("Content-Disposition", String.Format("attachment; filename={0}{1}", Path.GetFileNameWithoutExtension(ShortFileName), Path.GetExtension(ShortFileName).ToUpper()));

                using (FileStream FileReader = new FileStream(FullFileName, FileMode.Open, FileAccess.Read))
                {
                    FromStreamToStream(FileReader, Response.OutputStream);
  
                    Response.OutputStream.Close();
                }

                Response.End();
                return;
            }
            else  // FullFileName == null
            {
                //檔案列表
                List<FileResponse> FileResponseList = new List<FileResponse>();
                // get your files (names)                   
                string[] FileNames = Directory.GetFiles(FilesPath);
                // Now read the creation time for each file
                //DateTime[] creationTimes = new DateTime[FileNames.Length];
                //for (int i = 0; i < FileNames.Length; i++)
                //    creationTimes[i] = new FileInfo(FileNames[i]).CreationTime;
                // sort it
                //Array.Sort(creationTimes, FileNames);
                
                DateTime TimeRange = DateTime.Now.AddDays(-1); //一天內
                
                foreach (string FileName in FileNames)
                {
                    if (new FileInfo(FileName).CreationTime > TimeRange)
                    {
                        FileResponseList.Add(CreateFileResponse(FileName, new FileInfo(FileName).Length, String.Empty));
                    }
                }

                SerializeUploaderResponse(FileResponseList);
            }            
        }
        else if (Request.HttpMethod.ToUpper() == HttpMethods.POST && Request.QueryString["file"] == null) // ---- POST 的處理片段  -----------------------------------
        {
            List<FileResponse> FileResponseList = new List<FileResponse>();

            for (int FileIndex = 0; FileIndex < Request.Files.Count; FileIndex++)  //利用 HttpPostedFile 方式來將檔案寫入到 Server
            {
                HttpPostedFile File = Request.Files[FileIndex];

                string FileName = String.Format(@"{0}\{1}", FilesPath, Path.GetFileName(File.FileName));
                string ErrorMessage = String.Empty;
                
                for (int Attempts = 0; Attempts < ATTEMPTS_TO_WRITE; Attempts++)
                {
                    ErrorMessage = String.Empty;

                    if (System.IO.File.Exists(FileName))
                    {
                        FileName = String.Format(@"{0}\{1}_{2:yyyyMMddHHmmss.fff}{3}", FilesPath, Path.GetFileNameWithoutExtension(FileName), DateTime.Now, Path.GetExtension(FileName));
                    }
                    File.SaveAs(FileName);
                     break;
                }
               
                //FileResponseList.Add(CreateFileResponse(File.FileName, File.ContentLength, ErrorMessage));
				FileResponseList.Add(CreateFileResponse(FileName, File.ContentLength, ErrorMessage));
            }

            SerializeUploaderResponse(FileResponseList);
        }
        //else if (Request.HttpMethod.ToUpper() == HttpMethods.DELETE)
        // else if (Request.HttpMethod.ToUpper() == HttpMethods.POST && Request.QueryString["file"] == null) // ---- POST 的處理片段  -----------------------------------
		else if (Request.HttpMethod.ToUpper() == HttpMethods.POST && Request.QueryString["file"] != null)   // 刪除檔案之片段
        {			   
            bool SuccessfullyDeleted = true;          
            
            try
            {
                File.Delete(FullFileName);
            }
            catch
            {
                SuccessfullyDeleted = false;
            }

            Response.Write(String.Format("{{\"{0}\":{1}}}", ShortFileName, SuccessfullyDeleted.ToString().ToLower()));            
        }
        else
        {
            Response.StatusCode = 405;
            Response.StatusDescription = "Method not allowed";
            Response.End();

            return;
        }

        
        Response.End();
    }
    
    

</script>