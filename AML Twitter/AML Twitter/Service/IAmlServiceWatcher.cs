using System.Threading.Tasks;

namespace AML.Twitter.Service
{
    public interface IAmlServiceWatcher
    {
        Task CallService();
    }
}