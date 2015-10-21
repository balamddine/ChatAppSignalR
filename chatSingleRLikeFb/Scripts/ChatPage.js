
var chat = $.connection.chatHub;
var GlobalOfflineMsgCount = 0;

var emojiShortCut = [
["^-^", ">:(", "=(", "=@", "(y)", "(=note)", "(=journal)", "(=share)", "(=group)", "=)", "(=hsb)", "(=lfyel)", "(=lfgree)", "(=orange)", "(=Apple)", ],
["(=angry)", "(=sad)", "<:(", "=p", "=*", "<-*)", "=D", "=k", "(=cry1)", "(=sad1)", "(=STrawberry)", "(=Hamburger)", "(=COcktail)", "(=Cheers)", "(=Gift)", ],
["(=-k)", "<&>", "(=sad)", "(=disa)", "(=<p)", "(h5)", "(=hope)", "(=sad3)", "(=Finger)", "(=Sun)", "(=Pumpkin)", "(=Chrismess)", "(=Santa)", "(=Ballon)", "(=BIrthday)", ],
["", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ],
["", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ],
["", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ],
["", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ],
["", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ],
["", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ],
["", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ],
["", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ],
["", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ],
["", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ],
["", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ],
["", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ],
["", "", "", "", "", "", "", "", "", "", "", "", "", "", ""]
 ];
var userInformation = {
    UserID: "",
    UserName: "",
    UserStatus: "",
    UserStatusDesc: "",
    UserMood: "",
    UserEmail: "",
    UserLogo: ""
};
$(function () {

    $("#ConnectingStatus_sp").css("color", "Red");
    $("#ConnectingStatus_sp").html("Connecting to server...");

    // Declare a proxy to reference the hub. 
    var UserInfo = $("#UseInfo_hf").val() != "" ? $("#UseInfo_hf").val().split(',') : "";
    $.connection.hub.start().done(function () {
        $("#ConnectingStatus_sp").css("color", "Green");
        $("#ConnectingStatus_sp").html("Connected");
        userInformation.UserID = UserInfo[0];
        userInformation.UserName = UserInfo[1];
        userInformation.UserEmail = UserInfo[2];
        userInformation.UserLogo = UserInfo[3];
        userInformation.UserMood = UserInfo[4];
        userInformation.UserStatus = UserInfo[5];
        userInformation.UserStatusDesc = UserInfo[6];
        
        GetUserChatListInformation(userInformation.UserID, "GetUserFriendListInformation",false,true);
        GetUserChatListInformation(userInformation.UserID, "GetUserGroupListInformation",true,false);
        
        chat.server.adduser(UserInfo[0]);
       
    });
    $("#openFolder_a").click(function () {

        if (GlobalOfflineMsgCount == 0 || $("#ModalDiv").html()=="") {
            $(".GlobalOfflineMsg").html("");
            $(".GlobalOfflineMsg").hide();
        }
        $.pgwModal({
            target: '#ModalDiv',
            title: 'hidden chats '

        });

    });




});

window.GetUserChatListInformation = function (UserId, func, IsForGroup, IsForFriends) {
    $.ajax({
        type: 'POST',
        url: 'ChatPage.aspx/' + func,
        data: '{UserID:"' + UserId + '"}',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (data) {
            var Tab = data.d;
            var length = Tab.length;
            for (i = 0; i < length; i++) {

                if (IsForFriends) {
                    var ToAppend = "<div class='item' onclick=\"CreateChatWindow(" + Tab[i]["USER_ID"] + ",'" + Tab[i]["USER_USERNAME"] + "','" + Tab[i]["USER_LOGO"] + "')\">"
                              + "<img class='tooltip' src='" + Tab[i]["USER_LOGO"] + "' alt='" + Tab[i]["USER_USERNAME"] + "' title='" + Tab[i]["USER_USERNAME"] + "' />"
                              + "</div>";
                    $("#dv_FriendList").append(ToAppend);
                }
                if (IsForGroup) {
                    var ToGroupAppend = "<div class='item' onclick=\"Create_Group(this," + Tab[i]["GROUP_ID"] + ")\">"
                              + "<img class='tooltip' src='Styles/Group.png' alt='" + Tab[i]["GROUP_TITLE"] + "' title='" + Tab[i]["GROUP_TITLE"] + "' />"
                              + "<input type='hidden' id='hf_GroupName_Group_'" + Tab[i]["GROUP_ID"] + "' value='" + Tab[i]["GROUP_TITLE"] + "' />";
                    var Grp_participant = Tab[i]["USERS"];
                    var args = "";
                    for (var j = 0; j < Grp_participant.length; j++) {
                        args += Grp_participant[j]["USER_ID"] + ",";
                    }
                    args = args.slice(0, -1);
                    ToGroupAppend += "<input type='hidden' id='hf_Group_" + Tab[i]["GROUP_ID"] + "' value='" + args + "' />"
                                    + "</div>";
                    $("#dv_FriendList").owlCarousel().data('owlCarousel').addItem(ToGroupAppend);
                }

            }

            var owl2 = $("#dv_FriendList");
            owl2.owlCarousel({
                items: 7, //10 items above 1000px browser width
                navigation: true,   //2 items between 600 and 0
                itemsDesktop: [1199, 6],
                itemsDesktopSmall: [979, 6]
            });
            Tipped.create('.tooltip', { position: 'top' });
        },
        error: function (xhr) {
            alert("responseText: " + xhr.responseText);
        }
    });
}

