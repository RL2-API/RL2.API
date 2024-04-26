:: This creates a patch file in the Patches directory for the file specified in the argument ::
@echo off
git diff -p --no-index RL2-Source/%1 Assembly-CSharp/%1 > Patches/%1.patch