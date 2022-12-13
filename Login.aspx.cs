using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using cdbd_daphuongtien.App_Code;

public partial class Login : System.Web.UI.Page
{
    CsLib csLib = new CsLib();
    DtLib dtLib = new DtLib();
    static Account account = new Account();
    View view = new View();

    public string usernameTyped = "";
    public string passwordTyped = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        this.Title = "Đăng nhập - " + csLib.varSiteName;

        if (!Page.IsPostBack)
        {
            if ((string)Session["account_isLoggedIn"] == "Y")
            {
                lbLogout.Visible = true;
                lgAccount.Enabled = false;
                lblLoggedIn.Visible = true;
                lblLoggedIn.Text = "Tài khoản sau đây đang đăng nhập: <b>" + (string)Session["account_userId"]
                    + "</b> | <a href=\"Administrator.aspx\" target=\"_self\">Trang quản trị</a> | <a href=\"Default.aspx\" target=\"_self\">Trang chủ</a> |";
            }
            else
            {
                lbLogout.Visible = false;
                lgAccount.Enabled = true;
                lblLoggedIn.Visible = true;
                lblLoggedIn.Text = "Tài khoản chỉ được cấp bởi người quản trị hệ thống | " + "<a href=\"Default.aspx\">Trang chủ</a>";
            }
        }
    }

    protected void login_Authenticate(object sender, AuthenticateEventArgs e)
    {
        usernameTyped = this.lgAccount.UserName;
        passwordTyped = this.lgAccount.Password;

        e.Authenticated = false;

        string tempString = csLib.isValidString("usernameTyped", usernameTyped, true, false, true);
        if (!string.IsNullOrEmpty(tempString))
        {
            phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP(tempString)));
            return;
        }

        int accountCount = dtLib.getRowCount("TableAccount", false);
        if (accountCount > 0)
        {
            try
            {
                if (dtLib.checkStrRecordExist("id", "TableAccount", usernameTyped) == 1)
                {
                    account = account.accountGet(usernameTyped);
                    if (string.IsNullOrEmpty(account.id))
                    {
                        phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP("Không lấy được thông tin của tài khoản <b>" + usernameTyped + "</b>.")));
                        return;
                    }
                    else if (account.password != passwordTyped)
                    {
                        phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP("Mật khẩu không đúng cho tài khoản <b>" + usernameTyped + "</b>.")));
                        return;
                    }
                    else if (account.state != 1)
                    {
                        phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP("Tài khoản <b>" + usernameTyped + "</b> hiện không khả dụng.")));
                        return;
                    }
                    else
                    {
                        e.Authenticated = true;
                    }
                    
                }
                else
                {
                    phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP("Tài khoản <b>" + usernameTyped + "</b> chưa được đăng ký.")));
                    return;
                }
            }
            catch (Exception f)
            {
                csLib.toErrControl(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, f.Message, "Trang chủ", "Default.aspx");
                return;
            }
        }
    }

    protected void login_LoggedIn(object sender, EventArgs e)
    {
        Session["account_isLoggedIn"] = "Y";
        Session["account_userId"] = account.id;
        Response.BufferOutput = true;
        Response.Redirect("Administrator.aspx");
    }

    protected void lbLogout_Click(object sender, EventArgs e)
    {
        Session["account_isLoggedIn"] = null;
        Session["account_userId"] = null;
        Response.BufferOutput = true;
        Response.Redirect("Login.aspx");
    }

    protected void login_LoginError(object sender, EventArgs e)
    { }
}