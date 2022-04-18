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


public partial class API_MyCart : System.Web.UI.Page
{
    
    //string APIKey;
    string UserId;
    //string IMEI;

    API_BLL _aPI_BLL = new API_BLL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                UserId = (((Request.Form["UserId"] != null && Request.Form["UserId"] != "")) ? Request.Form["UserId"].ToString() : null);

                //if (Request.Form["APIKey"] != null && Request.Form["APIKey"] != "")
                //    APIKey = Request.Form["APIKey"].ToString();
                //else
                //    APIKey = null;

                //IMEI = (((Request.Form["IMEI"] != null && Request.Form["IMEI"] != "")) ? Request.Form["IMEI"].ToString() : null);

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

    private static string replaceSplChar(string Data)
    {
        Data = Data.Replace("%", "%25");
        Data = Data.Replace("<", "%3C");
        Data = Data.Replace(">", "%3E");
        Data = Data.Replace("&", "%26");
        Data = Data.Replace("+", "%2B");
        Data = Data.Replace("#", "%23");
        Data = Data.Replace("*", "%2A");
        Data = Data.Replace("!", "%21");
       // Data = Data.Replace(",", "%2C");
        Data = Data.Replace("'", "%27");
        Data = Data.Replace("=", "%3D");
        Data = Data.Replace("â‚¬", "%E2%82%AC");
        Data = Data.Replace("?", "%3F");
        //Data = Data.Replace(" ", "+");
        Data = Data.Replace("$", "%24");
        Data = Data.Replace("[", "");
        Data = Data.Replace("]", "");
        Data = Data.Replace("\"", "`");

        return Data;
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
                        string replaceText;

                        replaceText = replaceSplChar(ds.Tables[0].Rows[i][j].ToString());

                        JsonString.Append("\"" + ds.Tables[0].Columns[j].ColumnName.ToString() + "\":" + "\"" + replaceText.ToString() + "\",");
                    }
                    else if (j == ds.Tables[0].Columns.Count - 1)
                    {
                        string replaceText;

                        replaceText = replaceSplChar(ds.Tables[0].Rows[i][j].ToString());

                        JsonString.Append("\"" + ds.Tables[0].Columns[j].ColumnName.ToString() + "\":" + "\"" + replaceText.ToString() + "\"");
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
            da = _aPI_BLL.returnDataTable(" select m.*," +
                                          "  Photo= (select top 1 replace(x.ImageName, '" + ConfigurationSettings.AppSettings["FolderPath"] + "', " +
                                          " '" + ConfigurationSettings.AppSettings["FolderPathShow"] + "') from Itms x where x.ItmId = m.ItmId)" +
                                          ",Name = (select top 1 N.Name from Itms N where N.ItmId = m.ItmId) "+
                                          ",Address = (select top 1 StreetAddress from UserAddresses a where a.UserId = m.UserId)" +
                                          ",City = (select top 1 City from UserAddresses a  where a.UserId = m.UserId)" +
                                          ",TotalAmount = (select top 1 (T.MRP * m.Qty) from Itms T where m.ItmId = T.ItmId)" +
                                          ",GrandTotal = (select Sum(T.Amount) from MyCarts T where m.UserId = T.UserId)" +
                                          " from MyCarts m where m.UserId = '" + UserId + "' ");


            //da.Columns["PLRate"].ReadOnly = false;
            //da.Columns["NetRate"].ReadOnly = false;
            //da.Columns["GST"].ReadOnly = false;
            //da.Columns["TotalAmt"].ReadOnly = false;
            //da.Columns["GSTAmt"].ReadOnly = false;

            //for (int i = 0; i <= da.Rows.Count - 1; i++)
            //{
            //    string PLRate, NetRate, GST;
            //    DataTable dtRateAndGST = new DataTable();
            //    GetRateAndGST(da.Rows[i]["PartCode"].ToString(), CustCode, out PLRate, out NetRate, out GST);

            //    da.Rows[i]["PLRate"] = PLRate.Replace("\"","");
            //    da.Rows[i]["NetRate"] = NetRate.Replace("\"", "");
            //    da.Rows[i]["GST"] = GST.Replace("\"", "");

            //    decimal Amt = Convert.ToDecimal(NetRate.Replace("\"", "")) * Convert.ToDecimal(da.Rows[i]["Qty"].ToString());
            //    decimal GSTAmt = Amt * Convert.ToDecimal(GST.Replace("\"", "")) / 100;

            //    da.Rows[i]["GSTAmt"] = GSTAmt;
            //    da.Rows[i]["TotalAmt"] = GSTAmt + Amt;
            //}

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

    //[WebMethod]
    //public void GetRateAndGST(string PartCode, string CustCode, out string PLRate, out string NetRate, out string GST)
    //{
    //    ArrayList al = new ArrayList();

    //    DataTable dtPlant = new DataTable();

    //    var baseAddress = WebConfigurationManager.AppSettings["APIURL"].ToString();

    //    HttpWebRequest Http = (HttpWebRequest)WebRequest.Create(baseAddress);

    //    Http.ContentType = "application/json; charset=utf-8";

    //    Http.Method = "POST";
    //    byte[] lbPostBuffer = Encoding.Default.GetBytes("{PartCode:\"" + PartCode + "\",CustCode:\"" + CustCode + "\"}");

    //    Http.ContentLength = lbPostBuffer.Length;

    //    Stream PostStream = Http.GetRequestStream();
    //    PostStream.Write(lbPostBuffer, 0, lbPostBuffer.Length);
    //    PostStream.Close();

    //    HttpWebResponse WebResponse = (HttpWebResponse)Http.GetResponse();

    //    Stream responseStream = WebResponse.GetResponseStream();

    //    StreamReader Reader = new StreamReader(responseStream, Encoding.Default);

    //    string Html = Reader.ReadToEnd();

    //    JObject json = JObject.Parse(Html);

    //    var data = (JObject)JsonConvert.DeserializeObject(json.ToString());
    //    string actualData = data.ToString();

    //    JObject obj = new JObject();

    //    try
    //    {
    //        obj = JObject.Parse(actualData.ToString());
    //    }
    //    catch
    //    {
    //    }

    //    PLRate = obj.SelectToken("PLRate").ToString();
    //    NetRate = obj.SelectToken("NetRate").ToString();
    //    GST = obj.SelectToken("GST").ToString();

    //    WebResponse.Close();
    //    responseStream.Close();
    //}
}