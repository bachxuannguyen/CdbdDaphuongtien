<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="AlbumView.aspx.cs" Inherits="AlbumView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">

<script>(function (d, s, id) {
    var js, fjs = d.getElementsByTagName(s)[0];
    if (d.getElementById(id)) return;
    js = d.createElement(s); js.id = id;
    js.src = "https://connect.facebook.net/en_US/sdk.js#xfbml=1&version=v3.0";
    fjs.parentNode.insertBefore(js, fjs);
}(document, 'script', 'facebook-jssdk'));</script>

<div style="display:inline-block; width:100%; margin:5px;">
    <asp:Label runat="server" ID="lblTitle" CssClass="lblTitleA" Text=""></asp:Label>
    <asp:PlaceHolder runat="server" ID="phAlbumInfo"></asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="phMessage"></asp:PlaceHolder>
</div>
<div id="divMV" class="regularTwo">
    <div id="divMPreview" style="display:inline-block; width:99%; margin:5px;">
        <asp:PlaceHolder runat="server" ID="phAlbumPreview"></asp:PlaceHolder>
    </div>
</div>

</asp:Content>

