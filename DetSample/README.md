# DET Sample

I created a sample project for DETs in C#.  It was developed in Visual Studio 2013 using .NET 2.0, because we use Linux servers with Mono, and the latest Mono only works with .NET 2.0.  But the code here should work with more recent .NET versions too.

This demo shows the calls to set up a DET.  I put in a sample function to move data from one REDCap table to another one that has the same fields.  Kind of a ‘backup’ DET function.

The entire .NET project is included to demonstrate everything's location, like Tokens in Web.config.  I've made a class for the functions that moves data in/out of REDCap.  It is a simple project, but I use the basis/structure of this to do a lot of things with the DET.  I do all of the API calls with a flat csv format.

(*This description was paraphrased from [Chris Nefcy](http://www.rad.washington.edu/radiology-personnel/cnefcy).*)
