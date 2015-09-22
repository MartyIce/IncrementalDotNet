# IncrementalDotNet
Makes efficient use of .Net dependencies and recent changes to generate a series of build files.

This is a work in progress, but the intent is:

1) Compile a list of .Net project dependencies by analyzing file references in the csproj files
2) Determine what assemblies need to be rebuilt (comparing source file timestamps against output binary timestamps)
3) Compile sets of msbuild files that respect project dependencies, building as much as possible in each stage.
