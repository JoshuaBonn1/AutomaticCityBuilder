/* This code is directly ripped from https://stackoverflow.com/questions/2416748/how-do-you-simulate-mouse-click-in-c
 */

using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace RoadPlacer.Tools
{
    public class MouseOperations
    {
        [Flags]
        public enum MouseEventFlags
        {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010
        }

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out MousePoint lpMousePoint);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        public static void SetCursorPosition(int x, int y)
        {
            SetCursorPos(x, y);
        }

        public static void SetCursorPosition(Vector2 point)
        {
            SetCursorPos((int)point.x, (int)point.y);
        }

        public static Vector2 GetCursorPosition()
        {
            var gotPoint = GetCursorPos(out MousePoint currentMousePoint);
            return gotPoint? currentMousePoint.ToVector() : Vector2.zero;
        }

        public static void MouseEvent(MouseEventFlags value)
        {
            Vector2 position = GetCursorPosition();

            mouse_event
                ((int)value,
                 (int)position.x,
                 (int)position.y,
                 0,
                 0)
                ;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MousePoint
        {
            public int X;
            public int Y;

            public MousePoint(int x, int y)
            {
                X = x;
                Y = y;
            }

            public Vector2 ToVector()
            {
                return new Vector2(X, Y);
            }

            public override string ToString()
            {
                return "(" + X + ", " + Y + ")";
            }
        }
    }
}
