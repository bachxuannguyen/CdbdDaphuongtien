<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="SearchAlbum.aspx.cs" Inherits="SearchAlbum" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<script type="text/javascript">
<!--
    function btChoose_click() {
        var link = prompt('Nhập số trang: ');
        if (link == "" || link == null)
            return false;
        else {
            if (is_natural(link)) {
                if (parseInt(link) > 0 && parseInt(link) <= parseInt(document.getElementById('<%=hfPageCount.ClientID %>').value)) {
                    document.getElementById('<%=hfPageCurrent.ClientID %>').value = link;
                    return true;
                }
                else {
                    alert('Số thứ tự trang không đúng.');
                    return false;
                }
            }
            else {
                alert('Giá trị nhập vào không hợp lệ.');
                return false;
            }
        }
    }
    function is_natural(s) {
        var n = parseInt(s, 10);
        return n >= 0 && n.toString() === s;
    }
//-->
</script>

<asp:HiddenField runat="server" ID="hfPageCurrent" />
<asp:HiddenField runat="server" ID="hfPageCount" />

<div style="display:inline-block; width:100%; margin:10px 10px 5px 10px;">
    <asp:Label runat="server" ID="lblTitle" CssClass="lblTitleA" Text="DANH MỤC ALBUM"></asp:Label>
    &nbsp;
    <asp:Label runat="server" ID="lblSearchResult"></asp:Label>
</div>
<div style="display:inline-block; width:100%; margin:5px 10px 10px 10px;">
    <asp:TextBox runat="server" ID="tbKeyword" Width="30%" Visible="true"></asp:TextBox>
    <asp:DropDownList runat="server" ID="ddlKeywordType" Visible="true">
        <asp:ListItem Value="1" Text="Tên album"></asp:ListItem>
        <asp:ListItem Value="2" Text="Mô tả album"></asp:ListItem>
    </asp:DropDownList>
    <asp:Button runat="server" ID="btSearch" Text="Tìm kiếm" OnClick="btSearch_Click" />
    <asp:PlaceHolder runat="server" ID="phMessage"></asp:PlaceHolder>
</div>
<div id="divSA" class="regularTwo">
    <div style="display:inline-block; width:100%;">
        <asp:PlaceHolder runat="server" ID="phSearchResult"></asp:PlaceHolder>
    </div>
    <div style="display:inline-block; width:100%; text-align:right;">
        <asp:Label runat="server" ID="lblSearchResult2"></asp:Label>
        <asp:Button runat="server" ID="btPrevious" Text="Trang trước" Visible="false" OnClick="btPrevious_Click" />
        <asp:Button runat="server" ID="btNext" Text="Trang sau" Visible="false" OnClick="btNext_Click" />
        <asp:Button runat="server" ID="btChoose" Text="Chọn trang" Visible="false" OnClientClick="return btChoose_click()" OnClick="btChoose_Click" />
    </div>
</div>

</asp:Content>

