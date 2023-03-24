using System;
using Cysharp.Threading.Tasks;
using Maniac.Utils;
using UnityEngine;

namespace Maniac.UISystem.Command
{
    public class ShowScreenCommand : Maniac.Command.Command
    {
        private UIManager uiManager => Locator<UIManager>.Instance;
        private object parameter;
        private object result;
        private Type uiType;
        private BaseUI ui;

        public override async UniTask Execute()
        {
            ui = await uiManager.Show(uiType, parameter);
            if (ui == null)
            {
                Debug.Log($"Something wrong with {uiType} UI.");
                return;
            }
            ui.OnClose += (param) => result = param;
            await WaitCompletion();
        }

        public async UniTask<object> ExecuteAndReturnResult()
        {
            await Execute();
            if (HasValidResult())
                return result;
            else
                return default;
        }

        private bool HasValidResult()
        {
            return result != null;
        }

        public static ShowScreenCommand Create<T>(object parameter = null) where T : BaseUI
        {
            return new ShowScreenCommand()
            {
                parameter = parameter,
                uiType = typeof(T)
            };
        }

        private async UniTask WaitCompletion()
        {
            await ui.WaitCompletion();
        }
    }
}