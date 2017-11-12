﻿using Autofac;
using Surging.Core.CPlatform;
using Surging.Core.DotNetty;
using Surging.Core.EventBusRabbitMQ;
using Surging.Core.ProxyGenerator.Utilitys;
using Surging.Core.ServiceHosting;
using Surging.Core.ServiceHosting.Internal.Implementation;
using Surging.Core.System.Ioc;
using Surging.Core.Zookeeper;
//using Surging.Core.Zookeeper.Configurations;
using System.Text;
using System;
using Surging.Core.Consul;
using Surging.Core.Consul.Configurations;

namespace surging.test
{
	class Program
	{
		static void Main(string[] args)
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			var host = new ServiceHostBuilder()
				.RegisterServices(option =>
				{
					option.Initialize();
					option.RegisterServices();
					option.RegisterRepositories();
					option.RegisterModules();
					option.RegisterServiceBus();
				})
				.RegisterServices(builder =>
				{
					builder.AddMicroService(option =>
					{
						option.AddServiceRuntime();
						//option.UseZooKeeperManager(new ConfigInfo("127.0.0.1:2181"));
						option.UseConsulManager(new ConfigInfo("192.168.255.128:8500"));
						option.UseDotNettyTransport();
						option.UseRabbitMQTransport();
						option.AddRabbitMQAdapt();
						builder.Register(p => new CPlatformContainer(ServiceLocator.Current));
					});
				})
				.SubscribeAt()
				.UseServer("127.0.0.1", 98)
				.UseStartup<Startup>()
				.Build();

			using (host.Run())
			{
				Console.WriteLine($"服务端启动成功，{DateTime.Now}。");
			}
		}
	}
}
