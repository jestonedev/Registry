using System.Threading;
using Settings;

namespace Registry.DataModels
{
    internal sealed class DataAccessLockers
    {
        public static readonly object DataModelLockObj = new object();

        // Не больше MaxDBConnectionCount потоков одновременно делают запросы к БД
        public static readonly Semaphore DbAccessSemaphore = new Semaphore(RegistrySettings.MaxDbConnectionCount, RegistrySettings.MaxDbConnectionCount);
    }
}
