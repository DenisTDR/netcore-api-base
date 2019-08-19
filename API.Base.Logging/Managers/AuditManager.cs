using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using API.Base.Logging.Models.Entities;
using API.Base.Web.Base.Database.DataLayer;
using API.Base.Web.Base.Database.Repository;
using API.Base.Web.Base.Misc.PatchBag;
using Newtonsoft.Json;

namespace API.Base.Logging.Managers
{
    internal class AuditManager : IAuditManager, IDisposable
    {
        private readonly IRepository<AuditEntity> _repo;

        private readonly ConcurrentQueue<EntityPatchBag<AuditEntity>> _auditQueue =
            new ConcurrentQueue<EntityPatchBag<AuditEntity>>();

        private Thread _workerThread;

        private SemaphoreSlim _semaphore;

        internal AuditManager(IDataLayer dataLayer)
        {
//            Console.WriteLine("AuditManager ctor");
            _repo = dataLayer.Repo<AuditEntity>();

            _semaphore = new SemaphoreSlim(0);
            try
            {
                _workerThread = new Thread(async () => { await WorkerMethod(); });
                _workerThread.Start();
            }
            catch (Exception exc)
            {
                Console.WriteLine($"AuditManager.constructor Exception: {exc.Message} ");
                throw;
            }
        }

        public void Store(AuditEntity audit)
        {
            _auditQueue.Enqueue(new EntityPatchBag<AuditEntity> {Model = audit});
            _semaphore.Release();
        }

        public void UpdateAuditLog(EntityPatchBag<AuditEntity> epbae)
        {
            _auditQueue.Enqueue(epbae);
            _semaphore.Release();
        }

        private bool _working = false;

        private async Task WorkerMethod()
        {
//            Console.WriteLine("Audit worker started");
            _working = true;
            while (_working)
            {
                if (!_auditQueue.IsEmpty)
                {
                    while (!_auditQueue.IsEmpty)
                    {
                        if (_auditQueue.TryDequeue(out var epb))
                        {
                            try
                            {
                                if (!string.IsNullOrEmpty(epb.Id))
                                {
//                                    Console.WriteLine("patching audit");
                                    var existingAudit =
                                        (await _repo.FindOne(audit => audit.TraceIdentifier == epb.Id));
                                    if (existingAudit != null)
                                    {
                                        epb.Id = existingAudit.Selector;
                                        await _repo.Patch(epb);
                                    }
                                }
                                else
                                {
//                                    Console.WriteLine("adding audit");
                                    await _repo.Add(epb.Model);
                                }
                            }
                            catch (Exception exc)
                            {
                                Console.WriteLine("AuditManager Exception:" + exc.Message);
                                Console.WriteLine(JsonConvert.SerializeObject(epb));
                            }
                        }
                    }
                }

//                Console.WriteLine("waiting for semaphore");
                await _semaphore.WaitAsync();
//                Console.WriteLine("got green semaphore");
            }

            Console.WriteLine("Audit worker stopped");
        }

        public void Dispose()
        {
            _working = false;
        }
    }
}