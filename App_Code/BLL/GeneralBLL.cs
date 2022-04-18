using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Data;
using System.Text;

public class GeneralBLL
{
    string _UserName;
    string _FormName;
    bool _IsAdmin;
    GeneralDAL _GeneralDAL;

    public string FormName
    {
        get
        {
            return _FormName;
        }
        set
        {
            _FormName = value;
        }
    }
    public string UserName
    {
        get
        {
            return _UserName;
        }
        set
        {
            _UserName = value;
        }
    }
    public bool IsAdmin
    {
        get
        {
            return _IsAdmin;
        }
        set
        {
            _IsAdmin = value;
        }
    }
    public decimal CalcUMConvVal(decimal qtyPriUM, decimal qtyAltUM)
    {
        try
        {
            return (qtyAltUM / qtyPriUM);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public decimal CalcAltUMQty(decimal qtyPriUM, decimal umConvVal, int umDecimalPlacesAlt)
    {
        try
        {
            if (umDecimalPlacesAlt > 0)
            {
                return Math.Round((qtyPriUM * umConvVal), umDecimalPlacesAlt);
            }
            else
            {
                if (Math.Floor((qtyPriUM * umConvVal)) != Math.Round((qtyPriUM * umConvVal), 4))
                {
                    return Math.Round((qtyPriUM * umConvVal) + Convert.ToDecimal(0.5), umDecimalPlacesAlt);
                }
                else
                {
                    return Math.Round((qtyPriUM * umConvVal), umDecimalPlacesAlt);
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public decimal CalcPriUMQty(decimal qtyAltUM, decimal umConvVal, int umDecimalPlacesPri)
    {
        try
        {
            if (umDecimalPlacesPri > 0)
            {
                return Math.Round((qtyAltUM / umConvVal), umDecimalPlacesPri);
            }
            else
            {
                if (Math.Floor((qtyAltUM * umConvVal)) != Math.Round((qtyAltUM * umConvVal), 4))
                {
                    return Math.Round((qtyAltUM / umConvVal) + Convert.ToDecimal(0.5), umDecimalPlacesPri);
                }
                else
                {
                    return Math.Round((qtyAltUM / umConvVal), umDecimalPlacesPri);
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public decimal CalcRatePerAltUM(decimal ratePerPriUM, decimal uMConvVal, Int32 crncyDecimalPlaces)
    {
        try
        {
            return Math.Round((ratePerPriUM / uMConvVal), crncyDecimalPlaces);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public decimal CalcRatePerPriUM(decimal ratePerAltUM, decimal uMConvVal, Int32 crncyDecimalPlaces)
    {
        try
        {
            return Math.Round((ratePerAltUM * uMConvVal), crncyDecimalPlaces);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public DateTime GetFYStartDate()
    {
        try
        {
            return Convert.ToDateTime("01-APR-2010");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    } 

    public string ConvertINRFormat(double _value, int _DecimalDigits)
    {
        try
        {
            NumberFormatInfo nFmtInfo = new NumberFormatInfo();
            nFmtInfo.CurrencyDecimalDigits = _DecimalDigits;
            int[] INDSizes = { 3, 2, 2 };
            int[] USSizes = { 3, 3, 3 };
            string strCountry = "IND";
            if (strCountry == "IND")
            {
                nFmtInfo.CurrencyDecimalSeparator = ".";
                nFmtInfo.CurrencyGroupSeparator = ",";
                nFmtInfo.CurrencySymbol = ""; //Rs.
                nFmtInfo.CurrencyGroupSizes = INDSizes;
            }
            else if (strCountry == "US")
            {
                nFmtInfo.CurrencyDecimalSeparator = ".";
                nFmtInfo.CurrencyGroupSeparator = ",";
                nFmtInfo.CurrencySymbol = "$";
                nFmtInfo.CurrencyGroupSizes = USSizes;
            }
            //double dblValue = 1234567890;
            return _value.ToString("C", nFmtInfo);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    } 


    public string GetHTMLfromDataTable(DataTable DtResult, string ReportTitle, int DecimalReq, string StyleCssHeader, string Style1Css, string Style1ColumnNumbers, string Style1RowNumbers, int ShowRowTotal_StartColumnNo, int ShowGrandTotal_StartRowNo, bool showRowTotalAtLastColumn, bool showGrandTotalAtLastRow, out decimal TotalAmt, string tableID)
    {
        string dispString = "";
        try
        {

            string DecHas = "0";
            string rowColor = "";

            if (DecimalReq > 0)
            {
                DecHas += ".";
                for (int Hi = 0; Hi < DecimalReq; Hi++)
                    DecHas += "#";

            }
            dispString = dispString + "<table " + ((tableID == null) ? "" : "id=" + tableID.ToString() + "") + " border='1' cellspacing='0' cellpadding='5' runat='server'>";
            dispString = dispString + "<tr><td style='text-align:left;background-color:rgb(229,50,56);color:white' Colspan='" + DtResult.Columns.Count + 1 + "'><b><u>" + ReportTitle + "</u></b></td></tr>";
            //dispString = dispString + "<thead>";
            dispString = dispString + "<tr>";
            int ia = 0;
            for (int j = 0; j <= DtResult.Columns.Count - 1; j++)
            {
                ia++;
                dispString = dispString + "<th " + StyleCssHeader + " scope='col'>" + DtResult.Columns[j].ColumnName;
                dispString = dispString + "</th>";

            }
            if (DtResult.Rows.Count == 0)
            {
                dispString = dispString + "<tr>";
                dispString = dispString + "<td colspan=" + ia + " " + StyleCssHeader + " scope='col' style='text-align:center;'>No Record Found";
                dispString = dispString + "</tr>";
            }
            if (showRowTotalAtLastColumn)
                dispString = dispString + "<th " + StyleCssHeader + ">Total</th>";

            dispString = dispString + "</tr>";
            //dispString = dispString + "<thead>";
            decimal result;
            decimal Row_Total = 0;
            decimal GrandTotal_Total = 0;
            bool IsPrint = false;
            for (int i = 0; i <= DtResult.Rows.Count - 1; i++)
            {
                dispString = dispString + "<tr>";

                Row_Total = 0;

                for (int j = 0; j <= DtResult.Columns.Count - 1; j++)
                {
                    if (DtResult.Rows[i][j] != DBNull.Value)
                    {
                        if ((decimal.TryParse(DtResult.Rows[i][j].ToString(), out result)))
                        {
                            if (Style1ColumnNumbers.IndexOf("<" + (j + 1).ToString() + ">") != -1 || (Style1RowNumbers.IndexOf("<" + (i + 1).ToString() + ">") != -1))
                            {
                                dispString = dispString + "<td align=right " + Style1Css + ">";
                            }
                            else
                            {
                                dispString = dispString + "<td align=right " + Style1Css + ">";
                            }
                            if (j >= ShowRowTotal_StartColumnNo && showRowTotalAtLastColumn)
                            {
                                Row_Total += Convert.ToDecimal(DtResult.Rows[i][j].ToString());
                            }
                            if (i >= ShowGrandTotal_StartRowNo && ShowGrandTotal_StartRowNo != -1)
                            {
                                GrandTotal_Total += Convert.ToDecimal(DtResult.Rows[i][j].ToString());
                            }
                            dispString = dispString + result.ToString(DecHas);
                            dispString = dispString + "</td>";
                        }
                        else if (DtResult.Columns[j].DataType == typeof(DateTime))
                        {
                            if (Style1ColumnNumbers.IndexOf("<" + (j + 1).ToString() + ">") != -1 || (Style1RowNumbers.IndexOf("<" + (i + 1).ToString() + ">") != -1))
                            {
                                dispString = dispString + "<td align=left " + Style1Css + ">";
                            }
                            else
                            {
                                dispString = dispString + "<td align=left>";
                            }

                            dispString = dispString + Convert.ToDateTime(DtResult.Rows[i][j]).ToString("dd-MMM-yyyy hh:mm:ss tt");
                            dispString = dispString + "</td>";
                        }
                        else
                        {
                            if (Style1ColumnNumbers.IndexOf("<" + (j + 1).ToString() + ">") != -1 || (Style1RowNumbers.IndexOf("<" + (i + 1).ToString() + ">") != -1))
                            {
                                dispString = dispString + "<td align=left " + Style1Css + ">";
                            }
                            else
                            {
                                dispString = dispString + "<td align=left>";
                            }

                            //if (j + 1 > ShowRowTotal_StartColumnNo)
                            //{
                            //    Row_Total += Convert.ToDecimal(DtResult.Rows[i][j].ToString());
                            //}
                            //if (i + 1 > ShowGrandTotal_StartRowNo && j + 1 > ShowRowTotal_StartColumnNo)
                            //{
                            //    GrandTotal_Total += Convert.ToDecimal(DtResult.Rows[i][j].ToString());
                            //}
                            dispString = dispString + DtResult.Rows[i][j].ToString().Replace(".00000", "");
                            dispString = dispString + "</td>";
                        }
                    }
                    else
                    {
                        if (Style1ColumnNumbers.IndexOf("<" + (j + 1).ToString() + ">") != -1 || (Style1RowNumbers.IndexOf("<" + (i + 1).ToString() + ">") != -1))
                        {
                            dispString = dispString + "<td align=left " + Style1Css + ">";
                        }
                        else
                        {
                            dispString = dispString + "<td align=left>";
                        }
                        dispString = dispString + "</td>";
                    }
                }


                if (showRowTotalAtLastColumn)
                {
                    dispString = dispString + "<td align=right  " + Style1Css + ">";
                    dispString = dispString + Row_Total.ToString(DecHas);
                    dispString = dispString + "</td>";
                }
                dispString = dispString + "</tr>";
            }
            dispString = dispString + "<tr>";
            if (showGrandTotalAtLastRow)
            {
                dispString = dispString + "<td " + StyleCssHeader + "  colspan='" + ShowRowTotal_StartColumnNo + "'>Grand Total:</td>";
                for (int j = 0; j <= DtResult.Columns.Count - 1; j++)
                {
                    Row_Total = 0;
                    for (int i = 0; i <= DtResult.Rows.Count - 1; i++)
                    {
                        if (i + 1 > ShowGrandTotal_StartRowNo && j + 1 > ShowRowTotal_StartColumnNo)
                        {
                            if (DtResult.Rows[i][j] != DBNull.Value)
                            {
                                try
                                {
                                    if (DtResult.Rows[i][j] != "")
                                        Row_Total += Convert.ToDecimal(DtResult.Rows[i][j].ToString());
                                }
                                catch
                                { }
                            }
                            IsPrint = true;
                        }
                    }
                    if (IsPrint == true)
                    {
                        dispString = dispString + "<td align=right " + StyleCssHeader + ">";
                        dispString = dispString + Row_Total.ToString(DecHas);
                        dispString = dispString + "</td>";
                    }
                }
                if (showRowTotalAtLastColumn)
                {
                    dispString = dispString + "<td align=right " + StyleCssHeader + ">";
                    dispString = dispString + GrandTotal_Total.ToString(DecHas);
                    dispString = dispString + "</td>";
                }
                dispString = dispString + "</tr>";
            }
            dispString = dispString + "</table>";
            TotalAmt = Convert.ToDecimal(GrandTotal_Total.ToString(DecHas));
            return dispString;
        }
        catch (Exception eX)
        {
            _GeneralDAL.CloseSQLConnection();
            throw new Exception(eX.Message);
        }


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
                        JsonString.Append("\"" + ds.Tables[0].Columns[j].ColumnName.ToString() + "\":" + "\"" + ds.Tables[0].Rows[i][j].ToString().Replace("\\", "\\\\").Replace("/", "\\/").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t").Replace("\x08", "\\f").Replace("\x0c", "\\b") + "\",");
                    }
                    else if (j == ds.Tables[0].Columns.Count - 1)
                    {
                        JsonString.Append("\"" + ds.Tables[0].Columns[j].ColumnName.ToString() + "\":" + "\"" + ds.Tables[0].Rows[i][j].ToString().Replace("\\", "\\\\").Replace("/", "\\/").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t").Replace("\x08", "\\f").Replace("\x0c", "\\b") + "\"");
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
}