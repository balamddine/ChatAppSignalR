<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ChatPage.aspx.cs" Inherits="ChatPage" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Chat page</title>
    <link href="Styles/owl.carousel.css" rel="stylesheet" />
    <link href="Styles/owl.theme.css" rel="stylesheet" />
    <link href="Styles/ChatPage.css" rel="stylesheet" type="text/css" />
    <link href="Styles/tipped.css" rel="stylesheet" type="text/css" media="screen" />
    <link href="Styles/chatbubble.css" rel="stylesheet" type="text/css" />
    <link href="Styles/pgwmodal.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui.css" rel="stylesheet" type="text/css" />

    <script src="Scripts/jquery.js" type="text/javascript"></script>
    <script src="Scripts/jquery-ui.js" type="text/javascript"></script>
    <script src="Scripts/pgwmodal.min.js" type="text/javascript"></script>
    <script src="Scripts/jquery.elastic.source.js" type="text/javascript"></script>
    <script src="Scripts/notification/jquery.noty.js" type="text/javascript"></script>
    <script src="Scripts/notification/bottomLeft.js" type="text/javascript"></script>
    <script src="Scripts/notification/default.js" type="text/javascript"></script>
    <script src="Scripts/owl.carousel.js" type="text/javascript"></script>
    <script src="Scripts/tipped.js" type="text/javascript"></script>
    <script src="Scripts/jquery.signalR-1.1.4.js" type="text/javascript"></script>
    <script type="text/javascript" src="signalr/hubs"></script>
    <script src="Scripts/ChatPage.js" type="text/javascript"></script>
    <style type="text/css">
        
    </style>
    <script type="text/javascript">
      
    </script>
    <style type="text/css">
        #dv_FriendList .item
        {
            margin: 3px;
        }
        #dv_FriendList .item img
        {
            display: block;
            width: 128px !important;
            height: 70px !important;
            cursor: pointer;
        }
        #dv_GroupList .item
        {
            margin: 3px;
        }
        #dv_GroupList .item img
        {
            display: block;
            width: 128px !important;
            height: 70px !important;
            cursor: pointer;
        }
    </style>
    <script type="text/javascript">
   
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:HiddenField ID="UseInfo_hf" runat="server" ClientIDMode="Static" />
    <span id="ConnectingStatus_sp"></span>
    <br />
    <div id="Settings_dv" style="border: 1px dotted #868686; width: 100%">
        <div id='cssmenu'>
            <ul>
                <li class='active'><a href="ChatPage.aspx"><span>Home</span></a></li>
                <li><a href='javascript:void(0)' id="openFolder_a"><span>Hidden chat</span> <span
                    class="offline_msg GlobalOfflineMsg"></span></a></li>
                <li><a href='javascript:void(0)' id="Start a new Chat" onclick="Start_New_Chat();"><span>
                    Start a New chat</span></a></li>
                <li><a href='javascript:void(0)' id="Create_Group" onclick="init_Group();"><span>Create
                    Group</span></a></li>
                <li><a href='javascript:void(0)' onclick="logout();"><span>logout</span></a></li>
            </ul>
        </div>
    </div>
    <br />
   
    <div id="dv_FriendList"  class="owl-carousel owl-theme">
    </div>
    <br />
   
    <div id="chatContainer">
    </div>
    <div class="bubble" id="emoji_bubble">
    </div>
    <div id="Options_bubble">
    </div>
    <div id="ModalDiv" style="display: none;">
    </div>
    <div id="Start_NewChat_Modal" style="display: none;">
        <div class="SettingBubble StartNewChat">
            <i>Friend:</i><input type='text' id='txt_autoComplete' />
        </div>
    </div>
    <div id="CreateGroup_dv" style="display: none;">
        <div class="SettingBubble CreateGroup">
            <table>
                <tr>
                    <td>
                        <i>Title:</i>
                    </td>
                    <td>
                        <input type='text' id='txt_GroupTitle' />
                    </td>
                </tr>
                <tr>
                    <td>
                        <i>Friend:</i>
                    </td>
                    <td>
                        <input type='text' id='txt_GroupautoComplete' />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <input type="button" style="float: right;" id="CreateGroup_btn" onclick="Create_Group(this,-1);"
                            value="Create" />
                    </td>
                </tr>
            </table>
            <div id="Participants">
            </div>
        </div>
    </div>
    </form>
</body>
</html>
