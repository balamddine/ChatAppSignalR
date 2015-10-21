using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Collections.Generic;


public class SQLHelper
{
    public SqlConnection connection;

    public SQLHelper(ConnectionStrings connectionstring)
    {
        switch (connectionstring)
        {
            case ConnectionStrings.WebSiteConnectionString:
                connection = new SqlConnection(WebSiteConnectionString);
                break;
            
            default:
                break;
        }
        //connection = new SqlConnection(this.readconnStringFromXmlFile("bin/connection.xml"));
    }
    
    public DataTable getQueryResult(string QueryString)
    {
        DataTable tb = new DataTable();
        SqlDataAdapter adp = new SqlDataAdapter(QueryString, connection);
        adp.Fill(tb);
        connection.Close();
        return tb;
    }
    public DataTable getQueryResultWithParameters(string query, SqlParameter[] coll)
    {
        DataTable tb = new DataTable();
        SqlCommand com = new SqlCommand(query, connection);
        com.Parameters.AddRange(coll);
        SqlDataAdapter adp = new SqlDataAdapter(com);
        adp.Fill(tb);
        connection.Close();
        return tb;
    }
    public DataSet getProcedureResult(string ProcedureName, SqlParameter[] coll)
    {
        DataSet tb = new DataSet();
        SqlDataAdapter adp = new SqlDataAdapter(ProcedureName, connection);
        adp.SelectCommand.CommandType = CommandType.StoredProcedure;
        adp.SelectCommand.Parameters.AddRange(coll);
        adp.Fill(tb);
        connection.Close();
        return tb;
    }

    public void Insert(string InsertQuery)
    {
        SqlDataAdapter adp = new SqlDataAdapter();
        adp.InsertCommand = new SqlCommand(InsertQuery, connection);
        if (connection.State == System.Data.ConnectionState.Closed)
        {
            connection.Open();
        }
        adp.InsertCommand.ExecuteNonQuery();
        connection.Close();
    }
    public void InsertWithParameters(string InsertQuery, SqlParameter[] coll)
    {
        SqlDataAdapter adp = new SqlDataAdapter();
        SqlCommand com = new SqlCommand(InsertQuery, connection);
        com.Parameters.AddRange(coll);
        adp.InsertCommand = com;
        if (connection.State == System.Data.ConnectionState.Closed)
        {
            connection.Open();
        }
        adp.InsertCommand.ExecuteNonQuery();
        connection.Close();
    }
    public void Update(string UpdateQuery)
    {
        SqlDataAdapter adp = new SqlDataAdapter();
        adp.UpdateCommand = new SqlCommand(UpdateQuery, connection);
        if (connection.State == System.Data.ConnectionState.Closed)
        {
            connection.Open();
        }
        adp.UpdateCommand.ExecuteNonQuery();
        connection.Close();
    }
    public void UpdateWithParameters(string UpdateQuery, SqlParameter[] coll)
    {
        SqlDataAdapter adp = new SqlDataAdapter();
        SqlCommand com = new SqlCommand(UpdateQuery, connection);
        com.Parameters.AddRange(coll);
        adp.UpdateCommand = com;
        if (connection.State == System.Data.ConnectionState.Closed)
        {
            connection.Open();
        }
        adp.UpdateCommand.ExecuteNonQuery();
        connection.Close();
    }
    public void Delete(string DeleteQuery)
    {
        SqlDataAdapter adp = new SqlDataAdapter();
        adp.DeleteCommand = new SqlCommand(DeleteQuery, connection);
        if (connection.State == System.Data.ConnectionState.Closed)
        {
            connection.Open();
        }
        adp.DeleteCommand.ExecuteNonQuery();
        connection.Close();
    }




    public static string WebSiteConnectionString
    {
        get { return System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString; }
    }
    
    public enum ConnectionStrings { WebSiteConnectionString = 1 }
}