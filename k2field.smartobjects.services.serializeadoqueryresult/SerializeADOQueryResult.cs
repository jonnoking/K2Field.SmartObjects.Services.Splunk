using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using SourceCode.SmartObjects.Services.ServiceSDK.Objects;
using Attributes = SourceCode.SmartObjects.Services.ServiceSDK.Attributes;
using SourceCode.SmartObjects.Services.ServiceSDK.Types;

using SourceCode.Data.SmartObjectsClient;
using SourceCode.Hosting.Client.BaseAPI;
using SourceCode.SmartObjects.Client;

namespace K2Field.SmartObjects.Services.SerializeADOQueryResult
{
    [Attributes.ServiceObject("SerializeADOQueryResult", "Serialize ADO Query Result", "Serialize ADO Query Result")]
    public class SerializeADOQueryResult
    {
        private string adoquery = "";
        [Attributes.Property("ADOQuery", SoType.Memo, "ADO Query", "ADO Query")]
        public string ADOQuery
        {
            get { return adoquery; }
            set { adoquery = value; }
        }

        private string jsonresult = "";
        [Attributes.Property("JSONResult", SoType.Memo, "JSON Result", "JSON Result")]
        public string JSONResult
        {
            get { return jsonresult; }
            set { jsonresult = value; }
        }

        private string xmlresult = "";
        [Attributes.Property("XMLResult", SoType.Memo, "XML Result", "XML Result")]
        public string XMLResult
        {
            get { return xmlresult; }
            set { xmlresult = value; }
        }


        [Attributes.Method("ADOQuerytoJSON", SourceCode.SmartObjects.Services.ServiceSDK.Types.MethodType.Read, "ADO Query to JSON", "ADO Query to JSON",
        new string[] { "ADOQuery" }, //required property array (no required properties for this sample)
        new string[] { "ADOQuery" }, //input property array (no optional input properties for this sample)
        new string[] { "ADOQuery", "JSONResult" })] //return property array (2 properties for this example)
        public SerializeADOQueryResult ADOQuerytoJSONExecute()
        {
            DataTable results = QuerySmartObject();

            StringBuilder sb = new StringBuilder();

            int rowCount = 0;

            if (results.Rows.Count > 1)
            {
                sb.Append("[");
                sb.Append("\n");
            }

            foreach (DataRow r in results.Rows)
            {
                rowCount++;

                sb.Append("{");
                sb.Append("\n");
                int colCount = 0;
                foreach (DataColumn c in r.Table.Columns)
                {
                    colCount++;
                    sb.Append("\"");
                    sb.Append(c.ColumnName);
                    sb.Append("\"");
                    sb.Append(": ");
                    sb.Append("\"");

                    // all values treated as strings
                    string val = string.Empty;
                    if (!r.IsNull(c))
                    {
                        val = r[c].ToString();
                        val = val.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");

                    }
                    sb.Append(val);
                    sb.Append("\"");
                    if (colCount < r.Table.Columns.Count)
                    {
                        sb.Append(",");
                        sb.Append("\n");
                    }
                }
                sb.Append("\n");
                sb.Append("}");

                if (rowCount < results.Rows.Count)
                {
                    sb.Append(",");
                    sb.Append("\n");
                }

            }
            if (results.Rows.Count > 1)
            {
                sb.Append("\n");
                sb.Append("]");
            }

            this.JSONResult = sb.ToString();


            return this;
        }

        [Attributes.Method("ADOQuerytoXML", SourceCode.SmartObjects.Services.ServiceSDK.Types.MethodType.Read, "ADO Query to XML", "ADO Query to XML",
        new string[] { "ADOQuery" }, //required property array (no required properties for this sample)
        new string[] { "ADOQuery" }, //input property array (no optional input properties for this sample)
        new string[] { "ADOQuery", "XMLResult" })] //return property array (2 properties for this example)
        public SerializeADOQueryResult ADOQuerytoXMLExecute()
        {

            DataTable results = QuerySmartObject();

            StringBuilder sb = new StringBuilder();

            int rowCount = 0;
            sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.Append("\n");
            if (results.Rows.Count > 1)
            {
                sb.Append("<results>");
                sb.Append("\n");
            }

            foreach (DataRow r in results.Rows)
            {
                rowCount++;

                sb.Append("<row>");
                sb.Append("\n");
                int colCount = 0;
                foreach (DataColumn c in r.Table.Columns)
                {
                    colCount++;
                    sb.Append("<");
                    sb.Append(c.ColumnName);
                    sb.Append(">");
                    // not parsing special characters or adding cdata sections
                    // stripping line breaks
                    string val = string.Empty;
                    if (!r.IsNull(c))
                    {
                        val = r[c].ToString();
                        val = val.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");

                    }
                    sb.Append(val);
                    sb.Append("</");
                    sb.Append(c.ColumnName);
                    sb.Append(">");
                    if (colCount < r.Table.Columns.Count)
                    {
                        sb.Append("\n");
                    }
                }
                sb.Append("\n");
                sb.Append("</row>");

                if (rowCount < results.Rows.Count)
                {
                    sb.Append("\n");
                }

            }
            if (results.Rows.Count > 1)
            {
                sb.Append("\n");
                sb.Append("</results>");
            }

            this.XMLResult = sb.ToString();


            return this;
        }


        private DataTable QuerySmartObject()
        {
            DataTable results = new DataTable();

            string sql_query = adoquery;

            //SourceCode.SmartObjects.Client.SmartObjectClientServer server = new SmartObjectClientServer();
            SCConnectionStringBuilder SmOBuilder = new SCConnectionStringBuilder();
            SmOBuilder.Host = "localhost";
            SmOBuilder.Port = 5555;
            SmOBuilder.Integrated = true;
            SmOBuilder.IsPrimaryLogin = true;

            using (SOConnection connection = new SOConnection(SmOBuilder.ToString()))
            using (SOCommand command = new SOCommand(sql_query, connection))
            using (SODataAdapter adapter = new SODataAdapter(command))
            {
                connection.DirectExecution = true;
                adapter.Fill(results);
            }

            return results;
        }

    }
}
