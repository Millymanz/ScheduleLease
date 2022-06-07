using System.Threading.Tasks;

namespace OrbitalWitnessAPI.Interfaces
{
    public interface IRestClient
    {
        Task<string> GetFromRestService(string url);
    }
}
