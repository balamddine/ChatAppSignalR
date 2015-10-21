using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Xml;

/// <summary>
/// Summary description for Convertor
/// </summary>
public class Convertor
{
	public Convertor()
	{
		
	}
    public static DataSet ConvertXMLToDataTable(string xmlString)
    {
        DataSet dataset = new DataSet();
        dataset.ReadXml(xmlString);
        return dataset.Tables.Count > 0 ? dataset : null;
    }
    public static void CreateXmlFile(string fileName, string rootElementName)
    {
        XmlTextWriter writer = new XmlTextWriter(fileName, System.Text.Encoding.UTF8);
        writer.WriteStartDocument();
        writer.Formatting = Formatting.Indented;
        writer.Indentation = 2;
        writer.WriteStartElement(rootElementName);
        writer.WriteEndElement();
        writer.WriteEndDocument();
        writer.Close();
    }
    public static string ConvertDataTableToXML(DataTable sourceTable)
    {
        string xmlString = "";
        using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
        {
            sourceTable.WriteXml(ms);
            System.Xml.XmlTextWriter x = new System.Xml.XmlTextWriter(ms, new System.Text.UTF8Encoding(false));
            x.Formatting = System.Xml.Formatting.Indented;
            //using (x)
            //{
            xmlString = System.Text.Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length);
            //}
        }
        return xmlString;
    }
}