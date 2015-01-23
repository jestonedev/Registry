﻿using Registry.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.CalcDataModels
{
    public class CalcAsyncConfig
    {
        public EntityType Entity { get; set; }
        public int? IdObject { get; set; }

        public CalcAsyncConfig(EntityType entity, int? idObject)
        {
            this.Entity = entity;
            this.IdObject = idObject;
        }
    }
}
