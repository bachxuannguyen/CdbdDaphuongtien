using cdbd_daphuongtien.App_Code;
using System;
using System.Web.UI;

public partial class _Default : Page
{
    SearchEngine searchEngine = new SearchEngine();
    CsLib csLib = new CsLib();

    public int countAlbumTot = 0;
    public int countMediaTot = 0;
    public int countMediaImg = 0;
    public int countMediaInf = 0;
    public int countMediaVid = 0;
    public int countMediaSou = 0;
    public int countMediaAni = 0;
    public int countTagTot = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        //lbSearchTag.Text = "[Tìm kiếm dựa trên tag]";

        this.Title = "Trang chủ";

        bool allowDisabled = false;
        if (csLib.isLoggedIn())
            allowDisabled = true;

        countAlbumTot = searchEngine.getAlbumCount(allowDisabled);
        countMediaTot = searchEngine.getMediaCount(allowDisabled, 0);
        countMediaImg = searchEngine.getMediaCount(allowDisabled, 1);
        countMediaInf = searchEngine.getMediaCount(allowDisabled, 2);
        countMediaVid = searchEngine.getMediaCount(allowDisabled, 3);
        countMediaSou = searchEngine.getMediaCount(allowDisabled, 4);
        countMediaAni = searchEngine.getMediaCount(allowDisabled, 5);
        countTagTot = searchEngine.getTagCount(true);

        if (countAlbumTot < 0)
            countAlbumTot = 0;
        if (countMediaTot < 0)
            countMediaTot = 0;
        if (countMediaImg < 0)
            countMediaImg = 0;
        if (countMediaInf < 0)
            countMediaInf = 0;
        if (countMediaVid < 0)
            countMediaVid = 0;
        if (countMediaSou < 0)
            countMediaSou = 0;
        if (countMediaAni < 0)
            countMediaAni = 0;
        if (countTagTot < 0)
            countTagTot = 0;

        lbAlbumTot.Text = countAlbumTot.ToString() + " Album";
        lbMediaTot.Text = countMediaTot.ToString() + " Media";
        lbTagTot.Text = countTagTot.ToString() + " Tag";

        lbMediaImg.Text = "[" + countMediaImg.ToString() + " Hình ảnh]";
        lbMediaInf.Text = "[" + countMediaInf.ToString() + " Infographic]";
        lbMediaVid.Text = "[" + countMediaVid.ToString() + " Video]";
        lbMediaSou.Text = "[" + countMediaSou.ToString() + " Âm thanh]";
        lbMediaAni.Text = "[" + countMediaAni.ToString() + " Hoạt họa]";
    }

    protected void lbAlbumTot_Click(object sender, EventArgs e)
    {
        Response.BufferOutput = true;
        Response.Redirect("SearchAlbum.aspx");
    }

    protected void lbMediaTot_Click(object sender, EventArgs e)
    {
        Response.BufferOutput = true;
        Response.Redirect("SearchMedia.aspx");
    }

    protected void lbMediaImg_Click(object sender, EventArgs e)
    {
        string s1 = "2";
        string s2 = "1";
        Session["searchMediaCrossPageFlag"] = "true";
        Session["searchMediaCrossPageData"] = s1 + "," + s2 + ",";
        Response.BufferOutput = true;
        Response.Redirect("SearchMedia.aspx");
    }

    protected void lbMediaInf_Click(object sender, EventArgs e)
    {
        string s1 = "2";
        string s2 = "2";
        Session["searchMediaCrossPageFlag"] = "true";
        Session["searchMediaCrossPageData"] = s1 + "," + s2 + ",";
        Response.BufferOutput = true;
        Response.Redirect("SearchMedia.aspx");
    }

    protected void lbMediaVid_Click(object sender, EventArgs e)
    {
        string s1 = "2";
        string s2 = "3";
        Session["searchMediaCrossPageFlag"] = "true";
        Session["searchMediaCrossPageData"] = s1 + "," + s2 + ",";
        Response.BufferOutput = true;
        Response.Redirect("SearchMedia.aspx");
    }

    protected void lbMediaSou_Click(object sender, EventArgs e)
    {
        string s1 = "2";
        string s2 = "4";
        Session["searchMediaCrossPageFlag"] = "true";
        Session["searchMediaCrossPageData"] = s1 + "," + s2 + ",";
        Response.BufferOutput = true;
        Response.Redirect("SearchMedia.aspx");
    }

    protected void lbMediaAni_Click(object sender, EventArgs e)
    {
        string s1 = "2";
        string s2 = "5";
        Session["searchMediaCrossPageFlag"] = "true";
        Session["searchMediaCrossPageData"] = s1 + "," + s2 + ",";
        Response.BufferOutput = true;
        Response.Redirect("SearchMedia.aspx");
    }

    protected void btSearch_Click(object sender, EventArgs e)
    {
        if (ddlSearch.SelectedValue == "1")
        {
            if (!string.IsNullOrEmpty(tbSearch.Text))
            {
                string s1 = "1";
                string s2 = tbSearch.Text;
                Session["searchMediaCrossPageFlag"] = "true";
                Session["searchMediaCrossPageData"] = s1 + "," + s2 + ",";
            }
            Response.BufferOutput = true;
            Response.Redirect("SearchMedia.aspx");
        }
        else
        {
            if (!string.IsNullOrEmpty(tbSearch.Text))
            {
                string s1 = "1";
                string s2 = tbSearch.Text;
                Session["searchAlbumCrossPageFlag"] = "true";
                Session["searchAlbumCrossPageData"] = s1 + "," + s2 + ",";
            }
            Response.BufferOutput = true;
            Response.Redirect("SearchAlbum.aspx");
        }
    }

    protected void lbTagTot_Click(object sender, EventArgs e)
    {
        Response.BufferOutput = true;
        Response.Redirect("SearchTag.aspx");
    }
}