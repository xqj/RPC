﻿using Hayaa.RPC.Common.Config;
using Hayaa.RPC.Service.Util;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Hayaa.RPC.Service.Client
{
   public class ServiceMethdoProxy
    {
        public static T Invoke<T>(String interfaceName, String methodName, Dictionary<String, Object> paramater)
        {
            T result = default(T);
            try
            {
                ClientHelper.Instance.EnQueue(new Protocol.MethodMessage()
                {
                     InterfaceName=interfaceName,
                      Method=methodName,
                       Paramater=paramater,
                        MsgID=Guid.NewGuid().ToString("N")
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }
      
    }
}
