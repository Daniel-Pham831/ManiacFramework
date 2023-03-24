using Newtonsoft.Json;

namespace Maniac.ProfileSystem
{
    public interface IProfileRecord { }
    public class ProfileRecord : IProfileRecord
    {
        public virtual void Save()
        {
            ProfileManager.Save(this);
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}