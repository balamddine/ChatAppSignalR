using System;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
namespace SignalRChat
{
    public class ChatHub : Hub  
    {
        private static List<User> _Users_lst= new List<User>() ;
        public static List<User> Users_lst { get { return _Users_lst; } set { _Users_lst = value; } }

        private static List<Groups> _Groups_lst = new List<Groups>();
        public static List<Groups> Groups_lst { get { return _Groups_lst; } set { _Groups_lst = value; } }

        public string [,] emojiShortCut = new string [,] {
{"^-^", ">:(", "=(", "=@", "(y)", "(=note)", "(=journal)", "(=share)", "(=group)", "=)",  },
{"(=angry)", "(=sad)", "<:(", "=p", "=*", "<-*)", "=D", "=k", "(=cry1)", "(=sad1)",  },
{"(=-k)", "<&>", "(=sad)", "(=disa)", "(=<p)", "(h5)", "(=hope)", "(=sad3)", "(=Finger)", "(=Sun)",  },
{"", "", "", "", "", "", "", "", "", "", },
{"", "", "", "", "", "", "", "", "", "", },
{"", "", "", "", "", "", "", "", "", "",},
{"", "", "", "", "", "", "", "", "", "", },
{"", "", "", "", "", "", "", "", "", "", },
{"", "", "", "", "", "", "", "", "", "", },
{"", "", "", "", "", "", "", "", "", "",},
{"", "", "", "", "", "", "", "", "", "",},
{"", "", "", "", "", "", "", "", "", "",},
{"", "", "", "", "", "", "", "", "", "",},
{"", "", "", "", "", "", "", "", "", "", },
{"", "", "", "", "", "", "", "", "", "",},
{"", "", "", "", "", "", "", "", "", "",}
        };
        #region Messages Handling
        public void sendMsg(string message,string FriendID,string UserID,string type)
        {
            if (message.Trim() == "" || string.IsNullOrEmpty(message) || message.Replace("<br/>", "") == "") return;
                SendMsgToClient(message, FriendID, UserID); 
        }
        public void SendMsgToClient(string message, string FriendID, string UserID)
        {
            message = ReplaceMsgByEmojiImageIfExist(message);
            message = ReplaceMsgHyperLinkIfExist(message);
            User user = Users_lst.Find(x => x.USER_ID == UserID);
            User Friend = Users_lst.Find(x => x.USER_ID == FriendID);
            InsertMsgToDataBase(message, FriendID, UserID,false,"");
            if (Friend != null && user != null)
            {

                try
                {
                    Clients.Client(Friend.USER_CONTEXT).MessageReceived(message, UserID, "", user.USER_LOGO, DateTime.Now.ToString("hh:mm:tt"), user.USER_USERNAME);
                    Clients.Client(user.USER_CONTEXT).MessageReceived(message, "", FriendID, "", DateTime.Now.ToString("hh:mm:tt"), "");
                }
                catch (Exception ex)
                {

                }
            }
            else if (Friend == null && user != null) // sending an offline Message
            {

                try
                {
                    Clients.Client(user.USER_CONTEXT).MessageReceived(message, "", FriendID, "", DateTime.Now.ToString("hh:mm:tt"), "");
                }
                catch (Exception ex)
                {

                }
            }
            else if (Friend == null && user == null)
            {
                HttpContext.Current.Response.Redirect("~/LoginPage.aspx");
            }
        }
        public void sendIsTypingMsg(string FriendID, string UserID,bool IsBackSpace)
        {
            User Friend = Users_lst.Find(x => x.USER_ID == FriendID);
            User user = Users_lst.Find(x => x.USER_ID == UserID);
            if (Friend != null)
            { 
               try
                {
                    string message =  user.USER_USERNAME + " is typing...";
                    Clients.Client(Friend.USER_CONTEXT).isTypingMessageReceived(UserID, message,IsBackSpace);
                }
                catch (Exception ex)
                {

                }
            }
        }
        #endregion

