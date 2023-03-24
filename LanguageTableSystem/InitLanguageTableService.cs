using Cysharp.Threading.Tasks;
using Maniac.Services;
using Maniac.Utils;

namespace Maniac.LanguageTableSystem
{
    public class InitLanguageTableService : Service
    {
        private readonly LanguageTable _languageTable;

        public InitLanguageTableService(LanguageTable languageTable)
        {
            _languageTable = languageTable;
        }
        
        public override async UniTask<IService.Result> Execute()
        {
            _languageTable.ChangeLanguage(_languageTable.GetDefaultLanguage());
            _languageTable.Init();
            
            Locator<LanguageTable>.Set(_languageTable);
            return IService.Result.Success;
        }
    }
}