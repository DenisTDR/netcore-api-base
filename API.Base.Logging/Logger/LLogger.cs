using System;
using API.Base.Logging.Managers.AuditManager;
using API.Base.Logging.Managers.LogManager;
using API.Base.Logging.Managers.UiLogManager;
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
        private readonly IUiLogManager _uiLogManager;

        public string Tag { get; private set; }

        public LLogger()
        {
            var dataLayer = DataLayerFactory.BuildDataLayer();
            _logManager = new LogManager();
            _auditManager = new AuditManager(dataLayer);
            _uiLogManager = new UiLogManager(dataLayer);
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

        public void Log(ILog log)
        {
            if (log is LogsAuditEntity entity)
            {
                LogAudit(entity);
            }
            else
            {
                LogLog(log as LogEntity);
            }
        }

        public void Log(LogLevel level, ILog log)
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

        private void LogAudit(LogsAuditEntity logsAudit)
        {
            logsAudit.Created = DateTime.Now;
            logsAudit.Tag = Tag;
            _auditManager.Store(logsAudit);
        }

        public void UpdateAuditLog(EntityPatchBag<LogsAuditEntity> epbae)
        {
            _auditManager.UpdateStoredLog(epbae);
        }

        public void UiLog(LogsUiEntity log)
        {
            _uiLogManager.Store(log);
        }

        public void UpdateUiLog(EntityPatchBag<LogsUiEntity> epbule)
        {
            _uiLogManager.UpdateStoredLog(epbule);
        }
    }
}