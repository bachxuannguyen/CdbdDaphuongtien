<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="MediaEdit.aspx.cs" Inherits="MediaEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">

<div style="display:inline-block; width:100%; margin:5px;">
    <asp:Label runat="server" ID="lblTitle" CssClass="lblTitleAdm" Text="Quản lý media"></asp:Label>
    <br />
    <a href="MediaUpload.aspx">[Tải media lên]</a>
    <a href="Administrator.aspx">[Trang quản trị]</a>
    <a href="Default.aspx">[Trang chủ]</a>
    <asp:PlaceHolder runat="server" ID="phMessage"></asp:PlaceHolder>
</div>
<div class="regularTwo">
    <div class="regularOne">
        <asp:Button runat="server" ID="btOk" Text="Chấp nhận" OnClick="btOk_Click" />
        <asp:Button runat="server" ID="btCancel" Text="Hủy bỏ" OnClick="btCancel_Click" />  
    </div>
    <div class="regularOne">
        <asp:Label runat="server" style="font-weight:bold; color:#ad3636;" ID="lblMediaInfo"></asp:Label>
    </div>
    <br />
    <div class="regularOne">
        <asp:Label runat="server" ID="lblMediaName" style="font-weight:bold;" Text="Tên media: "></asp:Label>
        <br />
        <asp:TextBox runat="server" Width="100%" ID="tbMediaName"></asp:TextBox>
    </div>
    <div class="regularOne">
        <asp:Label runat="server" ID="lblMediaState" style="font-weight:bold;" Text="Trạng thái: "></asp:Label>
        <br />
        <asp:DropDownList runat="server" ID="ddlMediaState">
            <asp:ListItem Value="1" Text="Khả dụng"></asp:ListItem>
            <asp:ListItem Value="0" Text="Không khả dụng"></asp:ListItem>
        </asp:DropDownList>
    </div>
    <br />
    <div class="regularOne">
        <asp:Label runat="server" ID="lblMediaDesc" style="font-weight:bold;" Text="Mô tả: "></asp:Label>
        <br />
        <asp:TextBox runat="server"  Width="100%" ID="tbMediaDesc"></asp:TextBox>
    </div>
    <div class="regularOne" style="vertical-align:middle;">
        <asp:CheckBox runat="server" ID="chkIsInfographic" Enabled="false" Text="Infographic" />
    </div>
    <br />
    <div class="regularOne">
        <asp:Label runat="server" style="font-weight:bold;" ID="lblMediaTag" Text="Tag: "></asp:Label>
        <asp:PlaceHolder runat="server" ID="phTag"></asp:PlaceHolder>
    </div>
    <br />
    <div class="regularOne">
        <asp:Label runat="server" ID="lblMediaThumnail" style="font-weight:bold;" Text="Xem trước: "></asp:Label>
        <br />
        <asp:PlaceHolder runat="server" ID="phMediaThumnail"></asp:PlaceHolder>
    </div>
</div>

</asp:Content>

