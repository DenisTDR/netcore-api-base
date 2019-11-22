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
    internal class DbStoredLogManager<T> where T : DbStoredLog
    {
        private readonly IRepository<T> _repo;

        private readonly ConcurrentQueue<EntityPatchBag<T>> _logQueue =
            new ConcurrentQueue<EntityPatchBag<T>>();

        private Thread _workerThread;

        private SemaphoreSlim _semaphore;

        internal DbStoredLogManager(IDataLayer dataLayer)
        {
//            Console.WriteLine("AuditManager ctor");
            _repo = dataLayer.Repo<T>();

            _semaphore = new SemaphoreSlim(0);
            try
            {
                _workerThread = new Thread(async () => { await WorkerMethod(); });
                _workerThread.Start();
            }
            catch (Exception exc)
            {
                Console.WriteLine($"{GetType().Name}.constructor Exception: {exc.Message} ");
                throw;
            }
        }

        public void Store(T audit)
        {
            _logQueue.Enqueue(new EntityPatchBag<T> {Model = audit});
            _semaphore.Release();
        }

        public void UpdateStoredLog(EntityPatchBag<T> epbae)
        {
            _logQueue.Enqueue(epbae);
            _semaphore.Release();
        }

        private bool _working = false;

        private async Task WorkerMethod()
        {
//            Console.WriteLine("Audit worker started");
            _working = true;
            while (_working)
            {
                if (!_logQueue.IsEmpty)
                {
                    while (!_logQueue.IsEmpty)
                    {
                        if (_logQueue.TryDequeue(out var epb))
                        {
                            try
                            {
                                if (!string.IsNullOrEmpty(epb.Id))
                                {
//                                    Console.WriteLine("patching audit");
                                    var existingLog =
                                        await _repo.FindOne(audit =>
                                            audit.TraceIdentifier == epb.Id || audit.Id == epb.Id);
                                    if (existingLog != null)
                                    {
                                        epb.Id = existingLog.Id;
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
                                Console.WriteLine($"{GetType().Name} Exception:" + exc.Message);
                                Console.WriteLine(JsonConvert.SerializeObject(epb));
                            }
                        }
                    }
                }

//                Console.WriteLine("waiting for semaphore");
                await _semaphore.WaitAsync();
//                Console.WriteLine("got green semaphore");
            }

            Console.WriteLine($"{GetType().Name} worker stopped");
        }

        public void Dispose()
        {
            _working = false;
        }
    }
}