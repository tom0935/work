using System;
using System.Collections;
using System.IO;
using Renci.SshNet;

namespace IntranetSystem.Models
{
    public class SFTPOperation
    {

        private SftpClient sftp;
        /// <summary>  
        /// SFTP連接狀態  
        /// </summary>  
        public bool Connected { get { return sftp.IsConnected; } }

        /// <summary>  
        /// 建構子  
        /// </summary>  
        /// <param name="ip">IP</param>  
        /// <param name="port">端口</param>  
        /// <param name="user">用戶名</param>  
        /// <param name="pwd">密碼</param>  
        public SFTPOperation(string ip, string port, string user, string pwd)
        {
            sftp = new SftpClient(ip, Int32.Parse(port), user, pwd);
        }

        /// <summary>  
        /// 連接SFTP  
        /// </summary>  
        /// <returns>true成功</returns>  
        public bool Connect()
        {
            try
            {
                if (!Connected)
                {
                    sftp.Connect();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("連接SFTP失敗：{0}", ex.Message));
            }
        }

        /// <summary>  
        /// 中斷SFTP  
        /// </summary>   
        public void Disconnect()
        {
            try
            {
                if (sftp != null && Connected)
                {
                    sftp.Disconnect();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("中斷SFTP失敗：{0}", ex.Message));
            }
        }

        /// <summary>  
        /// SFTP檔案上傳
        /// </summary>  
        /// <param name="localPath">本地路徑</param>  
        /// <param name="remotePath">遠端路徑</param>  
        public void Put(string localPath, string remotePath)
        {
            try
            {
                using (var file = File.OpenRead(localPath))
                {
                    Connect();
                    sftp.UploadFile(file, remotePath);
                    Disconnect();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("SFTP檔案上傳失敗：{0}", ex.Message));
            }
        }

        /// <summary>  
        /// SFTP獲取資料
        /// </summary>  
        /// <param name="remotePath">遠端路徑</param>  
        /// <param name="localPath">本地路徑</param>  
        public void Get(string remotePath, string localPath)
        {
            try
            {
                Connect();
                var byt = sftp.ReadAllBytes(remotePath);
                Disconnect();
                File.WriteAllBytes(localPath, byt);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("SFTP檔案獲取失敗：{0}", ex.Message));
            }

        }

        /// <summary>  
        /// 删除SFTP文件   
        /// </summary>  
        /// <param name="remoteFile">遠端路徑</param>  
        public void Delete(string remoteFile)
        {
            try
            {
                Connect();
                sftp.Delete(remoteFile);
                Disconnect();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("SFTP檔案刪除失敗：{0}", ex.Message));
            }
        }

        /// <summary>  
        /// 獲取SFTP文件列表  
        /// </summary>  
        /// <param name="remotePath">遠端目錄</param>  
        /// <param name="fileSuffix">檔案後綴</param>  
        /// <returns></returns>  
        public ArrayList GetFileList(string remotePath, string fileSuffix)
        {
            try
            {
                Connect();
                var files = sftp.ListDirectory(remotePath);
                Disconnect();
                var objList = new ArrayList();
                foreach (var file in files)
                {
                    string name = file.Name;
                    if (name.Length > (fileSuffix.Length + 1) && fileSuffix == name.Substring(name.Length - fileSuffix.Length))
                    {
                        objList.Add(name);
                    }
                }
                return objList;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("SFTP檔案列表獲取失敗：{0}", ex.Message));
            }
        }

        /// <summary>  
        /// 移動SFTP檔案  
        /// </summary>  
        /// <param name="oldRemotePath">舊遠端路徑</param>  
        /// <param name="newRemotePath">新遠端路徑</param>  
        public void Move(string oldRemotePath, string newRemotePath)
        {
            try
            {
                Connect();
                sftp.RenameFile(oldRemotePath, newRemotePath);
                Disconnect();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("SFTP檔案移動失敗：{0}", ex.Message));
            }
        }

    }
}