import sys
import subprocess
import os
import shutil
import platform

patchtool = "patch.exe" if platform.system() == "Windows" else "patch"

dirname = os.path.dirname(__file__)

argv = sys.argv


def print_help():
	print("Toolchain utility for the RL2 Mod Loader")
	print(f"Usage: {argv[0]} <command> [args...]")
	print()
	print("Allowed commands:")
	print(" - help - Display this message")
	print(" - decompile - Decompile the game assembly")
	print(" - prepare-to-patch - Decompile the game assembly but stop after applying patches")
	print(" - cleanup - Remove all generated files")


def main():
	if len(argv) < 2:
		print_help()
		exit(1)
	match argv[1]:
		case "help":
			print_help()
		case "decompile":
			cmd_decompile()
		case "cleanup":
			cmd_cleanup()
		case "prepare-to-patch":
			cmd_decompile(True)
		case _:
			print_help()
			exit(1)


def ensure_dotnet():
	try:
		ver = subprocess.run(["dotnet", "--version"], capture_output=True)
		print("dotnet version:", ver.stdout.decode("utf-8").rstrip())
	except Exception as e:
		print("dotnet binary not available, please install it")
		print(str(e))
		exit(1)


def ensure_ilspycmd():
	try:
		ver = subprocess.run(["ilspycmd", "--version"], capture_output=True)
		print(
			"ilspy version",
			ver.stdout.decode("utf-8").rstrip().replace("\r", "").replace("\n", " | "),
		)
	except Exception as e:
		print(
			"ilspycmd binary not available, please install it using: dotnet tool install ilspycmd -g"
		)
		print(str(e))
		exit(1)


def ensure_patch():
	global patchtool
	try:
		ver = subprocess.run([patchtool, "--version"], capture_output=True)
		print("patch version", ver.stdout.decode("utf-8").split("\n")[0].rstrip())
	except Exception as e:
		print(f"{patchtool} binary not available, please install it (GNU patch)")
		print(
			f"If the error message below says something about permissions, rename your patch file to p4tch.exe"
		)
		print(str(e))
		try:
			patchtool = "p4tch.exe"
			attempt2 = subprocess.run([patchtool, "--version"], capture_output=True)
			print(
				"patch version", attempt2.stdout.decode("utf-8").split("\n")[0].rstrip()
			)
		except Exception as e2:
			print(f"2nd attempt with {patchtool} failed, aborting...")
			print(str(e2))
			exit(1)

def cmd_decompile(patch_only = False):
	ensure_dotnet()
	ensure_ilspycmd()
	ensure_patch()
	assembly_path = input("Assembly-CSharp.dll path: ")
	if not os.path.exists(assembly_path):
		print("Assembly-CSharp.dll not found, aborting...")
		exit(1)
	output_dir = os.path.join(dirname, "../RL2-Source")
	if os.path.exists(output_dir):
		shutil.rmtree(output_dir)
		shutil
	os.mkdir(output_dir)
	cmd = ["ilspycmd", "--nested-directories", "-p", "-o", output_dir, assembly_path]
	print("Decompiling the game...", cmd)
	try:
		ilspy = subprocess.run(cmd)
	except:
		print("Failed to decompile the game, aborting...")
		exit(1)
	print("The game has been decompiled, applying patches...")
	patch_dir = os.path.join(dirname, "../Patches")
	for file in sorted(os.listdir(patch_dir)):
		patch_path = os.path.join(dirname, "../Patches", file)
		if not os.path.isfile(patch_path):
			continue
		print(f"- {file}")
		try:
			cmd = [patchtool, "-i", patch_path, "-p", "1", "--binary"]
			# Why is this so dumb?
			if platform.system() == "Windows":
				og = patch_path.replace("\\", "/")
				cmd = [
					"wsl.exe",
					"patch",
					"-i",
					"/mnt/" + og[0].lower() + og[2:],
					"-p",
					"1",
					"--binary"
				]
			patch = subprocess.run(cmd)
		except Exception as e:
			print("Failed to apply patch, aborting...")
			print(str(e))
			exit(1)
	print("Patches have been applied, copying the source code")
	if patch_only:
		print("Stopping decompilation process as patch-only mode has been requested")
		exit(0)
	target_dir = os.path.join(dirname, "../Assembly-CSharp")
	for file in os.listdir(target_dir):
		if file == "ModLoader" or file == "Assembly-CSharp.csproj":
			continue
		path = os.path.join(target_dir, file)
		if os.path.isfile(path):
			os.remove(path)
		if os.path.isdir(path):
			shutil.rmtree(path)
	for file in os.listdir(output_dir):
		if file == "Assembly-CSharp.csproj":
			continue
		shutil.move(os.path.join(output_dir, file), target_dir)
	shutil.rmtree(output_dir)
	print("Source code is ready, copying libs...")
	lib_dir = os.path.dirname(os.path.abspath(assembly_path))
	target_lib_dir = os.path.join(target_dir, "lib")
	print(target_lib_dir)
	if not os.path.exists(target_lib_dir):
		os.mkdir(target_lib_dir)
	for file in os.listdir(lib_dir):
		if file == "Assembly-CSharp.dll":
			continue
		path = os.path.join(lib_dir, file)
		shutil.copyfile(path, os.path.join(target_lib_dir, file))
	print("The environment is ready")


def cmd_cleanup():
	output_dir = os.path.join(dirname, "../RL2-Source")
	target_dir = os.path.join(dirname, "../Assembly-CSharp")
	for file in os.listdir(target_dir):
		if file == "ModLoader" or file == "Assembly-CSharp.csproj":
			continue
		path = os.path.join(target_dir, file)
		if os.path.isfile(path):
			os.remove(path)
		if os.path.isdir(path):
			shutil.rmtree(path)
	if os.path.exists(output_dir):
		shutil.rmtree(output_dir)

if __name__ == "__main__":
	main()