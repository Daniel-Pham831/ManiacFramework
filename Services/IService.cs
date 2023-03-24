using System;
using Cysharp.Threading.Tasks;
using Maniac.Utils.Extension;
using UnityEngine;

namespace Maniac.Services
{
    public interface IService
    {
        public enum Result
        {
            Success,
            Fail,
        }
        
        UniTask<Result> Execute();
    }

    public abstract class Service : IService
    {
        private float _startTime;
        protected virtual string Name => GetType().Name;
        public abstract UniTask<IService.Result> Execute();

        public virtual async UniTask Run()
        {
            _startTime = Time.realtimeSinceStartup;
            Debug.Log($"{Name.AddColor(Color.yellow)} Starts {_startTime.ToString().AddColor(Color.green)}.");

            IService.Result result = IService.Result.Fail;
            try
            {
                result = await Execute();
            }
            catch (Exception e)
            {
                // ignored
                Debug.LogError(e);
            }

            Finish(result);
        }

        private void Finish(IService.Result result)
        {
            if (result == IService.Result.Success)
            {
                Debug.Log($"{Name.AddColor("#009FDD")} " +
                          $"Finish {_startTime.AddColor("#92ED86")} --> {Time.realtimeSinceStartup.AddColor("#92ED86")} : " +
                          $"{(Time.realtimeSinceStartup - _startTime).AddColor(Color.green)}");
            }
            else
            {
                Debug.Log($"{Name.AddColor("#009FDD")} {"Fail".AddColor(Color.red)}");
            }
        }
    }
}