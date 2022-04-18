using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Configuration;
using System.Web.Script.Serialization;

public class GeneralDAL
{
    private SqlConnection _sqlConn;

    public string version = "VERSION 21.04.01 Date:01-Apr-2021 BBPC";

    public string DBServer = ConfigurationManager.AppSettings["SQLSERVER_SERVER"].ToString();
    public string DBName = ConfigurationManager.AppSettings["SQLSERVER_DB"].ToString();
    public string DBUser = ConfigurationManager.AppSettings["SQLSERVER_USER"].ToString();
    public string DBPwd = ConfigurationManager.AppSettings["SQLSERVER_PASSWORD"].ToString();

    public GeneralDAL()
    {
        _sqlConn = new SqlConnection();
    }

    ~GeneralDAL()
    {
        _sqlConn = null;
    }

    public void OpenSQLConnection()
    {
        try
        {
            if (_sqlConn.State == ConnectionState.Closed)
            {
                //_sqlConn.ConnectionString = @"Persist Security Info=False;User ID=sa;Password=sqlserver@123;Initial Catalog=3DKDH;Data Source=192.168.0.99\SQLEXPRESS";
                _sqlConn.ConnectionString = @"Persist Security Info=False;User ID=" + DBUser + ";Password=" + DBPwd + ";Initial Catalog=" + DBName + ";Data Source=" + DBServer + ""; //Live

                _sqlConn.Open();
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public void OpenSQLConnectionAccount()
    {
        try
        {
            if (_sqlConn.State == ConnectionState.Closed)
            {
                //_sqlConn.ConnectionString = @"Persist Security Info=False;User ID=sa;Password=sqlserver@123;Initial Catalog=3DKDH;Data Source=192.168.0.99\SQLEXPRESS";
                //_sqlConn.ConnectionString = @"Persist Security Info=False;User ID=sa;Password=sqlserver@123;Initial Catalog=" + ConfigurationManager.AppSettings["AccountDBName"] + ";Data Source=192.168.0.100\\SQL19";
                _sqlConn.ConnectionString = @"Persist Security Info=False;User ID=" + DBUser + ";Password=" + DBPwd + ";Initial Catalog=" + ConfigurationManager.AppSettings["AccountDBName"] + ";Data Source=" + DBServer + ""; //Live

                _sqlConn.Open();
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public SqlConnection ActiveSQLConnection()
    {
        return _sqlConn;
    }

    internal SqlTransaction BeginTransaction()
    {
        return _sqlConn.BeginTransaction();
    }

    public void CloseSQLConnection()
    {
        _sqlConn.Close();
    }
}
