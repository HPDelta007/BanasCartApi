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


public partial class API_InsertDistributorDealers : System.Web.UI.Page
{
    string DistributorDealerId;
    string DataType;
    string Name;
    string Address;
    string Lat;
    string Long;
    string States;
    string District;
    string CityVillage;
    string ContactPersonName;
    string WhatsAppMobileNo;
    string OtherMobileNo;
    string EmailId;
    string GSTNo;
    string PANNo;
    string BankName;
    string BankACNo;
    string BankIFSCCode;
    string ParentDistributorDealerId;
    string InsertedByUserId;

    string APIKey;

    HttpPostedFile ProfilePhoto;
    HttpPostedFile CoverPhoto;

    API_BLL _aPI_BLL = new API_BLL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                DataType = (((Request.Form["DataType"] != null && Request.Form["DataType"] != "")) ? Request.Form["DataType"].ToString() : null);
                Name = (((Request.Form["Name"] != null && Request.Form["Name"] != "")) ? Request.Form["Name"].ToString() : null);
                Address = (((Request.Form["Address"] != null && Request.Form["Address"] != "")) ? Request.Form["Address"].ToString() : null);
                Lat = (((Request.Form["Lat"] != null && Request.Form["Lat"] != "")) ? Request.Form["Lat"].ToString() : null);
                Long = (((Request.Form["Long"] != null && Request.Form["Long"] != "")) ? Request.Form["Long"].ToString() : null);
                States = (((Request.Form["States"] != null && Request.Form["States"] != "")) ? Request.Form["States"].ToString() : null);
                District = (((Request.Form["District"] != null && Request.Form["District"] != "")) ? Request.Form["District"].ToString() : null);
                CityVillage = (((Request.Form["CityVillage"] != null && Request.Form["CityVillage"] != "")) ? Request.Form["CityVillage"].ToString() : null);
                ContactPersonName = (((Request.Form["ContactPersonName"] != null && Request.Form["ContactPersonName"] != "")) ? Request.Form["ContactPersonName"].ToString() : null);
                WhatsAppMobileNo = (((Request.Form["WhatsAppMobileNo"] != null && Request.Form["WhatsAppMobileNo"] != "")) ? Request.Form["WhatsAppMobileNo"].ToString() : null);
                OtherMobileNo = (((Request.Form["OtherMobileNo"] != null && Request.Form["OtherMobileNo"] != "")) ? Request.Form["OtherMobileNo"].ToString() : null);
                EmailId = (((Request.Form["EmailId"] != null && Request.Form["EmailId"] != "")) ? Request.Form["EmailId"].ToString() : null);
                GSTNo = (((Request.Form["GSTNo"] != null && Request.Form["GSTNo"] != "")) ? Request.Form["GSTNo"].ToString() : null);
                PANNo = (((Request.Form["PANNo"] != null && Request.Form["PANNo"] != "")) ? Request.Form["PANNo"].ToString() : null);
                BankName = (((Request.Form["BankName"] != null && Request.Form["BankName"] != "")) ? Request.Form["BankName"].ToString() : null);
                BankACNo = (((Request.Form["BankACNo"] != null && Request.Form["BankACNo"] != "")) ? Request.Form["BankACNo"].ToString() : null);
                BankIFSCCode = (((Request.Form["BankIFSCCode"] != null && Request.Form["BankIFSCCode"] != "")) ? Request.Form["BankIFSCCode"].ToString() : null);
                ParentDistributorDealerId = (((Request.Form["ParentDistributorDealerId"] != null && Request.Form["ParentDistributorDealerId"] != "")) ? Request.Form["ParentDistributorDealerId"].ToString() : null);
                InsertedByUserId = (((Request.Form["InsertedByUserId"] != null && Request.Form["InsertedByUserId"] != "")) ? Request.Form["InsertedByUserId"].ToString() : null);

