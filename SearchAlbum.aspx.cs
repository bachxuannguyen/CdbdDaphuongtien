using cdbd_daphuongtien.App_Code;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class SearchAlbum : System.Web.UI.Page
{
    CsLib csLib = new CsLib();
    DtLib dtLib = new DtLib();
    SearchEngine searchEngine = new SearchEngine();
    Album album = new Album();
    View view = new View();

    public static int[] albumResult = { -1 };
    public static bool isSearched = false;

    static int glPageCount = 0;
    static int glPageCurrent = 1;
    static int glRowsPerPage = 0;

    static bool isByName = false;
    static bool isByDesc = false;
    static bool isByDatetime = false;
    static bool isByState = false;

    static string keyName = "";
    static string keyDesc = "";
    static int keyDtT = -1;
    static DateTime keyDtF = DateTime.MinValue;
    static DateTime keyDtS = DateTime.MinValue;
    static int keyState = -1;

    protected void Page_Load(object sender, EventArgs e)
    {
        this.Title = "Album - " + csLib.varSiteName;
        glRowsPerPage = csLib.varRowsPerPage;

        if (!Page.IsPostBack)
        {
            varInit();
            getSearchResults();
        }

        if (Page.IsPostBack && isSearched)
            showSearchResults();
    }

    protected void varInit()
    {
        Array.Resize(ref albumResult, 0);
        glPageCount = 0;
        glPageCurrent = 1;
        glRowsPerPage = csLib.varRowsPerPage;
        isSearched = false;

        isByName = false;
        isByDesc = false;
        isByDatetime = false;
        isByState = false;
        keyName = "";
        keyDesc = "";
        keyDtT = -1;
        keyDtF = DateTime.MinValue;
        keyDtS = DateTime.MinValue;
        keyState = -1;
    }

    protected void btSearch_Click(object sender, EventArgs e)
    {
        getSearchResults();
    }

    protected void getSearchResults()
    {
        varInit();

        if (Session["searchAlbumCrossPageFlag"] != null && Session["searchAlbumCrossPageFlag"].ToString() == "true")
        {
            string[] searchData = csLib.toStrArray(Session["searchAlbumCrossPageData"].ToString());
            switch (searchData[0])
            {
                case "1":
                    {
                        isByName = true;
                        keyName = searchData[1];
                        tbKeyword.Text = searchData[1];
                    }
                    break;
                default:
                    break;
            }
            Session["searchAlbumCrossPageFlag"] = null;
            Session["searchAlbumCrossPageFlag"] = null;
        }
        else
        {
            if (!string.IsNullOrEmpty(preCheck()))
            {
                phSearchResult.Controls.Clear();
                phMessage.Controls.Clear();
                phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP(preCheck())));
                showPageButtons();
                lblSearchResult.Text = "";
                lblSearchResult2.Text = "";
                return;
            }

            if (!string.IsNullOrEmpty(tbKeyword.Text))
            {
                if (ddlKeywordType.SelectedValue == "1")
                {
                    isByName = true;
                    keyName = tbKeyword.Text;
                }
                else
                {
                    isByDesc = true;
                    keyDesc = tbKeyword.Text;
                }
            }
        }

        albumResult = searchEngine.albumSearch(isByName, keyName, isByDesc, keyDesc, isByState, keyState, isByDatetime, keyDtT, keyDtF, keyDtS);
        if (csLib.isLoggedIn())
            albumResult = view.prepareAlbum(albumResult, true);
        else
            albumResult = view.prepareAlbum(albumResult, false);

        if (albumResult.Length > 0)
        {
            glPageCount = Convert.ToInt32(albumResult.Length / glRowsPerPage);
            if (albumResult.Length % glRowsPerPage != 0)
                glPageCount++;
            hfPageCount.Value = Convert.ToString(glPageCount);
            glPageCurrent = 1;
            btChoose.Visible = true;
            btPrevious.Visible = true;
            btNext.Visible = true;
            showPageButtons();
            isSearched = true;
        }
        showSearchResults();
    }

    protected void showSearchResults()
    {
        phSearchResult.Controls.Clear();
        lblSearchResult.Text = "";
        lblSearchResult2.Text = "";

        if (albumResult.Length > 0)
        {
            for (int i = 1 + (glPageCurrent - 1) * glRowsPerPage; i <= glRowsPerPage + (glPageCurrent - 1) * glRowsPerPage; i++)
            {
                if (i > albumResult.Length)
                    break;

                album.id = albumResult[i - 1];
                album = album.albumGet(album.id);

                string viewMessage = "";
                string viewIndex = "<span style=\"background-color:#" + csLib.varColorBlueBg + "; color:#ffffff;\">&nbsp;#" + i.ToString() + "&nbsp;</span>&nbsp;&nbsp;";
                string viewName = "<a class=\"hrefResultTitle\" href=\"AlbumView.aspx?albumid=" + csLib.encodeString(album.id.ToString()) + "\">" + album.name + "</a>";
                string viewDatetime = "<br /><b>Ngày tạo: </b>" + album.datetime.ToShortDateString();
                string viewMediaCount = "";
                int[] mediaCount = album.getMediaCount(album.id);
                if (mediaCount[0] == -1 || (mediaCount[0] <= 0 && mediaCount[1] <= 0 && mediaCount[2] <= 0 && mediaCount[3] <= 0 && mediaCount[4] <= 0))
                    viewMessage += "<br />" + view.toFailHtml("[Không thống kê được số lượng media của album này]");
                else
                {
                    viewMediaCount = "&nbsp;<b>Nội dung: </b>";
                    string c = "";
                    if (mediaCount[0] > 0)
                    {
                        viewMediaCount += mediaCount[0].ToString() + " ảnh";
                        c = ", ";
                    }
                    if (mediaCount[1] > 0)
                    {
                        viewMediaCount += c + mediaCount[1].ToString() + " infographic";
                        c = ", ";
                    }
                    if (mediaCount[2] > 0)
                    {
                        viewMediaCount += c + mediaCount[2].ToString() + " video";
                        c = ", ";
                    }
                    if (mediaCount[3] > 0)
                    {
                        viewMediaCount += c + mediaCount[3].ToString() + " âm thanh";
                        c = ", ";
                    }
                    if (mediaCount[4] > 0)
                    {
                        viewMediaCount += c + mediaCount[4].ToString() + " hoạt họa";
                        c = ", ";
                    }
                }
                if (album.state != 1)
                    viewMessage += "<br />" + view.toFailHtml("[Album này hiện không khả dụng]");
                string viewDesc = "";
                if (!string.IsNullOrEmpty(album.description))
                    viewDesc = "<br /><b>Mô tả: </b>" + album.description;

                phSearchResult.Controls.Add(new LiteralControl("<div class=\"columnSearchResult2\">"));
                phSearchResult.Controls.Add(new LiteralControl(viewIndex));
                phSearchResult.Controls.Add(new LiteralControl(viewName));
                phSearchResult.Controls.Add(new LiteralControl(viewMessage));
                if (csLib.isLoggedIn())
                {
                    string viewEdit = "<br /><a href=\"AlbumCreate.aspx?option=" + csLib.encodeString("edit") + "&albumid=" + csLib.encodeString(album.id.ToString()) + "\">[Chỉnh sửa]</a>";
                    phSearchResult.Controls.Add(new LiteralControl(viewEdit));
                }
                phSearchResult.Controls.Add(new LiteralControl(viewDatetime));
                phSearchResult.Controls.Add(new LiteralControl(viewMediaCount));
                phSearchResult.Controls.Add(new LiteralControl(viewDesc));
                phSearchResult.Controls.Add(new LiteralControl("<hr>"));
                phSearchResult.Controls.Add(new LiteralControl("</div>"));
            }
            lblSearchResult.Text = "<div class=\"iconSResult\">" + albumResult.Length.ToString() + " kết quả, trang " + glPageCurrent + "/" + glPageCount + "</div>";
            lblSearchResult2.Text = "<div class=\"iconSResult\">" + albumResult.Length.ToString() + " kết quả, trang " + glPageCurrent + "/" + glPageCount + "</div>";
        }
        else
        {
            lblSearchResult.Text = "Không tìm thấy kết quả nào.";
            lblSearchResult2.Text = "";
            phSearchResult.Controls.Clear();
        }
    }

    protected string preCheck()
    {
        string outputStr = "";

        if (!string.IsNullOrEmpty(tbKeyword.Text))
        {
            if (!string.IsNullOrEmpty(csLib.isValidString("Từ khóa", tbKeyword.Text, true, false, true)))
            {
                outputStr += "Từ khóa không hợp lệ.";
                return outputStr;
            }
        }

        return outputStr;
    }

    protected void tagClickSearch(object sender, EventArgs e)
    {
        LinkButton linkButton = (LinkButton)sender;
        string lbIdArg = linkButton.CommandArgument;
        Session["searchTagCrossPageFlag"] = "true";
        Session["searchTagCrossPageData"] = "1" + "," + lbIdArg + ",";
        Response.BufferOutput = true;
        Response.Redirect("SearchTag.aspx");
    }

    protected void btChoose_Click(object sender, EventArgs e)
    {
        int x = 0;
        if (Int32.TryParse(hfPageCurrent.Value, out x) == true)
            csLib.toErrControl(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Số trang không đúng.", "Trang chủ", "Default.aspx");
        if (x > 0 & x <= glPageCount)
        {
            glPageCurrent = x;
            showPageButtons();
            showSearchResults();
        }
        else
            csLib.toErrControl(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Số trang không đúng.", "Trang chủ", "Default.aspx");
    }

    protected void btPrevious_Click(object sender, EventArgs e)
    {
        if (glPageCurrent - 1 > 0)
        {
            glPageCurrent--;
            showPageButtons();
            showSearchResults();
        }
    }

    protected void btNext_Click(object sender, EventArgs e)
    {
        if (glPageCurrent + 1 <= glPageCount)
        {
            glPageCurrent++;
            showPageButtons();
            showSearchResults();
        }
    }

    protected void showPageButtons()
    {
        if (glPageCurrent < glPageCount)
            btNext.Enabled = true;
        else
            btNext.Enabled = false;
        if (glPageCurrent > 1)
            btPrevious.Enabled = true;
        else
            btPrevious.Enabled = false;
        if (glPageCount > 1)
            btChoose.Enabled = true;
        else
            btChoose.Enabled = false;
    }
}