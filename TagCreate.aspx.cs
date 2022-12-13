using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using cdbd_daphuongtien.App_Code;

public partial class TagCreate : System.Web.UI.Page
{
    CsLib csLib = new CsLib();
    DtLib dtLib = new DtLib();
    static Tag tag = new Tag();
    View view = new View();

    static string qsOption = "";
    static string qsTagId = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        this.Title = "Tạo tag mới - " + csLib.varSiteName;

        if (!csLib.isLoggedIn())
        {
            csLib.toErrControl(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Đăng nhập quản trị để thực hiện thao tác này.", "Đăng nhập", "Login.aspx");
            return;
        }

        showCurrentTags();

        if (!Page.IsPostBack)
        {
            ddlPTag.Items.Clear();
            ListItem baseListItem = new ListItem();
            baseListItem.Value = "";
            baseListItem.Text = "Chọn";
            ddlPTag.Items.Add(baseListItem);
            string[] ptagId = tag.getAllPTagId(true);
            if (ptagId.Length > 0)
            {
                if (!string.IsNullOrEmpty(ptagId[0]))
                {
                    for (int i = 0; i < ptagId.Length; i++)
                    {
                        ListItem li = new ListItem();
                        li.Value = ptagId[i].ToString();
                        li.Text = ptagId[i].ToString();
                        ddlPTag.Items.Add(li);
                    }
                }
            }

            qsOption = csLib.decodeString(Request.QueryString["option"]);
            qsTagId = csLib.decodeString(Request.QueryString["tagid"]);
            if (!string.IsNullOrEmpty(csLib.isValidString("qsOption", qsOption, false, true, true)))
                qsOption = "create";
            if (qsOption != "create" && qsOption != "edit")
                qsOption = "create";
            if (qsOption == "edit")
            {
                if (!string.IsNullOrEmpty(csLib.isValidString("qsTagId", qsTagId, false, false, true)))
                {
                    csLib.toErrControl(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Mã tag này không hợp lệ", "Trang quản trị", "Administrator.aspx");
                    return;
                }
            }

            if (qsOption == "edit")
            {
                this.Title = "Chỉnh sửa tag - " + csLib.varSiteName;
                lblTitle.Text = "Tag | Chỉnh sửa";

                tbTagId.Enabled = false;
                tag = tag.tagGet(qsTagId);
                if (string.IsNullOrEmpty(tag.id))
                {
                    csLib.toErrControl(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Không tìm thấy mã tag này trong cơ sở dữ liệu.", "Trang quản trị", "Administrator.aspx");
                    return;
                }
                tbTagId.Text = tag.id;
                tbTagName.Text = tag.name;
                if (!string.IsNullOrEmpty(tag.ptag))
                {
                    try
                    {
                        ddlPTag.SelectedValue = tag.ptag;
                    }
                    catch { }
                }
                if (tag.state == 1)
                    cbIsEnable.Checked = true;
                else
                    cbIsEnable.Checked = false;

                lbTagDelete.Visible = true;
            }
        }
    }

    protected void varInit()
    { }

    protected void showCurrentTags()
    {
        phTagList.Controls.Clear();
        string[] allPTagId = tag.getAllPTagId(true);
        string nAStr = "";
        if (allPTagId.Length > 0)
        {
            phTagList.Controls.Add(new LiteralControl("<hr>"));
            phTagList.Controls.Add(new LiteralControl("<b>Tag khả dụng:</b><br />"));
            for (int i = 0; i < allPTagId.Length; i++)
            {
                nAStr = "";
                if (dtLib.getData_StrToInt("state", "TableTag", "id", allPTagId[i]) != 1)
                    nAStr = "-" + csLib.varStrDisabled;
                LinkButton linkButton = new LinkButton();
                linkButton.ID = allPTagId[i];
                linkButton.Text = "<b>[" + allPTagId[i] + nAStr + "]</b>";
                linkButton.CommandArgument = allPTagId[i];
                linkButton.Click += new EventHandler(editCurrentTag);
                this.Controls.Add(linkButton);
                phTagList.Controls.Add(linkButton);
                string[] myCTag = tag.getMyCTagId(allPTagId[i], false);
                if (myCTag.Length > 0)
                {
                    for (int j = 0; j < myCTag.Length; j++)
                    {
                        nAStr = "";
                        if (dtLib.getData_StrToInt("state", "TableTag", "id", myCTag[j]) != 1)
                            nAStr = "-" + csLib.varStrDisabled;
                        LinkButton linkButton2 = new LinkButton();
                        linkButton2.ID = myCTag[j];
                        linkButton2.Text = "[" + myCTag[j] + nAStr + "]";
                        linkButton2.CommandArgument = myCTag[j];
                        linkButton2.Click += new EventHandler(editCurrentTag);
                        this.Controls.Add(linkButton2);
                        phTagList.Controls.Add(linkButton2);
                    }
                }
                phTagList.Controls.Add(new LiteralControl("<br />"));
            }
        }
    }

    protected void btOk_Click(object sender, EventArgs e)
    {
        tag.id = tbTagId.Text;
        tag.name = tbTagName.Text;
        if (cbIsEnable.Checked)
            tag.state = 1;
        else
            tag.state = 0;
        tag.ptag = ddlPTag.SelectedValue;
        tag.ctag = "";

        try
        {
            if (!string.IsNullOrEmpty(ddlPTag.SelectedValue))
            {
                Tag pTag = new Tag();
                pTag.id = ddlPTag.SelectedValue;
                pTag = pTag.tagGet(pTag.id);
                if (!pTag.ctag.Contains(tag.id))
                    pTag.ctag += tag.id + ",";
                pTag.tagUpdate(pTag);
            }
        }
        catch
        { }

        if (qsOption == "edit")
        {
            if (!string.IsNullOrEmpty(tag.isValidTag(tag, false)))
            {
                phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP(tag.isValidTag(tag, false))));
                return;
            }
            else
            {
                tag.id = qsTagId;
                if (tag.tagUpdate(tag))
                    phMessage.Controls.Add(new LiteralControl(view.toSuccessHtmlP("Tag <b>[" + tag.id + "]</b> đã được chỉnh sửa.")));
                else
                {
                    csLib.toErrControl(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Thao tác không được thực hiện.", "Trang quản trị", "Administrator.aspx");
                    return;
                }
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(tag.isValidTag(tag, true)))
            {
                phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP(tag.isValidTag(tag, true))));
                return;
            }
            else
            {
                if (tag.tagInsert(tag))
                    phMessage.Controls.Add(new LiteralControl(view.toSuccessHtmlP("Tag <b>[" + tag.id + "]</b> đã được tạo mới.")));
                else
                {
                    csLib.toErrControl(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Thao tác không được thực hiện.", "Trang quản trị", "Administrator.aspx");
                    return;
                }
            }
        }
        disableControls();
        showCurrentTags();
    }