window.CreateChatWindow = function (FriendID, FriendName, FriendPic) {
    if ($("#Msg_container_dv_" + FriendID).length > 0) return; // if the chat window is opened allready return ;
    var div = $("#smallChatContainer_" + FriendID);
    if (div.length) { // check if the container is allready hidden 
        div.remove();
        if ($("#ModalDiv").html().trim() == "") { // if no hidden chat
            GlobalOfflineMsgCount = 0;
            $(".GlobalOfflineMsg").html("");
            $(".GlobalOfflineMsg").hide();
        }
    }
    var UserId = window.userInformation.UserID;
    var UserPicture = window.userInformation.UserLogo;
    $.ajax({
        type: 'POST',
        url: 'ChatPage.aspx/GetUserChatConversation',
        data: '{FriendID:"' + FriendID + '",FriendName:"' + FriendName + '",FriendPic:"' + FriendPic + '",UserPicture:"' + UserPicture + '"}',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (data) {
            var ToAppend = "<div class='Msg_container_dv' id='Msg_container_dv_" + FriendID + "' style='margin-right: 8px;' >"
                           + "<div class='Chat_Header_dv' id='Chat_Header_dv_" + FriendID + "'>"
                           + "<table width='100%'><tr>"
                           + "<td style='padding-left:2px;padding-top:3px; width:16px'>"
                           + "<img class='tooltip' src='" + FriendPic + "' title='" + FriendName + "' width='25px' height='25px' style='margin-top: -2px;' />"
                           + "</td>"
                           + "<td align='left' valign='centre'>"
                           + "<a  href='javascript:void(0)'   >" + FriendName + "</a>"
                           + "</td>"
                           + "<td align='right' width='23px'  style='vertical-align:middle;padding-right:2px;'><span class='settings_btn' title='options' onclick='OpenOptionsContainer(\"Msg_container_dv_" + FriendID + "\",\"txt_chat_msg_" + FriendID + "\"," + FriendID + ",\"" + FriendName + "\",\"" + FriendPic + "\");' ></span></td>"
                           + "<td align='right' width='23px'  style='vertical-align:middle;padding-right:4px;'><span class='close_btn' title='close window' onclick='closeChatwnd(\"Msg_container_dv_" + FriendID + "\");' ></span></td>"
                           + "</tr></table>"
                           + "</div>"
                          
                           + "<div class='Chat_Body_Container' id='Chat_Body_Container_" + FriendID + "'>"
                           + data.d
                           + "</div>"

                           + "<div class='Chat_Footer_Container' id='Chat_Footer_Container_" + FriendID + "'>"
                           + "<textarea id='txt_chat_msg_" + FriendID + "' onkeypress='SendMessage(event," + FriendID + ");' onkeyup='SendMessageBackSpace(event,"+FriendID+")' onfocus='txt_chat_msg_Focus(\"Chat_Header_dv_" + FriendID + "\");' ></textarea>"
                           + "<div class='emoji tooltip' title='emoji' onclick='openEmojiContainer(\"Msg_container_dv_" + FriendID + "\",\"txt_chat_msg_" + FriendID + "\");' /></div>"
                           
                           + "</div>";


            $("#chatContainer").append(ToAppend);
            scrollDivToBottom("Chat_Body_Container_" + FriendID);
            Tipped.create('.tooltip');
            $('#txt_chat_msg_' + FriendID).elastic();

        },
        error: function (xhr) {
            alert("responseText: " + xhr.responseText);
        }
    });
}
window.txt_chat_msg_Focus = function (headerDiv) {
    $("#emoji_bubble").css("display", "none");
    $("#Options_bubble").css("display", "none");
    document.title = "Chat page";
    clearInterval(flashDocument);
    $("#" + headerDiv).removeClass("animate"); // remove the animation if exist
}
/////////////////////////////////////// Emojies //////////////////////////////////////////////////////////////////////////////////////

window.openEmojiContainer = function (DivId, inputID) {
    var offsetleft = $("#" + DivId).offset().left - 35 + "px";
    var ToAppend = DrawEmojiTable(3, 10, inputID);

    $("#emoji_bubble").css("left", offsetleft);
    $("#emoji_bubble").css("display", "block");
    $("#Options_bubble").css("display", "none");
    $("#emoji_bubble").html(ToAppend);
   
}
window.DrawEmojiTable = function (row, ItemsPerRow, inputID) {
    var Append = "<div class='emojiContainer_div'><table width='99%' style='margin-left: 4px;'>";
    var size = 17;
    for (var i = 0; i < row; i++) {
        Append += "<tr>";
        var r = i * ItemsPerRow;
        for (var j = 0; j < ItemsPerRow; j++) {
            Append += "<td align='centre' style='padding-top:10px'>"
            Append += "<div class='emojiIcon' onclick='insertEmoji(this,\"" + inputID + "\")' id='" + emojiShortCut[i][j] + "' title='" + emojiShortCut[i][j] + "' style='background-position:0px -" + parseInt((r + j) * size - 1) + "px;'></div>";
            Append += "</td>";
        }
        Append += "</tr>";
    }
    Append += "</table>";
    return Append;
}

