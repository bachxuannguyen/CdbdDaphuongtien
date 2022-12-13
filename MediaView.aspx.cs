using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using cdbd_daphuongtien.App_Code;

public partial class MediaView : System.Web.UI.Page
{
    CsLib csLib = new CsLib();
    DtLib dtLib = new DtLib();
    static Media media = new Media();
    Album album = new Album();
    MediaFile mediaFile = new MediaFile();
    View view = new View();

    string[] tagArray;
    static string qsMediaId = "";
    static bool isSearch = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            varInit();

            qsMediaId = csLib.decodeString(Request.QueryString["mediaid"]);

            string qsMediaIdMsg = csLib.isValidString("Mã media", qsMediaId, false, false, true);
            if (!string.IsNullOrEmpty(qsMediaIdMsg))
            {
                phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP(qsMediaIdMsg)));
                return;
            }

            if (!int.TryParse(qsMediaId, out media.id))
            {
                phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP("Mã media này không hợp lệ.")));
                return;
            }

            media = media.mediaGet(media.id);
            if (media.id == 0)
            {
                phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP("Không đọc được thông tin của media này.")));
                return;
            }
            if (media.state != 1)
            {
                if (csLib.isLoggedIn())
                    phMessage.Controls.Add(new LiteralControl("<a href=\"MediaEdit.aspx?mediaid=" + csLib.encodeString(media.id.ToString()) + "\">[Chỉnh sửa]</a>"));
                phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP("Media này hiện không khả dụng.")));
                return;
            }

            this.Title = media.name + " - " + csLib.varSiteName;

            showContent();
        }

        if (Page.IsPostBack && isSearch)
            showContent();
    }

    protected void varInit()
    {
        qsMediaId = "";
        isSearch = false;
    }

    protected void showContent()
    {
        lblTitle.Text = media.name;

        phMediaGo.Controls.Clear();
        phMediaInfo.Controls.Clear();
        phMediaPreview.Controls.Clear();
        phMessage.Controls.Clear();

        string viewFileInfo = "<br />" + view.viewFileInfo(media);

        LinkButton viewType = new LinkButton();
        viewType.ID = "lbType" + media.id.ToString();
        viewType.Text = dtLib.getData_IntToStr("name", "TableMediaType", "id", media.type);
        viewType.CommandArgument = media.type.ToString();
        viewType.Click += new EventHandler(searchMediaType);

        string viewDatetime = "<br /><b>Ngày tạo: </b>" + media.datetime.ToShortDateString();
        string viewFbShare = "<br /><div class=\"fb-share-button\"";
        viewFbShare += " data-href=\"" + csLib.varHomePage + "/MediaView.aspx?mediaid=" + csLib.encodeString(media.id.ToString()) + "\"";
        viewFbShare += " data-layout=\"button\"></div>";
        string viewDownload = "<a href=\"FileDownload.ashx?filename=" + csLib.encodeString(media.url) + "\" target=\"_blank\">[Tải về]</a>";
        string viewDesc = "";
        if (!string.IsNullOrEmpty(media.description))
            viewDesc = "<br /><b>Mô tả: </b>" + media.description;
        
        phMediaInfo.Controls.Add(new LiteralControl(viewFileInfo));
        phMediaInfo.Controls.Add(new LiteralControl(viewDatetime));
        phMediaInfo.Controls.Add(new LiteralControl("&nbsp;<b>Loại media: </b>"));
        phMediaInfo.Controls.Add(viewType);      
        if (!string.IsNullOrEmpty(media.tag))
        {
            tagArray = csLib.toStrArray(media.tag);
            if (tagArray.Length > 0)
            {
                phMediaInfo.Controls.Add(new LiteralControl("<br /><b>Tag:</b> "));
                for (int i = 0; i < tagArray.Length; i++)
                {
                    LinkButton linkButton = new LinkButton();
                    linkButton.ID = "lbTag" + i.ToString();
                    linkButton.Text = "[" + tagArray[i] + "]";
                    linkButton.CommandArgument = tagArray[i];
                    linkButton.Click += new EventHandler(searchTag);
                    this.Controls.Add(linkButton);
                    phMediaInfo.Controls.Add(linkButton);
                }
            }
        }
        phMediaInfo.Controls.Add(new LiteralControl(viewDesc));
        phMediaInfo.Controls.Add(new LiteralControl(viewFbShare));
        phMediaInfo.Controls.Add(new LiteralControl("&nbsp;"));
        phMediaInfo.Controls.Add(new LiteralControl(viewDownload));
        if (csLib.isLoggedIn())
        {
            string viewEdit = "<a href=\"MediaEdit.aspx?mediaid=" + csLib.encodeString(media.id.ToString()) + "\">[Chỉnh sửa]</a>";
            phMediaInfo.Controls.Add(new LiteralControl(viewEdit));
            LinkButton linkButton = new LinkButton();
            linkButton.ID = "lbDel";
            linkButton.Text = "[Xóa]";
            linkButton.Click += new EventHandler(deleteMedia);
            this.Controls.Add(linkButton);
            phMediaInfo.Controls.Add(linkButton);
            Label lblDelMsg = new Label();
            lblDelMsg.ID = "lblDelMsg";
            lblDelMsg.Text = "<br />" + view.toFailHtml("Xác nhận xóa đối tượng này?");
            lblDelMsg.Visible = false;
            this.Controls.Add(lblDelMsg);
            phMediaInfo.Controls.Add(lblDelMsg);
            LinkButton lbCfDelete = new LinkButton();
            lbCfDelete.ID = "lbCfDel";
            lbCfDelete.Text = "[Xóa]";
            lbCfDelete.Click += new EventHandler(deleteConfirm);
            lbCfDelete.Visible = false;
            this.Controls.Add(lbCfDelete);
            phMediaInfo.Controls.Add(lbCfDelete);
            LinkButton lbCcDelete = new LinkButton();
            lbCcDelete.ID = "lbCcDel";
            lbCcDelete.Text = "[Hủy bỏ]";
            lbCcDelete.Click += new EventHandler(deleteCancel);
            lbCcDelete.Visible = false;
            this.Controls.Add(lbCcDelete);
            phMediaInfo.Controls.Add(lbCcDelete);
            Label lblDelResult = new Label();
            lblDelResult.ID = "lblDelResult";
            lblDelResult.Text = "";
            lblDelResult.Visible = false;
            this.Controls.Add(lblDelResult);
            phMediaInfo.Controls.Add(lblDelResult);
        }

        if (media.album > 0)
        {
            album = album.albumGet(media.album);
            if (album.id == 0)
                goto lblOne;

            string viewAlbum = "<a href=\"AlbumView.aspx?albumid=" + csLib.encodeString(album.id.ToString()) + "\"><b>" + album.name + "</b></a>";
          
            int[] albumMedia = csLib.toIntArray(album.media);
            int[] albumMediaAvailable = new int[0];
            if (albumMedia.Length == 0)
                goto lblOne;
            for (int j = 0; j < albumMedia.Length; j++)
            {
                if (dtLib.getData_IntToInt("state", "TableMedia", "id", albumMedia[j]) == 1)
                {
                    Array.Resize(ref albumMediaAvailable, albumMediaAvailable.Length + 1);
                    albumMediaAvailable[albumMediaAvailable.Length - 1] = albumMedia[j];
                }
            }
            if (albumMediaAvailable.Length > 0)
            {
                int countMediaTotal = albumMediaAvailable.Length;
                int indexMediaCurrent = 0;
                for (int i = 0; i < albumMediaAvailable.Length; i++)
                {
                    if (albumMediaAvailable[i] == media.id)
                    {
                        indexMediaCurrent = i;
                        break;
                    }
                }
                string viewAlbumCount = "<span style=\"background-color:#a42323; color:#ffffff;\">&nbsp;#" + (indexMediaCurrent + 1).ToString() + "/" + countMediaTotal.ToString() + "&nbsp;</span>&nbsp;";
                int goMNext = -1;
                if (albumMediaAvailable[indexMediaCurrent] == albumMediaAvailable[albumMediaAvailable.Length - 1])
                    goMNext = albumMediaAvailable[0];
                else
                    goMNext = albumMediaAvailable[indexMediaCurrent + 1];
                int goMPrevious = -1;
                if (albumMediaAvailable[indexMediaCurrent] == albumMediaAvailable[0])
                    goMPrevious = albumMediaAvailable[albumMediaAvailable.Length - 1];
                else
                    goMPrevious = albumMediaAvailable[indexMediaCurrent - 1];

                LinkButton viewNext = new LinkButton();
                viewNext.ID = "lbNext" + media.id.ToString();
                viewNext.Text = "[Tiếp]";
                viewNext.CommandArgument = goMNext.ToString();
                viewNext.Click += new EventHandler(goMedia);
                LinkButton viewPrevious = new LinkButton();
                viewPrevious.ID = "lbPrevious" + media.id.ToString();
                viewPrevious.Text = "[Trước]";
                viewPrevious.CommandArgument = goMPrevious.ToString();
                viewPrevious.Click += new EventHandler(goMedia);
                phMediaGo.Controls.Add(new LiteralControl(viewAlbumCount));
                this.Controls.Add(viewPrevious);
                this.Controls.Add(viewNext);
                phMediaGo.Controls.Add(viewPrevious);
                phMediaGo.Controls.Add(viewNext);
                phMediaGo.Controls.Add(new LiteralControl("<br />"));
                phMediaGo.Controls.Add(new LiteralControl("<b>Album:</b> "));
                phMediaGo.Controls.Add(new LiteralControl(viewAlbum));
                phMediaGo.Controls.Add(new LiteralControl("<hr>"));
            }
        }
        lblOne:
        string viewPreview = "<a href=\"" + media.url + "\" target=\"_blank\">" + view.viewThumnail(media.type, media.url, media.name, 3) + "</a>";
        phMediaPreview.Controls.Add(new LiteralControl(viewPreview));

        isSearch = true;
    }

    protected void searchMediaType(object sender, EventArgs e)
    {
        LinkButton linkButton = (LinkButton)sender;
        string lbArg = linkButton.CommandArgument;
        Session["searchMediaCrossPageFlag"] = "true";
        Session["searchMediaCrossPageData"] = "2" + "," + lbArg + ",";
        Response.BufferOutput = true;
        Response.Redirect("SearchMedia.aspx");
    }

    protected void goMedia(object sender, EventArgs e)
    {
        LinkButton linkButton = (LinkButton)sender;
        string lbArg = linkButton.CommandArgument;
        Response.BufferOutput = true;
        Response.Redirect("MediaView.aspx?mediaid=" + csLib.encodeString(lbArg));
    }

    protected void searchTag(object sender, EventArgs e)
    {
        LinkButton linkButton = (LinkButton)sender;
        string lbArg = linkButton.CommandArgument;
        Session["searchTagCrossPageFlag"] = "true";
        Session["searchTagCrossPageData"] = "1" + "," + lbArg + ",";
        Response.BufferOutput = true;
        Response.Redirect("SearchTag.aspx");
    }

    protected void deleteMedia(object sender, EventArgs e)
    {
        Label lbl = (Label)phMediaInfo.FindControl("lblDelMsg");
        LinkButton lbConfirm = (LinkButton)phMediaInfo.FindControl("lbCfDel");
        LinkButton lbCancel = (LinkButton)phMediaInfo.FindControl("lbCcDel");
        if (lbl != null && lbConfirm != null && lbCancel != null)
        {
            lbl.Visible = true;
            lbConfirm.Visible = true;
            lbCancel.Visible = true;
        }
    }

    protected void deleteConfirm(object sender, EventArgs e)
    {
        Media delMedia = new Media();
        delMedia.id = media.id;
        delMedia = delMedia.mediaGet(delMedia.id);
        Label lblResult = (Label)phMediaInfo.FindControl("lblDelResult");
        Label lblMsg = (Label)phMediaInfo.FindControl("lblDelMsg");
        LinkButton lbConfirm = (LinkButton)phMediaInfo.FindControl("lbCfDel");
        LinkButton lbCancel = (LinkButton)phMediaInfo.FindControl("lbCcDel");
        LinkButton lbDelete = (LinkButton)phMediaInfo.FindControl("lbDel");
        if (lblResult != null && lblMsg != null && lbConfirm != null && lbCancel != null)
        {
            int x = delMedia.mediaDelete(delMedia, true);
            if (x == 0)
                phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP("Không xóa được media này.")));
            else
            {
                if (x == 2)
                    phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP("Không xóa được tập tin khỏi hệ thống.")));
                else
                    phMessage.Controls.Add(new LiteralControl(view.toSuccessHtmlP("Media này đã được xóa thành công.")));

                phMessage.Controls.Add(new LiteralControl("<a href=\"SearchMedia.aspx\">[Danh mục media]</a>&nbsp;<a href=\"Default.aspx\">[Trang chủ]</a>"));
                phMediaInfo.Controls.Clear();
                phMediaGo.Controls.Clear();
                phMediaPreview.Controls.Clear();
            }
            
            lblMsg.Visible = false;
            lbConfirm.Visible = false;
            lbCancel.Visible = false;
            lbDelete.Visible = false;
        }
    }

    protected void deleteCancel(object sender, EventArgs e)
    {
        Label lbl = (Label)phMediaInfo.FindControl("lblDelMsg");
        LinkButton lbConfirm = (LinkButton)phMediaInfo.FindControl("lbCfDel");
        LinkButton lbCancel = (LinkButton)phMediaInfo.FindControl("lbCcDel");
        if (lbl != null && lbConfirm != null && lbCancel != null)
        {
            lbl.Visible = false;
            lbConfirm.Visible = false;
            lbCancel.Visible = false;
        }
    }
}