    protected void editCurrentTag(object sender, EventArgs e)
    {
        LinkButton linkButton = (LinkButton)sender;
        string tagId = linkButton.CommandArgument;
        Response.BufferOutput = true;
        Response.Redirect("TagCreate.aspx?option=" + csLib.encodeString("edit") + "&tagid=" + csLib.encodeString(tagId));
    }

    public void disableControls()
    {
        tbTagId.Enabled = false;
        tbTagName.Enabled = false;
        ddlPTag.Enabled = false;
        btOk.Enabled = false;
        btCancel.Enabled = false;
        cbIsEnable.Enabled = false;
    }

    protected void btCancel_Click(object sender, EventArgs e)
    {
        Response.BufferOutput = true;
        Response.Redirect("Administrator.aspx", false);
    }

    protected void lbDeleteConfirm_Click(object sender, EventArgs e)
    {
        if (!tag.tagDelete(tag.id))
        {
            phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP("Không xóa được tag này.")));
            return;
        }
        phMessage.Controls.Add(new LiteralControl(view.toSuccessHtmlP("Tag này đã được xóa.")));
        disableControls();
    }

    protected void lbDeleteCancel_Click(object sender, EventArgs e)
    {
        lblDeleteMsg.Visible = false;
        lbDeleteConfirm.Visible = false;
        lbDeleteCancel.Visible = false;
    }

    protected void lbTagDelete_Click(object sender, EventArgs e)
    {
        lblDeleteMsg.Visible = true;
        lbDeleteConfirm.Visible = true;
        lbDeleteCancel.Visible = true;
    }
}