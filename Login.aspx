<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<div style="display:inline-block; width:100%;">
    <asp:PlaceHolder runat="server" ID="phMessage"></asp:PlaceHolder>
    <asp:Login ID="lgAccount" runat="server" onauthenticate="login_Authenticate" 
        onloggedin="login_LoggedIn" onloginerror="login_LoginError" 
        FailureText="" 
        Font-Names="Verdana" Font-Size="Small" LoginButtonText="Chấp nhận" 
        PasswordLabelText="Mật khẩu:" PasswordRequiredErrorMessage="Nhập mật khẩu." 
        RememberMeText="Lưu thông tin đăng nhập" DisplayRememberMe="false" TitleText="" 
        UserNameLabelText="Tài khoản:" 
        UserNameRequiredErrorMessage="Nhập tên người dùng." InstructionText="" 
        BorderStyle="Solid" BorderWidth="0px" Width="320px">
        <LabelStyle CssClass="thongke_danhsach" />
        <CheckBoxStyle Font-Bold="False" Font-Italic="True" ForeColor="#333333" />
        <InstructionTextStyle Font-Italic="True" ForeColor="White" />
        <FailureTextStyle Font-Names="Verdana" Font-Size="Small" />
        <LoginButtonStyle CssClass="bt_login" />
        <TextBoxStyle />
        <TitleTextStyle Font-Bold="True" Font-Size="Large" />
    </asp:Login>
    <asp:Label runat="server" ID="lblLoggedIn" Visible="false"></asp:Label>               
    <asp:LinkButton ID="lbLogout" runat="server" Visible="False" OnClick="lbLogout_Click" >Đăng xuất</asp:LinkButton>
</div>
</asp:Content>
