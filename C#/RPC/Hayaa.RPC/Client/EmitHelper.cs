﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Reflection.Emit;

namespace Hayaa.RPC.Service.Client
{
    class EmitHelper
    {
        private static ModuleBuilder g_modBuilder = null;
        private static AssemblyBuilder g_assyBuilder = null;
        static EmitHelper()
        {
            //System.Reflection.AssemblyName 是用来表示一个Assembly的完整名称的
            AssemblyName assyName = new AssemblyName();
            //为要创建的Assembly定义一个名称（忽略版本号，Culture等信息）
            assyName.Name = "Hayaa.RPCProxy";
            g_assyBuilder = AssemblyBuilder.DefineDynamicAssembly(assyName, AssemblyBuilderAccess.Run);
            //获取ModuleBuilder，提供String参数作为Module名称
            g_modBuilder = g_assyBuilder.DefineDynamicModule("RPCProxy_RemoteServic");
        }
        //public static AssemblyBuilder AessemblyBuilderInstance
        //{
        //    get
        //    {
        //        return g_assyBuilder;
        //    }
        //}
        //public static ModuleBuilder ModuleBuilderInstance
        //{
        //    get
        //    {
        //        return g_modBuilder;
        //    }
        //}
        internal static object CreateClass(String assemblyName, String serviceName)
        {
            var assemblyList = AppDomain.CurrentDomain.GetAssemblies().ToList();
            Assembly interfaceAssembly = null;
            assemblyList.ForEach(a =>
            {
                if (a.FullName.Contains(assemblyName))
                {
                    interfaceAssembly = a;
                }
            });
            if (interfaceAssembly == null) return null;
            String className = serviceName + "Hayaa_ProxyClass";
            // 新类型的属性：要创建的是Class，而非Interface，Abstract Class等，而且是Public的
            TypeAttributes newTypeAttribute = TypeAttributes.Class | TypeAttributes.Public;
            //声明要创建的新类型的父类型
            Type newTypeParent;
            //声明要创建的新类型要实现的接口
            Type[] newTypeInterfaces;
            Type interfaceType = interfaceAssembly.GetType(serviceName);
            if (!interfaceType.IsInterface) return null;
            newTypeParent = null;
            newTypeInterfaces = new Type[] { interfaceType };
            //得到类型生成器            
            TypeBuilder typeBuilder = g_modBuilder.DefineType(className, newTypeAttribute, newTypeParent, newTypeInterfaces);
            //以下将为新类型声明方法
            //得到基类型的所有方法
            MethodInfo[] targetMethods = interfaceType.GetMethods();
            foreach (MethodInfo targetMethod in targetMethods)
            {
                //得到方法的各个参数的类型
                ParameterInfo[] paramInfo = targetMethod.GetParameters();
                Type[] paramType = new Type[paramInfo.Length];
                for (int i = 0; i < paramInfo.Length; i++)
                    paramType[i] = paramInfo[i].ParameterType;
                //传入方法签名，得到方法生成器
                MethodBuilder methodBuilder = typeBuilder.DefineMethod(targetMethod.Name, MethodAttributes.Public | MethodAttributes.Virtual, targetMethod.ReturnType, paramType);
                //由于要生成的是具体类，所以方法的实现是必不可少的。而方法的实现是通过Emit IL代码来产生的
                //得到IL生成器
                ILGenerator ilGen = methodBuilder.GetILGenerator();             
                
                //ilGen.Emit(OpCodes.Newobj, typeof(Dictionary<string, object>));
                //ilGen.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(String) }));
                ilGen.Emit(OpCodes.Ret);
            }
            return (typeBuilder.CreateType());
        }
    }
}
