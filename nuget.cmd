echo off

SET nugetDir=%3
echo ## Create Nuget package for %1 (%2) in %nugetDir%

::echo ## Create packages
dotnet pack %1 --no-build --configuration %2 --output %nugetDir% --include-symbols --verbosity minimal

::echo ## Check if Nuget Symbols directory exists, otherwise create one
set nugetSymbolsDir=%nugetDir%\symbols

::echo ## Move Nuget Symbols, to Symbols directory
IF NOT EXIST %nugetSymbolsDir% mkdir %nugetSymbolsDir%
move %nugetDir%\*.symbols.nupkg %nugetSymbolsDir%

::echo ## Complete