﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Grand.Core.Infrastructure.Mapper
{
    public interface IMapperProfile
    {
        /// <summary>
        /// Gets order of this configuration implementation
        /// </summary>
        int Order { get; }
    }
}
