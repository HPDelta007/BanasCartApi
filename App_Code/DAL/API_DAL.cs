using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using System.Configuration;
using System.Web.Script.Serialization;
using System.Net;
using System.IO;
using System.Text;

/// <summary>
/// Summary description for API_DAL
/// </summary>
public class API_DAL
{
    private GeneralDAL _generalDAL;

    public API_DAL()
    {
        _generalDAL = new GeneralDAL();
    }

    ~API_DAL()
    {
        _generalDAL = null;
    }

    public DataTable returnDataTable(string sql)
    {
        try
        {
            SqlCommand sqlCmd = new SqlCommand();
            DataTable dtItmNames = new DataTable();

            _generalDAL.OpenSQLConnection();

            sqlCmd.Connection = _generalDAL.ActiveSQLConnection();
            sqlCmd.CommandText = sql;

            dtItmNames.Load(sqlCmd.ExecuteReader());

            _generalDAL.CloseSQLConnection();

            return dtItmNames;
        }
        catch (Exception ex)
        {
            _generalDAL.CloseSQLConnection();
            throw new Exception(ex.Message);
        }
    }

    public void InsertUpdateNonQuery(string query)
    {
        SqlTransaction sqlTrans = null;
        try
        {
            SqlCommand sqlCmd = new SqlCommand();
            _generalDAL.OpenSQLConnection();

            sqlTrans = _generalDAL.BeginTransaction();
            sqlCmd.Connection = _generalDAL.ActiveSQLConnection();
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.Transaction = sqlTrans;
            sqlCmd.CommandTimeout = 300;

            sqlCmd.Connection = _generalDAL.ActiveSQLConnection();
            sqlCmd.CommandType = CommandType.Text;

            sqlCmd.CommandText = query;

            sqlCmd.ExecuteNonQuery();

            sqlTrans.Commit();
            _generalDAL.CloseSQLConnection();
        }
        catch (Exception ex)
        {
            _generalDAL.CloseSQLConnection();
            throw new Exception(ex.Message);
        }
    }

    public string InsertUpdateWithReturnIdentity(string query)
    {
        SqlTransaction sqlTrans = null;
        try
        {
            SqlCommand sqlCmd = new SqlCommand();
            _generalDAL.OpenSQLConnection();
            string id;

            sqlTrans = _generalDAL.BeginTransaction();
            sqlCmd.Connection = _generalDAL.ActiveSQLConnection();
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.Transaction = sqlTrans;
            sqlCmd.CommandTimeout = 300;

            sqlCmd.Connection = _generalDAL.ActiveSQLConnection();
            sqlCmd.CommandType = CommandType.Text;

            sqlCmd.CommandText = query;

            id = sqlCmd.ExecuteScalar().ToString();

            sqlTrans.Commit();
            _generalDAL.CloseSQLConnection();

            return id;
        }
        catch (Exception ex)
        {
            _generalDAL.CloseSQLConnection();
            throw new Exception(ex.Message);
        }
    }

    public DataTable CustomerPaymentStatus(DateTime? fromdate, DateTime? Todate, string LgrName, string LgrGrpId)
    {
        SqlCommand sqlCmd;
        DataTable Dt = new DataTable();

        try
        {
            _generalDAL.OpenSQLConnectionAccount();

            sqlCmd = new SqlCommand();
            sqlCmd.Connection = _generalDAL.ActiveSQLConnection();
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandTimeout = 300;
            sqlCmd.CommandText = "Select Name From Lgrs where AutotrNo = '" + LgrName + "'";
            string LgrNameForSP = null;
            try
            {
                LgrNameForSP = sqlCmd.ExecuteScalar().ToString();
            }
            catch
            {
                LgrNameForSP = "NULL";
            }

            sqlCmd = new SqlCommand("[StpCustomerPaymentStatusForDealerApp]", _generalDAL.ActiveSQLConnection());


            if (Todate != null)
            {
                sqlCmd.Parameters.Add("@FDate", ((DateTime)Todate).ToString("dd-MMM-yyyy"));
                sqlCmd.Parameters.Add("@TDate", ((DateTime)Todate).ToString("dd-MMM-yyyy"));
            }
            else
            {
                sqlCmd.Parameters.Add("@FDate", "");
                sqlCmd.Parameters.Add("@TDate", "");
            }

            sqlCmd.Parameters.Add("@State", "");

            sqlCmd.Parameters.Add("@Area", "");

            if (LgrNameForSP != null)
                sqlCmd.Parameters.Add("@LgrName", LgrNameForSP.ToString());
            else
                sqlCmd.Parameters.Add("@LgrName", "");

            sqlCmd.Parameters.Add("@CustomerPartyGroupTextListId", "");

            //if (LgrGrpId != null)
            //    sqlCmd.Parameters.Add("@LgrGrpId", LgrGrpId.ToString());
            //else
            sqlCmd.Parameters.Add("@LgrGrpId", "");

            sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlCmd.CommandTimeout = 300;

            Dt.Load(sqlCmd.ExecuteReader());

            _generalDAL.CloseSQLConnection();

            return Dt;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}