<%@ Page ContentType="application/json" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Runtime.Serialization.Json" %>
<%@ Import Namespace="System.Runtime.Serialization" %>
<%@ Import Namespace="System.Runtime.Serialization.Json" %>
<%@ Import Namespace="System.Linq" %>

<script language="C#" runat="server">    
    // 檔案的上傳路徑
    private string Upload_Directory =  @"d:\Source\tester0724\tester0724\Uploads\"; 
    private static readonly int BUFFER_SIZE = 4 * 1024 * 1024;
            
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
    private FileResponse CreateFileResponse(string fileName, long size, string error)
    {
        return new FileResponse()
        {
            name = Path.GetFileName(fileName),
            size = size,
            type = String.Empty,
            url = String.Format("{0}?{1}={2}", Request.Url.AbsoluteUri, "file", HttpUtility.UrlEncode(Path.GetFileName(fileName))),
            error = error,
            deleteUrl = String.Format("{0}?{1}={2}", Request.Url.AbsoluteUri, "file", HttpUtility.UrlEncode(Path.GetFileName(fileName))),
			deleteType = "POST"
        };
    }

    private void SerializeUploaderResponse(List<FileResponse> fileResponses)
    {  // 將物件序列化為 JavaScript 物件標記法 (JSON) 以及將 JSON 資料還原序列化為物件
        DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(UploaderResponse));
        // 將物件序列化為可以對應至 JavaScript 物件標記法 (JSON) 的 XML。 使用 XmlWriter 來寫入所有的物件資料，包括起始 XML 項目、內容和結尾項目。 
        Serializer.WriteObject(Response.OutputStream, new UploaderResponse(fileResponses.ToArray()));        
    }
    
    private void FromStreamToStream(Stream source, Stream destination) // 搭配 GET method 使用
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
        if (!Directory.Exists(Upload_Directory))  // 若目錄不存在則建立之
        {
            Directory.CreateDirectory(Upload_Directory);
        }
        string QueryFileName = Request["file"];  //從POST資料取出上傳的檔案名稱
        string FullFileName = null;     //用來放完整路徑
        string ShortFileName = null;  //存放檔名
        // 判斷上傳的檔名變數是否有內容 
        if (QueryFileName != null) // param specified, but maybe in wrong format (empty). else user will download json with listed files
        {
            ShortFileName = HttpUtility.UrlDecode(QueryFileName);     //取出檔名
            FullFileName = String.Format(@"{0}\{1}", Upload_Directory, ShortFileName);   // 結合完整路徑與檔名~~成為完整PATH
            if (QueryFileName.Trim().Length == 0 || !File.Exists(FullFileName))  //判斷檔案是否存在
            {
                Response.StatusCode = 404;
                Response.StatusDescription = "File not found";
                Response.End();
                return;
            }
        }       
        
        if (Request.HttpMethod.ToUpper() == "GET")   // ---- GET 的處理片段  -----------------------------------
        {           
            if (FullFileName != null)
            {
                Response.ContentType = "application/octet-stream";                   // http://www.digiblog.de/2011/04/android-and-the-download-file-headers/ :)
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
                string[] FileNames = Directory.GetFiles(Upload_Directory);
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
        }  //EOF --- if (Request.HttpMethod.ToUpper() == "GET")
        else if (Request.HttpMethod.ToUpper() == "POST" && Request.QueryString["file"] == null) // ---- POST 的處理片段  -----------------------------------
        {
            List<FileResponse> FileResponseList = new List<FileResponse>();
            for (int FileIndex = 0; FileIndex < Request.Files.Count; FileIndex++)  //利用 HttpPostedFile 方式來將檔案寫入到 Server
            {
                HttpPostedFile File = Request.Files[FileIndex];
                string FileName = String.Format(@"{0}\{1}", Upload_Directory, Path.GetFileName(File.FileName));
                string ErrorMessage = String.Empty;
                if (System.IO.File.Exists(FileName))  // 檔名重複時的處理，增加序號 _yyyyMMddHHmmss.fff
                {
                    FileName = String.Format(@"{0}\{1}_{2:yyyyMMddHHmmss.fff}{3}", Upload_Directory, Path.GetFileNameWithoutExtension(FileName), DateTime.Now, Path.GetExtension(FileName));
                }
                File.SaveAs(FileName);  // 將檔案寫入 Server 端
				FileResponseList.Add(CreateFileResponse(FileName, File.ContentLength, ErrorMessage));
            }
            SerializeUploaderResponse(FileResponseList);
        }  // EOF --- if (Request.HttpMethod.ToUpper() == "POST" && Request.QueryString["file"] == null)
        // 刪除檔案之片段  ---------------------------------------
		else if (Request.HttpMethod.ToUpper() == "POST" && Request.QueryString["file"] != null)   
        {			   
            bool SuccessfullyDeleted = true;          
            try
            {                File.Delete(FullFileName);            }
            catch
            {                SuccessfullyDeleted = false;            }
            Response.Write(String.Format("{{\"{0}\":{1}}}", ShortFileName, SuccessfullyDeleted.ToString().ToLower()));            
        }
        else // 不是 GET 也不是 POST 也不是做 DELETE
        {
            Response.StatusCode = 405;
            Response.StatusDescription = "Method not allowed";
            Response.End();
            return;
        }
        Response.End();
    }
</script>