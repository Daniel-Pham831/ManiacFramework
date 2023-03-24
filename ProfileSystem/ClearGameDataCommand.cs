using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Maniac.ProfileSystem
{
    public class ClearGameDataCommand : Command.Command
    {
        public override async UniTask Execute()
        {
            ProfileManager.ClearGameData();
            Application.Quit();
        }
    }
}