using System.Device.Gpio;
using System.Diagnostics;

namespace RPiPS2Comunication
{
    public class RPiPS2Com
    {
        public string LastExceptionMessage = "";

        private int clockPin;
        private int dataPin;

        private long ticksPerPulse = Stopwatch.Frequency / 1000000L * 40L; // 40us per pulse
        private long ticksPerPulseHalf = (Stopwatch.Frequency / 1000000L * 40L) / 2; // 20us per half of pulse
        private long ticksWaitAfterByte = Stopwatch.Frequency / 1000000L * 1000L;

        private GpioController controller = new GpioController();

        public RPiPS2Com(int ClockPin, int DataPin)
        {
            clockPin = ClockPin;
            dataPin = DataPin;
        }
        public bool Initialize()
        {
            if (controller.IsPinOpen(clockPin) || controller.IsPinOpen(dataPin))
            {
                LastExceptionMessage = "Pins can not be open.";
                return false;
            }
            if (!controller.IsPinModeSupported(clockPin, PinMode.InputPullUp) || !controller.IsPinModeSupported(dataPin, PinMode.InputPullUp))
            {
                LastExceptionMessage = "Pins can not be set as input.";
                return false;
            }
            if (!controller.IsPinModeSupported(clockPin, PinMode.Output) || !controller.IsPinModeSupported(dataPin, PinMode.Output))
            {
                LastExceptionMessage = "Pins can not be set as output.";
                return false;
            }

            controller.OpenPin(clockPin);
            controller.OpenPin(dataPin);

            SetHi(clockPin);
            SetHi(dataPin);

            return true;
        }
        public void PressButton(Buttons value)
        {
            Write(((byte)value));
        }
        public void ReleaseButton(Buttons value)
        {
            Write(0xF0);
            Write(((byte)value));
        }
        public void PressAndReleaseButton(Buttons value)
        {
            Write(((byte)value));
            Write(0xF0);
            Write(((byte)value));
        }
        public void WriteString(string value)
        {
            foreach (char ch in value)
            {
                char chUpper = Char.ToUpper(ch);
                Buttons button = GetButton(chUpper.ToString());
                if (button != 0)
                {
                    if (Char.IsUpper(ch))
                    {
                        PressButton(Buttons.LEFT_SHIFT);
                        PressAndReleaseButton(button);
                        ReleaseButton(Buttons.LEFT_SHIFT);
                    }
                    else
                    {
                        PressAndReleaseButton(button);
                    }
                }
            }
        }

