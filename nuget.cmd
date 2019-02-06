echo off

SET nugetDir="P:\Nuget\"
echo ## Create Nuget package for %1 (%2) in %nugetDir%

::echo ## Check if Nuget Symbols directory exists, otherwise create one
set nugetSymbolsDir=%nugetDir%\symbols
IF NOT EXIST %nugetSymbolsDir% mkdir %nugetSymbolsDir%

::echo ## Create packages
dotnet pack %1 --no-build --configuration %2 --output %nugetDir% --include-symbols --verbosity minimal

::echo ## Move Nuget Symbols, to Symbols directory
move %nugetDir%\*.symbols.nupkg %nugetSymbolsDir%

::echo ## Complete