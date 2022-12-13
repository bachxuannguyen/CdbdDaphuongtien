using cdbd_daphuongtien.App_Code;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class AccountCreate : System.Web.UI.Page
{
    CsLib csLib = new CsLib();
    DtLib dtLib = new DtLib();
    static Account account = new Account();
    View view = new View();

    static string qsOption = "";
    static string qsAccountId = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        this.Title = "Quản lý tài khoản - " + csLib.varSiteName;

        if (!csLib.isLoggedIn())
        {
            csLib.toErrControl(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Đăng nhập quản trị để thực hiện thao tác này.", "Đăng nhập", "Login.aspx");
            return;
        }

        if (!csLib.isRootLogin())
        {
            csLib.toErrControl(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Đăng nhập tài khoản <b>[" + csLib.varRootAccount + "]</b> để thực hiện thao tác này.", "Đăng nhập", "Login.aspx");
            return;
        }

        if (!Page.IsPostBack)
        {
            qsOption = csLib.decodeString(Request.QueryString["option"]);
            qsAccountId = csLib.decodeString(Request.QueryString["accountid"]);
            if (!string.IsNullOrEmpty(csLib.isValidString("qsOption", qsOption, false, true, true)))
                qsOption = "create";
            if (qsOption != "create" && qsOption != "edit")
                qsOption = "create";
            if (qsOption == "edit")
            {
                if (!string.IsNullOrEmpty(csLib.isValidString("qsAccountId", qsAccountId, false, false, true)))
                {
                    csLib.toErrControl(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Mã tài khoản không hợp lệ.", "Trang quản trị", "Administrator.aspx");
                    return;
                }
            }

            if (qsOption == "edit")
            {
                tbAccountId.Enabled = false;
                account = account.accountGet(qsAccountId);
                if (string.IsNullOrEmpty(account.id))
                {
                    csLib.toErrControl(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Không tìm thấy mã tài khoản này trong cơ sở dữ liệu.", "Trang quản trị", "Administrator.aspx");
                    return;
                }
                tbAccountId.Text = account.id;
                tbAccountName.Text = account.name;
                tbPassword1.Text = account.password;
                tbPassword2.Text = account.password;
                if (account.state == 1)
                    cbIsEnable.Checked = true;
                else
                    cbIsEnable.Checked = false;
                lbAccountDelete.Visible = true;
            }
        }
        showCurrentAccounts();
    }

    protected void varInit()
    { }

    protected void showCurrentAccounts()
    {
        phAccountList.Controls.Clear();
        string[] allAccountId = account.getAllAccountId(true);
        string nAStr = "";
        if (allAccountId.Length > 0)
        {
            phAccountList.Controls.Add(new LiteralControl("<hr>"));
            phAccountList.Controls.Add(new LiteralControl("<b>Danh mục tài khoản:</b><br />"));
            for (int i = 0; i < allAccountId.Length; i++)
            {
                nAStr = "";
                if (dtLib.getData_StrToInt("state", "TableAccount", "id", allAccountId[i]) != 1)
                    nAStr = csLib.varStrDisabled + "-";
                LinkButton linkButton = new LinkButton();
                linkButton.ID = allAccountId[i];
                linkButton.Text = "[" + nAStr + allAccountId[i] + "]";
                linkButton.CommandArgument = allAccountId[i];
                linkButton.Click += new EventHandler(editCurrentAccount);
                this.Controls.Add(linkButton);
                phAccountList.Controls.Add(linkButton);
            }
        }
    }

    protected void editCurrentAccount(object sender, EventArgs e)
    {
        LinkButton linkButton = (LinkButton)sender;
        string tagId = linkButton.CommandArgument;
        Response.BufferOutput = true;
        Response.Redirect("AccountCreate.aspx?option=" + csLib.encodeString("edit") + "&accountid=" + csLib.encodeString(tagId));
    }


    protected void btOk_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(csLib.isValidString("Mã tài khoản", tbAccountId.Text, false, false, true)))
        {
            phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP(csLib.isValidString("Mã tài khoản", tbAccountId.Text, false, false, true))));
            return;
        }
        else
            account.id = tbAccountId.Text;

        if (!string.IsNullOrEmpty(csLib.isValidString("Tên tài khoản", tbAccountName.Text, false, false, true)))
        {
            phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP(csLib.isValidString("Tên tài khoản", tbAccountId.Text, false, false, true))));
            return;
        }
        else
            account.name = tbAccountName.Text;

        if (cbIsEnable.Checked)
            account.state = 1;
        else
            account.state = 0;

        if (!string.IsNullOrEmpty(csLib.isValidString("Mật khẩu", tbPassword1.Text, true, false, true)))
        {
            phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP(csLib.isValidString("Mật khẩu", tbPassword1.Text, true, false, true))));
            return;
        }
        else if (tbPassword1.Text != tbPassword2.Text)
        {
            phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP("Mật khẩu không khớp.")));
            return;
        }
        else
            account.password = tbPassword1.Text;

        if (account.id == csLib.varRootAccount)
        {
            phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP("Không thể thao tác với tài khoản <b>[" + csLib.varRootAccount + "]</b>.")));
            return;
        }

        if (qsOption == "edit")
        {
            if (!string.IsNullOrEmpty(account.isValidAccount(account, false)))
            {
                phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP(account.isValidAccount(account, false))));
                return;
            }
            else
            {
                account.id = qsAccountId;
                if (account.accountUpdate(account))
                    phMessage.Controls.Add(new LiteralControl(view.toSuccessHtmlP("Tài khoản <b>[" + account.id + "]</b> đã được chỉnh sửa.")));
                else
                {
                    csLib.toErrControl(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Thao tác không được thực hiện.", "Trang quản trị", "Administrator.aspx");
                    return;
                }
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(account.isValidAccount(account, true)))
            {
                phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP(account.isValidAccount(account, true))));
                return;
            }
            else
            {
                if (account.accountInsert(account))
                    phMessage.Controls.Add(new LiteralControl(view.toSuccessHtmlP("Tài khoản <b>[" + account.id + "]</b> đã được tạo mới.")));
                else
                {
                    csLib.toErrControl(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Thao tác không được thực hiện.", "Trang quản trị", "Administrator.aspx");
                    return;
                }
            }
        }
        disableControls();
        showCurrentAccounts();
    }

    protected void btCancel_Click(object sender, EventArgs e)
    {
        Response.BufferOutput = true;
        Response.Redirect("Administrator.aspx", false);
    }

    public void disableControls()
    {
        tbAccountId.Enabled = false;
        tbAccountName.Enabled = false;
        tbPassword1.Enabled = false;
        tbPassword2.Enabled = false;
        btOk.Enabled = false;
        btCancel.Enabled = false;
        cbIsEnable.Enabled = false;
    }

    protected void lbToNewAcc_Click(object sender, EventArgs e)
    {
        Response.BufferOutput = true;
        Response.Redirect("AccountCreate.aspx", false);
    }

    protected void lbToAdm_Click(object sender, EventArgs e)
    {
        Response.BufferOutput = true;
        Response.Redirect("Administrator.aspx", false);
    }

    protected void lbToHome_Click(object sender, EventArgs e)
    {
        Response.BufferOutput = true;
        Response.Redirect("Default.aspx", false);
    }

    protected void lbDeleteConfirm_Click(object sender, EventArgs e)
    {
        if (!account.accountDelete(account.id))
        {
            phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP("Không xóa được tài khoản này.")));
            return;
        }
        phMessage.Controls.Add(new LiteralControl(view.toSuccessHtmlP("Tài khoản này đã được xóa.")));
        disableControls();
    }

    protected void lbDeleteCancel_Click(object sender, EventArgs e)
    {
        lblDeleteMsg.Visible = false;
        lbDeleteConfirm.Visible = false;
        lbDeleteCancel.Visible = false;
    }

    protected void lbAccountDelete_Click(object sender, EventArgs e)
    {
        lblDeleteMsg.Visible = true;
        lbDeleteConfirm.Visible = true;
        lbDeleteCancel.Visible = true;
    }
}