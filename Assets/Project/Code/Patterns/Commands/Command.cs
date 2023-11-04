using System.Threading.Tasks;

namespace Project.Code.Patterns.Commands
{
    public interface Command
    {
        Task Execute(object context = null);
    }
}