window.insertEmoji = function (sender, inputID) {
    if (!$("#" + inputID).prop("disabled")) { // check if user is not offline 
        var html = $("#" + inputID).val();
        $("#" + inputID).val(html + sender.id);
        $("#" + inputID).focus();
    }
    $("#emoji_bubble").css("display", "none");
}

///////////////////////////////////////////////// End Emojies //////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////// Options //////////////////////////////////////////////////////////////////////
window.OpenOptionsContainer = function (DivId, inputID, OpenerID, FriendName, FriendPic) {
    var offsetleft = $("#" + DivId).offset().left + 70 + "px";
    var offsettop = $("#" + DivId).offset().top + 29 + "px";
    $("#Options_bubble").css("left", offsetleft);
    $("#Options_bubble").css("top", offsettop);
    $("#Options_bubble").css("display", "block");
    $("#emoji_bubble").css("display", "none");

    var ToAppend;
    if (FriendName == -1 && FriendPic == -1) { // opening settings in GroupChat
        ToAppend = "<ul>"
                    + "<li id='HideWnd_" + OpenerID + "' onclick='ExecuteOptionMenu(" + OpenerID + ",\"Hidewnd\",\"" + FriendName + "\",\"" + FriendPic + "\");'><a href='javascript:void(0)'>Hide chat window</a></li>"
                    + "<li id='ViewParticipant_" + OpenerID + "' onclick='ExecuteOptionMenu(" + OpenerID + ",\"ViewParticipant\");'><a href='javascript:void(0)'>View participant</a></li>"
                    + "<li id='addFiles_" + OpenerID + "' onclick='ExecuteOptionMenu(" + OpenerID + ",\"addFiles\");'><a href='javascript:void(0)'>add files...</a></li>"
                    + "</ul>"
                    + "<div class='fileinput_dv'><input type='file' id='addFiles" + OpenerID + "' onchange=FileInputChange(event," + OpenerID + ") /></div>";
    }
    else {
        ToAppend = "<ul>"
                    + "<li id='HideWnd_" + OpenerID + "' onclick='ExecuteOptionMenu(" + OpenerID + ",\"Hidewnd\",\"" + FriendName + "\",\"" + FriendPic + "\");'><a href='javascript:void(0)'>Hide chat window</a></li>"
                    + "<li id='ClearWnd_" + OpenerID + "' onclick='ExecuteOptionMenu(" + OpenerID + ",\"ClearWnd\");'><a href='javascript:void(0)'>Clear chat window</a></li>"
                    + "<li id='addFiles_" + OpenerID + "' onclick='ExecuteOptionMenu(" + OpenerID + ",\"addFiles\");'><a href='javascript:void(0)'>add files...</a></li>"
                    + "</ul>"
                    + "<div class='fileinput_dv'><input type='file' id='addFiles" + OpenerID + "' onchange=FileInputChange(event," + OpenerID + ") /></div>";
    }
    $("#Options_bubble").html(ToAppend);

}

window.ExecuteOptionMenu = function (OpenerID, args, FriendName, FriendPic) {
    switch (args) {
        case "ClearWnd": CLearChatWindow(OpenerID); break;
        case "addFiles": AddFiles(OpenerID); break;
        case "Hidewnd": HideChat(OpenerID, FriendName, FriendPic); break;
        case "ViewParticipant": ViewGroupParticipant(); break;
    }
    $("#Options_bubble").css("display", "none");
}

