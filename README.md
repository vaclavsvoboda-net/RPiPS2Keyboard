# RPiPS2Keyboard
Raspberry Pi 4 - PS2 Keyboard emulator

This project was originally created to emulate keyboard for REVEX devices. User easily scan code with ZEBRA device and Raspberry Pi push it to PS2 port.

You can create application to send UDP broadcast message like "RPiPS2Kbd|1|Hello world!" and Raspberry Pi receive it and push it to PS2 port.

## Setup Reaspbrry Pi - SW   (with monitor)
1. Write "Raspberry Pi OS Lite (64-bit)" to SD card using Raspberry Pi Imager.
2. Run Raspberry Pi and install dotNET:

   `$curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel Current`
   
   `$echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.bashrc`
   
   `$echo 'export PATH=$PATH:$HOME/.dotnet' >> ~/.bashrc`
   
   `$source ~/.bashrc`
   
3. Create directory:

   `$mkdir ps2keyboard`
   
4. Edit `crontab` to run application after boot:

   `$crontab -e`
   
   add this line to the end `@reboot /home/pi/.dotnet/dotnet /home/pi/ps2keyboard.dll >> /home/pi/ps2keyboard/my.log 2>&1`
   
5. Connect Raspberry Pi to Wifi:

   Use this tool `$sudo raspi-config`
   
6. Check Ryspberry Pi IP address:

   `$ifconfig`
   
   You can also check broadcast IP address.
   
7. Copy RPiPS2Keyboard project published files to Raspberry Pi:

   On your windows machine run command prompt and use command:
   
   `scp -r c:\publish\* pi@yourRaspberryIpAddress:/home/pi/ps2keyboard`
   
   Password should be "pi".
   
8. Resart you Raspberry Pi.

## Setup Raspberry Pi - HW
