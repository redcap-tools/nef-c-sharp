using System;
//using System.Collections.Generic;
using System.Text;

namespace WHATEVER-NAMESPACE YOU ARE USING IN SAMPLECODE.CS
{
    #region CSVReadLines
    class CSVDoc
    {
        #region Data Members

        int _intPos = 0;
        string _strCSVData = null;

        #endregion Data Members

        #region Public Methods

        public CSVDoc(string strCSVData)
        {
            _strCSVData = strCSVData;
            _intPos = 0;
        }

        public string ReadLine()
        {
            string strValue = null;
            int intStart = _intPos;

            while (_intPos < _strCSVData.Length)
            {
                // Special handling for quoted field
                if (_strCSVData[_intPos] == '"')
                {
                    _intPos++;

                    while (_intPos < _strCSVData.Length)
                    {
                        // Test for quote character inside quotes
                        if (_strCSVData[_intPos] == '"')
                        {
                            // Found one
                            _intPos++;

                            break;

                            // If two quotes together, keep going
                            // Otherwise, indicates end of value, should look for \n
                            /*
                            if (_intPos >= _strCSVData.Length || _strCSVData[_intPos] != '"')
                            {
                                _intPos++;
                                break;
                            }
                             * */
                        }
                        _intPos++;
                    }
                }
                else
                {
                    // outside of quote field, so look for \n
                    if (_intPos < _strCSVData.Length && _strCSVData[_intPos] != '\n')
                    {
                        _intPos++;
                    }
                    else
                        break;
                }
            }

            if (_intPos > intStart)
            {
                strValue = _strCSVData.Substring(intStart, _intPos - intStart);
                _intPos++;
            }

            return (strValue);
        }

        #endregion Public Methods

    }
    #endregion CSVReadLines


    #region CSVReadFields
    class CSVLine
    {
        #region Data Members

        int _intPos = 0;
        string _strCSVLine = null;

        #endregion Data Members

        #region Public Methods

        public CSVLine(string strCSVLine)
        {
            _strCSVLine = strCSVLine;
            _intPos = 0;
        }

        public string ReadField()
        {
            string strValue = null;
            int intStart = _intPos;

            while (_intPos < _strCSVLine.Length)
            {
                // Special handling for quoted field
                if (_strCSVLine[_intPos] == '"')
                {
                    // Skip initial quote
                    _intPos++;

                    // Parse quoted value
                    intStart = _intPos;
                    while (_intPos < _strCSVLine.Length)
                    {
                        // Test for quote character inside quotes
                        if (_strCSVLine[_intPos] == '"')
                        {
                            // Found one
                            _intPos++;

                            // If two quotes together, keep one
                            // Otherwise, indicates end of value
                            if (_intPos >= _strCSVLine.Length || _strCSVLine[_intPos] != '"')
                            {
                                _intPos--;
                                break;
                            }
                        }
                        _intPos++;
                    }
                    strValue = _strCSVLine.Substring(intStart, _intPos - intStart);
                    break;
                }
                else
                {
                    // Parse unquoted value
                    intStart = _intPos;
                    while (_intPos < _strCSVLine.Length && _strCSVLine[_intPos] != ',')
                        _intPos++;
                    strValue = _strCSVLine.Substring(intStart, _intPos - intStart);
                    break;
                }
            }

            // Eat up to and including next comma
            while (_intPos < _strCSVLine.Length && _strCSVLine[_intPos] != ',')
                _intPos++;
            if (_intPos < _strCSVLine.Length)
                _intPos++;

            return (strValue);
        }

        #endregion Public Methods

    }
    #endregion CSVReadFields
}