// hide chat when user click on hide chat window
window.HideChat = function (FriendID, FriendName, FriendPic) {
    $("#Msg_container_dv_" + FriendID).remove();
    CreateRelativeUserDivContainer(FriendID, FriendName, FriendPic);
}
// Create a small container for user inside the model Div
window.CreateRelativeUserDivContainer = function (FriendID, FriendName, FriendPic) {
    var div = $("#smallChatContainer_" + FriendID);
    var Template = "<div id='smallChatContainer_" + FriendID + "' onclick='ReopenChatwnd(" + FriendID + ",\"" + FriendName + "\",\"" + FriendPic + "\",this);'>"
                + "<div class='smallChatContainer'>"
                + "<img src='" + FriendPic + "' width='25' height='25' />"
                + "<span>" + FriendName + "</span>"
                + "</div>"
                +"<div class='offline_msg UserOffline_msg' id='offline_msg_" + FriendID + "'></div>"
                "</div><br>";
    if (!div.length) {
     $("#ModalDiv").append(Template);
    }

}
// re-open the chat window when the user click on the user small container
window.ReopenChatwnd = function (FriendID, FriendName, FriendPic) {
    $.pgwModal('close');

    CreateChatWindow(FriendID, FriendName, FriendPic);
    var usrNotification = $("#offline_msg_" + FriendID).html() == "" ? 0 : parseInt($("#offline_msg_" + FriendID).html());
    GlobalOfflineMsgCount -= usrNotification;
    $(".GlobalOfflineMsg").html(GlobalOfflineMsgCount);
    $("#offline_msg_" + FriendID).html("");
    $("#offline_msg_" + FriendID).hide("");
    $("#smallChatContainer_" + FriendID).remove();
    if ($("#ModalDiv").html().trim() == "") { // if no hidden chat
        GlobalOfflineMsgCount = 0;
        $(".GlobalOfflineMsg").html("");
        $(".GlobalOfflineMsg").hide();
    }
    
    $('#txt_chat_msg_' + FriendID).focus();
}
// clear chat history when the user click on clear chat window
window.CLearChatWindow = function (FriendID) {
    if ($("#Chat_Body_Container_" + FriendID).html() != "") { //check if there is messages in the chat box
        $.ajax({
            type: 'POST',
            url: 'ChatPage.aspx/CLearChatWindow',
            data: '{FriendID:"' + FriendID + '"}',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (data) {
                $("#Chat_Body_Container_" + FriendID).html("");
            },
            error: function (xhr) {
                alert("responseText: " + xhr.responseText);
            }
        });
    }
}
// trigger the file upload
window.AddFiles = function (FriendId) {
    $("#addFiles" + FriendId).click();
}
// trigger the change event of file upload and send the user file to server for manipulation
window.FileInputChange = function (evt, FriendID) {
    var file = evt.target.files[0]
    var FileName = file.name;
    var type = file.type;
    var userID = userInformation.UserID;
    var reader = new FileReader();

    // Closure to capture the file information.
    reader.onload = (function (theFile) {
        return function (e) {
            // Render thumbnail.
            var msg = e.target.result;
            SaveFileMsgToServer(msg,type,FriendID,userID);
          };
    })(file);
    // Read in the image file as a data URL.
    reader.readAsDataURL(file);
}
// save file to server
window.SaveFileMsgToServer = function (msg, type, FriendID, userID) {
    ShowLoaderImage(FriendID);
    $.ajax({
        type: 'POST',
        url: 'ChatPage.aspx/SaveFileMsg',
        data: "{fileData:'" + msg + "',type:'" + type + "'}",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (data) {
            if (data.d != "-1") 
               chat.server.sendMsg(data.d, FriendID, userID, type);
        },
        error: function (xhr) {
            alert("responseText: " + xhr.responseText);
        }
    });

}

///////////////////////////////////////////////// End Options //////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////// Send receive messages //////////////////////////////////////////////////////////////////////
var keyPressCount = 0;
window.SendMessage = function (e, FriendID) {

    var msg = $("#txt_chat_msg_" + FriendID).val();
    var userID = window.userInformation.UserID;
    if (keyPressCount++ % 10 == 0) {
        chat.server.sendIsTypingMsg(FriendID, userID, false);
    }
   
    if (e.which == 13 || e.keyCode == 13) { // enter hit
        chat.server.sendMsg(msg, FriendID, userID, "text");
        e.preventDefault(); // to prevent the page from posting back 
        return false;
    }

}
window.SendMessageBackSpace = function (e, FriendID) {
    var msg = $("#txt_chat_msg_" + FriendID).val();
    var userID = window.userInformation.UserID;
    if ( (e.which == 8 || e.keyCode == 8) && (msg==false) ) { // backspace is hit 
        chat.server.sendIsTypingMsg(FriendID, userID, true);
    }
}
var GroupKeyPress = 0;
window.SendGroupMessage = function (e, GroupID) {
    var msg = $("#txt_chat_msg_Group_" + GroupID).val();
    var userID = window.userInformation.UserID;

    if (GroupKeyPress++ % 10 == 0) {
        chat.server.sendGroupIsTypingMsg(GroupID, userID, false);
    }
    if (e.which == 13 || e.keyCode == 13) { // enter hit
        chat.server.sendGroupMsg(msg, GroupID, userID, "text");
        e.preventDefault(); // to prevent the page from posting back 
        return false;
    }

}
window.SendGroupMessageBackSpace = function (e, GroupID) {
    var msg = $("#txt_chat_msg_Group_" + GroupID).val();
    var userID = window.userInformation.UserID;
    if ((e.which == 8 || e.keyCode == 8) && (msg==false) ) { // backspace is hit 
        chat.server.sendGroupIsTypingMsg(GroupID, userID, true);
    }

 }
