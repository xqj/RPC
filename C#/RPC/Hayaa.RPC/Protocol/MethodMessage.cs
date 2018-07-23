﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Hayaa.RPC.Service.Protocol
{
    [Serializable]
   public class MethodMessage
    {
        public String InterfaceName { set; get; }
        public String Method { set; get; }
        public Dictionary<String,Object> Paramater { set; get; }
    }
}