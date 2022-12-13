<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content runat="server" ID="Content1" ContentPlaceHolderID="MainContent">
<div id="divDfTbSearch" style="text-align:center; padding:15px 5px 10px 5px;">
    <asp:TextBox runat="server" CssClass="tbDefault" ID="tbSearch"></asp:TextBox>
    <asp:DropDownList runat="server" ID="ddlSearch" CssClass="ddlDefault">
        <asp:ListItem Value="1" Text="Media" Selected="True"></asp:ListItem>
        <asp:ListItem Value="2" Text="Album"></asp:ListItem>
    </asp:DropDownList>
</div>
<div id="divDfBtSearch" style="text-align:center; padding-bottom:10px;">
    <asp:Button runat="server" ID="btSearch" CssClass="btDefault" Text="Tìm kiếm" OnClick="btSearch_Click" />
</div>
<div id="divMediaStat" style="text-align:center;">
    <asp:LinkButton runat="server" ID="lbMediaImg" OnClick="lbMediaImg_Click"></asp:LinkButton>
    <asp:LinkButton runat="server" ID="lbMediaInf" OnClick="lbMediaInf_Click"></asp:LinkButton>
    <asp:LinkButton runat="server" ID="lbMediaVid" OnClick="lbMediaVid_Click"></asp:LinkButton>
    <asp:LinkButton runat="server" ID="lbMediaSou" OnClick="lbMediaSou_Click"></asp:LinkButton>
    <asp:LinkButton runat="server" ID="lbMediaAni" OnClick="lbMediaAni_Click"></asp:LinkButton>
</div>
<div id="divDfDesc" class="rowDfDesc">
    <div id="divDfAlbum" class="columnDfAlbum">
        <div style="display:inline-block; vertical-align:top; text-align:center;">
            <img src="images/icoalbum.png" alt="" style="max-width:80px;" />
            <br />
            <asp:LinkButton runat="server" ID="lbAlbumTot" CssClass="hrefDefault" OnClick="lbAlbumTot_Click"></asp:LinkButton>
            <br />
            Là một hình thức tổ chức lưu trữ các tập tin đa phương tiện, ghi lại những khoảnh khắc đáng nhớ theo dòng các sự kiện nổi bật trong hoạt động của nhà trường.
        </div>
    </div>
    <div id="divDfMedia" class="columnDfMedia">
        <div style="display:inline-block; vertical-align:top; text-align:center;">
            <img src="images/icomedia.png" alt="" style="max-width:80px;" />
            <br />
            <asp:LinkButton runat="server" ID="lbMediaTot" CssClass="hrefDefault" OnClick="lbMediaTot_Click"></asp:LinkButton>
            <br />
            Hình ảnh, video, âm thanh, hoạt họa... là các hình thức lưu trữ đa phương tiện, mang đến cho người dùng cách tiếp cận thông tin nhanh chóng và dễ dàng ghi nhớ.
        </div>   
    </div>
    <div id="div1" class="columnDfTag">
        <div style="display:inline-block; vertical-align:top; text-align:center;">
            <img src="images/icotag.png" alt="" style="max-width:80px;" />
            <br />
            <asp:LinkButton runat="server" ID="lbTagTot" CssClass="hrefDefault" OnClick="lbTagTot_Click"></asp:LinkButton>
            <br />
            Việc gắn nhãn cho album hay media là cách tiếp cận hiệu quả cho việc quản lý các đối tượng dựa trên thuộc tính nhằm tối ưu việc tìm kiếm và truy xuất dữ liệu.
        </div>   
    </div>
</div>
</asp:Content>