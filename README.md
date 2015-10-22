# nef-c-sharp
Several people have been asking [Chris Nefcy](http://www.rad.washington.edu/radiology-personnel/cnefcy) for his  C# code that calls REDCap's API.  We decided that this new org would be a good place to distribute it.

These functions push/pull [DataTable](https://msdn.microsoft.com/en-us/library/system.data.datatable.aspx)s to/from [REDCap](http://project-redcap.org/).  It can be used in an .aspx program, or in a windows desktop program, a DET program, etc.  The code uses csv flat for all of the imports/exports from REDCap, which is much more compact than json or xml in returning data; we find CSV is a good fit capturing the rectangular structure returned by REDCap.

These aren't an encapsulated class; instead copy and paste this code into your existing program.  The code has comments and console output, in an effort describe the function and intent.  (Admittedly this is rough, but Chris doesn't currently have the time to make them into classes or something more elegant, and we thought this was the best short-term solution for distributing the functions.)

# Important Functions
Here is a subset of functions in the code.  The definitions of the frequent parameters are
```
strRecordsSelect: Any records you want separated by ','; all records if ""
strFields: Particular fields you want, separated by ','; all fields if ""
strEvents: Particular events you want, separated by ','; all events if ""
strForms: Particular forms you want, separated by ','; all forms if ""
boolLabels: false=raw; true=label
boolAccessGroups: false=no access group returned; true=access group returned (should be false, see note below)
```

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
Will return a csv string to import, given a data table if given any field string (like "studyid,redcap_event_name,field1,field2") will only put those fields in the csv string
```cs
public string GetCSVFromTable(DataTable dtData, string strFields)
```

### `RCImportCSVFlat`
Will import a csv string into REDCap. Remember that ONLY data exported in 'raw' mode can be imported (I think 'label' will not work)
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
