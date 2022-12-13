using cdbd_daphuongtien.App_Code;
using System;
using System.Web;
public class View
{
    CsLib csLib = new CsLib();
    Media media = new Media();
    MediaFile mediaFile = new MediaFile();

    public View()
    { }

    public int[] prepareAlbum(int[] inputAlbumId, bool showDisabled)
    {
        int[] outputAlbumId = new int[0];
        Album album = new Album();

        if (inputAlbumId.Length > 0)
        {
            for (int i = 0; i < inputAlbumId.Length; i++)
            {
                album = album.albumGet(inputAlbumId[i]);
                if (album.id > 0)
                {
                    if (showDisabled || (!showDisabled && album.state == 1))
                    {
                        Array.Resize(ref outputAlbumId, outputAlbumId.Length + 1);
                        outputAlbumId[outputAlbumId.Length - 1] = inputAlbumId[i];
                    }
                }
            }
        }

        return outputAlbumId;
    }

    public int[] prepareMedia(int[] inputMediaId, bool showDisabled)
    {
        int[] outputMediaId = new int[0];
        Media media = new Media();

        if (inputMediaId.Length > 0)
        {
            for (int i = 0; i < inputMediaId.Length; i++)
            {
                media = media.mediaGet(inputMediaId[i]);
                if (media.id > 0)
                {
                    if (showDisabled || (!showDisabled && media.state == 1))
                    {
                        Array.Resize(ref outputMediaId, outputMediaId.Length + 1);
                        outputMediaId[outputMediaId.Length - 1] = inputMediaId[i];
                    }
                }
            }
        }

        return outputMediaId;
    }

    public string[] prepareTag(string[] inputTagId, bool showDisabled)
    {
        string[] outputTagId = new string[0];
        Tag tag = new Tag();

        if (inputTagId.Length > 0)
        {
            for (int i = 0; i < inputTagId.Length; i++)
            {
                tag = tag.tagGet(inputTagId[i]);
                if (!string.IsNullOrEmpty(tag.id))
                {
                    if (showDisabled || (!showDisabled && tag.state == 1))
                    {
                        Array.Resize(ref outputTagId, outputTagId.Length + 1);
                        outputTagId[outputTagId.Length - 1] = inputTagId[i];
                    }
                }
            }
        }

        return outputTagId;
    }

    public string viewFileInfo(Media media)
    {
        string outputString = "";

        outputString += "<div class=\"iconMInfo\">" + (mediaFile.getFileSize(media.url) / (1024)).ToString() + "KB" + "</div>&nbsp;";
        if (media.type == 1 || media.type == 2)
        {
            int[] fileInfo = mediaFile.getImageDimensions(media.url);
            outputString += "<div class=\"iconMInfo2\">" + fileInfo[0].ToString() + "x" + fileInfo[1].ToString() + "px" + "</div>&nbsp;";
        }
        else if (media.type == 3)
        {
            outputString += "<div class=\"iconMInfo2\">" + mediaFile.getVideoInfo(HttpContext.Current.Server.MapPath("./files/ffmpeg"), HttpContext.Current.Server.MapPath(media.url), false) + "px" + "</div>&nbsp;";
            outputString += "<div class=\"iconMInfo\">" + mediaFile.getVideoInfo(HttpContext.Current.Server.MapPath("./files/ffmpeg"), HttpContext.Current.Server.MapPath(media.url), true) + "s" + "</div>&nbsp;";
        }

        return outputString;
    }

    public string viewThumnail(int mediaType, string mediaUrl, string mediaAlt, int viewMode)
    {
        string outputHtml = "";

        int mediaSizePer = 0;
        int mediaSizePx = 0;
        if (viewMode == 1)
        {
            mediaSizePer = csLib.varSmallViewPer;
            mediaSizePx = csLib.varSmallViewPx;
        }
        else if (viewMode == 2)
        {
            mediaSizePer = csLib.varMediumViewPer;
            mediaSizePx = csLib.varMediumViewPx;
        }
        else
        {
            mediaSizePer = csLib.varLargeViewPer;
            mediaSizePx = csLib.varLargeViewPx;
        }

        switch (mediaType)
        {
            case 1:
            case 2:
                {
                    if (viewMode != 1)
                        outputHtml = "<img src=\"" + mediaUrl + "\" alt=\"" + mediaAlt + "\" style=\"max-width:" + mediaSizePer + "%;\">";
                    else
                        outputHtml = "<img src=\"" + mediaUrl + "\" alt=\"" + mediaAlt + "\" style=\"height:" + mediaSizePx + "px;\">";
                }
                break;
            case 3:
                {
                    if (viewMode != 1)
                        outputHtml = "<video style=\"width:" + mediaSizePer + "%;\" controls><source src=\"" + mediaUrl + "\" type=\"video/mp4\"></video>";
                    else
                        outputHtml = "<video style=\"height:" + mediaSizePx + "px;\" controls><source src=\"" + mediaUrl + "\" type=\"video/mp4\"></video>";
                }
                break;
            case 4:
                {
                    outputHtml = "<audio controls><source src=\"" + mediaUrl + "\" type=\"audio/mpeg\"></audio>";
                }
                break;
            case 5:
            default:
                {
                    outputHtml = "Media URL: " + mediaUrl;
                }
                break;
        }

        return outputHtml;
    }

    public string toFailHtmlP(string inputString)
    {
        string outputString = "";

        outputString = "<p><span style=\"color:#" + csLib.varColorRed + ";\">" + inputString + "</span></p>";

        return outputString;
    }

    public string toFailHtml(string inputString)
    {
        string outputString = "";

        outputString = "<span style=\"color:#" + csLib.varColorRed + ";\">" + inputString + "</span>";

        return outputString;
    }

    public string toSuccessHtmlP(string inputString)
    {
        string outputString = "";

        outputString = "<p><span style=\"color:#" + csLib.varColorGreen + ";\">" + inputString + "</span></p>";

        return outputString;
    }

    public string toSuccessHtml(string inputString)
    {
        string outputString = "";

        outputString = "<span style=\"color:#" + csLib.varColorGreen + ";\">" + inputString + "</span>";

        return outputString;
    }

    public string viewErrMsg()
    {
        try
        {
            string errMessage = HttpContext.Current.Session["err_sender"].ToString() + ": " + HttpContext.Current.Session["err_message"].ToString();
            string linkDisplay = "<a href=\"" + HttpContext.Current.Session["err_linkUrl"].ToString() + "\">" + HttpContext.Current.Session["err_linkDisplay"].ToString() + "</a>";
            return "<p>" + errMessage + "<br />" + linkDisplay + "</p>";
        }
        catch
        {
            return "";
        }
    }

    public string viewErrTracer()
    {
        string outputStr = "";

        try
        {
            string[] time = csLib.toStrArray(HttpContext.Current.Session["procedure_Time"].ToString());
            string[] name = csLib.toStrArray(HttpContext.Current.Session["procedure_Name"].ToString());
            string[] message = csLib.toStrArray(HttpContext.Current.Session["procedure_Message"].ToString());
            string[] parameter = csLib.toStrArray(HttpContext.Current.Session["procedure_Parameter"].ToString());

            if (time.Length > 0)
            {
                for (int i = 0; i < time.Length; i++)
                    outputStr += "[" + (i + 1).ToString() + "] [" + time[i] + "] [" + name[i] + "] [" + message[i] + "] [" + parameter[i] + "]<br />";
                return outputStr;
            }
            else
                return "";
        }
        catch
        {
            return "";
        }
    }
}