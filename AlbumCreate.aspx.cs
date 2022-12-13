using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using cdbd_daphuongtien.App_Code;

public partial class AlbumCreate : System.Web.UI.Page
{
    CsLib csLib = new CsLib();
    DtLib dtLib = new DtLib();
    MediaType mediaType = new MediaType();
    static Album album = new Album();
    static Media[] mediaCurrent;
    static Media[] mediaUploaded;
    View view = new View();

    static int countMCurrent = 0;
    static int countMUploaded = 0;
    static bool isUploadedLinkButtonClicked = false;
    static bool isCurrentLinkButtonClicked = false;
    static int basedMediaId = -1;
    static bool[] mediaCurrentMsk;

    Tag tag = new Tag();
    static string[] allTagId = { "" };
    static bool[] allCheckedTagId = { false };
    static string[] currentAlbumTag;

    static string randomUIdPrefix = "";
    static string randomCIdPrefix = "";

    string uploadFolder = "";
    string[] allowedFileTypes = { "" };

    static string qsOption = "";
    static string qsAlbumId = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        this.Title = "Tạo album mới - " + csLib.varSiteName;

        if (!csLib.isLoggedIn())
        {
            csLib.toErrControl(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Đăng nhập quản trị để thực hiện thao tác này.", "Đăng nhập", "Login.aspx");
            return;
        }

        if (!Page.IsPostBack)
        {
            varInit();

            qsOption = csLib.decodeString(Request.QueryString["option"]);
            qsAlbumId = csLib.decodeString(Request.QueryString["albumid"]);
            if (!string.IsNullOrEmpty(csLib.isValidString("Thao tác", qsOption, false, true, true)))
                qsOption = "create";
            if (qsOption != "create" && qsOption != "edit")
                qsOption = "create";

            if (qsOption == "edit")
            {
                this.Title = "Chỉnh sửa album - " + csLib.varSiteName;
                lblTitle.Text = "Album | Chỉnh sửa";

                if (!string.IsNullOrEmpty(csLib.isValidString("Mã album", qsAlbumId, false, false, true)))
                {
                    csLib.toErrControl(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Mã album này không hợp lệ.", "Trang quản trị", "Administrator.aspx");
                    return;
                }
                if (!int.TryParse(qsAlbumId, out album.id))
                {
                    csLib.toErrControl(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Mã album này không hợp lệ.", "Trang quản trị", "Administrator.aspx");
                    return;
                }
                album = album.albumGet(album.id);
                if (album.id == 0)
                {
                    csLib.toErrControl(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Không đọc được thông tin của album này.", "Trang quản trị", "Administrator.aspx");
                    return;
                }
                lblAlbumInfo.Text += "[ID: " + album.id.ToString() + "]";
                lblAlbumInfo.Text += "[Datetime: " + album.datetime.ToString() + "]";
                tbAlbumDesc.Text = album.description;
                tbAlbumName.Text = album.name;
                ddlAlbumState.SelectedValue = album.state.ToString();

                buildMediaCurrent();
            }
        }

        if (Page.IsPostBack && isUploadedLinkButtonClicked)
            buildMediaUploadedPh();

        if (Page.IsPostBack && isCurrentLinkButtonClicked)
            buildMediaCurrentPh();

        buildTagPlaceHolder();
    }

    public void varInit()
    {
        countMUploaded = 0;
        isUploadedLinkButtonClicked = false;
        basedMediaId = dtLib.getNewIntId("TableMedia", "id");
        if (basedMediaId < 0)
        {
            csLib.toErrControl(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Không khởi tạo được mã media mới cho album này.", "Trang quản trị", "Administrator.aspx");
            return;
        }
        basedMediaId = basedMediaId + 1;
        Array.Resize(ref mediaUploaded, 0);

        countMCurrent = 0;
        isCurrentLinkButtonClicked = false;
        Array.Resize(ref mediaCurrentMsk, 0);
        Array.Resize(ref mediaCurrent, 0);

        Array.Resize(ref allTagId, 0);
        Array.Resize(ref allCheckedTagId, 0);
        Array.Resize(ref currentAlbumTag, 0);

        Random r = new Random();
        int curInt = r.Next(111, 999);
        int uplInt = r.Next(111, 999);
        while (uplInt == curInt)
            uplInt = r.Next(111, 999);
        string randomUIdPrefix = curInt.ToString();
        string randomCIdPrefix = uplInt.ToString();

        album = new Album();
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
                    phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP(uploadedFile.FileName + ": Loại tập tin không được hỗ trợ.")));
                else
                {
                    countMUploaded++;
                    resizeMediaUploadedArray(countMUploaded);

                    fileName = csLib.createFileNamePrefix(8) + Path.GetExtension(uploadedFile.FileName.ToLower());
                    filePath = Path.Combine(Server.MapPath(uploadFolder), fileName);
                    uploadedFile.SaveAs(filePath);

                    basedMediaId++;
                    mediaUploaded[countMUploaded - 1].id = basedMediaId;
                    mediaUploaded[countMUploaded - 1].album = -1;
                    mediaUploaded[countMUploaded - 1].author = -1;
                    mediaUploaded[countMUploaded - 1].datetime = DateTime.Now;
                    mediaUploaded[countMUploaded - 1].description = "";
                    mediaUploaded[countMUploaded - 1].name = fileName;
                    mediaUploaded[countMUploaded - 1].state = 1;
                    mediaUploaded[countMUploaded - 1].tag = "";
                    mediaUploaded[countMUploaded - 1].url = uploadFolder + "/" + fileName;
                    mediaUploaded[countMUploaded - 1].type = mediaType.getIdByExtension(Path.GetExtension(uploadedFile.FileName).ToLower());
                    mediaUploaded[countMUploaded - 1].description = "";
                    mediaUploaded[countMUploaded - 1].tag = "";
                }
            }

            isUploadedLinkButtonClicked = true;
            buildMediaUploadedPh();
        }
    }

