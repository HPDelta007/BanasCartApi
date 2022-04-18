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


public partial class API_ItmGroup : System.Web.UI.Page
{
    string Code;
    string Level;
    string APIKey;
    string CustCode;
    int? TotalPages;
    int? PageNumber;
    int? PageOffset;

    API_BLL _aPI_BLL = new API_BLL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                Code = (((Request.Form["Code"] != null && Request.Form["Code"] != "")) ? Request.Form["Code"].ToString() : null);
                Level = (((Request.Form["Level"] != null && Request.Form["Level"] != "")) ? Request.Form["Level"].ToString() : null);

                if (Request.Form["APIKey"] != null && Request.Form["APIKey"] != "")
                    APIKey = Request.Form["APIKey"].ToString();
                else
                    APIKey = null;

                CustCode = (((Request.Form["CustCode"] != null && Request.Form["CustCode"] != "")) ? Request.Form["CustCode"].ToString() : null);

                if (Request.Form["TotalPages"] != null && Request.Form["TotalPages"] != "")
                    TotalPages = Convert.ToInt32(Request.Form["TotalPages"].ToString());
                else
                    TotalPages = null;

                if (Request.Form["PageNumber"] != null && Request.Form["PageNumber"] != "")
                    PageNumber = Convert.ToInt32(Request.Form["PageNumber"].ToString());
                else
                    PageNumber = null;

                if (Request.Form["PageOffset"] != null && Request.Form["PageOffset"] != "")
                    PageOffset = Convert.ToInt32(Request.Form["PageOffset"].ToString());
                else
                    PageOffset = null;

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
        Data = Data.Replace(",", "%2C");
        Data = Data.Replace("'", "%27");
        Data = Data.Replace("=", "%3D");
        Data = Data.Replace("â‚¬", "%E2%82%AC");
        Data = Data.Replace("?", "%3F");
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
            if (Code == null)
            {
                da = _aPI_BLL.returnDataTable(" select m.Lvl, m.Code, m.MasterType, m.Name as Name, m.Alias, m.ParentGrp, (select count(*) from MasterGrp x where x.ParentGrp = m.Code and x.Lvl = (m.Lvl + 1)) as 'SubCount' " +
                                              " , null as 'Photo' " +
                                              " ,null as LastOrdDt,null as PriceListRate " +
                                              " ,null as MRP,null as PurchaseRate,Case when m.Name = 'OTHER' or m.Name = 'POP SONALIKA' then 9 else 1 end as OrdBy,0 as PhotoOrderBy,0 as ItmOrderBy " +
                                              " ,TotalPages = 0 " + 
                                              " from MasterGrp m " +
                                              " inner join ItmGrpShowInApps i on i.Code = m.Code " +
                                              " where m.Lvl = 1 " +
                                              " and (select count(*) from MasterGrp x where x.ParentGrp = m.Code and x.Lvl = (m.Lvl + 1)) <> 0 " +
                                              " order by OrdBy,m.Name ");
            }
            else
            {
                da = _aPI_BLL.returnDataTable(" select case when m.MasterType = 6 then 0 else m.Lvl end as Lvl, m.Code, m.MasterType, m.Name as Name " +
                                              ", m.Alias, m.ParentGrp, (select count(*) from MasterGrp x where x.ParentGrp = m.Code and x.Lvl = (m.Lvl + 1)) as 'SubCount' " +
                                              " , REPLACE(((select TOP 1 z.FilePath from " + ConfigurationManager.AppSettings["DBName"].ToString() + "..FU z " +
                                              " inner join " + ConfigurationManager.AppSettings["DBName"].ToString() + "..Itms y on z.RecordId = y.ItmId " +
                                              " Where y.Alias = t.Alias order by z.LnNo)),'" + ConfigurationManager.AppSettings["ERPItmPhotoPath"].ToString() + "','" + ConfigurationManager.AppSettings["ERPItmPhotoURL"].ToString() + "') as 'Photo' " +
                                              " ,ls.LastOrdDt,ISNULL(b.Rate,t.StdRate) as PriceListRate " +
                                              " ,0.00 as MRP,0.00 as PurchaseRate,Case when m.Name = 'OTHER' or m.Name = 'POP SONALIKA' then 9 else 1 end as OrdBy " +
                                              " ,Case when (Select COUNT(*) From " + ConfigurationManager.AppSettings["DBName"].ToString() + "..FU f where f.RecordId = t.ItmId) > 0 then 1 else 0 end as PhotoOrderBy " +
                                              " ,(Select SUM(Qty) From " + ConfigurationManager.AppSettings["DBName"].ToString() + "..DeliNoteLns dl where dl.ItmId = t.ItmId) as ItmOrderBy " +
                                              " ,TotalPages = (SELECT [dbo].[ConvertHHMMForAPI](count(*)," + PageOffset + ") " +
                                              " from MasterGrp m inner join ItmGrpShowInApps i on i.Code = m.Code " +
                                              " left join Itms t on t.Code = m.Code " +
                                              " where m.Lvl = " + Level + " " +
                                              " and ParentGrp = " + Code + "  " +
                                              " ) " + 
                                              " from MasterGrp m inner join ItmGrpShowInApps i on i.Code = m.Code " +
                                              " left join Itms t on t.Code = m.Code " +
                                              " left join (Select sl.ItmCode,MAX(s.SODt) as LastOrdDt from SOLns sl inner join SOs s on s.SOId = sl.SOId " + (CustCode != null ? " Where s.CustomerCode = '" + CustCode + "'" : "") + " group by sl.ItmCode) ls on ls.ItmCode = m.Code " +
                                              " left join (Select pl.ItmId,pl.Rate,ROW_NUMBER() Over(Partition By pl.ItmId order by p.EffectiveDt desc) as LnNo from PLLns pl inner join PLs p on p.PLId = pl.PLId " +
                                              " where (p.EffectiveDt is null or p.EffectiveDt <= CONVERT(date,GETDATE())) and (p.InEffectiveDt is null or p.InEffectiveDt >= CONVERT(date,GETDATE()))) b on b.ItmId = t.ItmId and b.LnNo = 1 " +
                                              " where m.Lvl = " + Level + " " +
                                              " and ParentGrp = " + Code + "  " +
                                              " order by OrdBy,PhotoOrderBy desc,ItmOrderBy desc,m.Name " +
                                              " OFFSET (" + PageOffset + ")*(" + PageNumber + ") ROWS" +
                                              " FETCH NEXT (" + PageOffset + ") ROWS ONLY");
            }

            da.Columns["MRP"].ReadOnly = false;
            da.Columns["PurchaseRate"].ReadOnly = false;

            for (int i = 0; i <= da.Rows.Count - 1; i++)
            {
                try
                {
                    string PLRate, NetRate, GST;
                    DataTable dtRateAndGST = new DataTable();
                    GetRateAndGST(da.Rows[i]["Alias"].ToString(), CustCode, out PLRate, out NetRate, out GST);

                    da.Rows[i]["MRP"] = PLRate.Replace("\"", "");
                    da.Rows[i]["PurchaseRate"] = NetRate.Replace("\"", "");
                }
                catch
                {
                    da.Rows[i]["MRP"] = 0.00;
                    da.Rows[i]["PurchaseRate"] = 0.00;
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

    [WebMethod]
    public void GetRateAndGST(string PartCode, string CustCode, out string PLRate, out string NetRate, out string GST)
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