using System;

namespace Logger
{
    public interface ILogger
    {        
        void Trace(string formattedText, params object[] args);
        void Debug(string formattedText, params object[] args);
        void Info(string formattedText, params object[] args);
        void Warn(string formattedText, params object[] args);
        void Error(string formattedText, params object[] args);
        void Fatal(string formattedText, params object[] args);

        /// <summary>
        /// Логирование исключения при выполняемом действии
        /// </summary>
        /// <param name="action"></param>
        void LogException(Action action);

        /// <summary>
        /// Логирование исключения при выполняемом действии
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        T LogException<T>(Func<T> action);

        /// <summary>
        /// Логирование исключения при выполняемом действии
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="faultAction">дейстивие если сработало исключение</param>
        /// <returns></returns>
        T LogException<T>(Func<T> action, Func<T> faultAction);

        IDisposable Measure(string message);

        T Measure<T>(Func<T> func, string message, params object[] parameters);

        IDisposable Measure(string message, params object[] parameters);

        IDisposable StartMeasure(string message);
    }
}