    protected void buildMediaUploadedPh()
    {
        if (countMUploaded > 0)
        {
            phMediaUploaded.Controls.Clear();

            for (int i = 0; i < countMUploaded; i++)
            {
                phMediaUploaded.Controls.Add(new LiteralControl("<div style=\"display:inline-block; margin:5px 7px 5px 0px;\">"));
                if (mediaUploaded[i].state == 1)
                {
                    LinkButton linkButton = new LinkButton();
                    linkButton.ID = randomUIdPrefix + "lbU" + i.ToString();
                    linkButton.Text = "[Xóa-" + i.ToString() + "]";
                    linkButton.CommandArgument = i.ToString();
                    linkButton.Click += new EventHandler(removeMediaUploaded);

                    Label label = new Label();
                    label.ID = randomUIdPrefix + "lblU" + i.ToString();
                    label.Text = "[" + dtLib.getData_IntToStr("name", "TableMediaType", "id", mediaUploaded[i].type) + "][" + mediaUploaded[i].id + "]";

                    this.Controls.Add(label);
                    this.Controls.Add(linkButton);       
                    phMediaUploaded.Controls.Add(linkButton);
                    phMediaUploaded.Controls.Add(label);
                    phMediaUploaded.Controls.Add(new LiteralControl("<br />"));
                    phMediaUploaded.Controls.Add(new LiteralControl(view.viewThumnail(mediaUploaded[i].type, mediaUploaded[i].url, mediaUploaded[i].name, 1)));
                }
                phMediaUploaded.Controls.Add(new LiteralControl("</div>"));
            }
        }
    }

    protected void resizeMediaUploadedArray(int newSize)
    {
        Array.Resize(ref mediaUploaded, newSize);
        mediaUploaded[newSize - 1] = new Media();
    }

    protected void removeMediaUploaded(object sender, EventArgs e)
    {
        LinkButton linkButton = (LinkButton)sender;
        string indexString = linkButton.CommandArgument;
        int indexInt = -1;
        int.TryParse(indexString, out indexInt);
        mediaUploaded[indexInt].state = 0;
        buildMediaUploadedPh();
    }

    protected string createMUploadedStr()
    {
        string outputStr = "";

        if (mediaUploaded.Length > 0)
        {
            for (int i = 0; i < mediaUploaded.Length; i++)
            {
                if (mediaUploaded[i].state == 1)
                    outputStr += mediaUploaded[i].id + ",";
            }
        }

        return outputStr;
    }


