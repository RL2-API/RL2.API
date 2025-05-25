rm -rf package

mkdir package
cd bin/Release
tar -a -cf ../../package/RL2.API.zip RL2.API
echo Packaged RL2.API
tar -a -cf ../../package/ExampleMod.zip ExampleMod
echo Packaged ExampleMod
cd ../..
