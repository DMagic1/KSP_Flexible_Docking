#!/bin/bash

#
# This script needs to be run on Linux and OSX to copy
# the missing texture files.  Double-clicking on it
# should be sufficient

clear

echo -e "/n/nThis batch file will copy the missing textures from the inaccessable"
echo "Squad/zDeprecated directory "
echo -e "/n/n"
echo "Press return to continue"
read yn

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null && pwd )"
[ ! -d $DIR/Squad ] && mkdir $DIR/Squad
cd $DIR/Squad


mkdir Squad
cd Squad
mkdir dockingPort_v1
copy ../../Squad/zDeprecated/Parts/Utility/dockingPort_v1/model000.dds dockingPort_v1

mkdir dockingPortJr_v1
copy ../../Squad/zDeprecated/Parts/Utility/dockingPortJr_v1/model000.dds dockingPortJr_v1

mkdir dockingPortSr_v1
copy ../../Squad/zDeprecated/Parts/Utility/dockingPortSr_v1/model000.dds dockingPortSr_v1

echo -e "/n/nThe files have been copied/n"
echo "Press return to continue"
read yn


