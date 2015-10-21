<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Register.aspx.cs" Inherits="Register" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="Styles/loginPage.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .star
        {
            color: #FF0000;
            float: right;
            position: relative;
            right: -13px;
            top: -26px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server" class="login">
    <div style="float: right;">
        (<span style="color: red">*</span>) are required filed</div>
    <br />
    <br />
    <div>
        <p>
            <label for="name_txt">
                name:</label>
            <asp:TextBox ID="name_txt" runat="server"></asp:TextBox>
            <br />
            <br />
        </p>
        <p>
            <label for="Username_txt">
                Username:</label>
            <asp:TextBox ID="Username_txt" runat="server"></asp:TextBox><span class="star">*</span>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="required"
                ControlToValidate="Username_txt" ValidationGroup="RegisterValidation_grp"></asp:RequiredFieldValidator>
        </p>
        <p>
            <label for="Email_txt">
                Email:</label>
            <asp:TextBox ID="Email_txt" runat="server"></asp:TextBox><span class="star">*</span>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="required"
                ControlToValidate="Email_txt" ValidationGroup="RegisterValidation_grp"></asp:RequiredFieldValidator>
        </p>
        <p>
            <label for="password">
                Password:</label>
            <asp:TextBox ID="Password_txt" runat="server" TextMode="Password"></asp:TextBox><span
                class="star">*</span>
            <asp:RequiredFieldValidator ID="req_vld" runat="server" ErrorMessage="required" ControlToValidate="Password_txt" ValidationGroup="RegisterValidation_grp"></asp:RequiredFieldValidator>
        </p>
        <p>
            <label for="Logo">
                Logo:</label>
            <asp:FileUpload ID="Logo_Upload" runat="server" />
        </p>
        <a href="LoginPage.aspx" style="position:relative;top:44px;">Cancel</a>
        <div style="float: right; position: relative; top: 22px;">
            <button type="submit" class="login-button" id="Submit_btn" runat="server" validationgroup="RegisterValidation_grp" onserverclick="Register_btn_Click">
                Login</button>
        </div>
    </div>
    <asp:Label ID="Error_msg" runat="server" ForeColor="Red" ></asp:Label>
    </form>
</body>
</html>
