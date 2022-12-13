using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using cdbd_daphuongtien.App_Code;

public partial class MediaUpload : System.Web.UI.Page
{
    CsLib csLib = new CsLib();
    DtLib dtLib = new DtLib();
    MediaType mediaType = new MediaType();
    static Media[] media;
    View view = new View();

    static int mediaCount = 0;
    static bool isLinkButtonClicked = false;
    static int basedMediaId = -1;

    string uploadFolder = "";
    string[] allowedFileTypes = { "" };

    protected void Page_Load(object sender, EventArgs e)
    {
        this.Title = "Tải lên media - " + csLib.varSiteName;

        if (!csLib.isLoggedIn())
        {
            csLib.toErrControl(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Đăng nhập quản trị để thực hiện thao tác này.", "Đăng nhập", "Login.aspx");
            return;
        }

        if (!Page.IsPostBack)
        {
            mediaCount = 0;
            isLinkButtonClicked = false;
            basedMediaId = dtLib.getNewIntId("TableMedia", "id") - 1;
            if (basedMediaId < 0)
            {
                csLib.toErrControl(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Không khởi tạo được mã media mới.", "Trang quản trị", "Administrator.aspx");
                return;
            }
        }
        if (Page.IsPostBack && isLinkButtonClicked)
        {
            buildMediaUploadedPlaceHolder();
        }
    }

    protected void btMediaUpload_Click(object sender, EventArgs e)
    {
        bool isValidFileType;
        string fileName = "";
        string filePath = "";
        Array.Resize(ref allowedFileTypes, csLib.varAllowedFileTypes.Length);
        allowedFileTypes = csLib.varAllowedFileTypes;
        uploadFolder = csLib.varUploadFolder;

        if (fuMedia.HasFiles)
        { 
            foreach (HttpPostedFile uploadedFile in fuMedia.PostedFiles)
            {
                isValidFileType = false;

                for (int i = 0; i < allowedFileTypes.Length; i++)
                {
                    if (Path.GetExtension(uploadedFile.FileName).ToLower() == "." + allowedFileTypes[i].ToLower())
                    {
                        isValidFileType = true;
                        break;
                    }
                }

                if (!isValidFileType)
                {
                    phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP(uploadedFile.FileName + ": Loại tập tin không được hỗ trợ.")));
                    continue;
                }
                else
                {
                    mediaCount++;
                    resizeMediaArray(mediaCount);

                    fileName = csLib.createFileNamePrefix(8) + Path.GetExtension(uploadedFile.FileName.ToLower());
                    filePath = Path.Combine(Server.MapPath(uploadFolder), fileName);
                    uploadedFile.SaveAs(filePath);

                    basedMediaId++;
                    media[mediaCount - 1].id = basedMediaId;
                    media[mediaCount - 1].album = -1;
                    media[mediaCount - 1].author = -1;
                    media[mediaCount - 1].datetime = DateTime.Now;
                    media[mediaCount - 1].description = "";
                    media[mediaCount - 1].name = fileName;
                    media[mediaCount - 1].state = 1;
                    media[mediaCount - 1].tag = "";
                    media[mediaCount - 1].url = uploadFolder + "/" + fileName;
                    media[mediaCount - 1].type = mediaType.getIdByExtension(Path.GetExtension(uploadedFile.FileName).ToLower());
                    media[mediaCount - 1].description = "";
                    media[mediaCount - 1].tag = "";
                }
            }

            isLinkButtonClicked = true;
            buildMediaUploadedPlaceHolder();
        }
    }

    protected void buildMediaUploadedPlaceHolder()
    {
        if (mediaCount > 0)
        {
            phUploadedMedia.Controls.Clear();
            for (int i = 0; i < mediaCount; i++)
            {
                phUploadedMedia.Controls.Add(new LiteralControl("<div style=\"display:inline-block; margin:5px 7px 5px 0px;\">"));
                if (media[i].state == 1)
                {
                    LinkButton linkButton = new LinkButton();
                    linkButton.ID = "lb" + i.ToString();
                    linkButton.Text = "[Xóa-" + i.ToString() + "]";
                    linkButton.CommandArgument = i.ToString();
                    linkButton.Click += new EventHandler(removeMedia);

                    Label label = new Label();
                    label.ID = "lbl" + i.ToString();
                    label.Text = "[" + dtLib.getData_IntToStr("name", "TableMediaType", "id", media[i].type) + "][" + media[i].id + "]";

                    this.Controls.Add(label);
                    this.Controls.Add(linkButton);              
                    phUploadedMedia.Controls.Add(linkButton);
                    phUploadedMedia.Controls.Add(label);
                    phUploadedMedia.Controls.Add(new LiteralControl("<br />"));
                    phUploadedMedia.Controls.Add(new LiteralControl(view.viewThumnail(media[i].type, media[i].url, media[i].name, 1)));
                }
                phUploadedMedia.Controls.Add(new LiteralControl("</div>"));
            }
        }
    }

    protected void removeMedia(object sender, EventArgs e)
    {
        LinkButton linkButton = (LinkButton)sender;
        string indexString = linkButton.CommandArgument;
        int indexInt = -1;
        int.TryParse(indexString, out indexInt);
        media[indexInt].state = 0;
        buildMediaUploadedPlaceHolder();
    }

    protected void resizeMediaArray(int newSize)
    {
        Array.Resize(ref media, newSize);
        media[newSize - 1] = new Media();
    }

    protected void btOk_Click(object sender, EventArgs e)
    {
        if (mediaCount > 0)
        {
            for (int i = 0; i < mediaCount; i++)
            {
                if (media[i].state == 1)
                {
                    if (!media[i].mediaInsert(media[i]))
                    {
                        csLib.toErrControl(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Không lưu được thông tin media " + media[i].id.ToString() + "vào hệ thống.", "Administrator.aspx", "Trang quản trị");
                        return;
                    }
                }
                else
                    if (File.Exists(Server.MapPath(media[i].url)))
                    {
                        try
                        {
                            File.Delete(Server.MapPath(media[i].url));
                        }
                        catch (Exception f)
                        {
                            csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, f.Message, media[i].url);
                        }
                    }
            }
            phMessage.Controls.Add(new LiteralControl(view.toSuccessHtmlP("Thao tác đã được thực hiện.")));
            phUploadedMedia.Controls.Clear();
            disableControls();
        }
        else
        {
            phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP("Không có tập tin nào được chọn để tải lên.")));
            return;
        }
    }

    protected void btCancel_Click(object sender, EventArgs e)
    {
        if (mediaCount > 0)
        {
            for (int i = 0; i < mediaCount; i++)
            {
                if (File.Exists(Server.MapPath(media[i].url)))
                {
                    try
                    {
                        File.Delete(Server.MapPath(media[i].url));
                    }
                    catch (Exception f)
                    {
                        csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, f.Message, media[i].url);
                    }
                }
            }
        }
        Response.BufferOutput = true;
        Response.Redirect("Administrator.aspx", false);
    }

    public void disableControls()
    {
        fuMedia.Enabled = false;
        btMediaUpload.Enabled = false;
        btOk.Enabled = false;
        btCancel.Enabled = false;
    }
}