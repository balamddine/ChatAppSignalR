using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.IO;

public partial class Register : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
  
    protected void Register_btn_Click(object sender, EventArgs e)
    {
        string name = name_txt.Text;
        string Username = Username_txt.Text;
        string Password = Password_txt.Text;
        string Email = Email_txt.Text;
        string FileName= new FileInfo(Logo_Upload.PostedFile.FileName).Name;
        string LogoFile = "~/images/" + FileName;
        byte[] Logo = null;
        if (!string.IsNullOrEmpty(Logo_Upload.PostedFile.FileName))
        {
            string Path = Server.MapPath(LogoFile);
                 Logo_Upload.PostedFile.SaveAs(Path);
            Logo = ImagingTools.ConvertImgToBinary(LogoFile);
            if (File.Exists(Path))
            {
                File.Delete(Path);
            }
        }
        if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
        { 
            RegisterUser(name,Username,Password,Logo,Email);
        }
    }

    public void RegisterUser(string Name,string username,string pass,byte[] img,string email) 
    { 
         string query = "INSERT INTO [Chat_db].[dbo].[users]([USER_NAME],[USER_USERNAME],[USER_PASSWORD],[USER_LOGO],[USER_EMAIL])"
                        +" VALUES (@USER_NAME,@USER_USERNAME,@USER_PASSWORD,@USER_LOGO,@USER_EMAIL)";
              List<SqlParameter> List= new List<SqlParameter>();
              List.Add(new SqlParameter("@USER_NAME",Name));
              List.Add(new SqlParameter("@USER_USERNAME",username));
              List.Add(new SqlParameter("@USER_PASSWORD",pass));
              List.Add(new SqlParameter("@USER_LOGO",img));
              List.Add(new SqlParameter("@USER_EMAIL",email));
          try
          {
              new SQLHelper(SQLHelper.ConnectionStrings.WebSiteConnectionString).InsertWithParameters(query, List.ToArray());
              Error_msg.Text = "";

              Response.Redirect("LoginPage.aspx");
          }
          catch (Exception ex)
          { 
            Error_msg.Text="Error in registration ";
          }   
    }
}