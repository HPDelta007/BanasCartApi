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


public partial class API_InsertComplaintVisit : System.Web.UI.Page
{
    string ComplaintVisitId;
    string ComplaintId;
    DateTime? VisitDate;
    string ProductType;
    string PumpSerialNo;
    string MotorSerialNo;
    string Depth;
    string AMPWithOutLoad;
    string AMPWithLoad;
    string AMPCapacitor;
    string CableType;
    string CableSize;
    string CableBrand;
    string CableConnection;
    string CableLength;
    string BorwellWaterLevel;
    string BorewellColumnSize;
    string Voltage;
    string Red;
    string Blue;
    string Yellow;
    string CompaintDetail;
    string VisitedByUserId;
    string Status;
    DateTime? SolvedOn;
    string SolutionRemarks;
    string APIKey;

    API_BLL _aPI_BLL = new API_BLL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                ComplaintId = (((Request.Form["ComplaintId"] != null && Request.Form["ComplaintId"] != "")) ? Request.Form["ComplaintId"].ToString() : null);
                VisitDate = (((Request.Form["VisitDate"] != null && Request.Form["VisitDate"] != "")) ? Convert.ToDateTime(Request.Form["VisitDate"].ToString()) : Convert.ToDateTime(null));
                ProductType = (((Request.Form["ProductType"] != null && Request.Form["ProductType"] != "")) ? Request.Form["ProductType"].ToString() : null);
                PumpSerialNo = (((Request.Form["PumpSerialNo"] != null && Request.Form["PumpSerialNo"] != "")) ? Request.Form["PumpSerialNo"].ToString() : null);
                MotorSerialNo = (((Request.Form["MotorSerialNo"] != null && Request.Form["MotorSerialNo"] != "")) ? Request.Form["MotorSerialNo"].ToString() : null);
                Depth = (((Request.Form["Depth"] != null && Request.Form["Depth"] != "")) ? Request.Form["Depth"].ToString() : null);
                AMPWithOutLoad = (((Request.Form["AMPWithOutLoad"] != null && Request.Form["AMPWithOutLoad"] != "")) ? Request.Form["AMPWithOutLoad"].ToString() : null);
                AMPWithLoad = (((Request.Form["AMPWithLoad"] != null && Request.Form["AMPWithLoad"] != "")) ? Request.Form["AMPWithLoad"].ToString() : null);
                AMPCapacitor = (((Request.Form["AMPCapacitor"] != null && Request.Form["AMPCapacitor"] != "")) ? Request.Form["AMPCapacitor"].ToString() : null);
                CableType = (((Request.Form["CableType"] != null && Request.Form["CableType"] != "")) ? Request.Form["CableType"].ToString() : null);
                CableSize = (((Request.Form["CableSize"] != null && Request.Form["CableSize"] != "")) ? Request.Form["CableSize"].ToString() : null);
                CableBrand = (((Request.Form["CableBrand"] != null && Request.Form["CableBrand"] != "")) ? Request.Form["CableBrand"].ToString() : null);
                CableConnection = (((Request.Form["CableConnection"] != null && Request.Form["CableConnection"] != "")) ? Request.Form["CableConnection"].ToString() : null);
                CableLength = (((Request.Form["CableLength"] != null && Request.Form["CableLength"] != "")) ? Request.Form["CableLength"].ToString() : null);
                BorwellWaterLevel = (((Request.Form["BorwellWaterLevel"] != null && Request.Form["BorwellWaterLevel"] != "")) ? Request.Form["BorwellWaterLevel"].ToString() : null);
                BorewellColumnSize = (((Request.Form["BorewellColumnSize"] != null && Request.Form["BorewellColumnSize"] != "")) ? Request.Form["BorewellColumnSize"].ToString() : null);
                Voltage = (((Request.Form["Voltage"] != null && Request.Form["Voltage"] != "")) ? Request.Form["Voltage"].ToString() : null);
                Red = (((Request.Form["Red"] != null && Request.Form["Red"] != "")) ? Request.Form["Red"].ToString() : null);
                Blue = (((Request.Form["Blue"] != null && Request.Form["Blue"] != "")) ? Request.Form["Blue"].ToString() : null);
                Yellow = (((Request.Form["Yellow"] != null && Request.Form["Yellow"] != "")) ? Request.Form["Yellow"].ToString() : null);
                CompaintDetail = (((Request.Form["CompaintDetail"] != null && Request.Form["CompaintDetail"] != "")) ? Request.Form["CompaintDetail"].ToString() : null);
                VisitedByUserId = (((Request.Form["VisitedByUserId"] != null && Request.Form["VisitedByUserId"] != "")) ? Request.Form["VisitedByUserId"].ToString() : null);
                Status = (((Request.Form["Status"] != null && Request.Form["Status"] != "")) ? Request.Form["Status"].ToString() : null);
                SolvedOn = (((Request.Form["SolvedOn"] != null && Request.Form["SolvedOn"] != "")) ? Convert.ToDateTime(Request.Form["SolvedOn"].ToString()) : Convert.ToDateTime(null));
                SolutionRemarks = (((Request.Form["SolutionRemarks"] != null && Request.Form["SolutionRemarks"] != "")) ? Request.Form["SolutionRemarks"].ToString() : null);

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
            ComplaintVisitId = _aPI_BLL.InsertUpdateWithReturnIdentity(" insert into ComplaintVisits (ComplaintId, LnNo, VisitDate, ProductType, PumpSerialNo, MotorSerialNo, Depth, " +
                                        " AMPWithOutLoad, AMPWithLoad, AMPCapacitor, CableType, CableSize, CableBrand, CableConnection, CableLength, BorwellWaterLevel,  " +
                                        " BorewellColumnSize, Voltage, Red, Blue, Yellow, CompaintDetail, VisitedByUserId, Status, SolvedOn, SolutionRemarks, InsertedOn, " +
                                        " LastUpdatedOn, InsertedByUserId, LastUpdatedByUserId) values " +
                                        " (" +
                                        " " + ((ComplaintId == null) ? "NULL" : "'" + ComplaintId.ToString() + "'") + " " +
                                        " , (select isnull(max(isnull(LnNo, 0)) + 1, 1) from ComplaintVisits where ComplaintId = '" + ComplaintId.ToString() + "') " +
                                        " , " + ((VisitDate == null) ? "NULL" : "'" + Convert.ToDateTime(VisitDate).ToString("dd-MMM-yyyy") + "'") + " " +
                                        " , " + ((ProductType == null) ? "NULL" : "'" + ProductType.ToString() + "'") + " " +
                                        " , " + ((PumpSerialNo == null) ? "NULL" : "'" + PumpSerialNo.ToString().Replace("'", "''") + "'") + " " +
                                        " , " + ((MotorSerialNo == null) ? "NULL" : "'" + MotorSerialNo.ToString().Replace("'", "''") + "'") + " " +
                                        " , " + ((Depth == null) ? "NULL" : "'" + Depth.ToString().Replace("'", "''") + "'") + " " +
                                        " , " + ((AMPWithOutLoad == null) ? "NULL" : "'" + AMPWithOutLoad.ToString().Replace("'", "''") + "'") + " " +
                                        " , " + ((AMPWithLoad == null) ? "NULL" : "'" + AMPWithLoad.ToString().Replace("'", "''") + "'") + " " +
                                        " , " + ((AMPCapacitor == null) ? "NULL" : "'" + AMPCapacitor.ToString().Replace("'", "''") + "'") + " " +
                                        " , " + ((CableType == null) ? "NULL" : "'" + CableType.ToString().Replace("'", "''") + "'") + " " +
                                        " , " + ((CableSize == null) ? "NULL" : "'" + CableSize.ToString().Replace("'", "''") + "'") + " " +
                                        " , " + ((CableBrand == null) ? "NULL" : "'" + CableBrand.ToString().Replace("'", "''") + "'") + " " +
                                        " , " + ((CableConnection == null) ? "NULL" : "'" + CableConnection.ToString().Replace("'", "''") + "'") + " " +
                                        " , " + ((CableLength == null) ? "NULL" : "'" + CableLength.ToString().Replace("'", "''") + "'") + " " +
                                        " , " + ((BorwellWaterLevel == null) ? "NULL" : "'" + BorwellWaterLevel.ToString().Replace("'", "''") + "'") + " " +
                                        " , " + ((BorewellColumnSize == null) ? "NULL" : "'" + BorewellColumnSize.ToString().Replace("'", "''") + "'") + " " +
                                        " , " + ((Voltage == null) ? "NULL" : "'" + Voltage.ToString().Replace("'", "''") + "'") + " " +
                                        " , " + ((Red == null) ? "NULL" : "'" + Red.ToString().Replace("'", "''") + "'") + " " +
                                        " , " + ((Blue == null) ? "NULL" : "'" + Blue.ToString().Replace("'", "''") + "'") + " " +
                                        " , " + ((Yellow == null) ? "NULL" : "'" + Yellow.ToString().Replace("'", "''") + "'") + " " +
                                        " , " + ((CompaintDetail == null) ? "NULL" : "N'" + CompaintDetail.ToString().Replace("'", "''") + "'") + " " +
                                        " , " + ((VisitedByUserId == null) ? "NULL" : "'" + VisitedByUserId.ToString().Replace("'", "''") + "'") + " " +
                                        " , " + ((Status == null) ? "NULL" : "'" + Status.ToString().Replace("'", "''") + "'") + " " +
                                        " , " + ((SolvedOn == null) ? "NULL" : "'" + Convert.ToDateTime(SolvedOn).ToString("dd-MMM-yyyy").Replace("'", "''") + "'") + " " +
                                        " , " + ((SolutionRemarks == null) ? "NULL" : "N'" + SolutionRemarks.ToString().Replace("'", "''") + "'") + " " +
                                        " , GETDATE(), GETDATE(), '" + VisitedByUserId + "', NULL " +
                                        ");select @@IDENTITY;");

            if (ComplaintVisitId != null)
                da = _aPI_BLL.returnDataTable("  select ComplaintId from ComplaintVisits where ComplaintVisitId = '" + ComplaintVisitId.ToString() + "'  ");
            else
                da = null;

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
}