                ProfilePhoto = (((Request.Files["ProfilePhoto"] != null && Request.Files["ProfilePhoto"].FileName.ToString() != "")) ? Request.Files["ProfilePhoto"] : null);
                CoverPhoto = (((Request.Files["CoverPhoto"] != null && Request.Files["CoverPhoto"].FileName.ToString() != "")) ? Request.Files["CoverPhoto"] : null);

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
            DistributorDealerId = _aPI_BLL.InsertUpdateWithReturnIdentity(" insert into DistributorDealers (DataType, Name, Address, Lat, Long, States, " +
                                        " District, CityVillage, ContactPersonName, WhatsAppMobileNo, OtherMobileNo, EmailId, GSTNo, PANNo, BankName, BankACNo, " +
                                        "  BankIFSCCode, ParentDistributorDealerId, InsertedOn, LastUpdatedOn, InsertedByUserId, " +
                                        "  LastUpdatedByUserId) values " +
                                        " (" +
                                        " " + ((DataType == null) ? "NULL" : "'" + DataType.ToString() + "'") + " " +
                                        " , " + ((Name == null) ? "NULL" : "'" + Name.ToString().Replace("'", "''") + "'") + " " +
                                        " , " + ((Address == null) ? "NULL" : "'" + Address.ToString().Replace("'", "''") + "'") + " " +
                                        " , " + ((Lat == null) ? "NULL" : "'" + Lat.ToString().Replace("'", "''") + "'") + " " +
                                        " , " + ((Long == null) ? "NULL" : "'" + Long.ToString().Replace("'", "''") + "'") + " " +
                                        " , " + ((States == null) ? "NULL" : "'" + States.ToString().Replace("'", "''") + "'") + " " +
                                        " , " + ((District == null) ? "NULL" : "N'" + District.ToString().Replace("'", "''") + "'") + " " +
                                        " , " + ((CityVillage == null) ? "NULL" : "'" + CityVillage.ToString().Replace("'", "''") + "'") + " " +
                                        " , " + ((ContactPersonName == null) ? "NULL" : "'" + ContactPersonName.ToString().Replace("'", "''") + "'") + " " +
                                        " , " + ((WhatsAppMobileNo == null) ? "NULL" : "'" + WhatsAppMobileNo.ToString().Replace("'", "''") + "'") + " " +
                                        " , " + ((OtherMobileNo == null) ? "NULL" : "'" + OtherMobileNo.ToString().Replace("'", "''") + "'") + " " +
                                        " , " + ((EmailId == null) ? "NULL" : "'" + EmailId.ToString().Replace("'", "''") + "'") + " " +
                                        " , " + ((GSTNo == null) ? "NULL" : "'" + GSTNo.ToString().Replace("'", "''") + "'") + " " +
                                        " , " + ((PANNo == null) ? "NULL" : "'" + PANNo.ToString().Replace("'", "''") + "'") + " " +
                                        " , " + ((BankName == null) ? "NULL" : "'" + BankName.ToString().Replace("'", "''") + "'") + " " +
                                        " , " + ((BankACNo == null) ? "NULL" : "'" + BankACNo.ToString().Replace("'", "''") + "'") + " " +
                                        " , " + ((BankIFSCCode == null) ? "NULL" : "'" + BankIFSCCode.ToString().Replace("'", "''") + "'") + " " +
                                        " , " + ((ParentDistributorDealerId == null) ? "NULL" : "'" + ParentDistributorDealerId.ToString().Replace("'", "''") + "'") + " " +
                                        " , GETDATE(), GETDATE(), '" + InsertedByUserId + "', NULL " +
                                        ");select @@IDENTITY;");


            if (ProfilePhoto != null && ProfilePhoto.FileName != "")
            {
                string[] getExtenstion = ProfilePhoto.FileName.Split('.');
                string oExtension = getExtenstion[getExtenstion.Length - 1].ToString();
                string FileNameForInsert = ProfilePhoto.FileName.Replace("." + oExtension, "");

                var filePath = ConfigurationManager.AppSettings["FolderPath"].ToString() + FileNameForInsert + DistributorDealerId.ToString() + "-PF." + oExtension;
                ProfilePhoto.SaveAs(filePath);

                _aPI_BLL.InsertUpdateNonQuery(" update DistributorDealers set ProfilePhoto = '" + filePath.ToString() + "' where DistributorDealerId = '" + DistributorDealerId.ToString() + "' ");
            }

            if (CoverPhoto != null && CoverPhoto.FileName != "")
            {
                string[] getExtenstion = CoverPhoto.FileName.Split('.');
                string oExtension = getExtenstion[getExtenstion.Length - 1].ToString();
                string FileNameForInsert = CoverPhoto.FileName.Replace("." + oExtension, "");

                var filePath = ConfigurationManager.AppSettings["FolderPath"].ToString() + FileNameForInsert + DistributorDealerId.ToString() + "-CP." + oExtension;
                CoverPhoto.SaveAs(filePath);

                _aPI_BLL.InsertUpdateNonQuery(" update DistributorDealers set CoverPhoto = '" + filePath.ToString() + "' where DistributorDealerId = '" + DistributorDealerId.ToString() + "' ");
            }


            if (DistributorDealerId != null)
                da = _aPI_BLL.returnDataTable("  select Name from DistributorDealers where DistributorDealerId = '" + DistributorDealerId.ToString() + "'  ");
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