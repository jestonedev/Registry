using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Security
{
    [Flags]
    public enum Priveleges
    {
        None = 0,
        RegistryRead = 1,
        RegistryReadWrite = 3,
        RegistryDirectoriesReadWrite = 5,
        RegistryAnalize = 9,
        RegistryAll = Priveleges.RegistryReadWrite | Priveleges.RegistryDirectoriesReadWrite | Priveleges.RegistryAnalize,
        TenancyRead = 16,
        TenancyReadWrite = 48,
        TenancyDirectoriesReadWrite = 80,
        TenancyAnalize = 144,
        TenancyAll = Priveleges.TenancyReadWrite | Priveleges.TenancyDirectoriesReadWrite | Priveleges.TenancyAnalize,
        ClaimsRead = 256,
        ClaimsReadWrite = 768,
        ClaimsDirectoriesReadWrite = 1280,
        ClaimsAnalize = 2304,
        ClaimsAll = Priveleges.ClaimsReadWrite | Priveleges.ClaimsDirectoriesReadWrite | Priveleges.ClaimsAnalize,
        PrivRead = 4096,
        PrivReadWrite = 12288,
        PrivDirectoriesReadWrite = 20480,
        PrivAnalize = 36864,
        PrivAll = Priveleges.PrivReadWrite | Priveleges.PrivDirectoriesReadWrite | Priveleges.PrivAnalize,
        ExchangeRead = 65536,
        ExchangeReadWrite = 196608,
        ExchangeDirectoriesReadWrite = 327680,
        ExchangeAnalize = 589824,
        ExchangeAll = Priveleges.ExchangeReadWrite | Priveleges.ExchangeDirectoriesReadWrite | Priveleges.ExchangeAnalize,
        GeneralRead = 1048576,
        GeneralReadWrite = 3145728,
        GeneralDirectoriesReadWrite = 5242880,
        GeneralAnalize = 9437184,
        GeneralAll = Priveleges.GeneralReadWrite | Priveleges.GeneralDirectoriesReadWrite | Priveleges.GeneralAnalize,
        All = Priveleges.RegistryAll | Priveleges.TenancyAll | Priveleges.ClaimsAll | Priveleges.PrivAll | Priveleges.ExchangeAll | Priveleges.GeneralAll
    }
}
