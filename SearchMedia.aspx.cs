using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using cdbd_daphuongtien.App_Code;

public partial class SearchMedia : System.Web.UI.Page
{
    CsLib csLib = new CsLib();
    DtLib dtLib = new DtLib();
    SearchEngine searchEngine = new SearchEngine();
    Media media = new Media();
    MediaType mediaType = new MediaType();
    View view = new View();

    public static int[] mediaResult = { -1 };
    public static bool isSearched = false;

    static int glPageCount = 0;
    static int glPageCurrent = 1;
    static int glRowsPerPage = 0;

    static bool isByName = false;
    static bool isByDesc = false;
    static bool isByType = false;
    static bool isByDatetime = false;
    static bool isByState = false;
    static bool isByAuthor = false;

    static string keyName = "";
    static string keyDesc = "";
    static int keyType = -1;
    static int keyDtT = -1;
    static DateTime keyDtF = DateTime.MinValue;
    static DateTime keyDtS = DateTime.MinValue;
    static int keyState = -1;
    static string keyAuthor = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        this.Title = "Media - " + csLib.varSiteName;
        glRowsPerPage = csLib.varRowsPerPage;

        if (!Page.IsPostBack)
        {
            varInit();

            int[] mediaTypeValue = mediaType.getAllId();
            string[] mediaTypeName = mediaType.getAllName();
            if (mediaTypeValue.Length > 0)
            {
                for (int i = 0; i < mediaTypeValue.Length; i++)
                {
                    ListItem li = new ListItem();
                    li.Value = mediaTypeValue[i].ToString();
                    li.Text = mediaTypeName[i].ToString();
                    ddlMediaType.Items.Add(li);
                }
            }
            
            getSearchResults();
        }

