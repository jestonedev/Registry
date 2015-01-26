﻿using System;
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
        RegistryWriteNotMunicipal = 2,
        RegistryReadWriteNotMunicipal = 3,
        RegistryWriteMunicipal = 268435456,
        RegistryReadWriteMunicipal = 268435457,
        RegistryWriteAll = Priveleges.RegistryWriteMunicipal | Priveleges.RegistryWriteNotMunicipal,
        RegistryDirectoriesWrite = 4,
        RegistryDirectoriesReadWrite = 5,
        RegistryAnalize = 9,
        RegistryAll = Priveleges.RegistryReadWriteMunicipal | Priveleges.RegistryReadWriteNotMunicipal | 
            Priveleges.RegistryDirectoriesReadWrite | Priveleges.RegistryAnalize,
        TenancyRead = 16,
        TenancyWrite = 32,
        TenancyReadWrite = 48,
        TenancyDirectoriesWrite = 64,
        TenancyDirectoriesReadWrite = 80,
        TenancyAnalize = 144,
        TenancyAll = Priveleges.TenancyReadWrite | Priveleges.TenancyDirectoriesReadWrite | Priveleges.TenancyAnalize,
        ClaimsRead = 256,
        ClaimsWrite = 512,
        ClaimsReadWrite = 768,
        ClaimsDirectoriesWrite = 1024,
        ClaimsDirectoriesReadWrite = 1280,
        ClaimsAnalize = 2304,
        ClaimsAll = Priveleges.ClaimsReadWrite | Priveleges.ClaimsDirectoriesReadWrite | Priveleges.ClaimsAnalize,
        PrivRead = 4096,
        PrivWrite = 8192,
        PrivReadWrite = 12288,
        PrivDirectoriesWrite = 16536,
        PrivDirectoriesReadWrite = 20480,
        PrivAnalize = 36864,
        PrivAll = Priveleges.PrivReadWrite | Priveleges.PrivDirectoriesReadWrite | Priveleges.PrivAnalize,
        ExchangeRead = 65536,
        ExchangeWrite = 131072,
        ExchangeReadWrite = 196608,
        ExchangeDirectoriesWrite = 262144,
        ExchangeDirectoriesReadWrite = 327680,
        ExchangeAnalize = 589824,
        ExchangeAll = Priveleges.ExchangeReadWrite | Priveleges.ExchangeDirectoriesReadWrite | Priveleges.ExchangeAnalize,
        GeneralRead = 1048576,
        GeneralWrite = 2097152,
        GeneralReadWrite = 3145728,
        GeneralDirectoriesWrite = 4194304,
        GeneralDirectoriesReadWrite = 5242880,
        GeneralAnalize = 9437184,
        ResettleRead = 16777216,
        ResettleWrite = 33554432,
        ResettleReadWrite = 50331648,
        ResettleDirectoriesWrite = 67108864,
        ResettleDirectoriesReadWrite = 83886080,
        ResettleAnalize = 150994944,
        ResettleAll = Priveleges.ResettleReadWrite | Priveleges.ResettleDirectoriesReadWrite | Priveleges.ResettleAnalize,
        GeneralAll = Priveleges.GeneralReadWrite | Priveleges.GeneralDirectoriesReadWrite | Priveleges.GeneralAnalize,
        All = Priveleges.RegistryAll | Priveleges.TenancyAll | Priveleges.ClaimsAll | Priveleges.PrivAll | Priveleges.ExchangeAll | Priveleges.GeneralAll | Priveleges.ResettleAll
    }
}