chat.client.isTypingMessageReceived = function (UserID, message, IsBackSpace) {
    var div = $("#inline_chat_msg_div_" + UserID);
    if (!div.length) {
        if (!IsBackSpace) {
            $("#Chat_Body_Container_" + UserID).append("<table width='100%' id='inline_chat_msg_div_" + UserID + "'><tr><td align='left'><div  class='inline_chat_msg_div' >" + message + "</div></td></tr></table>")
        }

        scrollDivToBottom("Chat_Body_Container_" + UserID);

    }
    else {
        if (IsBackSpace)
        { div.remove(); }
    }

}


chat.client.MessageReceived = function (message, UserID, FriendID, FriendPic, Date, Username) {

    var SmallHiddendiv = $("#smallChatContainer_" + UserID);
    var isTypingDiv = $("#inline_chat_msg_div_" + UserID);
    var UploadMsgDiv = $("#Upload_msg_Div_" + FriendID);
    var SenderId = window.userInformation.UserID;
    var ToAppend = "";
    if (FriendPic == "") {
        ToAppend += "<table width='100%'><tr><td>"
        + " <div class='chatbubble tooltip'  title='" + Date + "' >"
                                 + "<p>" + message
        //                       + "<div class='time'>&nbsp;&nbsp;<span>" + Date + "</span></div>"
                                 + "</p>"
                                 + "</div>"
                                 + "</td></tr></table>";
       
        $("#Chat_Body_Container_" + FriendID).append(ToAppend);
        scrollDivToBottom("Chat_Body_Container_" + FriendID);
        $("#txt_chat_msg_" + FriendID).val("");


    }
    else {
        ToAppend += "<table width='100%'><tr><td>"
        + "<div class='chatbubble chatbubble-alt white tooltip' title='" + Date + "' >"
                                 + "<p>" + message
        //                       + "<div class='time'>&nbsp;&nbsp;<span>" + Date + "</span></div>"
                                 + "</p>"
                                 + "</div>"
                                 + "</td></tr></table>";
        if ($("#Chat_Body_Container_" + UserID).length > 0) {
            if (isTypingDiv.length) {
                isTypingDiv.remove();
            }

            $("#Chat_Body_Container_" + UserID).append(ToAppend);
            scrollDivToBottom("Chat_Body_Container_" + UserID);
            $("#txt_chat_msg_" + UserID).val("");
        }
        else {
            if (!SmallHiddendiv.length) {
                CreateChatWindow(UserID, Username, FriendPic);
                setTimeout(function () {
                    $("#Chat_Header_dv_" + UserID).addClass("animate");
                }, 1000);
                flashDocumentTitle(document.title, Username + " send you a message");
            }
        }

        // insert Notification Sound
        $('body').append("<embed src='Scripts/Notification/NotificationSound.wav' hidden='true' autostart='true' loop='false' style='position:absolute;top:-1000px;'>");
    }
    Tipped.create('.tooltip');
    if (UploadMsgDiv.length) {
        UploadMsgDiv.remove();
    }

    if (SmallHiddendiv.length) {
        GlobalOfflineMsgCount++;
        $(".GlobalOfflineMsg").show();
        $("#offline_msg_" + UserID).show();

        $(".GlobalOfflineMsg").html(GlobalOfflineMsgCount);
        var userNotCount = $("#offline_msg_" + UserID).html() == "" ? 0 : parseInt($("#offline_msg_" + UserID).html());
        $("#offline_msg_" + UserID).html(userNotCount + 1);

        SmallHiddendiv.append("<span class='lst_msg'>"+message+" "+Date+"</span>");
    }

}



///////////////////////////////////////////////// End Send receive messages //////////////////////////////////////////////////////////////////////

window.logout = function () {
    chat.server.ondisconnected();
    setTimeout(function () { window.location = "LoginPage.aspx?action=logout" }, 250);
}

chat.client.OnUserDisconected = function (FriendID, FriendUserName) {
    var Msg = FriendUserName + " is offline";
    if ($("#Chat_Body_Container_" + FriendID).length > 0) {
        $("#Chat_Body_Container_" + FriendID).append("<div id='inline_chat_msg_div_" + FriendID + "' class='inline_chat_msg_div' >" + Msg + "</div>")
        scrollDivToBottom("Chat_Body_Container_" + FriendID);
        $("#txt_chat_msg_" + FriendID).prop('disabled', true); // disable typing when user is offline
        $("#txt_chat_msg_" + FriendID).val("you can't send message while offline");
    }
}
/////////////////// Notifications /////////////////////////////////////////////////////////////////////////////////////////////////////

