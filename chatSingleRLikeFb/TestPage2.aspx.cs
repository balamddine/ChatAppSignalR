using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

public partial class TestPage2 : System.Web.UI.Page
{
  
    protected void Page_Load(object sender, EventArgs e)
    {
         RadAjaxManager manager = RadAjaxManager.GetCurrent(Page);
         manager.AjaxSettings.AddAjaxSetting(btn_tirger_grd, rgd_NotificationGrid, AjaxLoadingPanel);
         if (!IsPostBack)
         {
             rgd_NotificationGrid.DataBind();
         }
    }
    private DataTable NotificationDt()
    { 
         return new SQLHelper(SQLHelper.ConnectionStrings.WebSiteConnectionString).getQueryResult("select * from notifications");
    }
    protected void rgd_NotificationGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        rgd_NotificationGrid.DataSource = NotificationDt();
    }
    protected void btn_tirger_grd_Click(object sender, EventArgs e)
    {
        rgd_NotificationGrid.Rebind();
    }
}