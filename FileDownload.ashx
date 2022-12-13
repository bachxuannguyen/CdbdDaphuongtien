<%@ WebHandler Language="C#" Class="FileDownload" %>

using System;
using System.Web;
using cdbd_daphuongtien.App_Code;

public class FileDownload : IHttpHandler 
{
    CsLib csLib = new CsLib();
    
    public void ProcessRequest (HttpContext context)
    {
        HttpRequest request = System.Web.HttpContext.Current.Request;
        string fileName = csLib.decodeString(request.QueryString["filename"]);
        
        System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
        response.ClearContent();
        response.Clear();
        response.ContentType = "text/plain";
        response.AddHeader("Content-Disposition",
                           "attachment; filename=" + fileName + ";");
        response.TransmitFile(HttpContext.Current.Server.MapPath(fileName));
        response.Flush();
        response.End();
    }
 
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}