        private bool Write(byte value)
        {
            if (controller.Read(clockPin) == PinValue.Low || controller.Read(dataPin) == PinValue.Low)
            {
                LastExceptionMessage = "Can not write, clock or data are LOW.";
                return false;
            }

            byte countOf1s = 0;

            // Start bit
            long ticksRemainToPulseEnd = SetLow(dataPin);
            WaitTicks(ticksRemainToPulseEnd - ticksPerPulseHalf);
            ticksRemainToPulseEnd = SetLow(clockPin);
            WaitTicks(ticksRemainToPulseEnd);
            ticksRemainToPulseEnd = SetHi(clockPin);
            WaitTicks(ticksRemainToPulseEnd - ticksPerPulseHalf);

            // Data
            for (int i = 0; i < 8; i++)
            {
                if ((value & 1) == 1)
                {
                    countOf1s++;
                    ticksRemainToPulseEnd = SetHi(dataPin);
                }
                else
                {
                    ticksRemainToPulseEnd = SetLow(dataPin);
                }
                WaitTicks(ticksRemainToPulseEnd - ticksPerPulseHalf);
                ticksRemainToPulseEnd = SetLow(clockPin);
                WaitTicks(ticksRemainToPulseEnd);
                ticksRemainToPulseEnd = SetHi(clockPin);
                WaitTicks(ticksRemainToPulseEnd - ticksPerPulseHalf);

                value >>= 1;
            }

            // Parity
            if (countOf1s % 2 == 0)
            {
                ticksRemainToPulseEnd = SetHi(dataPin);
            }
            else
            {
                ticksRemainToPulseEnd = SetLow(dataPin);
            }
            WaitTicks(ticksRemainToPulseEnd - ticksPerPulseHalf);
            ticksRemainToPulseEnd = SetLow(clockPin);
            WaitTicks(ticksRemainToPulseEnd);
            ticksRemainToPulseEnd = SetHi(clockPin);
            WaitTicks(ticksRemainToPulseEnd - ticksPerPulseHalf);

            // Stop
            ticksRemainToPulseEnd = SetHi(dataPin);
            WaitTicks(ticksRemainToPulseEnd - ticksPerPulseHalf);
            ticksRemainToPulseEnd = SetLow(clockPin);
            WaitTicks(ticksRemainToPulseEnd);
            ticksRemainToPulseEnd = SetHi(clockPin);
            WaitTicks(ticksRemainToPulseEnd - ticksPerPulseHalf);

            WaitTicks(ticksWaitAfterByte);

            return true;
        }
        private long SetHi(int pin)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            controller.SetPinMode(pin, PinMode.InputPullUp);
            controller.Write(pin, PinValue.High);
            stopwatch.Stop();
            return ticksPerPulse - stopwatch.ElapsedTicks;
        }
        private long SetLow(int pin)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            controller.SetPinMode(pin, PinMode.Output);
            controller.Write(pin, PinValue.Low);
            stopwatch.Stop();
            return ticksPerPulse - stopwatch.ElapsedTicks;
        }
        private void WaitTicks(long ticks)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (stopwatch.ElapsedTicks < ticks) { }
        }
        private Buttons GetButton(string value)
        {
            switch (value)
            {
                case "0":
                    {
                        value = "ZERO";
                        break;
                    }
                case "1":
                    {
                        value = "ONE";
                        break;
                    }
                case "2":
                    {
                        value = "TWO";
                        break;
                    }
                case "3":
                    {
                        value = "THREE";
                        break;
                    }
                case "4":
                    {
                        value = "FOUR";
                        break;
                    }
                case "5":
                    {
                        value = "FIVE";
                        break;
                    }
                case "6":
                    {
                        value = "SIX";
                        break;
                    }
                case "7":
                    {
                        value = "SEVEN";
                        break;
                    }
                case "8":
                    {
                        value = "EIGHT";
                        break;
                    }
                case "9":
                    {
                        value = "NINE";
                        break;
                    }
            }

            if (Enum.TryParse(typeof(Buttons), value.ToString(), out object? newValue))
            {
                return newValue == null ? 0 : (Buttons)newValue;
            }
            else
            {
                return 0;
            }
        }

        public enum Buttons : byte
        {
            ESCAPE = 0x76,
            F1 = 0x05,
            F2 = 0x06,
            F3 = 0x04,
            F4 = 0x0c,
            F5 = 0x03,
            F6 = 0x0b,
            F7 = 0x83,
            F8 = 0x0a,
            F9 = 0x01,
            F10 = 0x09,
            F11 = 0x78,
            F12 = 0x07,
            SCROLL_LOCK = 0x7e,
            ACCENT = 0x0e,
            ONE = 0x16,
            TWO = 0x1e,
            THREE = 0x26,
            FOUR = 0x25,
            FIVE = 0x2e,
            SIX = 0x36,
            SEVEN = 0x3d,
            EIGHT = 0x3e,
            NINE = 0x46,
            ZERO = 0x45,
            MINUS = 0x4e,
            EQUAL = 0x55,
            BACKSPACE = 0x66,
            TAB = 0x0d,
            Q = 0x15,
            W = 0x1d,
            E = 0x24,
            R = 0x2d,
            T = 0x2c,
            Y = 0x35,
            U = 0x3c,
            I = 0x43,
            O = 0x44,
            P = 0x4d,
            OPEN_BRACKET = 0x54,
            CLOSE_BRACKET = 0x5b,
            BACKSLASH = 0x5d,
            CAPS_LOCK = 0x58,
            A = 0x1c,
            S = 0x1b,
            D = 0x23,
            F = 0x2b,
            G = 0x34,
            H = 0x33,
            J = 0x3b,
            K = 0x42,
            L = 0x4b,
            SEMI_COLON = 0x4c,
            TICK_MARK = 0x52,
            ENTER = 0x5a,
            LEFT_SHIFT = 0x12,
            Z = 0x1a,
            X = 0x22,
            C = 0x21,
            V = 0x2a,
            B = 0x32,
            N = 0x31,
            M = 0x3a,
            COMMA = 0x41,
            PERIOD = 0x49,
            SLASH = 0x4a,
            RIGHT_SHIFT = 0x59,
            LEFT_CONTROL = 0x14,
            LEFT_ALT = 0x11,
            SPACE = 0x29,
            NUM_LOCK = 0x77,
            ASTERISK = 0x7c,
            NUMPAD_MINUS = 0x7b,
            NUMPAD_SEVEN = 0x6c,
            NUMPAD_EIGHT = 0x75,
            NUMPAD_NINE = 0x7d,
            PLUS = 0x79,
            NUMPAD_FOUR = 0x6b,
            NUMPAD_FIVE = 0x73,
            NUMPAD_SIX = 0x74,
            NUMPAD_ONE = 0x69,
            NUMPAD_TWO = 0x72,
            NUMPAD_THREE = 0x7a,
            NUMPAD_ZERO = 0x70,
            DECIMAL = 0x71
        }
    }
}