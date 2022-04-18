using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.Data;
using System.Collections;
using System.Web.Script.Serialization;
using System.Text;
using System.Data.SqlClient;


public partial class API_OrderBook : System.Web.UI.Page
{
    string UserId;
    string CustomerCode;
    string Remarks;
    
    string data;
    string APIKey;

    API_BLL _aPI_BLL = new API_BLL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                UserId = (((Request.Form["UserId"] != null && Request.Form["UserId"] != "")) ? Request.Form["UserId"].ToString() : null);
                CustomerCode = (((Request.Form["CustomerCode"] != null && Request.Form["CustomerCode"] != "")) ? Request.Form["CustomerCode"].ToString() : null);
                
                data = (((Request.Form["data"] != null && Request.Form["data"] != "")) ? Request.Form["data"].ToString() : null);
                
                Remarks = (((Request.Form["Remarks"] != null && Request.Form["Remarks"] != "")) ? Request.Form["Remarks"].ToString() : null);

                if (Request.Form["APIKey"] != null && Request.Form["APIKey"] != "")
                    APIKey = Request.Form["APIKey"].ToString();
                else
                    APIKey = null;

                Response.ContentType = "application/json";

                string ConfigAPIKey = ConfigurationManager.AppSettings["APIKey"].ToString();

                if (ConfigAPIKey == APIKey)
                {
                    Response.Write(selectdata());
                }
                else
                {
                    string sw = "";
                    StringBuilder s = new StringBuilder();
                    s.Append("Authentication Key is wrong.");
                    sw = GetReturnValue("209", "Authentication Key is wrong.", s);
                    Response.ContentType = "application/json";
                    Response.Write(sw.Replace("\\", "").Replace("\"[", "[").Replace("]\"", "]"));
                } 
            }
            catch (Exception ex)
            {
                string sw = "";
                StringBuilder s = new StringBuilder();
                s.Append(ex.Message);
                sw = GetReturnValue("209", "Data Get Issue", s);
                Response.ContentType = "application/json";
                Response.Write(sw.Replace("\\", "").Replace("\"[", "[").Replace("]\"", "]"));
            }
        }
    }

    public class ResponseData
    {
        public int? status { get; set; }
        public string message { get; set; }
        public ArrayList Result { get; set; }
    }

    public string DataTableToJsonObj(DataTable dt)
    {
        DataSet ds = new DataSet();
        ds.Merge(dt);
        StringBuilder JsonString = new StringBuilder();
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            JsonString.Append("[");
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                JsonString.Append("{");
                for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                {
                    if (j < ds.Tables[0].Columns.Count - 1)
                    {
                        JsonString.Append("\"" + ds.Tables[0].Columns[j].ColumnName.ToString() + "\":" + "\"" + ds.Tables[0].Rows[i][j].ToString() + "\",");
                    }
                    else if (j == ds.Tables[0].Columns.Count - 1)
                    {
                        JsonString.Append("\"" + ds.Tables[0].Columns[j].ColumnName.ToString() + "\":" + "\"" + ds.Tables[0].Rows[i][j].ToString() + "\"");
                    }
                }
                if (i == ds.Tables[0].Rows.Count - 1)
                {
                    JsonString.Append("}");
                }
                else
                {
                    JsonString.Append("},");
                }
            }
            JsonString.Append("]");
            return JsonString.ToString();
        }
        else
        {
            return null;
        }
    }

    public string selectdata()
    {
        DataTable da = new DataTable();
        StringBuilder st = new StringBuilder();
        string ReturnVal = "";

        try
        {
            DataTable dt = (DataTable)JsonConvert.DeserializeObject(data, (typeof(DataTable)));

            string No = null;
            string ItmPara = "";

            if (dt.Rows.Count > 0)
            {
                No = GetNo(Convert.ToDateTime(System.DateTime.Now), null);

                string id = _aPI_BLL.InsertUpdateWithReturnIdentity(" DECLARE  @SOId uniqueidentifier; SET @SOId = NewId(); " +
                                              " insert into SOs (SOId, SODt, SONo, UserId, CustomerCode, Remarks, InsertedOn, " +
                                              " LastUpdatedOn, InsertedByUserId, LastUpdatedByUserId) values " +
                                              " ( @SOId " +
                                              " , GETDATE() " +
                                              " , " + ((No == null) ? "NULL" : "'" + No.ToString().Replace("'", "''") + "'") + " " +
                                              " , " + ((UserId == null) ? "NULL" : "'" + UserId.ToString().Replace("'", "''") + "'") + " " +
                                              " , " + ((CustomerCode == null) ? "NULL" : "'" + CustomerCode.ToString().Replace("'", "''") + "'") + " " +
                                              " , " + ((Remarks == null) ? "NULL" : "N'" + Remarks.ToString().Replace("'", "''") + "'") + " " +
                                              " , GETDATE(), GETDATE(), '" + UserId + "', NULL " +
                                              ");select @SOId;");

                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {

                    ItmPara = ((dt.Rows[i]["ItmPara"] == DBNull.Value) ? "NULL" : "'" + dt.Rows[i]["ItmPara"].ToString().Replace("'", "''") + "'");

                    _aPI_BLL.InsertUpdateNonQuery(" insert into SOLns (SOLnId, SOId, LnNo, ItmCode, Qty, ExpectedDeliDt, Remarks, InsertedOn, LastUpdatedOn, InsertedByUserId, LastUpdatedByUserId) values " +
                                              " ( NEWID() " +
                                              " , " + ((id == null) ? "NULL" : "'" + id.ToString().Replace("'", "''") + "'") + " " +
                                              " , " + ((dt.Rows[i]["LnNo"] == DBNull.Value) ? "NULL" : "'" + dt.Rows[i]["LnNo"].ToString().Replace("'", "''") + "'") + " " +
                                              " , " + ((dt.Rows[i]["ItmCode"] == DBNull.Value) ? "NULL" : "'" + dt.Rows[i]["ItmCode"].ToString().Replace("'", "''") + "'") + " " +
                                              " , " + ((dt.Rows[i]["Qty"] == DBNull.Value) ? "NULL" : "'" + dt.Rows[i]["Qty"].ToString().Replace("'", "''") + "'") + " " +
                                              " , " + ((dt.Rows[i]["ExpectedDeliDt"] == DBNull.Value) ? "NULL" : "'" + Convert.ToDateTime(dt.Rows[i]["ExpectedDeliDt"]).ToString("dd-MMM-yyyy") + "'") + " " +
                                              " , " + ((ItmPara == null) ? "NULL" : "N'" + ItmPara.ToString().Replace("'", "''") + "'") + " " +
                                              " , GETDATE(), GETDATE(), '" + UserId + "', NULL " +
                                              ")");
                }

                _aPI_BLL.InsertUpdateNonQuery(" Declare @Count AS int;" +
                                 " SELECT @Count=COUNT(*)FROM SONos a WHERE  a.Dt='" + Convert.ToDateTime(System.DateTime.Now).ToString("dd-MMM-yyyy") + "'" +
                                 " IF(@Count=0)" +
                                 "   BEGIN" +
                                 "       INSERT INTO SONos (SONoId,Dt,Ct) VALUES (NEWID(),'" + Convert.ToDateTime(System.DateTime.Now).ToString("dd-MMM-yyyy") + "',1)" +
                                 "   END" +
                                 " ELSE" +
                                 "   BEGIN" +
                                 "       UPDATE SONos SET Ct=Ct+1 WHERE Dt='" + Convert.ToDateTime(System.DateTime.Now).ToString("dd-MMM-yyyy") + "'" +
                                 "   END");

                _aPI_BLL.InsertUpdateNonQuery(" delete from MyCarts where UserId = '" + UserId.ToString().Replace("'", "''") + "' ");

                da = _aPI_BLL.returnDataTable(" select * from SOs where SOId = '" + id + "' ");
            }

            st.Append(DataTableToJsonObj(da));

            if (da == null)
            {
                ReturnVal = GetReturnValue("209", "No Record Found", st);
            }

            if (st.ToString() == "[]" || st.ToString() == "")
            {
                ReturnVal = GetReturnValue("209", "No Record Found", st);
            }

            if (da.Rows.Count > 0)
            {
                ReturnVal = GetReturnValue("200", "Data Saved", st);
            }

            if (st.ToString() != "[]")
                return ReturnVal.Replace("\\", "").Replace("\"[", "[").Replace("]\"", "]");
            else
                return ReturnVal.Replace("\\", "").Replace("\"[]\"", "[]");
        }
        catch (Exception ex)
        {
            StringBuilder s = new StringBuilder();
            s.Append(ex.Message);
            ReturnVal = GetReturnValue("209", "Data Get Issue", s);
            return ReturnVal.Replace("\\", "").Replace("\"[", "[").Replace("]\"", "]");
        }
    }

    public string GetReturnValue(string Status, string Message, StringBuilder PassStringDataTable)
    {
        var r = new ReturnValue
        {
            status = Status,
            message = Message,
            result = PassStringDataTable.ToString()
        };
        return new JavaScriptSerializer().Serialize(r);
    }

    public string GetNo(DateTime date, SqlCommand pSqlCmd)
    {
        SqlCommand sqlCmd = new SqlCommand();
        SqlDataReader sqlDR;
        GeneralDAL _generalDAL = new GeneralDAL();
        string no;

        //If passed psqlCmd is null means This function is called from UI while data entry, then it need to open connection and return no
        //If Passed pSqlCmd is not null means, this function is called at time of Insert , So it should return no and  increment the count and if 
        //the record of the date is not present it should insert one new record for the date.
        if (pSqlCmd == null)
        {
            _generalDAL.OpenSQLConnection();
            sqlCmd.Connection = _generalDAL.ActiveSQLConnection();
        }
        else
        {
            sqlCmd = pSqlCmd;
        }

        //Sql Query to Return no
        string sql = "SELECT 'OSO/'+ Convert(varchar,a.Ct+1) +'/" + date.ToString("ddMMyy") + "' AS No" +
                     " FROM SONos a" +
                     " WHERE  a.Dt='" + ((DateTime)date).ToString("dd-MMM-yyyy") + "'";

        sqlCmd.CommandType = CommandType.Text;
        sqlCmd.CommandText = sql;
        sqlDR = sqlCmd.ExecuteReader();

        if (sqlDR.Read())
        {
            no = sqlDR["No"].ToString();
        }
        else
        {
            //If the record is not present in table for the specified date, it should generate new no start from 1 with date
            no = "OSO/1/" + date.ToString("ddMMyy");
        }
        sqlDR.Close();
        //Sql Query to Return no

        //Increment Count if it is called from Insert function of DAL means pSqlCmd != null
        if (pSqlCmd != null)
        {
            //sqlCmd.CommandText = " Declare @Count AS int;" +
            //                     " SELECT @Count=COUNT(*)FROM SONos a WHERE  a.Dt='" + date.ToString("dd-MMM-yyyy") + "'" +
            //                     " IF(@Count=0)" +
            //                     "   BEGIN" +
            //                     "       INSERT INTO SONos (SONoId,Dt,Ct) VALUES (NEWID(),'" + date.ToString("dd-MMM-yyyy") + "',1)" +
            //                     "   END" +
            //                     " ELSE" +
            //                     "   BEGIN" +
            //                     "       UPDATE SONos SET Ct=Ct+1 WHERE Dt='" + date.ToString("dd-MMM-yyyy") + "'" +
            //                     "   END";
            //sqlCmd.ExecuteNonQuery();
        }
        else
        {
            //If pSqlCmd is null, means This function is called at UI Entry time, so it should close the connection
            _generalDAL.CloseSQLConnection();
        }

        return no;
    }
}