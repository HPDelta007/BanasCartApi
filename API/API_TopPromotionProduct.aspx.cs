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
using System.Web.Services;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Web.Configuration;

public partial class API_TopPromotionProduct : System.Web.UI.Page
{
    string InsertedByUserId;
    string APIKey;
    string CustCode;
    API_BLL _aPI_BLL = new API_BLL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                if (Request.Form["InsertedByUserId"] != null && Request.Form["InsertedByUserId"] != "")
                    InsertedByUserId = Request.Form["InsertedByUserId"].ToString();
                else
                    InsertedByUserId = null;

                CustCode = (((Request.Form["CustCode"] != null && Request.Form["CustCode"] != "")) ? Request.Form["CustCode"].ToString() : null);

                if (Request.Form["APIKey"] != null && Request.Form["APIKey"] != "")
                    APIKey = Request.Form["APIKey"].ToString();
                else
                    APIKey = null;

                Response.ContentType = "application/json";

                string ConfigAPIKey = ConfigurationManager.AppSettings["APIKey"].ToString();

                if (ConfigAPIKey == APIKey)
                {
                    if (CustCode == null)
                    {
                        string sw = "";
                        StringBuilder s = new StringBuilder();
                        s.Append("Customer Code is compulsory.");
                        sw = GetReturnValue("209", "Customer Code is compulsory.", s);
                        Response.ContentType = "application/json";
                        Response.Write(sw.Replace("\\", "").Replace("\"[", "[").Replace("]\"", "]"));
                    }
                    else
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
            string where = null;

            where = "";

            if (InsertedByUserId != null)
            {
                if (where != "")
                {
                    where += " AND ";
                }
                where += " InsertedByUserId = '" + InsertedByUserId + "' ";
            }

            if (where != "")
            {
                where = " WHERE " + where;
            }

            da = _aPI_BLL.returnDataTable(" select top 5 Id, a.Name , a.TitleForPromotion," +
                                          " REPLACE(((select TOP 1 z.FilePath from " + ConfigurationManager.AppSettings["DBName"].ToString() + "..FU z " +
                                          " inner join " + ConfigurationManager.AppSettings["DBName"].ToString() + "..Itms y on z.RecordId = y.ItmId " +
                                          " Where y.Alias = a.PartCode order by z.LnNo)),'" + ConfigurationManager.AppSettings["ERPItmPhotoPath"].ToString() + "','" + ConfigurationManager.AppSettings["ERPItmPhotoURL"].ToString() + "') as Photos , " +
                                          " 'Promotion Title' as PromotionTitle " +
                                          " ,a.PartCode , a.Id as ProductCode , 0.00 as MRP,0.00 as PurchaseRate ,0.00 as GST " +
                                          " from vwTop5Products a " +
                                          " order by Id  ");

            da.Columns["MRP"].ReadOnly = false;
            da.Columns["PurchaseRate"].ReadOnly = false;
            da.Columns["GST"].ReadOnly = false;

            for (int i = 0; i <= da.Rows.Count - 1; i++)
            {
                try
                {
                    string PLRate, NetRate, GST;
                    DataTable dtRateAndGST = new DataTable();
                    GetRateAndGST(da.Rows[i]["PartCode"].ToString(), CustCode,out PLRate, out NetRate, out GST);

                    da.Rows[i]["MRP"] = PLRate.Replace("\"", "");
                    da.Rows[i]["PurchaseRate"] = NetRate.Replace("\"", "");
                    da.Rows[i]["GST"] = GST.Replace("\"", "");
                }
                catch
                {
                    da.Rows[i]["MRP"] = 0.00;
                    da.Rows[i]["PurchaseRate"] = 0.00;
                    da.Rows[i]["GST"] = 0.00;
                }
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
                ReturnVal = GetReturnValue("200", "Data Get", st);
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
    [WebMethod]
    public void GetRateAndGST(string PartCode,string CustCode, out string PLRate, out string NetRate, out string GST)
    {
        ArrayList al = new ArrayList();

        DataTable dtPlant = new DataTable();

        var baseAddress = WebConfigurationManager.AppSettings["APIURL"].ToString();

        HttpWebRequest Http = (HttpWebRequest)WebRequest.Create(baseAddress);

        Http.ContentType = "application/json; charset=utf-8";

        Http.Method = "POST";
        byte[] lbPostBuffer = Encoding.Default.GetBytes("{PartCode:\"" + PartCode + "\",CustCode:\"" + CustCode + "\"}");

        Http.ContentLength = lbPostBuffer.Length;

        Stream PostStream = Http.GetRequestStream();
        PostStream.Write(lbPostBuffer, 0, lbPostBuffer.Length);
        PostStream.Close();

        HttpWebResponse WebResponse = (HttpWebResponse)Http.GetResponse();

        Stream responseStream = WebResponse.GetResponseStream();

        StreamReader Reader = new StreamReader(responseStream, Encoding.Default);

        string Html = Reader.ReadToEnd();

        JObject json = JObject.Parse(Html);

        var data = (JObject)JsonConvert.DeserializeObject(json.ToString());
        string actualData = data.ToString();

        JObject obj = new JObject();

        try
        {
            obj = JObject.Parse(actualData.ToString());
        }
        catch
        {
        }

        PLRate = obj.SelectToken("PLRate").ToString();
        NetRate = obj.SelectToken("NetRate").ToString();
        GST = obj.SelectToken("GST").ToString();

        WebResponse.Close();
        responseStream.Close();
    }
}