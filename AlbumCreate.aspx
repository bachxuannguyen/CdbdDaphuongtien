<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="AlbumCreate.aspx.cs" Inherits="AlbumCreate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">

<div style="display:inline-block; width:100%; margin:5px;">
    <asp:Label runat="server" ID="lblTitle" CssClass="lblTitleAdm" Text="Album | Tạo mới"></asp:Label>
    <br />
    <a href="AlbumCreate.aspx">[Tạo album mới]</a>
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
        <asp:Label runat="server" ID="lblAlbumInfo" style="font-weight:bold; color:#2c629d;"></asp:Label>
    </div>
    <br />
    <div class="regularOne">
        <asp:Label runat="server" ID="lblAlbumName" style="font-weight:bold;" Text="Tên album: "></asp:Label>
        <br />
        <asp:TextBox runat="server" ID="tbAlbumName" Width="100%"></asp:TextBox>
    </div>
    <br />
    <div class="regularOne">
        <asp:Label runat="server" ID="lblAlbumDesc" style="font-weight:bold;" Text="Mô tả: "></asp:Label>
        <br />
        <asp:TextBox runat="server" ID="tbAlbumDesc" Width="100%"></asp:TextBox>
    </div>
    <br />
    <div class="regularOne">
        <asp:Label runat="server" ID="lblAlbumState" style="font-weight:bold;" Text="Trạng thái: "></asp:Label>
        <br />
        <asp:DropDownList runat="server" ID="ddlAlbumState">
            <asp:ListItem Value="1" Text="Khả dụng"></asp:ListItem>
            <asp:ListItem Value="0" Text="Không khả dụng"></asp:ListItem>
        </asp:DropDownList>
    </div>
    <br />
    <div class="regularOne">
        <asp:Label runat="server" ID="lblAlbumTag" style="font-weight:bold;" Text="Tag: "></asp:Label>
        <asp:PlaceHolder runat="server" ID="phTag"></asp:PlaceHolder>
    </div>
    <br />
    <div class="regularOne">
        <asp:Label runat="server" ID="lblAlbumMedia" style="font-weight:bold;" Text="Media: "></asp:Label>
    </div>
    <br />
    <div class="regularOne">
        <asp:FileUpload runat="server" ID="fuMedia" AllowMultiple="true" />  
    <asp:Button runat="server" ID="btMediaUpload" Text="Upload" OnClick="btMediaUpload_Click" />
    </div>
    <br />
    <div class="regularOne">
        <asp:PlaceHolder runat="server" ID="phMediaUploaded"></asp:PlaceHolder>
        <br />
        <asp:PlaceHolder runat="server" ID="phMediaCurrent"></asp:PlaceHolder>
    </div>
</div>

</asp:Content>