        #region User Handling
        public void adduser(string userid)
        {
            string query = "select users.*,[status].STATUS_DESCRIPTION from users  inner join [status] on [status].STATUS_NAME=users.USER_STATUS_ID "
                             + " where users.[USER_ID]=@USER_ID";
            List<SqlParameter> List = new List<SqlParameter>();
            List.Add(new SqlParameter("@USER_ID", userid));
            DataTable UserDt = new SQLHelper(SQLHelper.ConnectionStrings.WebSiteConnectionString).getQueryResultWithParameters(query, List.ToArray());
            User User = new User(Context.ConnectionId);
            User.USER_ID = userid;
            User.USER_CONTEXT = Context.ConnectionId;
            User.STATUS_DESCRIPTION = UserDt.Rows[0]["STATUS_DESCRIPTION"].ToString();
            User.USER_EMAIL = UserDt.Rows[0]["USER_EMAIL"].ToString();
            byte[] imageFile = UserDt.Rows[0]["USER_LOGO"] as byte[];
            string Logo = ImagingTools.ConvertBinImageToFile(imageFile, "images/ProfilePictures/", "img_" + UserDt.Rows[0]["USER_USERNAME"].ToString() + ".png");
            User.USER_LOGO = Logo;
            User.USER_MOOD = UserDt.Rows[0]["USER_MOOD"].ToString();
            User.USER_USERNAME = UserDt.Rows[0]["USER_USERNAME"].ToString();
            Users_lst.Add(User);
            List<string> OnlineFriendList = GetuserOnlineFriendList();
            if (OnlineFriendList.Count > 0)
            {
                if (OnlineFriendList != null)
                {
                    foreach (string item in OnlineFriendList)
                    {
                        User Friend = Users_lst.Find(x => x.USER_ID == item);
                        if (Friend != null)
                        {
                            Clients.Client(Friend.USER_CONTEXT).NotifyUserOnline(User.USER_ID,User.USER_USERNAME,User.USER_LOGO);
                        }
                    }
                }
            }
        }
        public void deleteUser()
        {
            if (FindUser() != null)
            {
                Users_lst.Remove(FindUser());
            }
        }
        public User FindUser()
        { 
           User user = Users_lst.Find(x => x.USER_CONTEXT == Context.ConnectionId);
           return user!=null?user:null;
        }
        public override Task OnDisconnected()
        {
            try
            {
                deleteUser();
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public void ondisconnected()
        {
            List<string> OnlineFriends = GetuserOnlineFriendList();
            User user = FindUser();
            if (OnlineFriends.Count > 0)
            {
                foreach (string FriendID in OnlineFriends)
                {
                    User Friend = Users_lst.Find(x => x.USER_ID == FriendID);
                    Clients.Client(Friend.USER_CONTEXT).OnUserDisconected(user.USER_ID, user.USER_USERNAME);
                }
            }
            deleteUser();
        }
        #endregion

        #region GroupHandling
        public void addgroup(string GroupID, string GroupName, string ParticipentsIds)
        {
            Groups Temp_grp = Groups_lst.Find(x => x.GROUP_ID == GroupID);
            if (Temp_grp != null) return;
            Groups grp = new Groups(GroupID, GroupName);
            List<User> Usrs_lst = new List<User>();
            if (ParticipentsIds != "") {
                string query = "select users.*,[status].STATUS_DESCRIPTION from users  inner join [status] on [status].STATUS_NAME=users.USER_STATUS_ID "
                                +" where USER_ID in ("+ParticipentsIds+")";
                DataTable dt = new SQLHelper(SQLHelper.ConnectionStrings.WebSiteConnectionString).getQueryResult(query);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow Row in dt.Rows)
                    {
                        User usr = new User();
                       
                        usr.USER_ID = Row["USER_ID"].ToString();
                        User Temp_usr = Users_lst.Find(x => x.USER_ID == Row["USER_ID"].ToString());
                        if (Temp_usr != null)
                        {
                            usr.USER_CONTEXT = Temp_usr.USER_CONTEXT; 
                        }
                        usr.STATUS_DESCRIPTION = Row["STATUS_DESCRIPTION"].ToString();
                        usr.USER_EMAIL = Row["USER_EMAIL"].ToString();
                        byte[] imageFile = Row["USER_LOGO"] as byte[];
                        string Logo = ImagingTools.ConvertBinImageToFile(imageFile, "images/ProfilePictures/", "img_" + Row["USER_USERNAME"].ToString() + ".png");
                        usr.USER_LOGO = Logo;
                        usr.USER_MOOD = Row["USER_MOOD"].ToString();
                        usr.USER_USERNAME = Row["USER_USERNAME"].ToString();
                        Usrs_lst.Add(usr);
                    }
                }
            }
            if (Users_lst.Count > 0) {
                grp.USERS = Usrs_lst;
            }
            Groups_lst.Add(grp);
            
        }
        public void sendGroupIsTypingMsg(string GroupID, string UserID, bool IsBackSpace)
        {
            Groups Group = Groups_lst.Find(x => x.GROUP_ID == GroupID);
            User user = Users_lst.Find(x => x.USER_ID == UserID);
            if (Group != null)
            {
                List<User> list_usr = Group.USERS;
                if (list_usr.Count > 0) {
                    foreach (User Friend in list_usr)
                    {
                        if (Friend != null)
                        {
                            try
                            {
                               // if (user.USER_ID != UserID) // send to all user except the sender 
                               // {
                                    string message = user.USER_USERNAME + " is typing...";
                                    if (!string.IsNullOrEmpty(Friend.USER_CONTEXT))
                                        Clients.Client(Friend.USER_CONTEXT).isTypingGroupMessageReceived(GroupID,UserID, message, IsBackSpace);
                               // }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }
            
            }



            
        }
        public void sendGroupMsg(string message, string GroupId, string UserID, string type)
        {
            if (message.Trim() == "" || string.IsNullOrEmpty(message) || message.Replace("<br/>", "") == "") return;
            SendGroupMsgToClient(message, GroupId, UserID);
        }
        public void SendGroupMsgToClient(string message, string GroupId, string UserID)
        {
            message = ReplaceMsgByEmojiImageIfExist(message);
            message = ReplaceMsgHyperLinkIfExist(message);
            User user = Users_lst.Find(x => x.USER_ID == UserID);
            Groups grp = Groups_lst.Find(x => x.GROUP_ID == GroupId);
            InsertMsgToDataBase(message, "", UserID,true,GroupId);
            if (grp != null)
            {

                try
                {
                    List<User> users = grp.USERS;
                    foreach (User usr in users)
                    {
                        if (usr.USER_ID == UserID)
                        {                                                         
                            Clients.Client(user.USER_CONTEXT).GroupMessageReceived(message, "", GroupId, "", DateTime.Now.ToString("hh:mm:tt"), "");
                        }
                        else {
                            Clients.Client(usr.USER_CONTEXT).GroupMessageReceived(message, UserID, GroupId, user.USER_LOGO, DateTime.Now.ToString("hh:mm:tt"), user.USER_USERNAME);
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            else if (grp == null && user != null) // sending an offline Message
            {

                try
                {
                    Clients.Client(user.USER_CONTEXT).MessageReceived(message, "", GroupId, "", DateTime.Now.ToString("hh:mm:tt"), "");
                }
                catch (Exception ex)
                {

                }
            }
            else if (grp == null && user == null)
            {
                HttpContext.Current.Response.Redirect("~/LoginPage.aspx");
            }
        }
        #endregion

        #region User status Handling
        public void notifyStatuschanged(string user, string status)
        {
            List<string> OnlineFriendList = GetuserOnlineFriendList();
            if (OnlineFriendList != null)
            {
                foreach (string item in OnlineFriendList)
                {
                    User Friend = Users_lst.Find(x => x.USER_ID == item);
                    if (Friend != null)
                    {
                        Clients.Client(Friend.USER_CONTEXT).NotifyStatus(user,status);
                    }
                }
            }
        }
        #endregion

        #region function 
        public List<string> GetuserOnlineFriendList()
        {
            User user = FindUser();
            if (user == null) return null;
            string query = "select FRIEND_ID from friends where [USER_ID]="+user.USER_ID+" union select [USER_ID] from friends where FRIEND_ID=" + user.USER_ID;
            List<string> list = new List<string>();
            DataTable dt = new SQLHelper(SQLHelper.ConnectionStrings.WebSiteConnectionString).getQueryResult(query);
            if (dt.Rows.Count > 0) {
                foreach (DataRow row in dt.Rows)
                {
                    User OnlineFriend = Users_lst.Find(x => x.USER_ID == row["FRIEND_ID"].ToString());
                    if(OnlineFriend!=null)
                    list.Add((string)row["FRIEND_ID"].ToString());
                }
            }
            return list;
        }
       
        public string ReplaceMsgByEmojiImageIfExist(string msg)
        {
            int size = 17; int Rows = 15; int cols = 10;
            for (int i = 0; i < Rows; i++) // 16 is the array rows
            {
                int r = i * 10;
                for (int j = 0; j < cols; j++)//15 is the array cols
                {
                    string shtct = emojiShortCut[i, j].ToString();
                    if (msg.Contains(shtct) && shtct != "")
                    {
                        int y = (r + j) * size - 1;
                        var image = "<span class='emojiIcon' style='background-position:0px -" + y + "px;'></span>";
                        msg = msg.Replace(shtct, image);
                    }
                }
            }
            return msg;
        }
        public string ReplaceMsgHyperLinkIfExist(string msg)
        {
            Regex r = new Regex(@"((?:(?:https?|ftp|gopher|telnet|file|notes|ms-help):(?://|\\\\)(?:www\.)?|www\.)[\w\d:#@%/;$()~_?\+,\-=\\.&]+)");
            Match m = r.Match(msg);
            string val = "";
            string[] args = { "http", "https", "ftp", "gopher", "telnet", "file", "notes", "ms-help" };
            bool contains = false;
            while (m.Success)
            {
                foreach (string s in args)
                {
                    if (m.Value.Contains(s))
                    {
                        contains = true;
                    }
                }
                if (!contains)
                    val = " <a href='http://" + m.Value + "' title='" + m.Value + "' target='_blank'>" + m.Value + "</a>";
                else
                    val = " <a href='" + m.Value + "' title='" + m.Value + "' target='_blank'>" + m.Value + "</a>";
                msg = msg.Replace(m.Value, val);
                m = m.NextMatch();
            }
            return msg;
        }
        public void InsertMsgToDataBase(string message, string FriendID, string UserID,bool isGroup,string GroupID)
        {
            if (!isGroup)
            {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@MESSAGE", message));
            param.Add(new SqlParameter("@USER_ID", UserID));
            param.Add(new SqlParameter("@FRIEND_ID", FriendID));
            
                string InsertQuery = " INSERT INTO [messages]([MESSAGE_BODY],[MESSAGE_SENDER_ID],[MESSAGE_RECEIVER_ID])VALUES(@MESSAGE,@USER_ID,@FRIEND_ID)";
                new SQLHelper(SQLHelper.ConnectionStrings.WebSiteConnectionString).InsertWithParameters(InsertQuery, param.ToArray());
            }
            else {
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@MESSAGE", message));
                param.Add(new SqlParameter("@USER_ID", UserID));
                param.Add(new SqlParameter("@GROUP_ID", GroupID));
                new SQLHelper(SQLHelper.ConnectionStrings.WebSiteConnectionString).getProcedureResult("SendGroupMessage", param.ToArray());
            }
        }

        public  bool IsBase64(string base64String)
        {
            if (base64String.Replace(" ", "").Length % 4 != 0)
            {
                return false;
            }

            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch (Exception exception)
            {
                return false;
            }
            
        }
        #endregion

       
    }
}