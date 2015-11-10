using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Configuration;

namespace DETSample
{
    public partial class _Default : System.Web.UI.Page
    {
        string project_id = "";
        string record = "";
        string redcap_event_name = "";
        string redcap_data_access_group = "";
        string instrument = "";
        string complete_flag_key = "";
        string complete_flag_value = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            // for debugging or to catch errors, i write strMessage to some database. you can do whatever you want.
            string strMessage = "";

            // this is used to store the params passed by the RC DET call
            ArrayList arrayKeys;

            try
            {
                arrayKeys = GetHTTPRequestParams();

                // get params passed to DET
                project_id = GetHTTPRequestValue(arrayKeys, "project_id");
                record = GetHTTPRequestValue(arrayKeys, "record");
                redcap_event_name = GetHTTPRequestValue(arrayKeys, "redcap_event_name");
                redcap_data_access_group = GetHTTPRequestValue(arrayKeys, "redcap_data_access_group");
                instrument = GetHTTPRequestValue(arrayKeys, "instrument");
                // get complete flag
                complete_flag_key = instrument + "_complete";
                complete_flag_value = GetHTTPRequestValue(arrayKeys, instrument + "_complete");

                // depending on the project_id, do whatever you want
                // you will have to fill in appropriate values for CAPITAL entries below
                if (project_id == "XX")
                {
                    if (redcap_event_name == "EVENT1")
                    {
                        if (instrument == "INSTRUMENT1")
                        {
                            // do some function
                        }

                        if (instrument == "INSTRUMENT2")
                        {
                            // do some function
                        }
                    }
                }

                if (project_id == "YY")
                {
                    // just sample code to read all data and move it to a different, but simliar, RC table
                    // moves YY data to XX

                    // move only if the form were marked as Completed
                    if (complete_flag_value == "2")
                        MoveAllData(record);
                }

            }
            catch (Exception ex)
            {
                // do what you want with error message
                strMessage = ex.Message.ToString();
            }

            // this will close the window when DET is done
            ClientScript.RegisterClientScriptBlock(this.GetType(), "CloseWindow", "<script>window.open('','_self','');window.close();</script>");

        }

        #region HTTPRequest Functions

        public string GetHTTPRequestValue(ArrayList alDETKeys, string strKey)
        {
            string strValue = "";

            foreach (DETKeys DETKey in alDETKeys)
            {
                if (DETKey.strKey == strKey)
                {
                    strValue = DETKey.strValue;
                }
            }

            return (strValue);
        }

        public ArrayList GetHTTPRequestParams()
        {
            ArrayList arrayResults = new ArrayList();

            DETKeys detKeys;

            NameValueCollection nvcKeys = Request.Form;

            foreach (string strKey in nvcKeys.AllKeys)
            {
                detKeys = new DETKeys();

                detKeys.strKey = strKey;
                detKeys.strValue = nvcKeys.Get(strKey);

                arrayResults.Add(detKeys);
            }

            return (arrayResults);
        }

        #endregion

        #region Sample Function (moves YY to XX)

        public void MoveAllData(string strRecordID)
        {
            DBAccess dbAccess = new DBAccess();

            DataTable dtYY;

            string strCSVContents = "";
            
            try
            {
                // get all data from YY
                // assumes first field is record_id and that you want all data from all events (if any)
                dtYY = dbAccess.GetTableFromAnyRC(dbAccess.strPostTokenYY, "record_id", "", "", "", "", false, false);

                // makes a csv string from all columns in YY data table
                strCSVContents = dbAccess.GetCSVFromTable(dtYY, "");

                // imports this YY csv string to RC XX using API, with overwrite
                dbAccess.RCImportCSVFlat(dbAccess.strPostTokenXX, strCSVContents, true);
            }
            catch (Exception ex)
            {
                // whatever you want to do with exception
                throw new Exception(ex.Message.ToString(), ex);
            }
            finally
            {
                // any actions needed here
            }
        }

        #endregion
    }

    public class DETKeys
    {
        public string strKey = "";
        public string strValue = "";
    }
}