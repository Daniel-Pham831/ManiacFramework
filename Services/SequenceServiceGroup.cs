using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Maniac.Services
{
    public class SequenceServiceGroup : Service
    {
        private readonly string _serviceGroupName;
        private readonly Queue<Service> _serviceQueue;
        protected override string Name => _serviceGroupName;

        public SequenceServiceGroup(string serviceGroupName)
        {
            _serviceGroupName = serviceGroupName;
            _serviceQueue = new Queue<Service>();
        }

        public void Add(Service newService)
        {
            _serviceQueue.Enqueue(newService);
        }

        public override async UniTask<IService.Result> Execute()
        {
            while (_serviceQueue.Count != 0)
            {
                var service = _serviceQueue.Dequeue();
                await service.Run();
            }

            return IService.Result.Success;
        }
    }
}