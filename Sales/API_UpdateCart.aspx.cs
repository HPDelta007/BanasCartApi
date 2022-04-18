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


public partial class API_UpdateCart : System.Web.UI.Page
{
    string UserId;
    string MyCartId;
    decimal? Qty;
    //string APIKey;

    API_BLL _aPI_BLL = new API_BLL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                UserId = (((Request.Form["UserId"] != null && Request.Form["UserId"] != "")) ? Request.Form["UserId"].ToString() : null);
                MyCartId = (((Request.Form["MyCartId"] != null && Request.Form["MyCartId"] != "")) ? Request.Form["MyCartId"].ToString() : null);
                Qty = (((Request.Form["Qty"] != null && Request.Form["Qty"] != "")) ? Convert.ToDecimal(Request.Form["Qty"].ToString()) : (decimal?)null);

                //if (Request.Form["APIKey"] != null && Request.Form["APIKey"] != "")
                //    APIKey = Request.Form["APIKey"].ToString();
                //else
                //    APIKey = null;

                Response.ContentType = "application/json";

                //string ConfigAPIKey = ConfigurationManager.AppSettings["APIKey"].ToString();

                //if (ConfigAPIKey == APIKey)
                //{
                    Response.Write(selectdata());
                //}
                //else
                //{
                //    string sw = "";
                //    StringBuilder s = new StringBuilder();
                //    s.Append("Authentication Key is wrong.");
                //    sw = GetReturnValue("209", "Authentication Key is wrong.", s);
                //    Response.ContentType = "application/json";
                //    Response.Write(sw.Replace("\\", "").Replace("\"[", "[").Replace("]\"", "]"));
                //} 
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
        DataTable da2 = new DataTable();
        DataTable dt = new DataTable();
        DataTable dt2 = new DataTable();
        StringBuilder st = new StringBuilder();
        string ReturnVal = "";

        try
        {
            if (Qty > 0)
            {
                da2 = _aPI_BLL.returnDataTable(" select m.*," +
                                         "  Photo= (select top 1 replace(x.ImageName, '" + ConfigurationSettings.AppSettings["FolderPath"] + "', " +
                                         " '" + ConfigurationSettings.AppSettings["FolderPathShow"] + "') from Itms x where x.ItmId = m.ItmId)" +
                                         ",Name = (select top 1 N.Name from Itms N where N.ItmId = m.ItmId) " +
                                         ",Address = (select top 1 StreetAddress from UserAddresses a where a.UserId = m.UserId)" +
                                         ",City = (select top 1 City from UserAddresses a  where a.UserId = m.UserId)" +
                                         ",TotalAmount = (select top 1 (T.MRP * m.Qty) from Itms T where m.ItmId = T.ItmId)" +
                                         ",GrandTotal = (select Sum(T.Amount) from MyCarts T where m.UserId = T.UserId)" +
                                         " from MyCarts m where m.UserId = '" + UserId + "' ");

                _aPI_BLL.InsertUpdateNonQuery(" update MyCarts set Qty = '" + Qty + "',Amount = '" + Convert.ToDecimal(Convert.ToDecimal(da2.Rows[0]["Rate"]) * Convert.ToDecimal(Qty)) + "', LastUpdatedOn = getdate(), LastUpdatedByUserId = '" + UserId + "' where MyCartId = '" + MyCartId + "' ");

                da = _aPI_BLL.returnDataTable(" select * from MyCarts where MyCartId = '" + MyCartId + "' ");

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
            else
            {
                _aPI_BLL.InsertUpdateNonQuery(" delete from MyCarts where MyCartId = '" + MyCartId + "' ");
                st.Append(DataTableToJsonObj(dt));

                if (dt == null)
                {
                    ReturnVal = GetReturnValue("200", "Data Delete", st);
                }

                if (st.ToString() == "[]" || st.ToString() == "")
                {
                    ReturnVal = GetReturnValue("200", "Data Delete", st);
                }

                if (dt.Rows.Count > 0)
                {
                    ReturnVal = GetReturnValue("200", "Data Delete", st);
                }

                if (st.ToString() != "[]")
                    return ReturnVal.Replace("\\", "").Replace("\"[", "[").Replace("]\"", "]");
                else
                    return ReturnVal.Replace("\\", "").Replace("\"[]\"", "[]");
            }
           
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