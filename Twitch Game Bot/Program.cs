using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;

namespace Twitch_Game_Bot
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            IRCClient irc = new IRCClient();
        }
    }

    public class IRCClient
    {
        TcpClient ircClient;
        NetworkStream ircStream;
        StreamReader ircReader;
        StreamWriter ircWriter;
        LogView lv;

        public IRCClient()
        {
            lv = new LogView();
            new Thread(delegate()
            {
                Application.Run(lv);
            }).Start();

            new Thread(delegate()
            {
                Application.Run(new TimeView());
            }).Start();

            Console.WriteLine("[--] Connecting...");
            ircClient = new TcpClient("199.9.250.229", 6667);
            ircStream = ircClient.GetStream();
            ircReader = new StreamReader(ircStream);
            ircWriter = new StreamWriter(ircStream);
            Console.WriteLine("[--] Connected");
            Console.Write("Paste oauth token: ");
            Send("PASS " + Console.ReadLine());
            Send("NICK TwitchPlaysMinecraft_");

            string cmd;

            while ((cmd = ircReader.ReadLine()) != null) {
                Console.WriteLine("[<-] " + cmd);

                if (cmd.Contains("PRIVMSG"))
                {
                    try
                    {
                        string action = cmd.Split(new string[] { "PRIVMSG #twitchplaysminecraft_ :" }, StringSplitOptions.None)[1];

                        string nick = cmd.Split('!')[0].Remove(0, 1);

                        new Thread(delegate()
                        {
                            new InputHandler(action, nick, lv);
                        }).Start();
                    }
                    catch
                    {

                    }
                }
                else if (cmd.Contains("End of /MOTD command"))
                {
                    Send("JOIN #twitchplaysminecraft_");
                }
                else if (cmd == "PING tmi.twitch.tv")
                {
                    Send("PONG tmi.twitch.tv");
                }
            }
        }

        public void Send(string msg)
        {
            Console.WriteLine("[->] " + msg);
            ircWriter.WriteLine(msg);
            ircWriter.Flush();
        }
    }

    public class InputHandler
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        public Dictionary<string, byte> KeyCodes = new Dictionary<string, byte>()
        {
            {"a",  0x1E},
            {"d", 0x20},
            {"w", 0x11},
            {"s", 0x1F}
        };

        public InputHandler(string action, string nick, LogView lv)
        {
            // move left
            if (action == "a")
            {
                lv.AddLog(nick + ":  move left");
                PressKey(Keys.A);
            }
            // move right
            else if (action == "d")
            {
                lv.AddLog(nick + ":  move right");
                PressKey(Keys.D);
            }
            // move forward
            else if (action == "w")
            {
                lv.AddLog(nick + ":  move forward");
                PressKey(Keys.W);
            }
            // move backward
            else if (action == "s")
            {
                lv.AddLog(nick + ":  move backward");
                PressKey(Keys.S);
            }
            // open/close inventory
            else if (action == "e")
            {
                lv.AddLog(nick + ":  open/close inv");
                PressKey(Keys.E);
            }
            // jump
            else if (action == "jump")
            {
                lv.AddLog(nick + ":  jump");
                PressKey(Keys.Space);
            }
            else if (action == "right")
            {
                lv.AddLog(nick + ":  look right");
                for (int i = 0; i < 100; i++)
                {
                    Cursor.Position = new System.Drawing.Point(Cursor.Position.X + 1, Cursor.Position.Y);
                    Thread.Sleep(1);
                }
            }
            else if (action == "left")
            {
                lv.AddLog(nick + ":  look left");
                for (int i = 0; i < 100; i++)
                {
                    Cursor.Position = new System.Drawing.Point(Cursor.Position.X - 1, Cursor.Position.Y);
                    Thread.Sleep(1);
                }
            }
            else if (action == "up")
            {
                lv.AddLog(nick + ":  look up");
                for (int i = 0; i < 100; i++)
                {
                    Cursor.Position = new System.Drawing.Point(Cursor.Position.X, Cursor.Position.Y - 1);
                    Thread.Sleep(1);
                }
            }
            else if (action == "down")
            {
                lv.AddLog(nick + ":  look down");
                for (int i = 0; i < 100; i++)
                {
                    Cursor.Position = new System.Drawing.Point(Cursor.Position.X, Cursor.Position.Y + 1);
                    Thread.Sleep(1);
                }
            }
            else if (action == "click")
            {
                lv.AddLog(nick + ":  left click");
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)Cursor.Position.X, (uint)Cursor.Position.Y, 0, 0);
            }
            else if (action == "rclick")
            {
                lv.AddLog(nick + ":  right click");
                mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, (uint)Cursor.Position.X, (uint)Cursor.Position.Y, 0, 0);
            }
            else if (action.StartsWith("click "))
            {
                try
                {
                    int time = Int32.Parse(action.Split(new String[] {"ick " }, StringSplitOptions.None)[1]);

                    if (time <= 20)
                    {
                        lv.AddLog(nick + ":  left click " + time + "s");
                        mouse_event(MOUSEEVENTF_LEFTDOWN, (uint)Cursor.Position.X, (uint)Cursor.Position.Y, 0, 0);
                        Thread.Sleep(time * 1000);
                        mouse_event(MOUSEEVENTF_LEFTUP, (uint)Cursor.Position.X, (uint)Cursor.Position.Y, 0, 0);
                    }
                }
                catch
                {

                }
            }
        }

        void PressKey(Keys key)
        {
            const int KEYEVENTF_EXTENDEDKEY = 0x1;
            const int KEYEVENTF_KEYUP = 0x2;
            // I had some Compile errors until I Casted the final 0 to UIntPtr like this...
            keybd_event((byte)key, 0x45, KEYEVENTF_EXTENDEDKEY, (UIntPtr)0);
            Thread.Sleep(1000);
            keybd_event((byte)key, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (UIntPtr)0);
        }
    }
}
