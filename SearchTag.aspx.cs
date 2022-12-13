using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using cdbd_daphuongtien.App_Code;

public partial class SearchTag : System.Web.UI.Page
{
    CsLib csLib = new CsLib();
    DtLib dtLib = new DtLib();
    SearchEngine searchEngine = new SearchEngine();
    View view = new View();

    Media media = new Media();
    Album album = new Album();
    static Tag tag = new Tag();

    public static int[] mediaResult = new int[0];
    public static int[] albumResult = new int[0];
    public static int[] result = new int[0];

    public static int glPageCount = 0;
    public static int glPageCurrent = 1;
    public static int glRowsPerPage = 0;
    public static bool isSearched = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        this.Title = "Tag - " + csLib.varSiteName;
        glRowsPerPage = csLib.varRowsPerPage;

        if (!Page.IsPostBack)
        {
            VarInit();

            glRowsPerPage = csLib.varRowsPerPage;

            if (Session["searchTagCrossPageFlag"] != null && Session["searchTagCrossPageFlag"].ToString() == "true")
                showSearchResults();
        }

        if (Page.IsPostBack && isSearched)
            showSearchResults();

        buildTagList();
    }

    protected void VarInit()
    {
        tag = new Tag();
        Array.Resize(ref mediaResult, 0);
        Array.Resize(ref albumResult, 0);
        Array.Resize(ref result, 0);
        glPageCount = 0;
        glPageCurrent = 1;
        glRowsPerPage = csLib.varRowsPerPage;
        isSearched = false;
    }

    protected void buildTagList()
    {
        bool isAllowDisabled = true;

        string[] allPTagId = tag.getAllPTagId(isAllowDisabled);
        if (allPTagId.Length > 0)
        {
            phTagList.Controls.Add(new LiteralControl("<hr>"));
            phTagList.Controls.Add(new LiteralControl("<b>Tag khả dụng:</b><br />"));
            for (int i = 0; i < allPTagId.Length; i++)
            {
                LinkButton linkButton = new LinkButton();
                linkButton.ID = allPTagId[i];
                linkButton.Text = "<b>[" + allPTagId[i] + "]</b>";
                linkButton.CommandArgument = allPTagId[i];
                linkButton.Click += new EventHandler(tagClickSearch);
                this.Controls.Add(linkButton);
                phTagList.Controls.Add(linkButton);
                string[] myCTag = tag.getMyCTagId(allPTagId[i], false);
                if (myCTag.Length > 0)
                {
                    for (int j = 0; j < myCTag.Length; j++)
                    {
                        LinkButton linkButton2 = new LinkButton();
                        linkButton2.ID = myCTag[j];
                        linkButton2.Text = "[" + myCTag[j] + "]";
                        linkButton2.CommandArgument = myCTag[j];
                        linkButton2.Click += new EventHandler(tagClickSearch);
                        this.Controls.Add(linkButton2);
                        phTagList.Controls.Add(linkButton2);
                    }
                }
                phTagList.Controls.Add(new LiteralControl("<br />"));
            }
        }
    }

    protected void tagClickSearch(object sender, EventArgs e)
    {
        LinkButton linkButton = (LinkButton)sender;
        string clickedTag = linkButton.CommandArgument;
        tbKeyword.Text = clickedTag;
        string[] clickedTagArray = tag.getMyCTagId(clickedTag, true);
        if (clickedTagArray.Length > 0)
        {
            if (rbSearchMedia.Checked)
            {
                mediaResult = searchEngine.tagBasedSearch(clickedTagArray, 1, false, -1, false, -1, DateTime.MinValue, DateTime.MinValue);
                if (csLib.isLoggedIn())
                    mediaResult = view.prepareMedia(mediaResult, true);
                else
                    mediaResult = view.prepareMedia(mediaResult, false);

                if (mediaResult.Length > 0)
                {
                    glPageCount = Convert.ToInt32(mediaResult.Length / glRowsPerPage);
                    if (mediaResult.Length % glRowsPerPage != 0)
                        glPageCount++;
                }
            }
            else
            {
                albumResult = searchEngine.tagBasedSearch(clickedTagArray, 2, false, -1, false, -1, DateTime.MinValue, DateTime.MinValue);
                if (csLib.isLoggedIn())
                    albumResult = view.prepareAlbum(albumResult, true);
                else
                    albumResult = view.prepareAlbum(albumResult, false);

                if (albumResult.Length > 0)
                {
                    glPageCount = Convert.ToInt32(albumResult.Length / glRowsPerPage);
                    if (albumResult.Length % glRowsPerPage != 0)
                        glPageCount++;
                }
            }
            btChoose.Visible = true;
            btPrevious.Visible = true;
            btNext.Visible = true;
            hfPageCount.Value = Convert.ToString(glPageCount);
            glPageCurrent = 1;
            showPageButtons();
            isSearched = true;
            showSearchResults();
        }
    }

    protected void btSearch_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(preCheck()))
        {
            VarInit();
            phMessage.Controls.Clear();
            phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP(preCheck())));
            lblSearchResult.Text = "";
            lblSearchResult2.Text = "";
            phSearchResult.Controls.Clear();
            showPageButtons();
            return;
        }

        bool isByDatetime = false;
        bool isByState = false;
        
        string[] tagArray = tag.getMyCTagId(tbKeyword.Text, true);

        if (tagArray.Length > 0)
        {      
            if (rbSearchMedia.Checked)
            {
                mediaResult = searchEngine.tagBasedSearch(tagArray, 1, isByState, -1, isByDatetime, -1, DateTime.MinValue, DateTime.MinValue);
                if (csLib.isLoggedIn())
                    mediaResult = view.prepareMedia(mediaResult, true);
                else
                    mediaResult = view.prepareMedia(mediaResult, false);

                if (mediaResult.Length > 0)
                {
                    glPageCount = Convert.ToInt32(mediaResult.Length / glRowsPerPage);
                    if (mediaResult.Length % glRowsPerPage != 0)
                        glPageCount++;
                }
            }
            else
            {
                albumResult = searchEngine.tagBasedSearch(tagArray, 2, isByState, -1, isByDatetime, -1, DateTime.MinValue, DateTime.MinValue);
                if (csLib.isLoggedIn())
                    albumResult = view.prepareAlbum(albumResult, true);
                else
                    albumResult = view.prepareAlbum(albumResult, false);

                if (albumResult.Length > 0)
                {
                    glPageCount = Convert.ToInt32(albumResult.Length / glRowsPerPage);
                    if (albumResult.Length % glRowsPerPage != 0)
                        glPageCount++;
                }
            }
            btChoose.Visible = true;
            btPrevious.Visible = true;
            btNext.Visible = true;
            hfPageCount.Value = Convert.ToString(glPageCount);
            glPageCurrent = 1;
            showPageButtons();
            isSearched = true;
            showSearchResults();
        }
    }

    protected string preCheck()
    {
        string outputStr = "";

        if (string.IsNullOrEmpty(tbKeyword.Text))
        {
            outputStr += "Từ khóa không được để trống.";
            return outputStr;
        }
        else if (!string.IsNullOrEmpty(csLib.isValidString("Từ khóa", tbKeyword.Text, true, false, true)))
        {
            outputStr += "Từ khóa không hợp lệ.";
            return outputStr;
        }

        return outputStr;
    }

    protected void showSearchResults()
    {
        phSearchResult.Controls.Clear();

        if (Session["searchTagCrossPageFlag"] != null && Session["searchTagCrossPageFlag"].ToString() == "true")
        {
            string[] searchData = csLib.toStrArray(Session["searchTagCrossPageData"].ToString());

            bool isByState = false;
            bool isByDatetime = false;     
            DateTime dtFirst = DateTime.MinValue;
            DateTime dtSecond = DateTime.MinValue;

            string[] tagArray = tag.getMyCTagId(searchData[1], true);

            if (tagArray.Length > 0)
            {
                if (searchData[0] == "1")
                {
                    mediaResult = searchEngine.tagBasedSearch(tagArray, 1, isByState, -1, isByDatetime, -1, DateTime.MinValue, DateTime.MinValue);
                    if (csLib.isLoggedIn())
                        mediaResult = view.prepareMedia(mediaResult, true);
                    else
                        mediaResult = view.prepareMedia(mediaResult, false);

                    if (mediaResult.Length > 0)
                    {
                        glPageCount = Convert.ToInt32(mediaResult.Length / glRowsPerPage);
                        if (mediaResult.Length % glRowsPerPage != 0)
                            glPageCount++;
                    }

                    rbSearchMedia.Checked = true;
                    rbSearchAlbum.Checked = false;
                    tbKeyword.Text = searchData[1];
                }
                else
                {
                    albumResult = searchEngine.tagBasedSearch(tagArray, 2, isByState, -1, isByDatetime, -1, DateTime.MinValue, DateTime.MinValue);
                    if (csLib.isLoggedIn())
                        albumResult = view.prepareAlbum(albumResult, true);
                    else
                        albumResult = view.prepareAlbum(albumResult, false);

                    if (albumResult.Length > 0)
                    {
                        glPageCount = Convert.ToInt32(albumResult.Length / glRowsPerPage);
                        if (albumResult.Length % glRowsPerPage != 0)
                            glPageCount++;
                    }

                    rbSearchAlbum.Checked = true;
                    rbSearchMedia.Checked = false;
                    tbKeyword.Text = searchData[1];
                }

                btChoose.Visible = true;
                btPrevious.Visible = true;
                btNext.Visible = true;
                hfPageCount.Value = Convert.ToString(glPageCount);
                glPageCurrent = 1;
                showPageButtons();
                isSearched = true;

                Session["searchTagCrossPageFlag"] = null;
                Session["searchTagCrossPageData"] = null;
            }
        }

        string[] objectTagArray;

        Array.Resize(ref result, 0);

        if (rbSearchMedia.Checked)
        {
            Array.Resize(ref result, mediaResult.Length);
            result = mediaResult;
        }
        else
        {
            Array.Resize(ref result, albumResult.Length);
            result = albumResult;
        }

        if (result.Length > 0)
        {
            if (rbSearchMedia.Checked)
            {
                for (int i = 1 + (glPageCurrent - 1) * glRowsPerPage; i <= glRowsPerPage + (glPageCurrent - 1) * glRowsPerPage; i++)
                {
                    if (i > result.Length)
                        break;

                    media.id = result[i - 1];
                    media = media.mediaGet(media.id);

                    string viewMessage = "";
                    string viewMediaIndex = "<span style=\"background-color:#a42323; color:#ffffff;\">&nbsp;#" + i.ToString() + "&nbsp;</span>&nbsp;&nbsp;";
                    string viewType = dtLib.getData_IntToStr("name", "TableMediaType", "id", media.type);
                    string viewName = "<a class=\"hrefResultTitle\" href=\"MediaView.aspx?mediaid=" + csLib.encodeString(media.id.ToString()) + "\">[" + viewType + "] " + media.name + "</a>";          
                    string viewDatetime = "<br /><b>Ngày tạo: </b>" + media.datetime.ToShortDateString();
                    string viewPreview = "<a href=\"MediaView.aspx?mediaid=" + csLib.encodeString(media.id.ToString()) + "\">" + view.viewThumnail(media.type, media.url, media.name, 3) + "</a>";        
                    if (media.state != 1)
                        viewMessage += "<br />" + view.toFailHtml("[Media này hiện không khả dụng]"); 
                        
                    phSearchResult.Controls.Add(new LiteralControl("<div class=\"columnSearchResult2\">"));                  
                    phSearchResult.Controls.Add(new LiteralControl(viewMediaIndex));
                    phSearchResult.Controls.Add(new LiteralControl(viewName));
                    phSearchResult.Controls.Add(new LiteralControl(viewMessage));
                    if (csLib.isLoggedIn())
                    {
                        string viewEdit = "<br /><a href=\"MediaEdit.aspx?mediaid=" + csLib.encodeString(media.id.ToString()) + "\">[Chỉnh sửa]</a>";
                        phSearchResult.Controls.Add(new LiteralControl(viewEdit));
                    }
                    phSearchResult.Controls.Add(new LiteralControl(viewDatetime));
                    if (!string.IsNullOrEmpty(media.tag))
                    {
                        objectTagArray = csLib.toStrArray(media.tag);
                        if (!string.IsNullOrEmpty(objectTagArray[0]))
                        {
                            phSearchResult.Controls.Add(new LiteralControl("<br /><b>Tag:</b> "));
                            for (int j = 0; j < objectTagArray.Length; j++)
                            {
                                LinkButton linkButton = new LinkButton();
                                linkButton.ID = media.id.ToString() + objectTagArray[j];
                                linkButton.Text = "[" + objectTagArray[j] + "]";
                                linkButton.CommandArgument = objectTagArray[j];
                                linkButton.Click += new EventHandler(tagClickSearch);
                                this.Controls.Add(linkButton);
                                phSearchResult.Controls.Add(linkButton);
                            }
                        }
                    }
                    phSearchResult.Controls.Add(new LiteralControl("<hr>"));
                    phSearchResult.Controls.Add(new LiteralControl(viewPreview));
                    phSearchResult.Controls.Add(new LiteralControl("</div>"));
                }
                lblSearchResult.Text = "<div class=\"iconSResult\">" + result.Length.ToString() + " kết quả, trang " + glPageCurrent + "/" + glPageCount + "</div>";
                lblSearchResult2.Text = "<div class=\"iconSResult\">" + result.Length.ToString() + " kết quả, trang " + glPageCurrent + "/" + glPageCount + "</div>";
            }
            else
            {
                for (int i = 1 + (glPageCurrent - 1) * glRowsPerPage; i <= glRowsPerPage + (glPageCurrent - 1) * glRowsPerPage; i++)
                {
                    if (i > result.Length)
                        break;

                    album.id = result[i - 1];
                    album = album.albumGet(album.id);

                    string viewMessage = "";
                    string viewAlbumIndex = "<span style=\"background-color:#" + csLib.varColorBlueBg + "; color:#ffffff;\">&nbsp;#" + i.ToString() + "&nbsp;</span>&nbsp;&nbsp;"; 
                    string viewName = "<a class=\"hrefResultTitle\" href=\"AlbumView.aspx?albumid=" + csLib.encodeString(album.id.ToString()) + "\">" + album.name + "</a>";
                    string viewDatetime = "<br /><b>Ngày tạo: </b>" + album.datetime.ToShortDateString();
                    string viewMediaCount = "";
                    int[] mediaCount = album.getMediaCount(album.id);
                    if (mediaCount[0] == -1)
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
                    if (album.state == 0)
                        viewMessage += "<br />" + view.toFailHtml("[Album này hiện không khả dụng]");
                    string viewDesc = "";
                    if (!string.IsNullOrEmpty(album.description))
                        viewDesc = "<br /><b>Mô tả: </b>" + album.description;

                    phSearchResult.Controls.Add(new LiteralControl("<div class=\"columnSearchResult2\">"));
                    phSearchResult.Controls.Add(new LiteralControl(viewAlbumIndex));
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
                    if (!string.IsNullOrEmpty(album.tag))
                    {
                        objectTagArray = csLib.toStrArray(album.tag);
                        if (!string.IsNullOrEmpty(objectTagArray[0]))
                        {
                            phSearchResult.Controls.Add(new LiteralControl("<br /><b>Tag:</b> "));
                            for (int j = 0; j < objectTagArray.Length; j++)
                            {
                                LinkButton linkButton = new LinkButton();
                                linkButton.ID = album.id.ToString() + objectTagArray[j];
                                linkButton.Text = "[" + objectTagArray[j] + "]";
                                linkButton.CommandArgument = objectTagArray[j];
                                linkButton.Click += new EventHandler(tagClickSearch);
                                this.Controls.Add(linkButton);
                                phSearchResult.Controls.Add(linkButton);
                            }
                        }
                    }
                    phSearchResult.Controls.Add(new LiteralControl("<hr>"));
                    phSearchResult.Controls.Add(new LiteralControl("</div>"));
                }
                lblSearchResult.Text = "<div class=\"iconSResult\">" + result.Length.ToString() + " kết quả, trang " + glPageCurrent + "/" + glPageCount + "</div>";
                lblSearchResult2.Text = "<div class=\"iconSResult\">" + result.Length.ToString() + " kết quả, trang " + glPageCurrent + "/" + glPageCount + "</div>";           
            }
        }
        else
        {
            lblSearchResult.Text = "Không tìm thấy kết quả nào.";
            lblSearchResult2.Text = "";
            phSearchResult.Controls.Clear();
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
}