using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class TestPage : System.Web.UI.Page
{
    public static int i = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        //string M = "Notification " + i++;
        //new SQLHelper(SQLHelper.ConnectionStrings.WebSiteConnectionString).Insert("INSERT INTO notifications(NOTIFICATION_NAME,STATUS,TYPE) VALUES ('"+M+"','Active','Message')");
        //Response.Redirect("TestPage2.aspx?Notify=T");
        i++;
        var hubContext = GlobalHost.ConnectionManager.GetHubContext<SignalRChat.ChatHub>();
        hubContext.Clients.All.notifychanges(i);

    }

    
}