using System;
using System.Diagnostics;
using NLog;
using NLog.Targets;

namespace Logger
{
    
    /// <summary>
    /// Реализация логгера на базе фреймворка NLog
    /// </summary>
    public class NLogger : ILogger
    {
        #region Fields

        private static readonly NLog.Logger logger = LogManager.GetLogger(String.Empty);

        #endregion

        
        public NLogger()
        {
           
        }

        /// <summary>
        /// Инициализация с указанием расположения файла лога
        /// </summary>
        /// <param name="fileTargetName">Идентификатор настройки в файле инициализации логгера</param>
        /// <param name="fileName">Путь к файлу логгера</param>
        public NLogger(string fileTargetName, string fileName)
        {
            if (fileTargetName == String.Empty) 
                return;

            var target = logger.Factory.Configuration.FindTargetByName(fileTargetName);
            if (target == null)
                return;

            if (target is FileTarget)
            {
                (target as NLog.Targets.FileTarget).FileName = fileName;
            }
        }
        
         
        #region ILogger

        public void Debug(string formattedText, params object[] args)
        {
            if (logger.IsDebugEnabled)
                logger.Debug(PrepareMessageForLog(formattedText, args));
        }

        public void Trace(string formattedText, params object[] args)
        {
            if (logger.IsTraceEnabled)
                logger.Trace(PrepareMessageForLog(formattedText, args));
        }

        public void Info(string formattedText, params object[] args)
        {
            if (logger.IsInfoEnabled)
                logger.Info(PrepareMessageForLog(formattedText, args));
        }

        public void Warn(string formattedText, params object[] args)
        {
            if (logger.IsWarnEnabled)
                logger.Warn(PrepareMessageForLog(formattedText, args));
        }

        public void Error(string formattedText, params object[] args)
        {
            if (logger.IsErrorEnabled)
                logger.Error(PrepareMessageForLog(formattedText, args));
        }

        public void Fatal(string formattedText, params object[] args)
        {
            if (logger.IsFatalEnabled)
                logger.Fatal(PrepareMessageForLog(formattedText, args));
        }

        /// <summary>
        /// Логирование исключения при выполняемом действии
        /// </summary>
        /// <param name="action"></param>
        public void LogException(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Логирование исключения при выполняемом действии
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public T LogException<T>(Func<T> action)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                Error(ex.ToString());
                throw;
            }
        }
        /// <summary>
        /// Логирование исключения при выполняемом действии
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="faultAction">дейстивие если сработало исключение</param>
        /// <returns></returns>
        public T LogException<T>(Func<T> action, Func<T> faultAction)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                Error(ex.ToString());
                return faultAction();
            }
        }

        /// <summary>
        ///  функция для логгера с использованием using, для подсчета времени выполнения
        ///  </summary>
        public IDisposable Measure(string message)
        {
            return new MeasureToLog(message, this);
        }

        public T Measure<T>(Func<T> func, string message, params object[] parameters)
        {
            using (Measure(message, parameters))
            {
                return func();
            }
        }

        public IDisposable Measure(string message, params object[] parameters)
        {
            return new MeasureToLog(String.Format(message, parameters), this);
        }

        public IDisposable StartMeasure(string message)
        {
            Info(string.Format(">> {0}", message));
            return new MeasureToLog(string.Format("<< {0}", message), this);
        }
        #endregion

        #region Private helpers

        private static string PrepareMessageForLog(string formattedText, params object[] args)
        {
            try
            {
                var proc = Process.GetCurrentProcess();
                var str = string.Format(formattedText, args);
                
                return string.Format("[{0}/{1}/{2}] {3}", 
                    
                    ((int)proc.PrivateMemorySize64 / 1024 / 1024).ToString(),
                    ((int)proc.VirtualMemorySize64 / 1024 / 1024).ToString(),
                    ((int)proc.PeakVirtualMemorySize64 / 1024 / 1024).ToString(), 
                    str);
            }
            catch
            {
                var res = formattedText != null
                              ? string.Format("Ошибка формирования сообщения для вывода в лог {0}", formattedText)
                              : string.Empty;
                return res;
            }
        }


        private class MeasureToLog : IDisposable
        {
            #region  fields
            /// <summary>
            /// логгер
            /// </summary>
            private readonly ILogger _logger;

            /// <summary>
            /// Время начала
            /// </summary>
            private readonly DateTime _time;
            /// <summary>
            /// Название точки
            /// </summary>
            private readonly string _message;
            #endregion

            #region  ctor

            /// <summary>
            /// Для измерения времени выполнения
            /// </summary>
            /// <param name="message"></param>Название точки измерение
            /// <param name="logger"></param>
            /// <param name="priority"></param>
            public MeasureToLog(string message, ILogger logger)
            {
                _logger = logger;
                _message = message;
                _time = DateTime.UtcNow;
            }
            #endregion

            /// <summary>
            /// При завершение выводится затраченое время
            /// </summary>
            public void Dispose()
            {
                var total = (DateTime.UtcNow - _time);
                if (total > TimeSpan.FromMilliseconds(100))
                {
                    _logger.Info("{0} ({1} мс)", _message, total.TotalMilliseconds);
                }
            }
        }
        #endregion

        
    }

    public class Timer : IDisposable
    {
        private readonly string timerName;
        private readonly ILogger logger;
        private readonly DateTime dt;

        public Timer(string timerName, ILogger logger)
        {
            this.timerName = timerName;
            this.logger = logger;
            dt = DateTime.Now;

            //if (logger != null)
            //    logger.Debug("Timer start: {0}", timerName);
        }
        public void Dispose()
        {
            if (logger != null)
                logger.Trace("{0:100} time: {1,15:0.0000000} sec", timerName.Length<100? timerName +new String(' ',100-timerName.Length): timerName, (DateTime.Now - dt).TotalSeconds);
        }
    }

    public class OutputTimer : IDisposable
    {
        private readonly string timerName;
        private readonly DateTime dt;

        public OutputTimer(string timerName)
        {
            this.timerName = timerName;
            dt = DateTime.Now;
            Debug.WriteLine("Timer start: {0}", timerName);
        }
        public void Dispose()
        {
            Debug.WriteLine("Timer end: {0} time: {1} sec", timerName, (DateTime.Now - dt).TotalSeconds);
        }
    }

}
