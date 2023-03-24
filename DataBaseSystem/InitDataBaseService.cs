using Cysharp.Threading.Tasks;
using Maniac.Services;

namespace Maniac.DataBaseSystem
{
    public class InitDataBaseService : Service
    {
        private readonly DataBase _dataBase;

        public InitDataBaseService(DataBase dataBase)
        {
            _dataBase = dataBase;
        }
        
        public override async UniTask<IService.Result> Execute()
        {
            _dataBase.InitializeDataBase();
            return IService.Result.Success;
        }
    }
}