    protected void buildMediaCurrent()
    {
        int[] tempArray = csLib.toIntArray(album.media);
        countMCurrent = tempArray.Length;

        if (tempArray.Length > 0)
        {
            Array.Resize(ref mediaCurrent, countMCurrent);
            Array.Resize(ref mediaCurrentMsk, countMCurrent);

            for (int i = 0; i < countMCurrent; i++)
            {
                mediaCurrentMsk[i] = true;
                mediaCurrent[i] = new Media();
                mediaCurrent[i].id = tempArray[i];
                mediaCurrent[i] = mediaCurrent[i].mediaGet(mediaCurrent[i].id);
                if (mediaCurrent[i].id == 0)
                    mediaCurrentMsk[i] = false;
            }
        }
        else
        {
            Array.Resize(ref mediaCurrent, 0);
            Array.Resize(ref mediaCurrentMsk, 0);
        }

        isCurrentLinkButtonClicked = true;
        buildMediaCurrentPh();
    }

    protected void buildMediaCurrentPh()
    {
        if (countMCurrent > 0)
        {
            phMediaCurrent.Controls.Clear();

            for (int i = 0; i < countMCurrent; i++)
            {
                if (mediaCurrentMsk[i])
                {
                    string mediaStateMsg = "";
                    phMediaCurrent.Controls.Add(new LiteralControl("<div style=\"display:inline-block; margin:5px 7px 5px 0px;\">"));
                    if (mediaCurrent[i].state != 1)
                        mediaStateMsg = csLib.varStrDisabled;

                    LinkButton linkButton = new LinkButton();
                    linkButton.ID = randomCIdPrefix + "lbC" + i.ToString();
                    linkButton.Text = "[Xóa-" + i.ToString() + "]";
                    linkButton.CommandArgument = i.ToString();
                    linkButton.Click += new EventHandler(removeMediaCurrent);

                    Label label = new Label();
                    label.ID = randomCIdPrefix + "lblC" + i.ToString();
                    label.Text = mediaStateMsg + "[" + dtLib.getData_IntToStr("name", "TableMediaType", "id", mediaCurrent[i].type) + "][" + mediaCurrent[i].id + "]";

                    this.Controls.Add(label);
                    this.Controls.Add(linkButton);
                    phMediaCurrent.Controls.Add(linkButton);
                    phMediaCurrent.Controls.Add(label);
                    phMediaCurrent.Controls.Add(new LiteralControl("<br />"));
                    phMediaCurrent.Controls.Add(new LiteralControl(view.viewThumnail(mediaCurrent[i].type, mediaCurrent[i].url, mediaCurrent[i].name, 1)));
                    phMediaCurrent.Controls.Add(new LiteralControl("</div>"));
                }
            }
        }
    }

    protected void removeMediaCurrent(object sender, EventArgs e)
    {
        LinkButton linkButton = (LinkButton)sender;
        string indexString = linkButton.CommandArgument;
        int indexInt = -1;
        int.TryParse(indexString, out indexInt);
        mediaCurrentMsk[indexInt] = false;
        buildMediaCurrentPh();
    }

    protected string createMCurrentStr()
    {
        string outputStr = "";

        if (mediaCurrent.Length > 0)
        {
            for (int i = 0; i < mediaCurrent.Length; i++)
            {
                if (mediaCurrentMsk[i] == true)
                    outputStr += mediaCurrent[i].id + ",";
            }
        }

        return outputStr;
    }


    protected void buildTagPlaceHolder()
    {
        phTag.Controls.Clear();

        string[] tempArray = tag.getAllTagId(false);
        if (tempArray.Length == 0)
            return;
        int tagCount = tempArray.Length;
        Array.Resize(ref allTagId, tagCount);
        Array.Resize(ref allCheckedTagId, tagCount);
        allTagId = tempArray;
        for (int k = 0; k < allCheckedTagId.Length; k++)
        {
            allCheckedTagId[k] = false;
        }

        currentAlbumTag = csLib.toStrArray(album.tag);

        for (int i = 0; i < tagCount; i++)
        {
            CheckBox checkBox = new CheckBox();
            checkBox.ID = randomUIdPrefix + allTagId[i];
            checkBox.Text = dtLib.getData_StrToStr("name", "TableTag", "id", allTagId[i]);
            if (currentAlbumTag.Length > 0)
                for (int j = 0; j < currentAlbumTag.Length; j++)
                    if (allTagId[i] == currentAlbumTag[j])
                    {
                        checkBox.Checked = true;
                        allCheckedTagId[i] = true;
                    }
            checkBox.Attributes["id"] = i.ToString();
            checkBox.CheckedChanged += new EventHandler(tagCbChanged);
            this.Controls.Add(checkBox);
            phTag.Controls.Add(checkBox);
        }
    }

    protected void tagCbChanged(object sender, EventArgs e)
    {
        CheckBox checkBox = (CheckBox)sender;
        int index = Convert.ToInt32(checkBox.Attributes["id"].ToString());
        allCheckedTagId[index] = !allCheckedTagId[index];
    }


