﻿using Hayaa.RPC.Common.Config;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Hayaa.RPC.Service.Server
{
    class MessageServer
    {
        IRPCProviderService g_IRPCProviderService = null;
        public MessageServer(IRPCProviderService IRPCProviderService_Interface)
        {
            g_IRPCProviderService = IRPCProviderService_Interface;
        }
        public void Run()
        {
            var serverConfig = ConfigHelper.Instance.GetComponentConfig();
            Int32 port = serverConfig.ProviderConfiguation.Port;
            IPAddress addr = IPAddress.Parse("0.0.0.0");
            try
            {              
                TcpListener server = new TcpListener(addr, port);
                server.AllowNatTraversal(true);
                server.Start();
                server.BeginAcceptTcpClient(new AsyncCallback(HandleTcpClientAccepted), server);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void HandleTcpClientAccepted(IAsyncResult ar)
        {
            TcpListener tcpListener = (TcpListener)ar.AsyncState;
            TcpClient client = tcpListener.EndAcceptTcpClient(ar);
            byte[] buffer = new byte[client.ReceiveBufferSize];
            NetworkStream stream = client.GetStream();
            stream.WriteAsync(buffer, 0, client.ReceiveBufferSize);
            String data= System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length);
        }
    }
}