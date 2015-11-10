using System;
using System.Data;
//using System.Data.OleDb;
using System.Diagnostics;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Web.Security;
using System.Configuration;
// use if have mono on linux
//using System.Security.Cryptography.X509Certificates;


namespace DETSample
{
	/// <summary>
	/// Summary description for DBAccess.
	/// </summary>
	public class DBAccess
	{
        string strURI = ConfigurationManager.AppSettings["RCURI"];

		// tokens for rcnut
        public string strPostTokenXX = ConfigurationManager.AppSettings["RCTOKENXX"];
        public string strPostTokenYY = ConfigurationManager.AppSettings["RCTOKENYY"];

		public DBAccess()
		{
			//
			// TODO: Add constructor logic here
			//
		}



		#region RedCap

        // get any RC data. Need Token and strReturnCheck (to see if error in data returning). rest are optional
        // strRecordsSelect: any records you want seperated by ','; all records if ""
        // strFields: Particular fields you want, seperated by ','; all fields if ""
        // strEvents: Particular events you want, seperated by ','; all events if ""
        // strForms: Particular forms you want, seperated by ','; all forms if ""
        // boolLabels: false=raw; true=label
        // boolAccessGroups: false=no access group returned; true=access group returned (should be false, see note below)
        // (note: can't import access group column if in your data table; ontology fields at the moment can't be imported too)
        public DataTable GetTableFromAnyRC(string strPostToken, string strReturnCheck, string strRecordsSelect, string strFields,
             string strEvents, string strForms, bool boolLabels, bool boolAccessGroups)
        {
            Debug.WriteLine("GetTableFromAnyRC()");

            string strRecord;
            CSVDoc csvDoc;
            string strPostParameters = "";
            int intReturnLength = strReturnCheck.Trim().Length;

            DataTable dtDataTable = new DataTable();
            DataRow drRecord;

            strPostParameters = "&content=record&format=csv&type=flat&eventName=unique";

            if (strReturnCheck == "")
            {
                throw new Exception("Error: " + "Must provide first field to check");
            }

            if (strRecordsSelect != "")
            {
                strPostParameters += "&records=" + strRecordsSelect;
            }

            if (strFields != "")
            {
                strPostParameters += "&fields=" + strFields;
            }

            if (strEvents != "")
            {
                strPostParameters += "&events=" + strEvents;
            }

            if (strForms != "")
            {
                strPostParameters += "&forms=" + strForms;
            }

            if (boolLabels)
                strPostParameters += "&rawOrLabel=label";
            else
                strPostParameters += "&rawOrLabel=raw";

            // probably should take out if you are going to import this exported data
            if (boolAccessGroups)
            {
                strPostParameters += "&exportDataAccessGroups=true";
            }

            byte[] bytePostData = Encoding.UTF8.GetBytes("token=" + strPostToken + strPostParameters);

            string strResponse = responseHTTP(bytePostData);

            // if no records found, there are no fields.  new in RC 6 and greater
            // have to deal with null DataTable or no rows in DataTable in your call to this function.
            // if Rc < 6, then it will return field names without any rows of data
            if (strResponse == "\n")
            {
                return (dtDataTable);
            }

            // should return the first field you expect. otherwise it is error
            if (strResponse.Substring(0, intReturnLength) != strReturnCheck)
            {
                throw new Exception("RC Error: " + strResponse);
            }

            csvDoc = new CSVDoc(strResponse);

            // first line of .csv is column names
            strRecord = csvDoc.ReadLine();

            // get column headers
            string[] strColumnHeaders = strRecord.Split(',');

            // set up table
            for (int i = 0; i < strColumnHeaders.Length; i++)
            {
                dtDataTable.Columns.Add(strColumnHeaders[i].ToString(), typeof(string));
            }

            // now read all data and assign to data table
            while ((strRecord = csvDoc.ReadLine()) != null)
            {
                CSVLine csvLine = new CSVLine(strRecord);

                drRecord = dtDataTable.NewRow();

                // now get fields
                for (int i = 0; i < strColumnHeaders.Length; i++)
                {
                    drRecord[i] = csvLine.ReadField();
                }

                dtDataTable.Rows.Add(drRecord);
            }

            return (dtDataTable);
        }

