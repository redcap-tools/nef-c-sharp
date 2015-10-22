# nef-c-sharp
Several people have been asking [Chris Nefcy](http://www.rad.washington.edu/radiology-personnel/cnefcy) for his  C# code that calls REDCap's API.  We decided that this new org would be a good place to distribute it.

These functions push/pull [DataTable](https://msdn.microsoft.com/en-us/library/system.data.datatable.aspx)s to/from [REDCap](http://project-redcap.org/).  It can be used in an .aspx program, or in a windows desktop program, a DET program, etc.  The code uses csv flat for all of the imports/exports from RC, which is much more compact than json or xml in returning data; we find CSV is a good fit capturing the rectangular structure returned by REDCap.

These aren't an encapsulated class; instead copy and paste this code into your existing program.  The code has comments and console output, in an effort describe the function and intent.  (Admittedly this is rough, but Chris doesn't currently have the time to make them into classes or something more elegant, and we thought this was the best short-term solution for distributing the functions.)
