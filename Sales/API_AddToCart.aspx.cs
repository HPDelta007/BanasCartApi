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


public partial class API_AddToCart : System.Web.UI.Page
{
    string UserId;
    string ItmId;
    string IMEI;
    string Rate;
    string Amount;
    decimal? Qty;

    API_BLL _aPI_BLL = new API_BLL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                UserId = (((Request.Form["UserId"] != null && Request.Form["UserId"] != "")) ? Request.Form["UserId"].ToString() : null);
                ItmId = (((Request.Form["ItmId"] != null && Request.Form["ItmId"] != "")) ? Request.Form["ItmId"].ToString() : null);
                IMEI = (((Request.Form["IMEI"] != null && Request.Form["IMEI"] != "")) ? Request.Form["IMEI"].ToString() : null);
                Rate = (((Request.Form["Rate"] != null && Request.Form["Rate"] != "")) ? Request.Form["Rate"].ToString() : null);
                Amount = (((Request.Form["Amount"] != null && Request.Form["Amount"] != "")) ? Request.Form["Amount"].ToString() : null);
                Qty = (((Request.Form["Qty"] != null && Request.Form["Qty"] != "")) ? Convert.ToDecimal(Request.Form["Qty"].ToString()) : (decimal?)null);
                
                Response.ContentType = "application/json";

                Response.Write(selectdata());
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
        DataTable dt = new DataTable();
        DataTable dt2 = new DataTable();
        StringBuilder st = new StringBuilder();
        string ReturnVal = "";

        try
        {

            dt = _aPI_BLL.returnDataTable("Select COUNT(*) from MyCarts where ItmId = '" + ItmId + "' " +
                            " and UserId = '" + UserId + "' ");

            if (Convert.ToInt32(dt.Rows[0][0]) == 0)
            {
              
                string id = _aPI_BLL.InsertUpdateWithReturnIdentity(" DECLARE  @MyCartId uniqueidentifier; SET @MyCartId = NewId(); " +
                    " insert into MyCarts (MyCartId, UserId, ItmId, Qty, InsertedOn, LastUpdatedOn, InsertedByUserId, LastUpdatedByUserId, Rate, Amount, IMEI) values " +
                    " ( @MyCartId " +
                     " , " + ((UserId == null) ? "NULL" : "'" + UserId.ToString() + "'") + " " +
                     " , " + ((ItmId == null) ? "NULL" : "'" + ItmId.ToString().Replace("'", "''") + "'") + " " +
                     " , " + ((Qty == null) ? "NULL" : "'" + Qty.ToString().Replace("'", "''") + "'") + " " +
                     " , GETDATE(), GETDATE(), '" + UserId + "', NULL " +
                     " , " + ((Rate == null) ? "NULL" : "'" + Rate.ToString().Replace("'", "''") + "'") + " " +
                     " , " + ((Rate == null) ? "NULL" : "'" + Convert.ToDecimal(Convert.ToDecimal(Qty) * Convert.ToDecimal(Rate)).ToString() + "'") + " " +
                     " , " + ((IMEI == null) ? "NULL" : "'" + IMEI.ToString().Replace("'", "''") + "'") + " " +
                     ");select @MyCartId;");

                da = _aPI_BLL.returnDataTable(" select * from MyCarts where MyCartId = '" + id + "' ");
            }
            else
            {
                dt2 = _aPI_BLL.returnDataTable("Select * from MyCarts where ItmId = '" + ItmId + "' " +
                            " and UserId = '" + UserId + "' ");

                int Qty2 = Convert.ToInt32 (Convert.ToInt32(dt2.Rows[0]["Qty"]) + Qty );

                int Rate2 = Convert.ToInt32(Convert.ToInt32(dt2.Rows[0]["Rate"]));

                _aPI_BLL.InsertUpdateNonQuery(" update MyCarts set Qty = '" + Qty2 + "' " +
                                    " ,InsertedOn = getdate() " +
                                    " ,InsertedByUserId = '" + UserId + "' " +
                                    " ,LastUpdatedOn = getdate() " +
                                    " ,LastUpdatedByUserId = '" + UserId + "' " +
                                    " ,Amount = '" + Convert.ToDecimal(Convert.ToDecimal(Qty2) * Convert.ToDecimal(Rate2)) + "' " +
                                    " where ItmId = '" + ItmId + "' and UserId = '" + UserId + "' ; ");

                da = _aPI_BLL.returnDataTable(" select * from MyCarts where ItmId = '" + ItmId + "' and UserId = '" + UserId + "'  ");
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
}