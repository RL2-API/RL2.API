curl -OL https://github.com/RL2-API/RL2.ModLoader.DevSetup/releases/download/1.0.0/RL2.ModLoader.DevSetup-v1.0.0.tar.gz
mkdir RL2.ModLoader.DevSetup
tar -xzvf ./"RL2.ModLoader.DevSetup-v1.0.0.tar.gz" -C ./RL2.ModLoader.DevSetup
rm ./"RL2.ModLoader.DevSetup-v1.0.0.tar.gz"

curl -OL https://github.com/MonoMod/MonoMod/releases/download/v22.07.31.01/MonoMod-22.07.31.01-net452.zip
mkdir lib
unzip MonoMod-22.07.31.01-net452.zip -d lib
rm MonoMod-22.07.31.01-net452.zip
