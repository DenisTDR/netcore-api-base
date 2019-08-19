using System;
using API.Base.Logging.Managers;
using API.Base.Logging.Models.Entities;
using API.Base.Web.Base.Factories;
using API.Base.Web.Base.Misc.PatchBag;
using Microsoft.Extensions.Logging;

namespace API.Base.Logging.Logger
{
    internal class LLogger : ILLogger
    {
        private readonly ILogManager _logManager;
        private readonly IAuditManager _auditManager;

        public string Tag { get; private set; }

        public LLogger()
        {
//            Console.WriteLine("LLogger ctor");

            var dataLayer = DataLayerFactory.BuildDataLayer();

            _logManager = new LogManager();
            _auditManager = new AuditManager(dataLayer);
        }

        public void SetTag(string tag)
        {
            if (!string.IsNullOrEmpty(Tag))
            {
                throw new Exception("Can't set tag second time.");
            }

            Tag = tag;
        }

        public void Log(LogLevel level, string message)
        {
            var log = new LogEntity
            {
                Created = DateTime.Now,
                Message = message,
                Level = level
            };
            Log(log);
        }

        public void Log(LogBaseEntity log)
        {
            if (log is AuditEntity entity)
            {
                LogAudit(entity);
            }
            else
            {
                LogLog(log as LogEntity);
            }
        }

        public void Log(LogLevel level, LogBaseEntity log)
        {
            log.Level = level;
            Log(log);
        }

        public void LogError(string message)
        {
            Log(LogLevel.Error, message);
        }

        public void LogWarn(string message)
        {
            Log(LogLevel.Warning, message);
        }

        public void LogInfo(string message)
        {
            Log(LogLevel.Information, message);
        }

        private void LogLog(LogEntity log)
        {
            log.Created = DateTime.Now;
            log.Tag = Tag;
            _logManager.StoreAsync(log);
        }

        private void LogAudit(AuditEntity audit)
        {
            audit.Created = DateTime.Now;
            audit.Tag = Tag;
            _auditManager.Store(audit);
        }

        public void UpdateAuditLog(EntityPatchBag<AuditEntity> epbae)
        {
            _auditManager.UpdateAuditLog(epbae);
        }
    }
}