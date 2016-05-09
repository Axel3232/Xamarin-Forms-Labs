using System.Threading.Tasks;

namespace XLabs.Platform.Services
{
    public interface IPermissionManager
    {
        Task<bool> CheckPermission(string[] permissions);
    }
}
