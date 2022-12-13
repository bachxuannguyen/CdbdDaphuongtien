<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="MediaUpload.aspx.cs" Inherits="MediaUpload" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">

<div style="display:inline-block; width:100%; margin:5px;">
    <asp:Label runat="server" ID="lblTitle" CssClass="lblTitleAdm" Text="MEDIA | Tải lên"></asp:Label>
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
    <br />
    <div class="regularOne">
        <asp:Label runat="server" ID="lblMediaUpload" Text="Tải lên: "></asp:Label>
        <asp:FileUpload runat="server" ID="fuMedia" AllowMultiple="true" />  
        <asp:Button runat="server" ID="btMediaUpload" Text="Upload" OnClick="btMediaUpload_Click" />
    </div>
    <br />
    <div class="regularOne">
        <asp:PlaceHolder runat="server" ID="phUploadedMedia"></asp:PlaceHolder>
    </div>
</div>
     
</asp:Content>

