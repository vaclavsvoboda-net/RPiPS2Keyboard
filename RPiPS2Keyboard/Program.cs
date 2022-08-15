using System;
using System.Net;
using System.Device.Gpio;
using RPiPS2Comunication;
using System.Diagnostics;

namespace RPiPS2Keyboard
{
    internal class Program
    {
        private static RPiPS2Com RPiPS2Com = new(24, 23);
        private static GpioController Controller = new();
        private static System.Net.Sockets.UdpClient UdpClient = new(new IPEndPoint(IPAddress.Any, 54321));

        private static byte KeyboardId = 0;

        private static byte PinAppRunLED = 25;
        private static byte PinNetworkLED = 7;
        private static byte PinTestButton = 21;
        private static byte PinKeyboard1 = 5;
        private static byte PinKeyboard2 = 12;
        private static byte PinKeyboard3 = 13;
        private static byte PinKeyboard4 = 16;

        private static bool AppRunLED = false;

        static void Main(string[] args)
        {
            Console.WriteLine($"App started at: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");

            if (!RPiPS2Com.Initialize())
            {
                Console.Write(RPiPS2Com.LastExceptionMessage);
                return;
            }

            // App running LED
            if (!Controller.IsPinModeSupported(PinAppRunLED, PinMode.Output))
            {
                Console.WriteLine($"Gpio pin {PinAppRunLED} can not be set.");
                return;
            }
            else
            {
                Controller.OpenPin(PinAppRunLED, PinMode.Output);
                Controller.Write(PinAppRunLED, PinValue.Low);
            }

            // Network LED
            if (!Controller.IsPinModeSupported(PinNetworkLED, PinMode.Output))
            {
                Console.WriteLine($"Gpio pin {PinNetworkLED} can not be set.");
                return;
            }
            else
            {
                Controller.OpenPin(PinNetworkLED, PinMode.Output);
                Controller.Write(PinNetworkLED, PinValue.Low);
            }

            // Test button
            if (!Controller.IsPinModeSupported(PinTestButton, PinMode.InputPullUp))
            {
                Console.WriteLine($"Gpio pin {PinTestButton} can not be set.");
                return;
            }
            else
            {
                Controller.OpenPin(PinTestButton, PinMode.InputPullUp);
            }

            // Keyboard 1 jumper
            if (!Controller.IsPinModeSupported(PinKeyboard1, PinMode.InputPullDown))
            {
                Console.WriteLine($"Gpio pin {PinKeyboard1} can not be set.");
                return;
            }
            else
            {
                Controller.OpenPin(PinKeyboard1, PinMode.InputPullUp);
            }

            // Keyboard 2 jumper
            if (!Controller.IsPinModeSupported(PinKeyboard2, PinMode.InputPullDown))
            {
                Console.WriteLine($"Gpio pin {PinKeyboard2} can not be set.");
                return;
            }
            else
            {
                Controller.OpenPin(PinKeyboard2, PinMode.InputPullUp);
            }

            // Keyboard 3 jumper
            if (!Controller.IsPinModeSupported(PinKeyboard3, PinMode.InputPullDown)) 
            {
                Console.WriteLine($"Gpio pin {PinKeyboard3} can not be set.");
                return;
            }
            else
            {
                Controller.OpenPin(PinKeyboard3, PinMode.InputPullUp);
            }

            // Keyboard 4 jumper
            if (!Controller.IsPinModeSupported(PinKeyboard4, PinMode.InputPullDown)) 
            {
                Console.WriteLine($"Gpio pin {PinKeyboard4} can not be set.");
                return;
            }
            else
            {
                Controller.OpenPin(PinKeyboard4, PinMode.InputPullUp);
            }

            if (Controller.Read(PinKeyboard1) == PinValue.Low) KeyboardId = 1;
            if (Controller.Read(PinKeyboard2) == PinValue.Low) KeyboardId = 2;
            if (Controller.Read(PinKeyboard3) == PinValue.Low) KeyboardId = 3;
            if (Controller.Read(PinKeyboard4) == PinValue.Low) KeyboardId = 4;

            Stopwatch stopwatch1000ms = new();
            stopwatch1000ms.Start();
            Stopwatch stopwatch100ms = new();
            stopwatch100ms.Start();

            while (true) 
            {
                Thread.Sleep(50);
                if (stopwatch1000ms.ElapsedMilliseconds > 1000)
                {
                    AppRunLED = !AppRunLED;
                    Controller.Write(PinAppRunLED, AppRunLED ? PinValue.High : PinValue.Low);
                    Controller.Write(PinNetworkLED, System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable() ? PinValue.High : PinValue.Low);
                    stopwatch1000ms.Restart();
                }
                if (stopwatch100ms.ElapsedMilliseconds > 100)
                {
                    if (Controller.Read(PinTestButton) == PinValue.Low)
                    {
                        RPiPS2Com.PressAndReleaseButton(RPiPS2Com.Buttons.A);
                        RPiPS2Com.PressAndReleaseButton(RPiPS2Com.Buttons.ENTER);
                        while (Controller.Read(PinTestButton) == PinValue.Low) { }
                    }
                    if (UdpClient.Available > 0)
                    {
                        byte[] buffer = new byte[UdpClient.Available];
                        UdpClient.Client.Receive(buffer);
                        string receivedString = System.Text.Encoding.UTF8.GetString(buffer);
                        if (receivedString.StartsWith("RPiPS2Kbd"))
                        {
                            string[] data = receivedString.Split('|');
                            if (data.Length == 3)
                            {
                                if (byte.TryParse(data[1], out byte receivedId))
                                {
                                    if (receivedId == KeyboardId)
                                    {
                                        RPiPS2Com.WriteString(data[2]);
                                        RPiPS2Com.PressAndReleaseButton(RPiPS2Com.Buttons.ENTER);
                                    }
                                }
                            }
                        }
                    }
                    stopwatch100ms.Restart();
                }
            }
        }
    }
}