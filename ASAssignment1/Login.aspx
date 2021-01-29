<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ASAssignment1.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="https://www.google.com/recaptcha/api.js?render=6LfkFDEaAAAAAKbWgUEZ5-3Dsp0CRR_TMm4j_JEh"></script>
    <style type="text/css">
        .auto-style3 {
            margin-left: 0px;
        }
        .auto-style5 {
            text-align: center;
        }
        fieldset {
            margin: auto;
        }
        </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="auto-style5">
        <div class="auto-style5">
                    <div>
                    <br />
                    <br />
                        <asp:Image ID="Image1" runat="server" Height="300px" ImageUrl="https://cdn.discordapp.com/attachments/687043714744320027/798910352095182868/Untitled-1.png" />
                    <br />
                    <asp:Label ID="Label1" runat="server" Text="Login" Font-Names="New Gulim" Font-Size="30pt"></asp:Label>
                        <br />
                <br />
                    </div>
            <hr style="height:2px;border-width:0;color:gray;background-color:gray;width:800px" />
        </div>
        <p class="auto-style5">
            <asp:HyperLink ID="HyperLink1" runat="server" Font-Names="Century Gothic" NavigateUrl="~/Login.aspx">Login</asp:HyperLink>
&nbsp;<asp:HyperLink ID="HyperLink2" runat="server" Font-Names="Century Gothic" NavigateUrl="~/Registration.aspx">Register</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="hlMyProfile" runat="server" Font-Names="Century Gothic" NavigateUrl="~/Success.aspx">My Profile</asp:HyperLink>
        </p>
        <p class="auto-style5">
            &nbsp;</p>
        <p class="auto-style5">
            <asp:Label ID="lbErrorMsg" runat="server" Text=""></asp:Label>
            </p>
        <fieldset style="width:900px;">
        <p class="auto-style5">
                    &nbsp;</p>
            <p class="auto-style5">
                    <asp:Label ID="Label2" runat="server" Text="E-mail:     " Font-Names="Century Gothic"></asp:Label>
                    <asp:TextBox ID="tbEmail" runat="server" CssClass="auto-style3"></asp:TextBox>
                </p>
        <p class="auto-style5">
                    <asp:Label ID="Label3" runat="server" Text="Password:" Font-Names="Century Gothic"></asp:Label>
                &nbsp; <asp:TextBox ID="tbPass" runat="server" TextMode="Password"></asp:TextBox>
        </p>
        <p class="auto-style5">
                    <asp:Button ID="btnLogin" runat="server" OnClick="btnLogin_Click" Text="Login" Font-Bold="True" Font-Names="New Gulim" Width="309px" />
                </p>

        <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>

    <br />
        </fieldset>
            </div>
            </form>
    <script>
        grecaptcha.ready(function () {
            grecaptcha.execute('6LfkFDEaAAAAAKbWgUEZ5-3Dsp0CRR_TMm4j_JEh', { action: 'Login' }).then(function (token) {
                document.getElementById("g-recaptcha-response").value = token;
            });
        });
    </script>
</body>
</html>
