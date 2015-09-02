﻿using System;
using System.Collections.Generic;
using OfflineMediaV3.Business.Enums;

namespace OfflineMediaV3.Business.Models.Configuration
{
    public class SourceConfigurationModel : SimpleSettingModel
    {
        public string SourceNameLong { get; set; }
        public string SourceNameShort { get; set; }

        public string LogicBaseUrl { get; set; }
        public string PublicBaseUrl { get; set; }

        public SourceEnum Source { get; set; }

        public List<FeedConfigurationModel> FeedConfigurationModels { get; set; } 
    }
}
