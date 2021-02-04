<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePass.aspx.cs" Inherits="ASAssignment1.ChangePass" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        fieldset {
            margin: auto;
        }
        .auto-style1 {
            text-align: center;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="auto-style1">
                        <asp:Image ID="Image1" runat="server" Height="300px" ImageUrl="https://cdn.discordapp.com/attachments/687043714744320027/798910352095182868/Untitled-1.png" />
                    <br />
                    <asp:Label ID="lbTitle" runat="server" Text="Change Password" Font-Names="New Gulim" Font-Size="30pt"></asp:Label>
                        <br />
                        </div>
            <hr style="height:2px;border-width:0;color:gray;background-color:gray;width:800px"/>
                        <div class="auto-style1">
            <br />
            <asp:HyperLink ID="HyperLink1" runat="server" Font-Names="Century Gothic" NavigateUrl="~/Login.aspx">Login</asp:HyperLink>
&nbsp;<asp:HyperLink ID="HyperLink2" runat="server" Font-Names="Century Gothic" NavigateUrl="~/Registration.aspx">Register</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="hlMyProfile" runat="server" Font-Names="Century Gothic" NavigateUrl="~/Success.aspx">My Profile</asp:HyperLink>
                            <br />
            <br />
                            <asp:Label ID="lbErrorMsg" runat="server"></asp:Label>
                            <br />
            <br />
            <fieldset style="width:900px;">

                <div class="auto-style1">

                <br />
                    <asp:Label ID="Label6" runat="server" Text="E-mail: "></asp:Label>
                    <asp:TextBox ID="tbEmail" runat="server"></asp:TextBox>
                    <br />
                    <br />
                <asp:Label ID="Label4" runat="server" Text="Old Password: "></asp:Label>
                <asp:TextBox ID="tbOldPass" runat="server" TextMode="Password"></asp:TextBox>
                <br />
                <br />
                <asp:Label ID="Label5" runat="server" Text="New Password: "></asp:Label>
                <asp:TextBox ID="tbNewPass" runat="server" TextMode="Password"></asp:TextBox>
                <br />
                <br />
                <asp:Button ID="btnChangePass" runat="server" Text="Change Password" OnClick="btnChangePass_Click" />
                    <br />
                <br />

                </div>

            </fieldset></div>
    </form>
</body>
</html>
