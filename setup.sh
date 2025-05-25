rm -rf RL2.ModLoader.DevSetup

echo "Downloading dev setup..."
echo ""
curl -OL https://github.com/RL2-API/RL2.ModLoader.DevSetup/releases/download/v2.0.0/RL2.ModLoader.DevSetup.tar.gz
mkdir RL2.ModLoader.DevSetup
tar -xzf ./"RL2.ModLoader.DevSetup.tar.gz" -C ./RL2.ModLoader.DevSetup
rm ./"RL2.ModLoader.DevSetup.tar.gz"

echo ""
cd RL2.ModLoader.DevSetup
./RL2.ModLoader.DevSetup.exe