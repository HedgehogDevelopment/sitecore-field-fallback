using System;

namespace FieldFallback.Logging
{
    public interface ILogger
    {
        void PushIndent();
        void PopIndent();
        
        void Info(string message);
        void Info(string message, params object[] args);

        void Debug(string message);
        void Debug(string message, params object[] args);

        void Warn(string message);
        void Warn(string message, params object[] args);

        void Error(string message);
        void Error(string message, params object[] args);
        void Error(string message, Exception exception);
    }
}