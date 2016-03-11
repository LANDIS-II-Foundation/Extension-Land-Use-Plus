using System.Reflection;

[assembly: AssemblyTitle("Land Use Extension")]
[assembly: AssemblyVersion("1.1.*")]
[assembly: AssemblyDescription("Land Use Extension for LANDIS-II")]
[assembly: AssemblyCopyright("2014 University of Notre Dame")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif