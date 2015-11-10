# API Example
Here is a subset of functions in the code.  

### `GetTableFromAnyRC`
Get any REDCap data. Need Token and strReturnCheck (to see if error in data returning). Rest are optional  (note: can't import access group column if in your data table; ontology fields at the moment can't be imported too).
```cs
public DataTable GetTableFromAnyRC(
  string strPostToken,
  string strReturnCheck,
  string strRecordsSelect,
  string strFields,
  string strEvents,
  string strForms,
  bool boolLabels,
  bool boolAccessGroups
)
```

### `GetCSVFromTable`
Will return a csv string to import, given a data table if given any field string (like `"studyid,redcap_event_name,field1,field2"`) will only put those fields in the csv string.
```cs
public string GetCSVFromTable(DataTable dtData, string strFields)
```

### `RCImportCSVFlat`
Will import a csv string into REDCap. Remember that ONLY data exported in 'raw' mode can be imported (I think 'label' will not work).
```cs
public string RCImportCSVFlat(string strPostToken, string strCSVContents, bool boolOverwrite)
```

### `GetData`
Sample GetData code. Demonstrates how to use above functions.
```cs
public void GetData()
```

### `SetData`
Sample SetData code.  Demonstrates how to use the above functions.
```cs
public void SetData ( DataTable dtData)
```

### Frequent Parameters
The definitions of the frequent parameters are:
```
strRecordsSelect: Any records you want separated by ','; all records if ""
strFields:        Particular fields you want, separated by ','; all fields if ""
strEvents:        Particular events you want, separated by ','; all events if ""
strForms:         Particular forms you want, separated by ','; all forms if ""
boolLabels:       false=raw; true=label
boolAccessGroups: false=no access group returned; true=access group returned (should be false, see note for `GetTableFromAnyRC()`)
```
