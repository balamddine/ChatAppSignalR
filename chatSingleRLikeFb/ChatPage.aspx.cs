using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Data;
using System.IO;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

public partial class ChatPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        GetUserInformations();
       
    }

    #region User
    public void GetUserInformations()
    {
        if (Session["USER_INFO"] == null) Response.Redirect("LoginPage.aspx");
        DataRow Row = ((DataTable)Session["USER_INFO"]).Rows[0];

        byte[] imageFile = Row["USER_LOGO"] as byte[];
        string Logo = ImagingTools.ConvertBinImageToFile(imageFile, "images/ProfilePictures/", "img_" + Row["USER_USERNAME"].ToString() + ".png");
        UseInfo_hf.Value += Row[0].ToString() + //id
                       "," + Row[2].ToString() +//name
                       "," + Row[5].ToString() +//email
                       "," + Logo +//logo
                       "," + Row[6].ToString() +//mood
                       "," + Row[7].ToString() +//statusid
                       "," + Row[9].ToString();//status desc
    }
    [WebMethod(EnableSession = true)]
    public static List<User> GetUserFriendListInformation(string UserID)
    {
        

        string query = " select u.*,s.STATUS_DESCRIPTION "
              +" from users u "
		      +" inner join friends f on u.[USER_ID] =f.FRIEND_ID "
              +" inner join [status] s on s.STATUS_NAME=u.USER_STATUS_ID "
              +" where f.[USER_ID]="+UserID
              +" union "
              +" select u.*,s.STATUS_DESCRIPTION "
              +            " from users u "
              +		      " inner join friends f on u.[USER_ID] =f.[USER_ID] "
              +            " inner join [status] s on s.STATUS_NAME=u.USER_STATUS_ID "
              +            " where f.FRIEND_ID="+UserID;
        DataTable dt = new SQLHelper(SQLHelper.ConnectionStrings.WebSiteConnectionString).getQueryResult(query);
        List<User> users = new List<User>();
        if (dt.Rows.Count > 0)
        {
            
            foreach (DataRow Row in dt.Rows)
            {
                byte[] imageFile = Row["USER_LOGO"] as byte[];
                string Logo = ImagingTools.ConvertBinImageToFile(imageFile, "images/ProfilePictures/", "img_" + Row["USER_USERNAME"].ToString() + ".png");
                User Friend = new User(Row[0].ToString(), Row[2].ToString(), Row[5].ToString(), Logo, Row[6].ToString(), Row[7].ToString(), Row[9].ToString());
                users.Add(Friend);
            }
        }
        return users;
    }
    [WebMethod(EnableSession = true)]
    public static List<User> GetUserFriendListInformationForAutoComplete(string txt)
    {
        try
        {
            string UserID = ((DataTable)HttpContext.Current.Session["USER_INFO"]).Rows[0]["USER_ID"].ToString();
            string query = " select u.*,s.STATUS_DESCRIPTION "
            + " from users u "
            + " inner join friends f on u.[USER_ID] =f.FRIEND_ID "
            + " inner join [status] s on s.STATUS_NAME=u.USER_STATUS_ID "
            + " where f.[USER_ID]=" + UserID + " AND u.USER_USERNAME like '%"+txt+"%'"
            + " union "
            + " select u.*,s.STATUS_DESCRIPTION "
            + " from users u "
            + " inner join friends f on u.[USER_ID] =f.[USER_ID] "
            + " inner join [status] s on s.STATUS_NAME=u.USER_STATUS_ID "
            + " where f.FRIEND_ID=" + UserID + " AND u.USER_USERNAME like '%" + txt + "%'";
            DataTable dt = new SQLHelper(SQLHelper.ConnectionStrings.WebSiteConnectionString).getQueryResult(query);
            List<User> users = new List<User>();
            if (dt.Rows.Count > 0)
            {

                foreach (DataRow Row in dt.Rows)
                {
                    byte[] imageFile = Row["USER_LOGO"] as byte[];
                    string Logo = ImagingTools.ConvertBinImageToFile(imageFile, "images/ProfilePictures/", "img_" + Row["USER_USERNAME"].ToString() + ".png");
                    User Friend = new User(Row[0].ToString(), Row[2].ToString(), Row[5].ToString(), Logo, Row[6].ToString(), Row[7].ToString(), Row[9].ToString());
                    users.Add(Friend);
                }
            }
            return users;
        }
        catch (Exception ex)
        {
            return null;
        }

    }
    [WebMethod(EnableSession = true)]
    public static string GetUserChatConversation(string FriendID, string FriendName, string FriendPic, string UserPicture)
    {
        try
        {
            string UserID = ((DataTable)HttpContext.Current.Session["USER_INFO"]).Rows[0]["USER_ID"].ToString();
            string ToAppend = "";
            string query = "SELECT * FROM [messages] "
                   + " WHERE ( (MESSAGE_SENDER_ID="+UserID+" and MESSAGE_RECEIVER_ID="+FriendID+"  ) OR (MESSAGE_SENDER_ID="+FriendID+" and MESSAGE_RECEIVER_ID="+UserID+" ) ) "
                   + " AND ( MESSAGE_VISIBLE_TO="+UserID+" OR MESSAGE_VISIBLE_TO IS NULL OR MESSAGE_VISIBLE_TO<>-1 AND MESSAGE_VISIBLE_TO<>"+FriendID+")";
            DataTable Chat_dt = new SQLHelper(SQLHelper.ConnectionStrings.WebSiteConnectionString).getQueryResult(query);
            if (Chat_dt != null && Chat_dt.Rows.Count > 0)
            {
                foreach (DataRow row in Chat_dt.Rows)
                {
                    if (row["MESSAGE_SENDER_ID"].ToString() == UserID)
                    {
                        ToAppend +="<table width='100%'><tr><td>"
                                 +" <div class='chatbubble tooltip'  title='" + Convert.ToDateTime(row["MESSAGE_CREATION_DATE"]).ToString("hh:mm:tt") + "' >"
                                 + "<p>" + row["MESSAGE_BODY"].ToString() 
                                 //+ "<div class='time'>&nbsp;&nbsp;<span>" + Convert.ToDateTime(row["MESSAGE_CREATION_DATE"]).ToString("hh:mm:tt") + "</span></div>"
                                 + "</p>"
                                 + "</div>"
                                 +"</td></tr></table>";
                                 
                    }
                    else
                    {
                        ToAppend += "<table width='100%'><tr><td>"
                                 +"<div class='chatbubble chatbubble-alt white tooltip' title='" + Convert.ToDateTime(row["MESSAGE_CREATION_DATE"]).ToString("hh:mm:tt") + "' >"
                                 + "<p>" + row["MESSAGE_BODY"].ToString() 
                                 //+"<div class='time'>&nbsp;&nbsp;<span>" + Convert.ToDateTime(row["MESSAGE_CREATION_DATE"]).ToString("hh:mm:tt") + "</span></div>"                             
                                 +"</p>"
                                 + "</div>"
                                 + "</td></tr></table>";
                                
                    }
                }// end foreach
            } // end if 
            return ToAppend;
        }
        catch (Exception ex)
        {
            HttpContext.Current.Response.Redirect("LoginPage.aspx");
            return "";
        }
    }
    #endregion

    #region function
    [WebMethod(EnableSession = true)]
    public static void CLearChatWindow(string FriendID)
    {
        string UserID = ((DataTable)HttpContext.Current.Session["USER_INFO"]).Rows[0]["USER_ID"].ToString();
        List<SqlParameter> param = new List<SqlParameter>();
        param.Add(new SqlParameter("@SENDER_ID",UserID));
        param.Add(new SqlParameter("@RECEIVER_ID",FriendID));
        new SQLHelper(SQLHelper.ConnectionStrings.WebSiteConnectionString).getProcedureResult("ClearChatHistory", param.ToArray());
    }
    [WebMethod(EnableSession = true)]
    public static string SaveFileMsg(string fileData,string type)
    {
        Regex r = new Regex("^data:(.*?);base64,(.*?)$");
        Match m = r.Match(fileData);
        if (m.Success)
        {
            string mimetype = m.Groups[1].Value;
            string Base64Data = m.Groups[2].Value;
            string msgpath= SaveFileToFolder(Base64Data,type,mimetype);
            FileInfo info = new FileInfo(msgpath);
            switch(info.Extension){
                case ".png": return "<a href='"+msgpath+"' target='_blank'><img src='"+ msgpath+"' width='100px' height='100px' /></a>";
                case ".mp4": return "<a href='" + msgpath + "' target='_blank' download  ><span class='download_dv' title='download'></span></a>"
                                      + " <video  class='video' onclick='javascript:if(this.paused) this.play();else this.pause();' style='cursor:pointer;margin-left: -10px;'>"
                                      + "<source src='" + msgpath + "' type='video/mp4' >"
                                      +"Your browser does not support the video tag."
                                      + "</video>";
                case ".mp3": return "<a href='" + msgpath + "' target='_blank' download  ><span class='download_dv' title='download'></span></a>"
                                      + " <audio controls  class='audio'  style='cursor:pointer;margin-left: -10px;'>"
                                      + "<source src='" + msgpath + "' type='audio/mp3' >"
                                      + "Your browser does not support the video tag."
                                      + "</audio>";
            }
        }
        return "-1";
    }
    public static string SaveFileToFolder(string FileData,string type,string mime)
    {
        string path=""; 
        string fileNameWitPath="";
        string smallPath = "";
        string Extension = "";
        string fileName = DateTime.Now.ToString().Replace("/", "-").Replace(" ", "- ").Replace(":", "");
        if (type.Contains("image"))
        {
            smallPath = "images/uploads/Pictures/";
            Extension = ".png";
        }
        else if (type.Contains("video"))
        {
            smallPath = "images/uploads/Videos/";
            Extension = ".mp4";
        }
        else if(type.Contains("audio"))
        {
            smallPath = "images/uploads/Audio/";
            Extension = ".mp3";
           
        }
        path = HttpContext.Current.Server.MapPath(smallPath);
        fileNameWitPath = path + fileName + Extension;
        using (FileStream fs = new FileStream(fileNameWitPath, FileMode.Create))
        {
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                byte[] data = Convert.FromBase64String(FileData);
                bw.Write(data);
                bw.Close();
            }
        }
        return smallPath + fileName + Extension;
    }
    #endregion

    #region Groups
    [WebMethod(EnableSession = true)]
    public static string CreateGroup(string GroupName, string ParticpentsIds)
    {
        string[] Participents = ParticpentsIds.Split(',');
        string UserID = ((DataTable)HttpContext.Current.Session["USER_INFO"]).Rows[0]["USER_ID"].ToString();
        DataTable dt = new DataTable("USERS");
        dt.Columns.Add("USER_ID");
        dt.Rows.Add(UserID);
        foreach (string s in Participents)
        {
            if (s != "")
            {
                dt.Rows.Add(s);
            }
        }
        dt.AcceptChanges();
        string ParticipentXML = Convertor.ConvertDataTableToXML(dt);
        List<SqlParameter> List = new List<SqlParameter>();
        List.Add(new SqlParameter("@GROUP_TITLE", GroupName));
        List.Add(new SqlParameter("@PARTICIPANT_XML", ParticipentXML));
        DataTable res = new SQLHelper(SQLHelper.ConnectionStrings.WebSiteConnectionString).getProcedureResult("CreateGroup", List.ToArray()).Tables[0];
        return res.Rows[0][0].ToString();
    }
    [WebMethod(EnableSession = true)]
    public static List<Groups> GetUserGroupListInformation(string UserID)
    {
        string query = "select g.GROUP_ID,g.GROUP_TITLE from [user-group] ug "
                      + " inner join groups g on g.GROUP_ID=ug.GROUP_ID and ug.[USER_ID]=" + UserID;
        DataTable dt = new SQLHelper(SQLHelper.ConnectionStrings.WebSiteConnectionString).getQueryResult(query);
        List<Groups> Groups = new List<Groups>();
        if (dt.Rows.Count > 0)
        {

            foreach (DataRow Row in dt.Rows)
            {
                string ParticQuery = "Select USER_ID from [user-group] where GROUP_ID=" + Row[0].ToString();
                DataTable Partdt = new SQLHelper(SQLHelper.ConnectionStrings.WebSiteConnectionString).getQueryResult(ParticQuery);
                List<User> usrs = new List<User>();
                foreach (DataRow row in Partdt.Rows) 
                {
                    User usr = new User(row["USER_ID"].ToString(),true);
                    usrs.Add(usr);
                }
                Groups Grp = new Groups(Row[0].ToString(), Row[1].ToString(), usrs);
                Groups.Add(Grp);
            }
        }
        return Groups;
    }
    [WebMethod(EnableSession = true)]
    public static string ShowGroup(string GroupID)
    {
        string UserID = ((DataTable)HttpContext.Current.Session["USER_INFO"]).Rows[0]["USER_ID"].ToString();
        string ToAppend = "";
        string query = "select DISTINCT m.*,u.USER_USERNAME from  groups g "
	                    +" inner join [message-group] mg on mg.GROUP_ID=g.GROUP_ID "
	                    +" inner join messages m on m.MESSAGE_ID=mg.MESSAGE_ID "
                        + "inner join users u on u.[USER_ID]=m.MESSAGE_SENDER_ID "
                        +" where g.GROUP_ID="+GroupID;
        DataTable dt = new SQLHelper(SQLHelper.ConnectionStrings.WebSiteConnectionString).getQueryResult(query);
        
        if (dt.Rows.Count > 0)
        {
             foreach (DataRow row in dt.Rows)
            {
                if (row["MESSAGE_SENDER_ID"].ToString() == UserID)
                {
                    ToAppend += "<table width='100%'><tr><td>"
                             + " <div class='chatbubble tooltip'  title='" + Convert.ToDateTime(row["MESSAGE_CREATION_DATE"]).ToString("hh:mm:tt") + "' >"
                             + "<p>" + row["MESSAGE_BODY"].ToString()
                        //+ "<div class='time'>&nbsp;&nbsp;<span>" + Convert.ToDateTime(row["MESSAGE_CREATION_DATE"]).ToString("hh:mm:tt") + "</span></div>"
                             + "</p>"
                             + "</div>"
                             + "</td></tr></table>";

                }
                else
                {
                    ToAppend += "<table width='100%'><tr><td>"
                             + "<div class='chatbubble chatbubble-alt white tooltip' title='" + Convert.ToDateTime(row["MESSAGE_CREATION_DATE"]).ToString("hh:mm:tt") + "' >"
                             + "<p class='GroupUsername' >" + row["USER_USERNAME"].ToString() + "</p>"
                             + "<p>" + row["MESSAGE_BODY"].ToString()+ "</p>"
                             + "</div>"
                             + "</td></tr></table>";

                }
            }
        }
        return ToAppend;
    }

    #endregion

}