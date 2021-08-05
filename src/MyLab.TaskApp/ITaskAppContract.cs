using System.Threading.Tasks;
using MyLab.ApiClient;

namespace MyLab.TaskApp
{
    [Api]
    public interface ITaskAppContract
    {
        [Post("processing")]
        Task PostProcessAsync();

        [Get("processing")]
        Task<TaskAppStatus> GetProcessStatusAsync();
    }
}
