using Maniac.Utils.Extension;
using Newtonsoft.Json;
using UnityEngine;

namespace Maniac.ProfileSystem
{
    public interface IProfileRecord { }
    public class ProfileRecord : IProfileRecord
    {
        public virtual void Save()
        {
            var isSuccess = ProfileManager.Save(this);
            if (isSuccess)
            {
                Debug.Log($"Save:{GetType().Name.AddColor("#Cb9ce0")} - {"Success".AddColor(Color.yellow)}");
            }
            else
            {
                Debug.Log($"Save:{GetType().Name.AddColor("#9025be")} - {"Fail".AddColor(Color.red)}-Please Check!");
            }
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}