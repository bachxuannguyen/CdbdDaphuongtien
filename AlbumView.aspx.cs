using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using cdbd_daphuongtien.App_Code;

public partial class AlbumView : System.Web.UI.Page
{
    CsLib csLib = new CsLib();
    DtLib dtLib = new DtLib();
    Media media = new Media();
    static Album album = new Album();
    View view = new View();

    string[] tagArray;

    static string qsAlbumId = "";

    static bool isSearch = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            varInit();

            qsAlbumId = csLib.decodeString(Request.QueryString["albumid"]);
 
            if (!string.IsNullOrEmpty(csLib.isValidString("Mã album", qsAlbumId, false, false, true)))
            {
                phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP("Mã album này không hợp lệ.")));
                return;
            }

            if (!int.TryParse(qsAlbumId, out album.id))
            {
                phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP("Mã album này không hợp lệ.")));
                return;
            }

            album = album.albumGet(album.id);
            if (album.id == 0)
            {
                phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP("Không đọc được thông tin của album này.")));
                return;
            }
            if (album.state != 1)
            {
                if (csLib.isLoggedIn())
                    phMessage.Controls.Add(new LiteralControl("<a href=\"AlbumCreate.aspx?option=" + csLib.encodeString("edit") + "&albumid=" + csLib.encodeString(album.id.ToString()) + "\">[Chỉnh sửa]</a>"));
                phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP("Album này hiện không khả dụng.")));
                return;
            }

            this.Title = album.name + " - " + csLib.varSiteName;

            showContent();
        }

        if (Page.IsPostBack && isSearch)
            showContent();
    }

    protected void varInit()
    {
        qsAlbumId = "";
        isSearch = false;
    }

    protected void showContent()
    {
        phAlbumInfo.Controls.Clear();
        phAlbumPreview.Controls.Clear();
        phMessage.Controls.Clear();

        string viewName = album.name;
        string viewDatetime = "<br /><b>Ngày tạo:</b> " + album.datetime.ToShortDateString();
        string viewDesc = "";
        if (!string.IsNullOrEmpty(album.description))
            viewDesc = "<br /><b>Mô tả: </b>" + album.description;
        string viewFbShare = "<br /><div class=\"fb-share-button\"";
        viewFbShare += " data-href=\"" + csLib.varHomePage + "/AlbumView.aspx?mediaid=" + csLib.encodeString(album.id.ToString()) + "\"";
        viewFbShare += " data-layout=\"button\"></div>";

        string viewMediaCount = "";
        int[] mediaCount = album.getMediaCount(album.id);
        if (mediaCount[0] == -1)
        {
            if (csLib.isLoggedIn())
                phMessage.Controls.Add(new LiteralControl("<a href=\"AlbumCreate.aspx?option=" + csLib.encodeString(album.id.ToString()) + "&albumid=" + csLib.encodeString(media.id.ToString()) + "\">[Chỉnh sửa]</a>"));
            phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP("Không thống kê được số lượng media của album này.")));
            return;
        }
        else
        {
            viewMediaCount += "<br />";
            if (mediaCount[0] > 0)
                viewMediaCount += "<div class=\"iconMInfo\">" + mediaCount[0].ToString() + " ảnh</div>&nbsp;";
            if (mediaCount[1] > 0)
                viewMediaCount += "<div class=\"iconMInfo2\">" + mediaCount[1].ToString() + " infographic</div>&nbsp;";
            if (mediaCount[2] > 0)
                viewMediaCount += "<div class=\"iconMInfo\">" + mediaCount[2].ToString() + " video</div>&nbsp;";
            if (mediaCount[3] > 0)
                viewMediaCount += "<div class=\"iconMInfo2\">" + mediaCount[3].ToString() + " âm thanh</div>&nbsp;";
            if (mediaCount[4] > 0)
                viewMediaCount += "<div class=\"iconMInfo\">" + mediaCount[4].ToString() + " hoạt họa</div>&nbsp;";
        }

        lblTitle.Text = viewName;
        phAlbumInfo.Controls.Add(new LiteralControl(viewMediaCount));
        phAlbumInfo.Controls.Add(new LiteralControl(viewDatetime));
        if (!string.IsNullOrEmpty(album.tag))
        {
            tagArray = csLib.toStrArray(album.tag);
            if (tagArray.Length > 0)
            {
                phAlbumInfo.Controls.Add(new LiteralControl("&nbsp;<b>Tag:</b> "));
                for (int i = 0; i < tagArray.Length; i++)
                {
                    LinkButton linkButton = new LinkButton();
                    linkButton.ID = "lbTag" + i.ToString();
                    linkButton.Text = "[" + tagArray[i] + "]";
                    linkButton.CommandArgument = tagArray[i];
                    linkButton.Click += new EventHandler(searchAlbumTag);
                    this.Controls.Add(linkButton);
                    phAlbumInfo.Controls.Add(linkButton);
                }
            }
        }
        phAlbumInfo.Controls.Add(new LiteralControl(viewDesc));
        phAlbumInfo.Controls.Add(new LiteralControl(viewFbShare));
        if (csLib.isLoggedIn())
        {
            string viewEdit = "&nbsp;<a href=\"AlbumCreate.aspx?option=" + csLib.encodeString("edit") + "&albumid=" + csLib.encodeString(album.id.ToString()) + "\">[Chỉnh sửa]</a>";
            phAlbumInfo.Controls.Add(new LiteralControl(viewEdit));
            LinkButton linkButton = new LinkButton();
            linkButton.ID = "lbDel";
            linkButton.Text = "[Xóa]";
            linkButton.Click += new EventHandler(deleteAlbum);
            this.Controls.Add(linkButton);
            phAlbumInfo.Controls.Add(linkButton);
            Label lblDelMsg = new Label();
            lblDelMsg.ID = "lblDelMsg";
            lblDelMsg.Text = "<br />" + view.toFailHtml("Xác nhận xóa đối tượng này?");
            lblDelMsg.Visible = false;
            this.Controls.Add(lblDelMsg);
            phAlbumInfo.Controls.Add(lblDelMsg);
            LinkButton lbCfDelete = new LinkButton();
            lbCfDelete.ID = "lbCfDel";
            lbCfDelete.Text = "[Chỉ xóa album]";
            lbCfDelete.Click += new EventHandler(deleteConfirm);
            lbCfDelete.Visible = false;
            this.Controls.Add(lbCfDelete);
            phAlbumInfo.Controls.Add(lbCfDelete);
            LinkButton lbCfDelete2 = new LinkButton();
            lbCfDelete2.ID = "lbCfDel2";
            lbCfDelete2.Text = "[Xóa cả media]";
            lbCfDelete2.Click += new EventHandler(deleteConfirm2);
            lbCfDelete2.Visible = false;
            this.Controls.Add(lbCfDelete2);
            phAlbumInfo.Controls.Add(lbCfDelete2);
            LinkButton lbCcDelete = new LinkButton();
            lbCcDelete.ID = "lbCcDel";
            lbCcDelete.Text = "[Hủy bỏ]";
            lbCcDelete.Click += new EventHandler(deleteCancel);
            lbCcDelete.Visible = false;
            this.Controls.Add(lbCcDelete);
            phAlbumInfo.Controls.Add(lbCcDelete);
            Label lblDelResult = new Label();
            lblDelResult.ID = "lblDelResult";
            lblDelResult.Text = "";
            lblDelResult.Visible = false;
            this.Controls.Add(lblDelResult);
            phAlbumInfo.Controls.Add(lblDelResult);
        }

        int[] mediaId = csLib.toIntArray(album.media);
        if (csLib.isLoggedIn())
            mediaId = view.prepareMedia(mediaId, true);
        else
            mediaId = view.prepareMedia(mediaId, false);
        if (mediaId.Length > 0)
        {
            for (int i = 0; i < mediaId.Length; i++)
            {
                media = media.mediaGet(mediaId[i]);
                int[] mediaArray = { media.id };
                if (csLib.isLoggedIn())
                    mediaArray = view.prepareMedia(mediaArray, true);
                else
                    mediaArray = view.prepareMedia(mediaArray, false);
                if (mediaArray.Length <= 0)
                    continue;
                else
                {
                    string viewMMessage = "";
                    string viewMIndex = "<span style=\"background-color:#a42323; color:#ffffff;\">&nbsp;#" + (i + 1).ToString() + "&nbsp;</span>&nbsp;&nbsp;";
                    string viewMPreview = "<a href=\"MediaView.aspx?mediaid=" + csLib.encodeString(media.id.ToString()) + "\">" + view.viewThumnail(media.type, media.url, media.name, 3) + "</a>";
                    if (media.state != 1)
                        viewMMessage += "<br />" + view.toFailHtml("[Media này hiện không khả dụng]");      
                    string viewType = dtLib.getData_IntToStr("name", "TableMediaType", "id", media.type);
                    string viewMName = "<a class=\"hrefResultTitle\" href=\"MediaView.aspx?mediaid=" + csLib.encodeString(media.id.ToString()) + "\">[" + viewType + "] " + media.name + "</a>";

                    phAlbumPreview.Controls.Add(new LiteralControl(viewMIndex));
                    phAlbumPreview.Controls.Add(new LiteralControl(viewMName));
                    phAlbumPreview.Controls.Add(new LiteralControl(viewMMessage));
                    if (!string.IsNullOrEmpty(media.tag))
                    {
                        string[] mediaTagArray = csLib.toStrArray(media.tag);
                        if (mediaTagArray.Length > 0)
                        {
                            phAlbumPreview.Controls.Add(new LiteralControl("<br /><b>Tag:</b> "));
                            for (int j = 0; j < mediaTagArray.Length; j++)
                            {
                                LinkButton linkButton = new LinkButton();
                                linkButton.ID = "lbA" + i.ToString() + "lbM" + j.ToString();
                                linkButton.Text = "[" + mediaTagArray[j] + "]";
                                linkButton.CommandArgument = mediaTagArray[j];
                                linkButton.Click += new EventHandler(searchMediaTag);
                                this.Controls.Add(linkButton);
                                phAlbumPreview.Controls.Add(linkButton);
                            }
                        }
                    }
                    phAlbumPreview.Controls.Add(new LiteralControl("<hr>"));
                    phAlbumPreview.Controls.Add(new LiteralControl(viewMPreview));
                    phAlbumPreview.Controls.Add(new LiteralControl("<br /><br />"));
                }
            }
        }

        isSearch = true;
    }

    protected void searchAlbumTag(object sender, EventArgs e)
    {
        LinkButton linkButton = (LinkButton)sender;
        string lbArg = linkButton.CommandArgument;
        Session["searchTagCrossPageFlag"] = "true";
        Session["searchTagCrossPageData"] = "2" + "," + lbArg + ",";
        Response.BufferOutput = true;
        Response.Redirect("SearchTag.aspx");
    }

    protected void searchMediaTag(object sender, EventArgs e)
    {
        LinkButton linkButton = (LinkButton)sender;
        string lbArg = linkButton.CommandArgument;
        Session["searchTagCrossPageFlag"] = "true";
        Session["searchTagCrossPageData"] = "1" + "," + lbArg + ",";
        Response.BufferOutput = true;
        Response.Redirect("SearchTag.aspx");
    }

    protected void deleteAlbum(object sender, EventArgs e)
    {
        Label lbl = (Label)phAlbumInfo.FindControl("lblDelMsg");
        LinkButton lbConfirm = (LinkButton)phAlbumInfo.FindControl("lbCfDel");
        LinkButton lbConfirm2 = (LinkButton)phAlbumInfo.FindControl("lbCfDel2");
        LinkButton lbCancel = (LinkButton)phAlbumInfo.FindControl("lbCcDel");
        if (lbl != null && lbConfirm != null && lbConfirm2 != null && lbCancel != null)
        {
            lbl.Visible = true;
            lbConfirm.Visible = true;
            lbConfirm2.Visible = true;
            lbCancel.Visible = true;
        }
    }

    protected void deleteConfirm(object sender, EventArgs e)
    {
        confirmDeleteAlbum(false, false);
    }

    protected void deleteConfirm2(object sender, EventArgs e)
    {
        confirmDeleteAlbum(true, true);
    }

    protected void confirmDeleteAlbum(bool isMediaDel, bool isFileDel)
    {
        Album delAlbum = new Album();
        delAlbum.id = album.id;
        delAlbum = delAlbum.albumGet(delAlbum.id);
        Label lblResult = (Label)phAlbumInfo.FindControl("lblDelResult");
        Label lblMsg = (Label)phAlbumInfo.FindControl("lblDelMsg");
        LinkButton lbConfirm = (LinkButton)phAlbumInfo.FindControl("lbCfDel");
        LinkButton lbConfirm2 = (LinkButton)phAlbumInfo.FindControl("lbCfDel2");
        LinkButton lbCancel = (LinkButton)phAlbumInfo.FindControl("lbCcDel");
        LinkButton lbDelete = (LinkButton)phAlbumInfo.FindControl("lbDel");
        if (lblResult != null && lblMsg != null && lbConfirm != null && lbConfirm2 != null && lbCancel != null)
        {
            int x = delAlbum.albumDelete(delAlbum, isMediaDel, isFileDel);
            if (x == 0)
                phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP("Không xóa được album này.")));
            else
            {
                if (x == 2)
                    phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP("Không xóa được hết media / tập tin media khỏi hệ thống.")));
                else
                    phMessage.Controls.Add(new LiteralControl(view.toSuccessHtmlP("Album này đã được xóa thành công.")));

                if (!isMediaDel && !string.IsNullOrEmpty(delAlbum.media))
                {
                    Media media = new Media();
                    int[] y = csLib.toIntArray(delAlbum.media);
                    if (y.Length > 0)
                    {
                        for (int i = 0; i < y.Length; i++)
                        {
                            media = media.mediaGet(y[i]);
                            if (media.id > 0)
                            {
                                media.album = -1;
                                if (!media.mediaUpdate(media))
                                    phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP("Không cập nhật được thông tin cho media <b>[" + media.id.ToString() + "]</b>.")));
                            }
                        }
                    }
                }

                phMessage.Controls.Add(new LiteralControl("<a href=\"SearchAlbum.aspx\">[Danh mục album]</a>&nbsp;<a href=\"Default.aspx\">[Trang chủ]</a>"));
                phAlbumInfo.Controls.Clear();
                phAlbumPreview.Controls.Clear();
            }

            lblMsg.Visible = false;
            lbConfirm.Visible = false;
            lbConfirm2.Visible = false;
            lbCancel.Visible = false;
            lbDelete.Visible = false;
        }
    }

    protected void deleteCancel(object sender, EventArgs e)
    {
        Label lbl = (Label)phAlbumInfo.FindControl("lblDelMsg");
        LinkButton lbConfirm = (LinkButton)phAlbumInfo.FindControl("lbCfDel");
        LinkButton lbConfirm2 = (LinkButton)phAlbumInfo.FindControl("lbCfDel2");
        LinkButton lbCancel = (LinkButton)phAlbumInfo.FindControl("lbCcDel");
        if (lbl != null && lbConfirm != null && lbConfirm2 != null && lbCancel != null)
        {
            lbl.Visible = false;
            lbConfirm.Visible = false;
            lbConfirm2.Visible = false;
            lbCancel.Visible = false;
        }
    }
}