chat.client.NotifyUserOnline = function (userID, user_UserName,user_pic) {
    var InlineMsgdiv = $("#inline_chat_msg_div_" + userID)
    if (InlineMsgdiv.length > 0) {
        InlineMsgdiv.remove();
        $("#txt_chat_msg_" + userID).val("");
        $("#txt_chat_msg_" + userID).prop('disabled', false); // enable typing when user is online
    }
    var Text ="<div onclick=\"NotificationClick("+userID+",'"+user_UserName+"','"+user_pic+"')\" > <img src='"+user_pic+"' width='32px' height='32px' class='tooltip' title='"+user_UserName+"'  />&nbsp;&nbsp;<span style='position: relative;top: -8px;'>"+ user_UserName + " is online!!</span></div>";
    appendNotification(Text);
}
window.appendNotification = function (Text) {

    var n = noty({
        text: Text,
        type: "alert",
        dismissQueue: true,
        layout: 'bottomLeft',
        theme: 'defaultTheme',
        timeout: 5000
    });

}
window.NotificationClick=function(friendid, Friendusername, friendpic) {

setTimeout(function(){
     CreateChatWindow(friendid, Friendusername, friendpic)
    },500);
}


//////////////////////////////End Notifications////////////////////////////////////////////////////////////////////////////////////////////////////

///////////////////////////// Useful Functions////////////////////////////////////////////////////////////////////////////////////////////////////

var flashDocument;
window.flashDocumentTitle = function (pageTitle, newMessageTitle) {
    flashDocument=setInterval(function () {
        if (document.title == pageTitle) {
            document.title = newMessageTitle;
        }
        else {
            document.title = pageTitle;
        }
    }, 800);
}

window.scrollDivToBottom = function (divID) {
    if ($("#" + divID).length)
        $("#" + divID).scrollTop($("#" + divID)[0].scrollHeight);
}
window.closeChatwnd = function (sender) {
    $("#" + sender).remove();
    $("#emoji_bubble").css("display", "none");
    $("#Options_bubble").css("display", "none");
}


// show a loader image when file is being uploaded
window.ShowLoaderImage = function (FriendID) {
    var container = $("#Msg_container_dv_" + FriendID);
    container.append("<div class='Upload_msg_Div' id='Upload_msg_Div_" + FriendID + "'>Hi</div>");
    // $("#loader").css("display", "block");
}
window.Start_New_Chat = function () {
    $("#Start_NewChat_Modal").show();
    $("#CreateGroup_dv").hide();
    $("#txt_autoComplete").autocomplete({

        source: function (request, response) {
            $.ajax({
                type: 'POST',
                url: 'ChatPage.aspx/GetUserFriendListInformationForAutoComplete',
                data: '{txt:"' + request.term + '"}',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (data) {
                    response($.map(data.d, function (item) {
                        return {
                            label: item.USER_USERNAME,
                            value: item.USER_ID,
                            icon: item.USER_LOGO
                        }
                        // return Template;
                    }))
                    //debugger;
                },
                error: function (result) {
                    alert("Error");
                }
            });
        },
        minLength: 1,
        select: function (event, ui) {
            CreateChatWindow(ui.item.value, ui.item.label, ui.item.icon);
            $("#Start_NewChat_Modal").hide();
            $("#txt_autoComplete").val("");
            return false;
        }
    }).autocomplete("instance")._renderItem = function (ul, item) {
        var Result = "<div id='smallChatContainer_" + item.value + "' >"
                                      + "<div class='smallChatContainer'>"
                                      + "<img src='" + item.icon + "' width='25' height='25' />"
                                      + "<span>" + item.label + "</span>"
                                      + "</div>"
                                      + "</div><br>";
        return $('<li>')
        .data('item.autocomplete', item)
        .append(Result)
        .appendTo(ul);
    };

}
//////////////////////////////////////////////END Useful Functions /////////////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////Groups///////////////////////////////////////////////////////////////////////////////////////////////

