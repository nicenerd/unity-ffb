﻿using System;
using System.Runtime.InteropServices;
using System.Linq;

namespace UnityFFB {

    public static class Utilities{
        /// <summary>
        /// Rescale value X from ranges A-B to ranges X-Y, bool to clamp values
        /// </summary>
        public static double SuperLerp(double x, double in_min, double in_max, double out_min, double out_max, bool clamp = false){
            if (clamp) x = Math.Max(in_min, Math.Min(x, in_max)); // If input above our range, clamp it
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }
    }

    /// <summary>
    /// Helper class to print out user friendly system error codes.
    /// 
    /// Taken from: https://stackoverflow.com/a/21174331/9053848
    /// 
    /// </summary>
    public static class WinErrors {
        #region definitions
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr LocalFree(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int FormatMessage(FormatMessageFlags dwFlags, IntPtr lpSource, uint dwMessageId, uint dwLanguageId, ref IntPtr lpBuffer, uint nSize, IntPtr Arguments);

        [Flags]
        private enum FormatMessageFlags : uint {
            FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100,
            FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200,
            FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000,
            FORMAT_MESSAGE_ARGUMENT_ARRAY = 0x00002000,
            FORMAT_MESSAGE_FROM_HMODULE = 0x00000800,
            FORMAT_MESSAGE_FROM_STRING = 0x00000400,
        }
        #endregion

        /// <summary>
        /// Gets a user friendly string message for a system error code
        /// </summary>
        /// <param name="errorCode">System error code</param>
        /// <returns>Error string</returns>
        public static string GetSystemMessage(int errorCode) {
            try {
                IntPtr lpMsgBuf = IntPtr.Zero;

                int dwChars = FormatMessage(
                    FormatMessageFlags.FORMAT_MESSAGE_ALLOCATE_BUFFER | FormatMessageFlags.FORMAT_MESSAGE_FROM_SYSTEM | FormatMessageFlags.FORMAT_MESSAGE_IGNORE_INSERTS,
                    IntPtr.Zero,
                    (uint)errorCode,
                    0, // Default language
                    ref lpMsgBuf,
                    0,
                    IntPtr.Zero);
                if (dwChars == 0) {
                    // Handle the error.
                    int le = Marshal.GetLastWin32Error();
                    return "Unable to get error code string from System - Error " + le.ToString();
                }

                string sRet = Marshal.PtrToStringAnsi(lpMsgBuf);

                // Free the buffer.
                lpMsgBuf = LocalFree(lpMsgBuf);
                return sRet;
            } catch (Exception e) {
                return "Unable to get error code string from System -> " + e.ToString();
            }
        }
    }

}
