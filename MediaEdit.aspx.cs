using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using cdbd_daphuongtien.App_Code;

public partial class MediaEdit : System.Web.UI.Page
{
    CsLib csLib = new CsLib();
    DtLib dtLib = new DtLib();
    static Media media = new Media();
    Tag tag = new Tag();
    MediaType mediaType = new MediaType();
    View view = new View();

    static string[] allTagId = { "" };
    static bool[] allCheckedTagId = { false };
    static string qsMediaId = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        this.Title = "Chỉnh sửa media - " + csLib.varSiteName;

        if (!csLib.isLoggedIn())
        {
            csLib.toErrControl(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Đăng nhập quản trị để thực hiện thao tác này.", "Đăng nhập", "Login.aspx");
            return;
        }

        if (!Page.IsPostBack)
        {
            qsMediaId = csLib.decodeString(Request.QueryString["mediaid"]);
            if (!string.IsNullOrEmpty(csLib.isValidString("Chuỗi truy vấn - Mã media", qsMediaId, false, false, true)))
            {
                csLib.toErrControl(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Mã media này không hợp lệ.", "Trang quản trị", "Administrator.aspx");
                return;
            }
            if (!int.TryParse(qsMediaId, out media.id))
            {
                csLib.toErrControl(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Mã media này không hợp lệ.", "Trang quản trị", "Administrator.aspx");
                return;
            }

            media = media.mediaGet(media.id);
            if (media.id == 0)
            {
                csLib.toErrControl(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Không đọc được thông tin của media này.", "Trang quản trị", "Administrator.aspx");
                return;
            }
            lblMediaInfo.Text += "[ID: " + media.id.ToString() + "]";
            lblMediaInfo.Text += "[Type: " + dtLib.getData_IntToStr("name", "TableMediaType", "id", media.type) +"]";
            lblMediaInfo.Text += "[Album: " + media.album.ToString() + "]";
            lblMediaInfo.Text += "[Author: " + media.author.ToString() + "]";
            lblMediaInfo.Text += "[Datetime: " + media.datetime.ToString() + "]";
            lblMediaInfo.Text += "[URL: " + media.url.ToString() + "]";
            tbMediaDesc.Text = media.description;
            tbMediaName.Text = media.name;
            ddlMediaState.SelectedValue = media.state.ToString();

            if (media.type == 1 || media.type == 2)
            {
                chkIsInfographic.Enabled = true;
                if (media.type == 1)
                    chkIsInfographic.Checked = false;
                else
                    chkIsInfographic.Checked = true;
            }
        }

        phMediaThumnail.Controls.Clear();
        phMediaThumnail.Controls.Add(new LiteralControl(view.viewThumnail(media.type, media.url, media.name, 3)));
        buildTagPlaceHolder();
    }

    protected void btOk_Click(object sender, EventArgs e)
    {
        media.name = tbMediaName.Text;
        media.description = tbMediaDesc.Text;
        media.state = Convert.ToInt32(ddlMediaState.SelectedValue);
        media.tag = "";
        if (media.type == 1 || media.type == 2)
        {
            if (chkIsInfographic.Checked)
                media.type = 2;
            else
                media.type = 1;
        }
        for (int i = 0; i < allCheckedTagId.Length; i++)
            if (allCheckedTagId[i])
                media.tag += allTagId[i] + ",";
        
        string tmpStr = media.isValidMedia(media);
        if (!string.IsNullOrEmpty(tmpStr))
        {
            phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP(tmpStr)));
            return;
        }
        else
        {
            if (!media.mediaUpdate(media))
            {
                phMessage.Controls.Add(new LiteralControl(view.toFailHtmlP("Không cập nhật được thông tin media vào hệ thống.")));
                return;
            }
            phMessage.Controls.Add(new LiteralControl(view.toSuccessHtmlP("Thao tác đã được thực hiện.")));
            disableControls();
        }
    }

    protected void btCancel_Click(object sender, EventArgs e)
    {
        Response.BufferOutput = true;
        Response.Redirect("Administrator.aspx", false);
    }

    protected void buildTagPlaceHolder()
    {
        phTag.Controls.Clear();

        string[] tempArray = tag.getAllTagId(false);
        if (tempArray.Length == 0)
            return;
        int tagCount = tempArray.Length;
        Array.Resize(ref allTagId, tagCount);
        Array.Resize(ref allCheckedTagId, tagCount);
        allTagId = tempArray;
        for (int k = 0; k < allCheckedTagId.Length; k++)
            allCheckedTagId[k] = false;

        string[] currentMediaTag = csLib.toStrArray(media.tag);

        for (int i = 0; i < tagCount; i++)
        {
            CheckBox checkBox = new CheckBox();
            checkBox.ID = allTagId[i];
            checkBox.Text = dtLib.getData_StrToStr("name", "TableTag", "id", allTagId[i]);
            if (currentMediaTag.Length > 0)
                for (int j = 0; j < currentMediaTag.Length; j++)
                    if (allTagId[i] == currentMediaTag[j])
                    {
                        checkBox.Checked = true;
                        allCheckedTagId[i] = true;
                    }
            checkBox.Attributes["id"] = i.ToString();
            checkBox.CheckedChanged += new EventHandler(tagCbChanged);
            this.Controls.Add(checkBox);
            phTag.Controls.Add(checkBox);
        }
    }

    protected void tagCbChanged(object sender, EventArgs e)
    {
        CheckBox checkBox = (CheckBox)sender;
        int index = Convert.ToInt32(checkBox.Attributes["id"].ToString());
        allCheckedTagId[index] = !allCheckedTagId[index];
    }

    protected void disableControls()
    {
        tbMediaName.Enabled = false;
        tbMediaDesc.Enabled = false;
        ddlMediaState.Enabled = false;
        btOk.Enabled = false;
        btCancel.Enabled = false;
    }
}