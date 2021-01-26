<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="ASAssignment1.Registration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function validate() {

            // Extract data from password textbox
            var str = document.getElementById('<%=tbPass.ClientID %>').value;

            // Check whether password length is less than 8
            if (str.length < 8) {
                document.getElementById("lbPwrdChck").innerHTML = "Password length must be greater than 8 characters.";
                document.getElementById("lbPwrdChck").style.color = "Red";
                return ("Too short.")
            }

            // Check whether there is 1 or more numerals
            else if (str.search(/[0-9]/) == -1) {
                document.getElementById("lbPwrdChck").innerHTML = "Password must contain at least 1 numeral.";
                document.getElementById("lbPwrdChck").style.color = "Red";
                return ("No numeral.")
            }

            // Check whether password contains lowercase
            else if (str.search(/[a-z/]/) == -1) {
                document.getElementById("lbPwrdChck").innerHTML = "Passowrd does not contain lowercase characters.";
                document.getElementById("lbPwrdChck").style.color = "Red";
                return ("No lowercase letters.")
            }

            // Check whether password contains uppercase
            else if (str.search(/[A-Z]/) == -1) {
                document.getElementById("lbPwrdChck").innerHTML = "Password does not contain uppercase characters.";
                document.getElementById("lbPwrdChck").style.color = "Red";
                return ("No uppercase characters.")
            }

            // Check whether password contains special characters
            else if (str.search(/[ `!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?~]/) == -1) {
                document.getElementById("lbPwrdChck").innerHTML = "Password does not contain special characters.";
                document.getElementById("lbPwrdChck").style.color = "Red";
                return ("No special characters.")
            }

            document.getElementById("lbPwrdChck").innerHTML = "";
        }

        function checkPwd() {
            var str = document.getElementById('<%=tbPass.ClientID %>').value;
            var str2 = document.getElementById('<%=tbCfmPass.ClientID %>').value;

            if (str != str2) {
                document.getElementById("lbPwrdChck").innerHTML = "Passwords do not match.";
            }
            else {
                document.getElementById("lbPwrdChck").innerHTML = "";
            }
        }

    </script>
    <script src="https://www.google.com/recaptcha/api.js?render=6LfkFDEaAAAAAKbWgUEZ5-3Dsp0CRR_TMm4j_JEh"></script>
    <style type="text/css">
        .auto-style3 {
            text-align: center;
        }
        .auto-style4 {
            font-family: "Century Gothic";
            font-weight: bold;
        }
        fieldset {
            margin: auto;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="auto-style3">
        <div>
            <br />
            <br />
            <asp:Image ID="Image1" runat="server" Height="300px" ImageUrl="https://cdn.discordapp.com/attachments/687043714744320027/798910352095182868/Untitled-1.png" />
            <br />
                    <asp:Label ID="Label1" runat="server" Text="Registration" Font-Names="New Gulim" Font-Size="30pt"></asp:Label>
                <br />
                </div>
        <div class="auto-style3">
            <hr style="height:2px;border-width:0;color:gray;background-color:gray;width:800px">
            <div>
            <br />
                <asp:HyperLink ID="hlLogin" runat="server" Font-Names="Century Gothic" NavigateUrl="~/Login.aspx">Login</asp:HyperLink>
&nbsp;<asp:HyperLink ID="hlRegister" runat="server" Font-Names="Century Gothic" NavigateUrl="~/Registration.aspx">Register</asp:HyperLink>
                &nbsp;<asp:HyperLink ID="hlMyProfile" runat="server" Font-Names="Century Gothic" NavigateUrl="~/Success.aspx">My Profile</asp:HyperLink>
                <br /><br />
            </div>
        </div>
        <fieldset style="width:900px;align-items:center;">
        <p>
                    &nbsp;</p>
            <p class="auto-style4">
                    <asp:Label ID="Label10" runat="server" Text="Login Information<br>" Font-Bold="True" Font-Names="Candara" Font-Size="20pt"></asp:Label>
                    <asp:Label ID="Label11" runat="server" Text="Enter the information you wish to login with.&lt;br&gt;" Font-Bold="False" Font-Names="Candara Light" Font-Italic="True"></asp:Label>
                </p>
            <p>
                    <asp:Label ID="Label5" runat="server" Text="E-mail:" Font-Bold="True" Font-Names="Century Gothic"></asp:Label>
                &nbsp;<asp:TextBox ID="tbEmail" runat="server"></asp:TextBox>
                </p>
        <p>
                    <asp:Label ID="Label6" runat="server" Text="Password:" Font-Bold="True" Font-Names="Century Gothic"></asp:Label>
                &nbsp;<asp:TextBox ID="tbPass" runat="server" TextMode="Password" onkeyup="javascript:validate()"></asp:TextBox>
                &nbsp;</p>
        <p>
                    <asp:Label ID="Label8" runat="server" Text="Confirm Password:" Font-Bold="True" Font-Names="Century Gothic"></asp:Label>
                &nbsp;<asp:TextBox ID="tbCfmPass" runat="server" TextMode="Password" onkeyup="javascript:checkPwd()"></asp:TextBox>
                &nbsp;</p>
            <p>
                    <asp:Label ID="lbPwrdChck" runat="server" Font-Names="New Gulim" ForeColor="Red"></asp:Label>
                </p>
            <p>
                    &nbsp;</p>
            </fieldset>
        <p>
            &nbsp;</p>
        <fieldset style="width:900px;">
        <p>
                    &nbsp;</p>
            <p>
                    <asp:Label ID="Label12" runat="server" Text="Personal Information&lt;br&gt;" Font-Bold="True" Font-Names="Candara" Font-Size="20pt"></asp:Label>
                    <asp:Label ID="Label13" runat="server" Text="Tell us more about yourself.&lt;br&gt;" Font-Bold="False" Font-Names="Candara Light" Font-Italic="True"></asp:Label>
                </p>
            <p>
                    <asp:Label ID="Label2" runat="server" Text="First Name:" Font-Bold="True" Font-Names="Century Gothic"></asp:Label>
                &nbsp;<asp:TextBox ID="tbFName" runat="server"></asp:TextBox>
                </p>
        <p>
                    <asp:Label ID="Label3" runat="server" Text="Last Name:" Font-Bold="True" Font-Names="Century Gothic"></asp:Label>
                &nbsp;<asp:TextBox ID="tbLName" runat="server"></asp:TextBox>
                </p>
        <p>
                    <asp:Label ID="Label7" runat="server" Text="Date of Birth:" Font-Bold="True" Font-Names="Century Gothic"></asp:Label>
                &nbsp;<asp:TextBox ID="tbDob" runat="server" TextMode="Date"></asp:TextBox>
                </p>
            <p>
                    &nbsp;</p>
            </fieldset>
        <p>
            &nbsp;</p>
        <fieldset style="width:900px;">
        <p>
                    &nbsp;</p>
            <p>
                    <asp:Label ID="Label14" runat="server" Text="Payment Information&lt;br&gt;" Font-Bold="True" Font-Names="Candara" Font-Size="20pt"></asp:Label>
                    <asp:Label ID="Label15" runat="server" Text="Save your credit card information for future uses.&lt;br&gt;" Font-Bold="False" Font-Names="Candara Light" Font-Italic="True"></asp:Label>
                </p>
            <p>
                    <asp:Label ID="Label4" runat="server" Text="Credit Card No." Font-Bold="True" Font-Names="Century Gothic"></asp:Label>
                &nbsp;<asp:TextBox ID="tbCc" runat="server"></asp:TextBox>
                </p>
        <p>
                    <asp:Label ID="Label9" runat="server" Text="CVV:" Font-Bold="True" Font-Names="Century Gothic"></asp:Label>
                &nbsp;<asp:TextBox ID="tbCcCVV" runat="server"></asp:TextBox>
                </p>
            <p>
                    &nbsp;</p>
            </fieldset>
        <p class="auto-style3">
                    <asp:Button ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" Text="Submit" BackColor="#99CCFF" BorderStyle="Groove" Font-Names="Century Gothic" Width="365px" />
                </p>
            <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>

            <asp:Label ID="lbl_gScore" runat="server" Text=""></asp:Label>
        <p>
            &nbsp;</p>
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
