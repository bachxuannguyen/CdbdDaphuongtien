using cdbd_daphuongtien.App_Code;
using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

public class MediaFile
{
    CsLib csLib = new CsLib();

    public MediaFile()
    { }

    public long getFileSize(string fileUrl)
    {
        long fileSize;
        try
        {
            fileSize = new FileInfo(HttpContext.Current.Server.MapPath(fileUrl)).Length;
        }
        catch (Exception e)
        {
            csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, fileUrl);
            return -1;
        }
        return fileSize;
    }

    public int[] getImageDimensions(string fileUrl)
    {
        int[] output = new int[2];

        try
        {
            Bitmap bitmap = new Bitmap(HttpContext.Current.Server.MapPath(fileUrl));
            output[0] = bitmap.Width;
            output[1] = bitmap.Height;
            bitmap.Dispose();
        }
        catch (Exception e)
        {
            csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, fileUrl);
            return new int[2];
        }

        return output;
    }

    public string getVideoInfo(string ffmpegfile, string sourceFile, bool getDurationVsDimensions)
    {
        try
        {
            using (System.Diagnostics.Process ffmpeg = new System.Diagnostics.Process())
            {
                String duration;  // soon will hold our video's duration in the form "HH:MM:SS.UU"
                String result;  // temp variable holding a string representation of our video's duration
                StreamReader errorReader;  // StringWriter to hold output from ffmpeg

                // we want to execute the process without opening a shell
                ffmpeg.StartInfo.UseShellExecute = false;
                //ffmpeg.StartInfo.ErrorDialog = false;
                ffmpeg.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                // redirect StandardError so we can parse it
                // for some reason the output comes through over StandardError
                ffmpeg.StartInfo.RedirectStandardError = true;

                // set the file name of our process, including the full path
                // (as well as quotes, as if you were calling it from the command-line)
                ffmpeg.StartInfo.FileName = ffmpegfile;

                // set the command-line arguments of our process, including full paths of any files
                // (as well as quotes, as if you were passing these arguments on the command-line)
                ffmpeg.StartInfo.Arguments = "-i " + sourceFile;

                // start the process
                ffmpeg.Start();

                // now that the process is started, we can redirect output to the StreamReader we defined
                errorReader = ffmpeg.StandardError;

                // wait until ffmpeg comes back
                ffmpeg.WaitForExit();

                // read the output from ffmpeg, which for some reason is found in Process.StandardError
                result = errorReader.ReadToEnd();

                // a little convoluded, this string manipulation...
                // working from the inside out, it:
                // takes a substring of result, starting from the end of the "Duration: " label contained within,
                // (execute "ffmpeg.exe -i somevideofile" on the command-line to verify for yourself that it is there)
                // and going the full length of the timestamp

                Regex reget = new Regex("(\\d{3,4})x(\\d{3,4})");
                Match match = reget.Match(result);
                int dimensionWidth = 0;
                int dimensionHeight = 0;
                if (match.Success)
                {
                    int.TryParse(match.Groups[1].Value, out dimensionWidth);
                    int.TryParse(match.Groups[2].Value, out dimensionHeight);
                }

                duration = result.Substring(result.IndexOf("Duration: ") + ("Duration: ").Length, ("00:00:00").Length);

                ffmpeg.Close();
                ffmpeg.Dispose();

                if (getDurationVsDimensions)
                    return duration;
                else
                    return dimensionWidth.ToString() + "x" + dimensionHeight.ToString();
                //return result;
            }
        }
        catch (Exception e)
        {
            csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, sourceFile);
            return "";
        }
    }

    public bool deleteFile(string fileUrl)
    {
        if (File.Exists(HttpContext.Current.Server.MapPath(fileUrl)))
        {
            try
            {
                File.Delete(HttpContext.Current.Server.MapPath(fileUrl));
                return true;
            }
            catch (Exception e)
            {
                csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message, fileUrl);
                return false;
            }
        }
        else
        {
            csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Tập tin này không tồn tại trên hệ thống.", fileUrl);
            return false;
        }
    }
}