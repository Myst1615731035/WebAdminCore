using System.Diagnostics;

namespace WebUtils
{
    /// <summary>
    /// svn操作辅助类
    /// </summary>
    public class SVNHelper
    {
        private static readonly string username = "g-s-2282";
        private static readonly string password = "CgtAMZIkxgyc6hIk";
        private static readonly string command = $"--username {username} --password {password} --no-auth-cache--trust-server-cert-failures=unknown-ca,cn-mismatch,not-yet-valid,expired,other";

        /// <summary>
        /// 批量添加文件并提交到svn
        /// </summary>
        /// <param name="files"></param>
        /// <param name="isStatic">是否只处理wwwroot内的文件</param>
        /// <returns></returns>
        public static async Task PushToSVN(List<string> files, bool isStatic = false)
        {
            FileInfo fi;
            var fis = new List<FileInfo>();
            files.ForEach(f => { fi = new FileInfo(f); if (fi.Exists) fis.Add(fi); });
            if (fis.Count == 0)
            {
                LogHelper.LogException(new Exception($"批量上传文件时，已查找到的文件数量为0"));
                return;
            }
            var fileStr = "";
            fis.ForEach(f => { fileStr = $"{fileStr} {f.FullName}"; });
            try
            {
                var update = Update(true);
                if (update != null && update.HasExited) if (AddToSvn(fileStr).HasExited) CommitToSvn(true);
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex);
            }
        }

        #region SVN操作
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="isStatic">是否只更新静态资源</param>
        /// <returns></returns>
        private static Process Update(bool isStatic = false)
        {
            if (ClearUp(isStatic).HasExited)
            {
                Process update = new Process();
                update.StartInfo.FileName = "svn";
                update.StartInfo.Arguments = $" update {(isStatic ? AppConfig.WebRootPath : AppConfig.ContentRootPath)} {command}";
                update.Start();
                update.WaitForExit();
                return update;
            }
            return null;
        }
        /// <summary>
        /// svn清理
        /// </summary>
        /// <param name="isStatic"></param>
        /// <returns></returns>
        private static Process ClearUp(bool isStatic = false)
        {
            Process clear = new Process();
            clear.StartInfo.FileName = "svn";
            clear.StartInfo.Arguments = $" cleanup {(isStatic ? AppConfig.WebRootPath : AppConfig.ContentRootPath)} {command}";
            clear.Start();
            clear.WaitForExit();
            return clear;
        }

        /// <summary>
        /// 添加文件
        /// </summary>
        /// <param name="filestr"></param>
        /// <returns></returns>
        private static Process AddToSvn(string filestr)
        {
            Process add = new Process();
            add.StartInfo.FileName = "svn";
            add.StartInfo.Arguments = $" add {filestr} --force {command}";
            add.Start();
            add.WaitForExit();
            return add;
        }

        /// <summary>
        /// 提交到svn
        /// </summary>
        /// <param name="isStatic"></param>
        /// <returns></returns>
        private static Process CommitToSvn(bool isStatic = false)
        {
            Process commit = new Process();
            commit.StartInfo.FileName = "svn";
            commit.StartInfo.Arguments = $" commit -m WebSiteService自动提交文件 {(isStatic ? AppConfig.WebRootPath : AppConfig.ContentRootPath)} {command}";
            commit.Start();
            commit.WaitForExit();
            return commit;
        }
        #endregion
    }
}
