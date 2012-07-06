using System;
using Sitecore;
using System.Diagnostics;

namespace FieldFallback.Logging
{
    internal class Logger : ILogger
    {
        private const string LOG_PREFIX = "FallbackValuesProvider ";

        private int _indent = 0;

        public bool Enabled { get; set; }

        public void Info(string message)
        {
            if (Enabled)
            {
                Sitecore.Diagnostics.Log.Info(FormatMessage(message), this);
            }
        }

        public void Info(string message, params object[] args)
        {
            Info(string.Format(message, args));
        }

        public void Error(string message)
        {
            if (Enabled)
            {
                Sitecore.Diagnostics.Log.Error(FormatMessage(message), this);
            }
        }

        public void Error(string message, Exception exception)
        {
            if (Enabled)
            {
                Sitecore.Diagnostics.Log.Error(FormatMessage(message), exception, this);
            }
        }

        public void Error(string message, params object[] args)
        {
            Error(string.Format(message, args));
        }

        public void Error(string message, Exception exception, params object[] args)
        {
            Error(string.Format(message, args), exception);
        }

        public void Debug(string message)
        {
            if (Enabled)
            {
                // Use SysInternals DebugView to see these on the server
                System.Diagnostics.Debug.WriteLine(FormatMessage(message));

                Trace(message);
            }
        }

        public void Debug(string message, params object[] args)
        {
            Debug(string.Format(message, args));
        }

        public void Warn(string message)
        {
            if (Enabled)
            {
                Sitecore.Diagnostics.Log.Warn(FormatMessage(message), this);
            }
        }

        public void Warn(string message, params object[] args)
        {
            Warn(string.Format(message, args));
        }

        public void PushIndent()
        {
            _indent++;
        }

        public void PopIndent()
        {
            _indent--;
            if (_indent < 0)
                _indent = 0;
        }

        private string FormatMessage(string message)
        {
            return string.Concat(LOG_PREFIX, GetIndents(), message);
        }

        private string GetIndents()
        {
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            for (int i = 0; i < _indent; i++)
            {
                s.Append("    ");
            }
            return s.ToString();
        }

        /// <summary>
        /// If the TRACE compilation symbol is defined and Trace is enabled in the web.config
        /// then output to trace so you can see it via http://localhost/trace.axd
        /// </summary>
        /// <param name="message">The message.</param>
        [Conditional("TRACE")]
        private void Trace(string message)
        {
            if (System.Web.HttpContext.Current != null)
            {
                System.Web.HttpContext.Current.Trace.Write(LOG_PREFIX, message);
            }
        }
    }
}