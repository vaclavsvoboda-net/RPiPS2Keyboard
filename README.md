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
   
4. Edit "crontab" to run application after boot:

   `$crontab -e`
   
   add this line to the end `@reboot /home/pi/.dotnet/dotnet /home/pi/ps2keyboard/RPiPS2Keyboard.dll >> /home/pi/ps2keyboard/my.log 2>&1`
   
5. Connect Raspberry Pi to Wifi:

   Use this tool `$sudo raspi-config`
   
6. Check Ryspberry Pi IP address:

   `$ifconfig`
   
   You can also check broadcast IP address.
   
7. Copy RPiPS2Keyboard project published files to Raspberry Pi:

   On your windows machine run command prompt and use command:
   
   `scp -r c:\yourPublishDirectory\* pi@yourRaspberryIpAddress:/home/pi/ps2keyboard`
   
   Password should be "pi".
   
8. Resart you Raspberry Pi.

## Setup Reaspbrry Pi - SW   (without monitor, with SSH, PowerShell, Nmap)
1. Write "Raspberry Pi OS Lite (64-bit)" to SD card using Raspberry Pi Imager.
2. Plug SD card to your desktop machine.
3. Find drive named "boot" and create two files there:

   File named "ssh". Empty file without extension.
   
   File named "wpa_supplicant.conf" with content:
   ```
   country=US
   ctrl_interface=DIR=/var/run/wpa_supplicant GROUP=netdev
   update_config=1

   network={
    ssid="your SSID name"
    psk="your password"
    key_mgmt=WPA-PSK
   }
   ```
4. Plug SD card to your Raspberry Pi and run it.
5. Use "Nmap" or something like this to check Raspberry Pi IP address. Ping scan is good enough.
   
   `nmap -sn 192.168.0.0/24` in my case.
   
7. If you have Raspberry Pi IP address, run PowerShell and use command:
   
   `ssh pi@yourRaspberryIpAddress`
   
   Password should be "pi".
   
7. If you are connected to Raspberry Pi with SSH install dotNET:

   `$curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel Current`
   
   `$echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.bashrc`
   
   `$echo 'export PATH=$PATH:$HOME/.dotnet' >> ~/.bashrc`
   
   `$source ~/.bashrc`
   
8. Create directory:

   `$mkdir ps2keyboard`
   
9. Edit "crontab" to run application after boot:

   `$crontab -e`
   
   add this line to the end `@reboot /home/pi/.dotnet/dotnet /home/pi/ps2keyboard/RPiPS2Keyboard.dll >> /home/pi/ps2keyboard/my.log 2>&1`
   
10. Copy RPiPS2Keyboard project published files to Raspberry Pi:

    On your windows machine run command prompt and use command:
    
    `scp -r c:\yourPublishDirectory\* pi@yourRaspberryIpAddress:/home/pi/ps2keyboard`
    
    Password should be "pi".
    
11. Resart you Raspberry Pi.


## Setup Raspberry Pi - HW
