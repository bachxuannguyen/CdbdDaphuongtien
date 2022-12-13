<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="AccountCreate.aspx.cs" Inherits="AccountCreate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">

<div style="display:inline-block; width:100%; margin:5px;">
    <asp:Label runat="server" ID="lblTitle" CssClass="lblTitleAdm" Text="Quản lý tài khoản"></asp:Label>
    <br />
    <a href="AccountCreate.aspx">[Tạo tài khoản mới]</a>
    <a href="Administrator.aspx">[Trang quản trị]</a>
    <a href="Default.aspx">[Trang chủ]</a>
    <asp:PlaceHolder runat="server" ID="phMessage"></asp:PlaceHolder>
</div>
<div class="regularTwo">
    <div style="width:100%; display:inline-block;">
        <div class="regularOne">
            <asp:Label runat="server" ID="lblAccountId" style="font-weight:bold;" Text="Mã tài khoản:"></asp:Label>
            <br />
            <asp:TextBox runat="server" ID="tbAccountId" Width="100%"></asp:TextBox>
        </div>
        <div class="regularOne">
            <asp:Label runat="server" ID="lblAccountName" style="font-weight:bold;" Text="Tên người dùng:"></asp:Label>
            <br />
            <asp:TextBox runat="server" ID="tbAccountName" Width="100%"></asp:TextBox>
        </div>
        <div class="regularOne">
            <asp:Label runat="server" ID="lblPassword" style="font-weight:bold;" Text="Mật khẩu:"></asp:Label>
            <br />
            <asp:TextBox runat="server" ID="tbPassword1" TextMode="Password" Width="100%"></asp:TextBox>
        </div>
        <div class="regularOne">
            <asp:Label runat="server" ID="lblPassword2" style="font-weight:bold;" Text="Mật khẩu nhập lại:"></asp:Label>
            <br />
            <asp:TextBox runat="server" ID="tbPassword2" TextMode="Password" Width="100%"></asp:TextBox>
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
            <asp:LinkButton runat="server" ID="lbAccountDelete" Text="[Xóa tài khoản này]" Visible="false" OnClick="lbAccountDelete_Click"></asp:LinkButton>
            <asp:Label runat="server" ID="lblDeleteMsg" Text="Xác nhận xóa tài khoản này?" Visible="false"></asp:Label>
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
            <asp:PlaceHolder runat="server" ID="phAccountList"></asp:PlaceHolder>
        </div>
    </div>
</div>

</asp:Content>

