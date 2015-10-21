using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;

public partial class LoginPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void Submit_btn_Click(object sender, EventArgs e)
    {
        string Username = Username_txt.Text;
        string Password = Password_txt.Text;
        if(!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
        LoginUser(Username, Password);
    }
    protected void LoginUser(string Username, string Password)
    {
        string query = "select users.*,status.STATUS_DESCRIPTION " 
                    + " from users inner join status on status.STATUS_NAME=users.USER_STATUS_ID "
                    +" where USER_USERNAME=@USER_USERNAME and USER_PASSWORD=@USER_PASSWORD";
        List<SqlParameter> List = new List<SqlParameter>();
        List.Add(new SqlParameter("@USER_USERNAME", Username));
        List.Add(new SqlParameter("@USER_PASSWORD", Password));
        try
        {
             DataTable dt=  new SQLHelper(SQLHelper.ConnectionStrings.WebSiteConnectionString).getQueryResultWithParameters(query, List.ToArray());
             Session["USER_INFO"] = dt;
             Error_msg.Text = "";
             Response.Redirect("ChatPage.aspx");
        }
        catch (Exception ex)
        {
             Error_msg.Text = "Login failed ";
        }   
    }
}