window.init_Group = function () {
    $("#CreateGroup_dv").show();
    $("#Start_NewChat_Modal").hide();

    $("#txt_GroupautoComplete").autocomplete({

        source: function (request, response) {
            $.ajax({
                type: 'POST',
                url: 'ChatPage.aspx/GetUserFriendListInformationForAutoComplete',
                data: '{txt:"' + request.term + '"}',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (data) {
                    response($.map(data.d, function (item) {
                        return {
                            label: item.USER_USERNAME,
                            value: item.USER_ID,
                            icon: item.USER_LOGO
                        }
                        // return Template;
                    }))
                    //debugger;
                },
                error: function (result) {
                    alert("Error");
                }
            });
        },
        minLength: 1,
        select: function (event, ui) {
            var Res = "<div id='smallChatContainer_group_" + ui.item.value + "' class='groupParticp'>"
                                      + "<div class='smallChatContainer'>"
                                      + "<img src='" + ui.item.icon + "' width='25' height='25' />"
                                      + "<span>" + ui.item.label + "</span>"
                                      + "</div>"
                                      + "</div><br>";
            
            $("#Participants").append(Res)
            $("#txt_GroupautoComplete").val("");
            return false;
        }
    }).autocomplete("instance")._renderItem = function (ul, item) {
        var Result = "<div>"
                    + "<div class='smallChatContainer'>"
                    + "<img src='" + item.icon + "' width='25' height='25' />"
                    + "<span>" + item.label + "</span>"
                    + "</div>"
                    + "</div><br>";
        var dv=$("#smallChatContainer_group_"+item.value);
        if(!dv.length) // if the user didn't yet select the friend
        return $('<li>')
        .data('item.autocomplete', item)
        .append(Result)
        .appendTo(ul);
    };

}
window.GetGroupParticipent = function () {
    var ids = "";
    $("#Participants").children(".groupParticp").each(function () {
        var child = $(this);
        var id = child.attr("id").replace(/^\D+/g, '');
        ids += id + ",";
    });
    return ids;
}
window.Create_Group = function (sender, GroupID) {
    var GroupName = $("#txt_GroupTitle").val() == "" ? "undefined" : $("#txt_GroupTitle").val();
    $("#CreateGroup_dv").hide();
    var PartchiddenElemnt = sender.getElementsByTagName("input")[1];
    var ParticipentsIds = "";
    if (PartchiddenElemnt != null) { ParticipentsIds = PartchiddenElemnt.value; }
    else {
        ParticipentsIds = GetGroupParticipent();
    }
    $("#Participants").html("");
    if (GroupID != -1) { // Show Group
        var GroupNamehiddenElemnt = sender.getElementsByTagName("input")[0];
        Show_Group(GroupID, GroupNamehiddenElemnt.value);

        chat.server.addgroup(GroupID, GroupNamehiddenElemnt.value, ParticipentsIds);
        return;
    }
    $.ajax({
        type: 'POST',
        url: 'ChatPage.aspx/CreateGroup',
        data: '{GroupName:"' + GroupName + '",ParticpentsIds:"' + ParticipentsIds + '"}',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (data) {
            var GroupID = data.d;

            ToAppend = "<div class='item' onclick=\"Create_Group(this,"+GroupID+")\">"
                              + "<img class='tooltip' src='Styles/Group.png' alt='" + GroupName + "' title='" + GroupName + "' />"
                              + "<input type='hidden' id='hf_GroupName_Group_'" + GroupID + "' value='" + GroupName + "' />";
                              + "<input type='hidden' id='hf_Group_" + GroupID + "' value='" + ParticipentsIds.slice(0, -1) + "' />"
                              +"</div>";
            $("#dv_FriendList").owlCarousel().data('owlCarousel').addItem(ToAppend);
            Show_Group(GroupID, GroupName);
            chat.server.addgroup(GroupID, GroupName, ParticipentsIds);
        },
        error: function (xhr) {
            alert("responseText: " + xhr.responseText);
        }
    });
}
window.Show_Group = function (GroupID, GroupName) {
    if ($("#Msg_container_dv_Group_" + GroupID).length > 0) return;
    var UserId = window.userInformation.UserID;
    var UserPicture = window.userInformation.UserLogo;
    $.ajax({
        type: 'POST',
        url: 'ChatPage.aspx/ShowGroup',
        data: '{GroupID:"' + GroupID + '"}',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (data) {
            var ToAppend = "<div class='Msg_container_dv' id='Msg_container_dv_Group_" + GroupID + "' style='margin-right: 8px;' >"
    + "<div class='Chat_Header_dv' id='Chat_Header_dv_Group_" + GroupID + "'>"
    + "<table width='100%'><tr>"
    + "<td style='padding-left:2px;padding-top:3px; width:16px'>"
    + "<img class='tooltip' src='../Styles/Group.png' title='" + GroupName + "' width='25px' height='25px' style='margin-top: -2px;' />"
    + "</td>"
    + "<td align='left' valign='centre'>"
    + "<a  href='javascript:void(0)'   >" + GroupName + "</a>"
    + "</td>"
    + "<td align='right' width='23px'  style='vertical-align:middle;padding-right:2px;'><span class='settings_btn' title='options' onclick='OpenOptionsContainer(\"Msg_container_dv_Group_" + GroupID + "\",\"txt_chat_msg_Group_" + GroupID + "\"," + GroupID + ",-1,-1);' ></span></td>"
    + "<td align='right' width='23px'  style='vertical-align:middle;padding-right:4px;'><span class='close_btn' title='close window' onclick='closeChatwnd(\"Msg_container_dv_Group_" + GroupID + "\");' ></span></td>"
    + "</tr></table>"
    + "</div>"

    + "<div class='Chat_Body_Container' id='Chat_Body_Container_Group_" + GroupID + "'>"
    + data.d
    + "</div>"

    + "<div class='Chat_Footer_Container' id='Chat_Footer_Container_Group_" + GroupID + "'>"
    + "<textarea id='txt_chat_msg_Group_" + GroupID + "' onkeypress='SendGroupMessage(event," + GroupID + ");' onkeyup='SendGroupMessageBackSpace(event,"+GroupID+")' onfocus='txt_chat_msg_Focus(\"Chat_Header_dv_Group_" + GroupID + "\");' ></textarea>"
    + "<div class='emoji tooltip' title='emoji' onclick='openEmojiContainer(\"Msg_container_dv_Group_" + GroupID + "\",\"txt_chat_msg_Group_" + GroupID + "\");' /></div>"

    + "</div>";


            $("#chatContainer").append(ToAppend);
            scrollDivToBottom("Chat_Body_Container_Group_" + GroupID);
            Tipped.create('.tooltip');
            $('#txt_chat_msg_Group_' + GroupID).elastic();

        },
        error: function (xhr) {
            alert("responseText: " + xhr.responseText);
        }
    });

}

