<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ContactForm.aspx.cs" Inherits="PineGroveWebForms.ContactForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Pine Grove - Contact Form</title>
    <link href="https://fonts.googleapis.com/css?family=Francois+One&display=swap" rel="stylesheet" />
    <style>
        ::selection
        {
            background:lightgreen;
        }
        ::-moz-selection
        {
            background:lightgreen;
        }
        body
        {
            background-color:#999999;
            font-family: 'Francois One', Arial, sans-serif;
        }
        table
        {
            width:80%;
            margin-left:auto;
            margin-right:auto;
        }
        td
        {
            text-align:center;
        }
        table tr td
        {
            padding:10px;
        }
        img
        {
            height:75px;
        }
        #imgRow
        {
            background-color:#333333;
        }
        .textbox
        {
            font-family: 'Francois One', Arial, sans-serif;
            background-color:#CCCCCC;
            border:2px solid forestgreen;
            border-radius:8px;
            outline:none;
            transition-duration:0.3s;
            padding:3px 10px;
        }
        .textbox:hover
        {
            background-color:#999999;
        }
        .textbox:active
        {
            background-color:forestgreen;
        }
        .button
        {
            font-family: 'Francois One', Arial, sans-serif;
            font-size:18px;
            background-color:#999999;
            border:2px solid forestgreen;
            padding:5px 20px;
            border-radius:8px;
            -webkit-transition-duration:0.3s;
            transition-duration:0.3s;
            cursor:pointer;
            outline:none;
        }
        .button:hover
        {
            background-color:forestgreen;
            color:white;
        }
        .button:active
        {
            background-color:limegreen;
            color:darkgray;
            border:2px solid limegreen;
        }
    </style>
    <script src="https://www.google.com/recaptcha/api.js" async defer></script>
</head>
<body>
    <form id="contactForm" runat="server">
        <table>
            <tr id="imgRow">
                <td style="width:100%; border:3px solid black;" colspan="4">
                    <img src="Images/pinegrovebanner.png" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:Label runat="server" Text="Contact Form" Font-Size="30" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:Label runat="server" Text="Questions or concerns? Or would you like more information from the church leadership? Fill out this form and we will get back to you as soon as possible!" Font-Size="18" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:Label runat="server" Text="* Indicates Required Field" />
                </td>
            </tr>
            <tr>
                <td style="text-align:right">
                    <asp:Label runat="server" Text="* First Name:" />
                </td>
                <td style="text-align:left; width:30%">
                    <asp:TextBox runat="server" ID="txtFirstName" Width="100%" CssClass="textbox" />
                </td>
                <td style="text-align:right">
                    <asp:Label runat="server" Text="* Last Name:" />
                </td>
                <td style="text-align:left; width:30%">
                    <asp:TextBox runat="server" ID="txtLastName" Width="100%" CssClass="textbox" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:Label runat="server" Text="* What would you like to discuss?" />
                </td>
            </tr>
            <tr>
                <td colspan="4" style="text-align:left">
                    <asp:TextBox runat="server" ID="txtComments" Width="100%" CssClass="textbox" Height="300" TextMode="MultiLine" />
                </td>
            </tr>
            <tr>
                <td style="text-align:right">
                    <asp:Label runat="server" Text="Email Address:" />
                </td>
                <td style="text-align:left; width:30%">
                    <asp:TextBox runat="server" ID="txtEmail" TextMode="Email" Width="100%" CssClass="textbox" />
                </td>
                <td style="text-align:right">
                    <asp:Label runat="server" Text="Phone Number:" />
                </td>
                <td style="text-align:left; width:30%">
                    <asp:TextBox runat="server" ID="txtPhoneNumber" TextMode="Phone" Width="100%" CssClass="textbox" />
                </td>
            </tr>
            <tr>
                <td colspan="2" style="width:50%">
                    <asp:CheckBox runat="server" ID="chkSubscribe" Text="Would you like to subscribe to get monthly emails on our church events?" CssClass="checkbox" />
                </td>
                <td colspan="2" style="width:50%">
                    <asp:CheckBox runat="server" ID="chkContactBack" Text="Is it okay for us to contact you to follow-up on your inquiry?" CssClass="checkbox" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <div class="g-recaptcha" data-sitekey="6LdBE60UAAAAAM-9L1S_S3HGGmHhhl4WjyAAelof" style="display:inline-block" />
                </td>
            </tr>
            <tr>
                <td colspan="2" style="text-align:right">
                    <asp:Button runat="server" ID="btnClear" Text="Clear" OnClientClick="document.getElementById('contactForm').reset();" CssClass="button"/>
                </td>
                <td colspan="2" style="text-align:left">
                    <asp:Button runat="server" ID="btnSubmit" Text="Submit" OnClientClick="return ValidateForm();" OnClick="BtnSubmit_Click" CssClass="button"/>
                </td>
            </tr>
        </table>
    </form>
    <script type="text/javascript">
        function ValidateForm() {
            var valid = true;
            var firstName = document.getElementsByClassName("textbox")[0].value;
            var lastName = document.getElementsByClassName("textbox")[1].value;
            var comments = document.getElementsByClassName("textbox")[2].value;
            var email = document.getElementsByClassName("textbox")[3].value;
            var phoneNumber = document.getElementsByClassName("textbox")[4].value;
            var subscribed = document.getElementById('<%=chkSubscribe.ClientID%>').checked;
            var respond = document.getElementById('<%=chkContactBack.ClientID%>').checked;
            var errorMessage = "";
            if (firstName.trim() == "") {
                errorMessage += "First name is required!\n";
                valid = false;
            }
            if (lastName.trim() == "") {
                errorMessage += "Last name is required!\n";
                valid = false;
            }
            if (comments.trim() == "") {
                errorMessage += "Comments are required!\n";
                valid = false;
            }
            if (phoneNumber.trim() !== "" && !phoneNumber.match(/^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$/)) {
                errorMessage += "Phone number is invalid!\n";
                valid = false;
            }
            if (subscribed && !email.match(/^[a-zA-Z0-9.!#$%&’*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$/)) {
                errorMessage += "A valid email is required if you wish to subscribe to our email list!\n";
                valid = false;
            }
            if (respond && !phoneNumber.match(/^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$/) && !email.match(/^[a-zA-Z0-9.!#$%&’*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$/)) {
                errorMessage += "A valid phone number or valid email is required if you wish for us to contact you about your request!";
                valid = false;
            }
            if (!valid) {
                alert(errorMessage);
            }
            return valid;
        }
    </script>
</body>
</html>