    protected void btOk_Click(object sender, EventArgs e)
    {
        string strMUploadedId = createMUploadedStr();
        string strMCurrentId = createMCurrentStr();

        album.description = tbAlbumDesc.Text;
        album.media = strMCurrentId + strMUploadedId;
        int[] albumMediaArray = csLib.toIntArray(album.media);
        if (albumMediaArray.Length > 0)
        {
            albumMediaArray = albumMediaArray.Distinct().ToArray();
            album.media = "";
            for (int i = 0; i < albumMediaArray.Length; i++)
                album.media += albumMediaArray[i].ToString() + ",";
        }
        album.name = tbAlbumName.Text;
        album.state = Convert.ToInt32(ddlAlbumState.SelectedValue);

        album.tag = "";
        for (int i = 0; i < allCheckedTagId.Length; i++)
            if (allCheckedTagId[i])
                album.tag += allTagId[i] + ",";

        if (!string.IsNullOrEmpty(album.isValidAlbum(album)))
        {
            phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP(album.isValidAlbum(album))));
            return;
        }
        
        if (countMUploaded > 0)
        {
            for (int i = 0; i < countMUploaded; i++)
            {
                if (mediaUploaded[i].state == 1)
                {
                    if (!mediaUploaded[i].mediaInsert(mediaUploaded[i]))
                    {
                        csLib.toErrControl(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Không lưu được thông tin media " + mediaUploaded[i].id.ToString() + "vào hệ thống.", "Administrator.aspx", "Trang quản trị");
                        return;
                    }
                }
                else
                    if (File.Exists(Server.MapPath(mediaUploaded[i].url)))
                    {
                        try
                        {
                            File.Delete(Server.MapPath(mediaUploaded[i].url));
                        }
                        catch (Exception f)
                        {
                            csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, f.Message, mediaUploaded[i].url);
                        }
                    }
            }
        }

        if (qsOption == "edit")
        {
            if (!album.albumUpdate(album))
            {
                phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP("Không cập nhật được thông tin album vào hệ thống.")));
                return;
            }
        }
        else
        {
            album.id = dtLib.getNewIntId("TableAlbum", "id");
            if (album.id == -1)
            {
                csLib.toErrControl(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Không khởi tạo được mã album mới.", "Trang quản trị", "Administrator.aspx");
                return;
            }
            album.datetime = DateTime.Now;
            if (!album.albumInsert(album))
            {
                phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP("Không lưu được thông tin album vào hệ thống.")));
                return;
            }
        }
        updateMediaAlbum(album.media);
        phMediaUploaded.Controls.Clear();
        phMediaCurrent.Controls.Clear();
        phMessage.Controls.Add(new LiteralControl(view.toSuccessHtmlP("Thao tác đã được thực hiện.")));
        disableControls();  
    }

    protected void updateMediaAlbum(string albumMediaString)
    {
        int[] mediaId = csLib.toIntArray(albumMediaString);
        if (mediaId.Length > 0)
        {
            for (int i = 0; i < mediaId.Length; i++)
            {
                Media tempMedia = new Media();
                tempMedia = tempMedia.mediaGet(mediaId[i]);
                if (tempMedia.id > 0)
                {
                    tempMedia.album = album.id;
                    if (!tempMedia.mediaUpdate(tempMedia))
                    {
                        phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP("Không cập nhật album được cho đối tượng media.")));
                    }
                }
            }
        }
    }

    protected void btCancel_Click(object sender, EventArgs e)
    {
        if (countMUploaded > 0)
        {
            for (int i = 0; i < countMUploaded; i++)
            {
                if (File.Exists(Server.MapPath(mediaUploaded[i].url)))
                {
                    try
                    {
                        File.Delete(Server.MapPath(mediaUploaded[i].url));
                    }
                    catch (Exception f)
                    {
                        csLib.errorTracer(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, f.Message, mediaUploaded[i].url);
                    }
                }
            }
        }
        Response.BufferOutput = true;
        Response.Redirect("Administrator.aspx", false);
    }

    protected void disableControls()
    {
        tbAlbumDesc.Enabled = false;
        tbAlbumName.Enabled = false;
        ddlAlbumState.Enabled = false;
        btOk.Enabled = false;
        btCancel.Enabled = false;
        btMediaUpload.Enabled = false;
        fuMedia.Enabled = false;
    }
}