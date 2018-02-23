mkdir ../temp
rm ../temp/tests.dll
mcs /target:library /sdk:2 ../tests/*.cs ../src/*.cs  -r:nunit.framework.dll -out:../temp/tests.dll
mono nunit-console.exe ../temp/tests.dll --labels=all
