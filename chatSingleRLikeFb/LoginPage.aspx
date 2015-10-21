<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LoginPage.aspx.cs" Inherits="LoginPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="Styles/loginPage.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/jquery-1.10.1.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(window).keypress(function (e) {
            if (e.which == 13 || e.keyCode == 13) { // enter
                $("#<%=Submit_btn.ClientID %>").trigger("click");
            }
        });
    
    </script>
</head>
<body>
    <form id="form1" runat="server" class="login">
    <p>
      <label for="login">Username:</label>
      <asp:TextBox ID="Username_txt" runat="server" ></asp:TextBox> &nbsp;
      <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="required" ControlToValidate="Username_txt" ValidationGroup="loginValidation_grp" ></asp:RequiredFieldValidator>
    </p>

    <p>
      <label for="password">Password:</label>
      <asp:TextBox ID="Password_txt" runat="server"  TextMode="Password"></asp:TextBox>&nbsp;
      <asp:RequiredFieldValidator ID="req_vld" runat="server" ErrorMessage="required" ControlToValidate="Password_txt" ValidationGroup="loginValidation_grp" ></asp:RequiredFieldValidator>
    </p>

    <p class="login-submit">
      <button type="submit" class="login-button" id="Submit_btn" runat="server" validationgroup="loginValidation_grp" onserverclick="Submit_btn_Click">Login</button>
    </p>
    <asp:Label ID="Error_msg" runat="server" ForeColor="Red" ></asp:Label>
    <p class="forgot-password"><a href="LoginPage.aspx">Forgot your password?</a>&nbsp;&nbsp;&nbsp;<a href="Register.aspx" style="float:right">Register</a></p>
    </form>
</body>
</html>
