<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TestPage2.aspx.cs" Inherits="TestPage2" %>

<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <script src="Scripts/jquery-1.10.1.min.js"></script>
     <script src="Scripts/notification/jquery.noty.js" type="text/javascript"></script>
    <script src="Scripts/notification/topCenter.js" type="text/javascript"></script>
    <script src="Scripts/notification/default.js" type="text/javascript"></script>
    <script src="Scripts/jquery.signalR-1.1.4.js" type="text/javascript"></script>
     <script type="text/javascript" src="/signalr/hubs"></script>
    <telerik:RadCodeBlock runat="server">
     <script type="text/javascript">
         var chat = $.connection.chatHub;
         var n = null;
         $(function () {
             $.connection.hub.start();
            chat.client.notifychanges = function (count) {
                
               
                 if ($(".noty_message").length == 0) {
                   n= noty({
                         text:'You have '+  count + ' New notification click to refresh',
                         type: "information",
                         dismissQueue: true,
                         layout: 'topCenter',
                         theme: 'defaultTheme',
                         timeout: false,//5000
                         closeWith: ['click'],
                         callback: {
                             afterClose: function () {
                                 $("#<%=btn_tirger_grd.ClientID%>").click();
                             }

                         }
                     });
                    
                 }
                 else {
                     if (n != null)
                         n.setText("You have "+  count + " New notification click to refresh");
                 }
             }
        });
        
    </script>
        </telerik:RadCodeBlock>
</head>
<body>
    <form id="form1" runat="server">
    <div runat="server">
    <h2>this page will be notified by a notification </h2>
        <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
        </telerik:RadScriptManager>
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        </telerik:RadAjaxManager>
        <telerik:RadAjaxLoadingPanel ID="AjaxLoadingPanel" runat="server"
            EnableSkinTransparency="True" RegisterWithScriptManager="True"
            BackgroundPosition="Center" EnableAjaxSkinRendering="True"
            HorizontalAlign="Center" MinDisplayTime="0" BorderStyle="NotSet"
            Skin="Telerik" EnableViewState="True" EnableEmbeddedBaseStylesheet="True"
            EnableEmbeddedSkins="True" EnableEmbeddedScripts="True" IsSticky="False">
        </telerik:RadAjaxLoadingPanel>
  
        <telerik:RadGrid ID="rgd_NotificationGrid" runat="server" AutoGenerateColumns="false" OnNeedDataSource="rgd_NotificationGrid_NeedDataSource">
            <MasterTableView CommandItemDisplay="Top" ShowHeadersWhenNoRecords="true" ShowHeader="true">
                <Columns>
                    <telerik:GridBoundColumn DataField="NOTIFICATION_ID" HeaderText="Id"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="NOTIFICATION_NAME" HeaderText="Name"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="STATUS" HeaderText="Status"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="TYPE" HeaderText="Type"></telerik:GridBoundColumn>
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
          <asp:Button ID="btn_tirger_grd" runat="server" style="visibility:hidden;" OnClick="btn_tirger_grd_Click" />
        <div class="noty_dv"></div>
    </div>
    </form>
</body>
</html>
