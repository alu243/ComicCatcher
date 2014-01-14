using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace Helpers
{
    public class NLogger
    {
        private static TextBox _txtBox = null;
        private static object _logger = null;
        private static object logger
        {
            get
            {
                try
                {
                    //if (null == _logger && System.IO.File.Exists(@".\Nlog.dll")) _logger = NLog.LogManager.GetCurrentClassLogger();
                    return _logger;
                }
                catch { return null; }
            }
        }

        public static void SetBox(TextBox txtBox)
        {
            _txtBox = txtBox;
        }

        public static void Debug(string msg)
        {
            if (null != _txtBox) _txtBox.AppendText(FormatTextBoxString(msg));
            //if (null != logger) ((NLog.Logger)logger).Debug(msg);
        }
        public static void Trace(string msg)
        {
            if (null != _txtBox) _txtBox.AppendText(FormatTextBoxString(msg));
            //if (null != logger) ((NLog.Logger)logger).Trace(msg);
        }

        public static void Info(string msg)
        {
            if (null != _txtBox) _txtBox.AppendText(FormatTextBoxString(msg));
            //if (null != logger) ((NLog.Logger)logger).Info(msg);
        }

        public static void Warn(string msg)
        {
            if (null != _txtBox) _txtBox.AppendText(FormatTextBoxString(msg));
            //if (null != logger) ((NLog.Logger)logger).Warn(msg);
        }

        public static void Error(string msg)
        {
            if (null != _txtBox) _txtBox.AppendText(FormatTextBoxString(msg));
            //if (null != logger) ((NLog.Logger)logger).Error(msg);
        }

        public static void Fatal(string msg)
        {
            if (null != _txtBox) _txtBox.AppendText(FormatTextBoxString(msg));
            //if (null != logger) ((NLog.Logger)logger).Fatal(msg);
        }

        private static string FormatTextBoxString(string msg)
        {
            return DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff ") + msg + Environment.NewLine;
        }
    }
}