        // will return a csv string to import, given a data table
        // if given any field string (like "studyid,redcap_event_name,field1,field2") will only put those fields in the csv string
        public string GetCSVFromTable(DataTable dtData, string strFields)
        {
            Debug.WriteLine("GetCSVFromTable()");

            string strCSVContents = "";

            int i = 0;
            int j = 0;

            try
            {
                string[] strFieldsArray = strFields.Split(',');

                //Write into csv format
                for (i = 0; i < dtData.Columns.Count; i++)
                {
                    if (strFields == "")
                    {
                        if (i > 0)
                            strCSVContents += ",";

                        strCSVContents += dtData.Columns[i].ColumnName;
                    }
                    else
                    {
                        if (InArray(strFieldsArray, dtData.Columns[i].ColumnName))
                        {
                            if (i > 0)
                                strCSVContents += ",";

                            strCSVContents += dtData.Columns[i].ColumnName;
                        }
                    }
                }

                strCSVContents += "\n";

                for (i = 0; i < dtData.Rows.Count; i++)
                {
                    for (j = 0; j < dtData.Columns.Count; j++)
                    {
                        if (strFields == "")
                        {
                            if (j > 0)
                                strCSVContents += ",";

                            // double quote all fields. replace any double quotes in field with escape clause.
                            // this allows things like inches (") to be put in fields, or any quote marks
                            strCSVContents += "\"" + dtData.Rows[i][j].ToString().Replace("\"", "\\\"") + "\"";
                        }
                        else
                        {
                            if (InArray(strFieldsArray, dtData.Columns[j].ColumnName))
                            {
                                if (j > 0)
                                    strCSVContents += ",";

                                // double quote all fields. replace any double quotes in field with escape clause.
                                // this allows things like inches (") to be put in fields, or any quote marks
                                strCSVContents += "\"" + dtData.Rows[i][j].ToString().Replace("\"", "\\\"") + "\"";
                            }
                        }
                    }

                    strCSVContents += "\n";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("RC Error: " + ex.Message.ToString(), ex);
            }

            return (strCSVContents);
        }

        // if value found in array, will return true
        private bool InArray(string[] strArray, string strText)
        {
            foreach (string strItem in strArray)
            {
                if (strItem == strText)
                {
                    return true;
                }
            }
            return false;
        }

        // will import a csv string into RC. 
        // remember that ONLY data exported in 'raw' mode can be imported (I think 'label' will not work)
        public string RCImportCSVFlat(string strPostToken, string strCSVContents, bool boolOverwrite)
        {
            string strPostParameters = "&content=record&format=csv&type=flat&overwriteBehavior=overwrite&data=" + strCSVContents;
            if (boolOverwrite)
                strPostParameters += "&overwriteBehavior=overwrite";
            strPostParameters += "&data=" + strCSVContents;

            byte[] bytePostData = Encoding.ASCII.GetBytes("token=" + strPostToken + strPostParameters);

            string strResponse = responseHTTP(bytePostData);

            // error if more than 9999 records (this num could change, but is just what I tried), or most likely, just has an error message.
            if (strResponse.Length > 4)
            {
                throw new Exception("RC Error: " + strResponse);
            }

            return (strResponse);
        }

        // makes the API call and returns response from request
        private string responseHTTP(byte[] bytePostData)
        {
            Debug.WriteLine("responseHTTP()");
            string strResponse = "";

            try
            {
                // added for mono on unix server (mine is limited to .net 2.0). should not need if don't have this environment
                // ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate,
                //                         X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

                HttpWebRequest webreqRedCap = (HttpWebRequest)WebRequest.Create(strURI);

                webreqRedCap.Method = "POST";
                webreqRedCap.ContentType = "application/x-www-form-urlencoded";
                webreqRedCap.ContentLength = bytePostData.Length;

                // Get the request stream and read it
                Stream streamData = webreqRedCap.GetRequestStream();
                streamData.Write(bytePostData, 0, bytePostData.Length);
                streamData.Close();

                HttpWebResponse webrespRedCap = (HttpWebResponse)webreqRedCap.GetResponse();

                //Now, read the response (the string), and output it.
                Stream streamResponse = webrespRedCap.GetResponseStream();
                StreamReader readerResponse = new StreamReader(streamResponse);

                strResponse = readerResponse.ReadToEnd();
            }
            catch (WebException exWE)
            {
                Stream streamWE = exWE.Response.GetResponseStream();
                StringBuilder sbResponse = new StringBuilder("", 65536);

                try
                {
                    byte[] readBuffer = new byte[1000];
                    int intCnt = 0;

                    for (; ; )
                    {
                        intCnt = streamWE.Read(readBuffer, 0, readBuffer.Length);

                        if (intCnt == 0)
                        {
                            // EOF
                            break;
                        }

                        sbResponse.Append(System.Text.Encoding.UTF8.GetString(readBuffer, 0, intCnt));
                    }

                }
                finally
                {
                    streamWE.Close();

                    strResponse = sbResponse.ToString();
                }
            }
            catch (Exception ex)
            {
                strResponse = ex.Message.ToString();
                //throw new Exception ( "RC Error. " +  ex.Message.ToString(), ex);
            }

            return (strResponse);
        }

		#endregion

	}
}
