using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Common.Logging;
using log4net;
using log4net.Appender;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace Jeegoordah.Core.Logging
{
    public class Logger
    {
        private readonly ILog logger;

        public static Logger For(object obj)
        {
            return new Logger(obj);
        }

        public static Logger For(Type type)
        {
            return new Logger(type);
        }

        public static void Configure(string logFile)
        {
            var hierarchy = (Hierarchy)LogManager.GetRepository();
            hierarchy.Root.RemoveAllAppenders();

            var fileAppender = new RollingFileAppender
            {
				RollingStyle = RollingFileAppender.RollingMode.Size,
                AppendToFile = true,
                LockingModel = new FileAppender.MinimalLock(),
                Encoding = Encoding.UTF8,
                File = logFile,
                MaxFileSize = 10 * 1024 * 1024,
                MaxSizeRollBackups = 0
            };
            var pl = new PatternLayout {ConversionPattern = "%date %-5level [%-10logger]   %m%n"};
            pl.ActivateOptions();
            fileAppender.Layout = pl;
            fileAppender.ActivateOptions();
            log4net.Config.BasicConfigurator.Configure(fileAppender);

#if DEBUG
            var debugAppender = new DebugAppender {Layout = pl};
            debugAppender.ActivateOptions();
            log4net.Config.BasicConfigurator.Configure(debugAppender);
#endif            
        }

        public Logger(object obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            logger = LogManager.GetLogger(obj.GetType());
        }

        public Logger(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            logger = LogManager.GetLogger(type);
        }

        public Logger(string name)
        {
            if (name == null) throw new ArgumentNullException("name");
            logger = LogManager.GetLogger(name);
        }

        public void I(string message, params object[] args)
        {
            logger.InfoFormat(message, args);
        }

        public void D(string message, params object[] args)
        {
            logger.DebugFormat(message, args);
        }

        public void E(string message, params object[] args)
        {
            logger.ErrorFormat(message, args);
        }

        public void E(string message, Exception ex)
        {
            if (ex == null) throw new ArgumentNullException("ex");
            logger.ErrorFormat("{0}{1}{2}", message, Environment.NewLine, SerializeException(ex));
        }

        public void E(Exception ex)
        {
            E("Unhandled {0}".F(ex.GetType().FullName), ex);
        }

        private string SerializeException(Exception exception)
        {
            var serializer = new XmlSerializer(typeof(SerializableException));
            using (var stream = new StringWriter())
            {
                serializer.Serialize(stream, new SerializableException(exception));
                return stream.ToString();
            }
        }
    }
}