        if (Page.IsPostBack && isSearched)
            showSearchResults();
    }

    protected void varInit()
    {
        Array.Resize(ref mediaResult, 1);
        mediaResult[0] = -1;
        glPageCount = 0;
        glPageCurrent = 1;
        glRowsPerPage = csLib.varRowsPerPage;
        isSearched = false;

        isByName = false;
        isByDesc = false;
        isByType = false;
        isByDatetime = false;
        isByState = false;
        isByAuthor = false;
        keyName = "";
        keyDesc = "";
        keyType = -1;
        keyDtT = -1;
        keyDtF = DateTime.MinValue;
        keyDtS = DateTime.MinValue;
        keyState = -1;
        keyAuthor = "";
    }

    protected void btSearch_Click(object sender, EventArgs e)
    {
        getSearchResults();
    }

    protected void getSearchResults()
    {
        varInit();

        if (Session["searchMediaCrossPageFlag"] != null && Session["searchMediaCrossPageFlag"].ToString() == "true")
        {
            string[] searchData = csLib.toStrArray(Session["searchMediaCrossPageData"].ToString());
            switch (searchData[0])
            {
                case "1":
                    {
                        isByName = true;
                        keyName = searchData[1];
                        tbKeyword.Text = searchData[1];
                    }
                    break;
                case "2":
                    {
                        isByType = true;
                        keyType = Convert.ToInt32(searchData[1]);
                        ddlMediaType.SelectedValue = searchData[1].ToString();
                    }
                    break;
                default:
                    break;
            }
            Session["searchMediaCrossPageFlag"] = null;
            Session["searchMediaCrossPageData"] = null;
        }
        else
        {
            if (!string.IsNullOrEmpty(preCheck()))
            {
                varInit();
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

            if (ddlMediaType.SelectedValue != "0")
            {
                isByType = true;
                keyType = Convert.ToInt32(ddlMediaType.SelectedValue);
            }
        }

        mediaResult = searchEngine.mediaSearch(isByName, keyName, isByType, keyType, isByDesc, keyDesc, isByState, keyState, isByAuthor, keyAuthor, isByDatetime, keyDtT, keyDtF, keyDtS);
        if (csLib.isLoggedIn())
            mediaResult = view.prepareMedia(mediaResult, true);
        else
            mediaResult = view.prepareMedia(mediaResult, false);

        if (mediaResult.Length > 0)
        {
            glPageCount = Convert.ToInt32(mediaResult.Length / glRowsPerPage);
            if (mediaResult.Length % glRowsPerPage != 0)
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

        if (mediaResult.Length > 0)
        {
            for (int i = 1 + (glPageCurrent - 1) * glRowsPerPage; i <= glRowsPerPage + (glPageCurrent - 1) * glRowsPerPage; i++)
            {
                if (i > mediaResult.Length)
                    break;

                media.id = mediaResult[i - 1];
                media = media.mediaGet(media.id);

                string viewMessage = "";
                string viewIndex = "<span style=\"background-color:#a42323; color:#ffffff;\">&nbsp;#" + i.ToString() + "&nbsp;</span>&nbsp;&nbsp;";
                string viewType = dtLib.getData_IntToStr("name", "TableMediaType", "id", media.type);
                string viewName = "<a class=\"hrefResultTitle\" href=\"MediaView.aspx?mediaid=" + csLib.encodeString(media.id.ToString()) + "\">[" + viewType + "] " + media.name + "</a>";
                string viewDatetime = "<br /><b>Ngày tạo: </b>" + media.datetime.ToShortDateString();
                string viewDesc = "";
                if (!string.IsNullOrEmpty(media.description))
                    viewDesc = "<br /><b>Mô tả:</b> " + media.description;               
                string viewAlbum = "";
                if (media.album > 0 && !string.IsNullOrEmpty(dtLib.getData_IntToStr("name", "TableAlbum", "id", media.album)))
                        viewAlbum = "<br /><b>Album: </b><a href=\"AlbumView.aspx?albumid=" + csLib.encodeString(media.album.ToString()) + "\">" + dtLib.getData_IntToStr("name", "TableAlbum", "id", media.album) + "</a>";
                string viewPreview = "<a href=\"MediaView.aspx?mediaid=" + csLib.encodeString(media.id.ToString()) + "\">" + view.viewThumnail(media.type, media.url, media.name, 3) + "</a>";
                if (media.state != 1)
                    viewMessage += "<br />" + view.toFailHtml("[Media này hiện không khả dụng]");               

                phSearchResult.Controls.Add(new LiteralControl("<div class=\"columnSearchResult2\">"));
                phSearchResult.Controls.Add(new LiteralControl(viewIndex));
                phSearchResult.Controls.Add(new LiteralControl(viewName));
                phSearchResult.Controls.Add(new LiteralControl(viewMessage));
                if (csLib.isLoggedIn())
                {
                    string viewEdit = "<br /><a href=\"MediaEdit.aspx?mediaid=" + csLib.encodeString(media.id.ToString()) + "\">[Chỉnh sửa]</a>";
                    phSearchResult.Controls.Add(new LiteralControl(viewEdit));
                }
                phSearchResult.Controls.Add(new LiteralControl(viewDatetime));
                phSearchResult.Controls.Add(new LiteralControl(viewAlbum));
                phSearchResult.Controls.Add(new LiteralControl(viewDesc));
                phSearchResult.Controls.Add(new LiteralControl("<hr>"));
                phSearchResult.Controls.Add(new LiteralControl(viewPreview));
                phSearchResult.Controls.Add(new LiteralControl("</div>"));
            }
            lblSearchResult.Text = "<div class=\"iconSResult\">" + mediaResult.Length.ToString() + " kết quả, trang " + glPageCurrent + "/" + glPageCount + "</div>";
            lblSearchResult2.Text = "<div class=\"iconSResult\">" + mediaResult.Length.ToString() + " kết quả, trang " + glPageCurrent + "/" + glPageCount + "</div>";
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

    protected void btChoose_Click(object sender, EventArgs e)
    {
        int x = 0;
        if (Int32.TryParse(hfPageCurrent.Value, out x) == false)
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