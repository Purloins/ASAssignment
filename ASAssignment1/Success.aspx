<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Success.aspx.cs" Inherits="ASAssignment1.Success" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            text-align: center;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="auto-style1">
            <div class="auto-style1">
                <br />
            <asp:Image ID="Image1" runat="server" Height="300px" ImageUrl="https://cdn.discordapp.com/attachments/687043714744320027/798910352095182868/Untitled-1.png" />
                <br />
                    <asp:Label ID="Label1" runat="server" Text="User Profile" Font-Names="New Gulim" Font-Size="26pt"></asp:Label>
                <br />
            </div>
        <div>
            <hr style="height:2px;border-width:0;color:gray;background-color:gray;width:800px" />
        </div>
        </div>
        <p class="auto-style1">
            <asp:HyperLink ID="HyperLink1" runat="server" Font-Names="Century Gothic" NavigateUrl="~/Login.aspx">Login</asp:HyperLink>
            &nbsp;<asp:HyperLink ID="HyperLink2" runat="server" Font-Names="Century Gothic" NavigateUrl="~/Registration.aspx">Register</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="hlMyProfile" runat="server" Font-Names="Century Gothic" NavigateUrl="~/Success.aspx">My Profile</asp:HyperLink>
        </p>
        <p class="auto-style1">
                    &nbsp;</p>
        <p class="auto-style1">
                    <asp:Label ID="Label2" runat="server" Text="User ID:" Font-Bold="True" Font-Names="Century Gothic"></asp:Label>
                &nbsp;<asp:Label ID="lbUserID" runat="server" Text="lbUserID" Font-Names="Century Gothic"></asp:Label>
                </p>
        <p class="auto-style1">
            <asp:Label ID="lbTimer" runat="server"></asp:Label>
        </p>
        <p class="auto-style1">
            &nbsp;</p>
        <p class="auto-style1">
                    <asp:Label ID="Label3" runat="server" Text="First Name:" Font-Bold="True" Font-Names="Century Gothic"></asp:Label>
                &nbsp;<asp:Label ID="lbFName" runat="server" Text="lbFName" Font-Names="Century Gothic"></asp:Label>
                </p>
        <p class="auto-style1">
                    <asp:Label ID="Label4" runat="server" Text="Last Name:" Font-Bold="True" Font-Names="Century Gothic"></asp:Label>
                &nbsp;<asp:Label ID="lbLName" runat="server" Text="lbLName" Font-Names="Century Gothic"></asp:Label>
                </p>
        <p class="auto-style1">
                    <asp:Label ID="Label5" runat="server" Text="Date of Birth:" Font-Bold="True" Font-Names="Century Gothic"></asp:Label>
                &nbsp;<asp:Label ID="lbDOB" runat="server" Text="lbDOB" Font-Names="Century Gothic"></asp:Label>
                </p>
        <p class="auto-style1">
                    <asp:Label ID="Label6" runat="server" Text="Joined on: " Font-Bold="True" Font-Names="Century Gothic"></asp:Label>
                    <asp:Label ID="lbDTR" runat="server" Text="lbDTR" Font-Names="Century Gothic"></asp:Label>
                </p>
        <p class="auto-style1">
            &nbsp;</p>
        <p class="auto-style1">
                    <asp:Label ID="lbCcNo" runat="server" Text="Credit Card No." Font-Bold="True" Font-Names="Century Gothic"></asp:Label>
                &nbsp;<asp:Label ID="lbCc" runat="server" Text="lbCc" Font-Names="Century Gothic"></asp:Label>
                </p>
        <p class="auto-style1">
                    <asp:Label ID="ctl1" runat="server" Text="CVV:" Font-Bold="True" Font-Names="Century Gothic"></asp:Label>
                &nbsp;<asp:Label ID="lbCcCVV" runat="server" Text="lbCcCVV" Font-Names="Century Gothic"></asp:Label>
                </p>
    <p class="auto-style1">
        <asp:Button ID="btnLogout" runat="server" OnClick="btnLogout_Click" Text="Logout" Width="257px" />
        </p>
        <p class="auto-style1">
        <asp:Button ID="btnChangePwd" runat="server" OnClick="btnChangePwd_Click" Text="Change Password" Width="257px" />
        </p>
    </form>
    </body>
</html>
