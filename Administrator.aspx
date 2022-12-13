<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Administrator.aspx.cs" Inherits="Administrator" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<div id="divAdm" class="rowAdm">
    <img src="images/config.png" style="width:150px;" />
    <br />
    <asp:LinkButton runat="server" ID="lbMediaUpload" style="font-weight:bold;" Text="[Media upload]" OnClick="lbMediaUpload_Click"></asp:LinkButton>
    &nbsp;
    <asp:LinkButton runat="server" ID="lbAlbumCreate" style="font-weight:bold;" Text="[Tạo mới album]" OnClick="lbAlbumCreate_Click"></asp:LinkButton>
    &nbsp;
    <asp:LinkButton runat="server" ID="lbTagMng" style="font-weight:bold;" Text="[Quản lý tag]" OnClick="lbTagMng_Click"></asp:LinkButton>
    &nbsp;
    <asp:LinkButton runat="server" ID="lbAccountMng" style="font-weight:bold;" Text="[Quản lý tài khoản]" OnClick="lbAccountMng_Click"></asp:LinkButton>
    &nbsp;
    <asp:LinkButton runat="server" ID="lbLogout" style="font-weight:bold;" Text="[Đăng xuất]" OnClick="lbLogout_Click"></asp:LinkButton>
</div>

</asp:Content>
