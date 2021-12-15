rem
rem This file should be run on Windows, in the SXT directory
rem
rem Double-clicking on the file while in Windows Explorer should
rem be sufficient

@echo off
echo.
echo This batch file will copy the missing textures from the inaccessable
echo Squad/zDeprecated directory 
echo.
echo.
pause

mkdir Squad
cd Squad
mkdir dockingPort_v1
copy ..\..\Squad\zDeprecated\Parts\Utility\dockingPort_v1\model000.dds dockingPort_v1

mkdir dockingPortJr_v1
copy ..\..\Squad\zDeprecated\Parts\Utility\dockingPortJr_v1\model000.dds dockingPortJr_v1

mkdir dockingPortSr_v1
copy ..\..\Squad\zDeprecated\Parts\Utility\dockingPortSr_v1\model000.dds dockingPortSr_v1



echo.
echo.
echo The files have been copied
echo.
pause