window.ViewGroupParticipant = function () {
    // to do 

}


chat.client.isTypingGroupMessageReceived = function (GroupID, Userid, message, IsBackSpace) {
    if (Userid == userInformation.UserID) return;
    var div = $("#inline_chat_msg_div_Group_" + GroupID);
    if (!div.length) {
        if (!IsBackSpace) {
            $("#Chat_Body_Container_Group_" + GroupID).append("<table width='100%'  id='inline_chat_msg_div_Group_" + GroupID + "'><tr><td align='left'><div  class='inline_chat_msg_div' >" + message + "</div></td></tr></table>")
        }

        scrollDivToBottom("Chat_Body_Container_Group_" + GroupID);

    }
    else {
        if (IsBackSpace)
        { div.remove(); }
    }

}

chat.client.GroupMessageReceived = function (message, UserID, GroupID, FriendPic, Date, Username) {

    var SmallHiddendiv = $("#smallChatContainer_Group_" + GroupID);
    var isTypingDiv = $("#inline_chat_msg_div_Group_" + GroupID);
    var UploadMsgDiv = $("#Upload_msg_Div_Group_" + GroupID);
    var SenderId = window.userInformation.UserID;
    var ToAppend = "";
    if (FriendPic == "") { // apend message to the user chat window
        ToAppend += "<table width='100%'><tr><td>"
        + " <div class='chatbubble tooltip'  title='" + Date + "' >"
                                 + "<p>" + message
        //                       + "<div class='time'>&nbsp;&nbsp;<span>" + Date + "</span></div>"
                                 + "</p>"
                                 + "</div>"
                                 + "</td></tr></table>";

        $("#Chat_Body_Container_Group_" + GroupID).append(ToAppend);
        scrollDivToBottom("Chat_Body_Container_Group_" + GroupID);
        $("#txt_chat_msg_Group_" + GroupID).val("");


    }
    else { // append msg to the friend chat wnd 
        ToAppend += "<table width='100%'><tr><td>"
        + "<div class='chatbubble chatbubble-alt white tooltip' title='" + Date + "' >"
         + "<p class='GroupUsername' >" + Username + "</p>"
                                 + "<p>" + message
        //                       + "<div class='time'>&nbsp;&nbsp;<span>" + Date + "</span></div>"
                                 + "</p>"
                                 + "</div>"
                                 + "</td></tr></table>";
        if ($("#Chat_Body_Container_Group_" + GroupID).length > 0) {
            if (isTypingDiv.length) {
                isTypingDiv.remove();
            }
            
            $("#Chat_Body_Container_Group_" + GroupID).append(ToAppend);
            scrollDivToBottom("Chat_Body_Container_Group_" + GroupID);
            $("#txt_chat_msg_Group_" + GroupID).val("");
        }
        else {
            if (!SmallHiddendiv.length) {
                //CreateGroup(UserID, Username, FriendPic);
                setTimeout(function () {
                    $("#Chat_Header_dv_Group_" + GroupID).addClass("animate");
                }, 1000);
                flashDocumentTitle(document.title, Username + " send you a message");
            }
        }

        // insert Notification Sound
        $('body').append("<embed src='Scripts/Notification/NotificationSound.wav' hidden='true' autostart='true' loop='false' style='position:absolute;top:-1000px;'>");
    }
    Tipped.create('.tooltip');
    if (UploadMsgDiv.length) {
        UploadMsgDiv.remove();
    }
    // check if group is hidden 
    if (SmallHiddendiv.length) {
        GlobalOfflineMsgCount++;
        $(".GlobalOfflineMsg").show();
        $("#offline_msg_Group_" + GroupID).show();

        $(".GlobalOfflineMsg").html(GlobalOfflineMsgCount);
        var userNotCount = $("#offline_msg_Group_" + GroupID).html() == "" ? 0 : parseInt($("#offline_msg_Group_" + GroupID).html());
        $("#offline_msg_Group_" + GroupID).html(userNotCount + 1);

        SmallHiddendiv.append("<span class='lst_msg'>" + message + " " + Date + "</span>");
    }

}
///////////////////////////// End Useful Functions////////////////////////////////////////////////////////////////////////////////////////////////////

///////////////////////////// cookies Functions////////////////////////////////////////////////////////////////////////////////////////////////////
/*
window.createCookie=function(name, value, days) {
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        var expires = "; expires=" + date.toGMTString();
    }
    else var expires = "";
    document.cookie = name + "=" + value + expires + "; path=/";
}

window.readCookie=function(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

window.eraseCookie=function(name) {
    createCookie(name, "", -1);
}
*/

///////////////////////////// End cookies Functions////////////////////////////////////////////////////////////////////////////////////////////////////