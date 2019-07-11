<%@ Page Async="true" Language="C#" AutoEventWireup="true" CodeBehind="EventsWebForm.aspx.cs" EnableEventValidation="true" Inherits="PineGroveWebForms.EventsWebForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Pine Grove - Events</title>
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
        .eventTable td 
        {
            text-align:center;
            font-family:'Francois One', Arial, sans-serif;
            padding:5px;
        }
        .eventTable 
        {
            margin-left:auto;
            margin-right:auto;
            padding:10px;
        }
        .pager
        {
            font-family:'Francois One', Arial, sans-serif;
        }
        a:hover
        {
            color:darkslategray;
        }
        a:visited
        {
            color:#333333;
        }
        a:active
        {
            color:lightgreen;
        }
        a
        {
            color:forestgreen;
            -webkit-transition-duration:0.3s;
            transition-duration:0.3s;
        }
        .register
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
        .register:hover, .list:hover
        {
            background-color:forestgreen;
            color:white;
        }
        .register:active, .list:active
        {
            background-color:limegreen;
            color:darkgray;
            border:2px solid limegreen;
        }
        .list
        {
            background-color:#999999;
            border:2px solid forestgreen;
            border-radius:8px;
            -webkit-transition-duration:0.3s;
            transition-duration:0.3s;
            outline:none;
            font-family: 'Francois One', Arial, sans-serif;
        }
        .registrationForm
        {
            display:none;
        }
        .registerDisabled
        {
            font-family: 'Francois One', Arial, sans-serif;
            font-size:18px;
            background-color:#999999;
            border:2px solid forestgreen;
            padding:5px 20px;
            border-radius:8px;
            cursor:not-allowed;
            outline:none;
            opacity:0.6;
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
    </style>
    <script src="https://www.google.com/recaptcha/api.js" async defer></script>
</head>
<body style="background-color: #999999">
    <form id="eventDetails" runat="server">
        <asp:FormView ID="eventView" runat="server" HorizontalAlign="Center" GridLines="None" AllowPaging="true" Width="80%" Height="95%" OnPageIndexChanging="EventView_PageIndexChanging">
            <PagerSettings Position="Bottom" Mode="Numeric" />
            <PagerStyle HorizontalAlign="Center" CssClass="pager" />
            <HeaderStyle BackColor="#333333" BorderWidth="3" BorderColor="Black" />
            <HeaderTemplate>
                <asp:Panel HorizontalAlign="Center" runat="server">
                    <asp:Image runat="server" ImageAlign="Middle" ImageUrl="~/Images/pinegrovebanner.png" Height="100" />
                </asp:Panel>
            </HeaderTemplate>
            <ItemTemplate>
                <table class="eventTable" runat="server">
                    <tr>
                        <td colspan="2">
                            <asp:Panel HorizontalAlign="Center" runat="server">
                                <asp:Image runat="server" ImageAlign="Middle" ImageUrl='<%#Eval("imgPicture")%>' Height="75" />
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="lblTitle" runat="server" Font-Size="30" Text='<%#Eval("txtTitle")%>' />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="lblDescription" runat="server" Font-Size="18" Text='<%#Eval("txtDescription")%>' />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" Font-Size="14" Text="Date and Time" Font-Underline="true" />
                        </td>
                        <td>
                            <asp:Label runat="server" Font-Size="14" Text="Address Information" Font-Underline="true" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" Font-Size="14" Text='<%#"From " + Eval("txtStartTime")%>' />
                        </td>
                        <td rowspan="2">
                            <asp:Label runat="server" Font-Size="14" Text='<%#Eval("txtAddress")%>' />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" Font-Size="14" Text='<%#"Until " + Eval("txtEndTime")%>' />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Button runat="server" Text="Register" CssClass="register" ID="btnRegister" OnClientClick="return Register_Clicked(this);" OnClick="BtnRegister_Click"/>
                        </td>
                    </tr>
                    <tr id="guestRow">
                        <td colspan="2">
                            <asp:Label ID="lblGuestPrompt" runat="server" Font-Size="14" Text="How many guests do you plan on bringing?" CssClass="registrationForm notVolunteering" />
                            <asp:DropDownList runat="server" Font-Size="14" ID="lstGuests" CssClass="list registrationForm notVolunteering" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Label runat="server" Font-Size="14" ID="lblMaximum" CssClass="registrationForm notVolunteering" Text='<%#Eval("txtMaximum")%>' />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:CheckBox runat="server" Text="Volunteering?" Font-Size="14" OnClick="Volunteering_CheckedChanged(this)" ID="chkVolunteering" CssClass="registrationForm"/>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Label runat="server" Font-Size="14" ID="lblUsername" CssClass="registrationForm" Text="Username:" />
                            <asp:TextBox runat="server" Font-Size="14" ID="txtUsername" CssClass="registrationForm textbox" Width="100px"/>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <div class="g-recaptcha registrationForm" data-sitekey="6LdBE60UAAAAAM-9L1S_S3HGGmHhhl4WjyAAelof" />
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
        </asp:FormView>
    </form>
    <script type="text/javascript">
        function Volunteering_CheckedChanged(sender) {
            var notVolunteering = document.getElementsByClassName("notVolunteering");
            for (var i = 0; i < notVolunteering.length; i++) {
                notVolunteering.item(i).style.display = sender.checked ? "none" : "inline";
            }
        }
        function Register_Clicked(sender) {
            var registrationForm = document.getElementsByClassName("registrationForm");
            if (sender.value == "Confirm") {
                if (document.getElementsByClassName("textbox").item(0).value.length == 0) {
                    alert("Username must not be empty!");
                    return false;
                }
                else {
                    return true;
                }
            }
            else {
                sender.value = "Confirm";
                var boolean = false;
                for (var i = 0; i < registrationForm.length; i++) {
                    if (registrationForm.item(i).style.display == "inline")
                        boolean = true;
                    registrationForm.item(i).style.display = "inline";
                }
                var captcha = document.getElementsByClassName("g-recaptcha").item(0);
                captcha.style.display = "inline-block";
                return boolean;
            }
        }
    </script>
</body>
</html>
