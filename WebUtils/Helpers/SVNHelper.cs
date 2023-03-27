using System.Diagnostics;

namespace WebUtils
{
    public class SVNHelper
    {
        /// <summary>
        /// 请使用绝对路径
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async Task PushFileToSVN(string filePath)
        {
            var fi = new FileInfo(filePath);
            if (!fi.Exists) 
            {
                LogHelper.LogException(new Exception($"上传文件到SVN，文件不存在，\r\n文件路径：{filePath}"));
                return;
            }
            try
            {
                // 执行添加命令
                Process process = new Process();
                process.StartInfo.FileName = "svn";
                process.StartInfo.Arguments = $" add {fi.FullName} --force";
                process.Start();
                process.WaitForExit();

                // 添加完成后，执行上传命令
                if (process.HasExited)
                {
                    Process process1 = new Process();
                    process1.StartInfo.FileName = "svn";
                    process1.StartInfo.Arguments = $" ci -m '' {fi.FullName}";
                    process1.Start();
                    process1.WaitForExit();
                }
            }catch(Exception ex)
            {
                LogHelper.LogException(ex);
            }
        }
    }
}
