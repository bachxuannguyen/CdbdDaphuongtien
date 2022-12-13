using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using cdbd_daphuongtien.App_Code;

public partial class Administrator : System.Web.UI.Page
{
    public CsLib csLib = new CsLib();

    protected void Page_Load(object sender, EventArgs e)
    {
        this.Title = "Quản trị - " + csLib.varSiteName;

        if (!csLib.isLoggedIn())
        {
            csLib.toErrControl(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Đăng nhập quản trị để thực hiện thao tác này.", "Đăng nhập", "Login.aspx");
            return;
        }  
    }
    protected void lbMediaUpload_Click(object sender, EventArgs e)
    {
        Response.BufferOutput = true;
        Response.Redirect("MediaUpload.aspx");
    }
    protected void lbAlbumCreate_Click(object sender, EventArgs e)
    {
        Response.BufferOutput = true;
        Response.Redirect("AlbumCreate.aspx");
    }
    protected void lbTagMng_Click(object sender, EventArgs e)
    {
        Response.BufferOutput = true;
        Response.Redirect("TagCreate.aspx");
    }
    protected void lbAccountMng_Click(object sender, EventArgs e)
    {
        Response.BufferOutput = true;
        Response.Redirect("AccountCreate.aspx");
    }
    protected void lbLogout_Click(object sender, EventArgs e)
    {
        Session["account_isLoggedIn"] = null;
        Session["account_userId"] = null;
        Response.BufferOutput = true;
        Response.Redirect("Login.aspx");
    }
}