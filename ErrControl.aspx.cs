using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using cdbd_daphuongtien.App_Code;

public partial class ErrControl : System.Web.UI.Page
{
    View view = new View();
    CsLib csLib = new CsLib();

    protected void Page_Load(object sender, EventArgs e)
    {
        this.Title = "Thông báo - " + csLib.varSiteName;

        if (!string.IsNullOrEmpty(view.viewErrMsg()))
            phErrMessage.Controls.Add(new LiteralControl(view.viewErrMsg()));
        else
            phErrMessage.Controls.Add(new LiteralControl("<p><a href=\"Default.aspx\">Trang chủ</a></p>"));

        if (csLib.isRootLogin())
        {
            phErrTracer.Controls.Add(new LiteralControl("<hr><b>Theo dấu lỗi:</b><br />"));
            if (!string.IsNullOrEmpty(view.viewErrTracer()))
                phErrTracer.Controls.Add(new LiteralControl(view.viewErrTracer()));
            else
                phErrTracer.Controls.Add(new LiteralControl("[Không có gì để hiển thị]"));
        }
    }
}