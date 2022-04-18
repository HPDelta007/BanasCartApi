using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for ClassA
/// </summary>
public class ClassA
{
	public ClassA()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public string status { get; set; }
    public string message { get; set; }
    public ClassB result { get; set; }

    public static ClassA FromDataRow(DataRow row)
    {
        DataTable Dt = new DataTable();
        ClassB result1 = new ClassB();
        if (row["Result"] != DBNull.Value)
        {
            result1.Code = (string)row["Result"];
        }

        var classA = new ClassA
        {
            status = (string)row["status"],
            message = (string)row["message"],
            result = result1
        };

        return classA;
    }
}