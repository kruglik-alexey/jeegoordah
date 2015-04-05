using System;
using Android.Util;

namespace Jeegoordah.Droid
{
    public static class Logger
    {
        public static void Info(object source, string message, params object[] messageArgs)
        {
            message = message.F(messageArgs);
            Log.Info(source.GetType().Name, "[{0}]\t{1}".F(DateTime.Now.ToString("HH:mm:ss.FFF"), message));
        }
    }
}

