<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="TagCreate.aspx.cs" Inherits="TagCreate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">

<div style="display:inline-block; width:100%; margin:5px;">
    <asp:Label runat="server" ID="lblTitle" CssClass="lblTitleAdm" Text="Tag | Tạo mới"></asp:Label>
    <br /> 
    <a href="TagCreate.aspx">[Tạo tag mới]</a>
    <a href="Administrator.aspx">[Trang quản trị]</a>
    <a href="Default.aspx">[Trang chủ]</a>
    <asp:PlaceHolder runat="server" ID="phMessage"></asp:PlaceHolder>
</div>
<div class="regularTwo">
    <div style="width:100%; display:inline-block;">
        <div class="regularOne">
            <asp:Label runat="server" ID="lblTagId" style="font-weight:bold;" Text="Mã tag:"></asp:Label>
            <br />
            <asp:TextBox runat="server" ID="tbTagId" Width="100%"></asp:TextBox>
        </div>
        <div class="regularOne">
            <asp:Label runat="server" ID="lblTagDesc" style="font-weight:bold;" Text="Tên tag:"></asp:Label>
            <br />
            <asp:TextBox runat="server" ID="tbTagName" Width="100%"></asp:TextBox>
        </div>
        <div class="regularOne">
            <asp:Label runat="server" ID="lblPTag" style="font-weight:bold;" Text="Trực thuộc:"></asp:Label>
            <br />
            <asp:DropDownList runat="server" ID="ddlPTag">
            </asp:DropDownList>
        </div>
    </div>
    <div style="width:100%; display:inline-block;">
        <div class="regularOne">
            <asp:Label runat="server" ID="lblState" style="font-weight:bold;" Text="Trạng thái:"></asp:Label>
            <asp:CheckBox runat="server" ID="cbIsEnable" Text="Khả dụng" />
        </div>
    </div>
    <div style="width:100%; display:inline-block;">
        <div class="regularOne">
            <asp:LinkButton runat="server" ID="lbTagDelete" Text="[Xóa tag này]" Visible="false" OnClick="lbTagDelete_Click"></asp:LinkButton>
            <asp:Label runat="server" ID="lblDeleteMsg" Text="Xác nhận xóa tag này?" Visible="false"></asp:Label>
            <asp:LinkButton runat="server" ID="lbDeleteConfirm" Text="[Xóa]" Visible="false" OnClick="lbDeleteConfirm_Click"></asp:LinkButton>
            <asp:LinkButton runat="server" ID="lbDeleteCancel" Text="[Hủy bỏ]" Visible="false" OnClick="lbDeleteCancel_Click"></asp:LinkButton>
        </div>
    </div>
    <div style="width:100%;">
        <div class="regularOne">
            <asp:Button runat="server" ID="btOk" Text="Chấp nhận" OnClick="btOk_Click" />
            <asp:Button runat="server" ID="btCancel" Text="Hủy bỏ" OnClick="btCancel_Click" />
        </div>
    </div>
    <div style="width:100%;">
        <div class="regularOne">
            <asp:PlaceHolder runat="server" ID="phTagList"></asp:PlaceHolder>
        </div>
    </div>
</div>

</asp:Content>

