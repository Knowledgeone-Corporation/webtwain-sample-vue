﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SpaServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SampleWebsiteVueJS
{
    public static class VueHelper
    {
        // default port number of 'npm run serve'
        private static int Port { set; get; }
        private static Uri DevelopmentServerEndpoint { set; get; }
        private static TimeSpan Timeout { get; } = TimeSpan.FromSeconds(30);
        // done message of 'npm run serve' command.
        private static string DoneMessage { get; } = "DONE  Compiled successfully in";

        public static void UseVueDevelopmentServer(this ISpaBuilder spa, string port = "3000")
        {
            if (string.IsNullOrEmpty(port))
            {
                port = "3000";
            }

            Port = int.Parse(port);

            DevelopmentServerEndpoint = new Uri($"http://localhost:{Port}");
            spa.UseProxyToSpaDevelopmentServer(async () =>
            {
                var loggerFactory = spa.ApplicationBuilder.ApplicationServices.GetService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("Vue");
                // if 'npm run serve' command was executed yourself, then just return the endpoint.
                if (IsRunning())
                {
                    return DevelopmentServerEndpoint;
                }

                // launch vue.js development server
                var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                var args = $"{(isWindows ? "/c npm " : "")}run serve -- --port {Port}";
                var processInfo = new ProcessStartInfo
                {
                    FileName = isWindows ? "cmd" : "npm",
                    Arguments = args,
                    WorkingDirectory = "client-app",
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                };
                var process = Process.Start(processInfo);
                var tcs = new TaskCompletionSource<int>();
                _ = Task.Run(() =>
                {
                    try
                    {
                        string line;
                        while ((line = process.StandardOutput.ReadLine()) != null)
                        {
                            logger.LogInformation(line);
                            if (!tcs.Task.IsCompleted && line.Contains(DoneMessage))
                            {
                                tcs.SetResult(1);
                            }
                        }
                    }
                    catch (EndOfStreamException ex)
                    {
                        logger.LogError(ex.ToString());
                        tcs.SetException(new InvalidOperationException("'npm run serve' failed.", ex));
                    }
                });
                _ = Task.Run(() =>
                {
                    try
                    {
                        string line;
                        while ((line = process.StandardError.ReadLine()) != null)
                        {
                            logger.LogError(line);
                        }
                    }
                    catch (EndOfStreamException ex)
                    {
                        logger.LogError(ex.ToString());
                        tcs.SetException(new InvalidOperationException("'npm run serve' failed.", ex));
                    }
                });

                var timeout = Task.Delay(Timeout);
                if (await Task.WhenAny(timeout, tcs.Task) == timeout)
                {
                    throw new TimeoutException();
                }

                return DevelopmentServerEndpoint;
            });

        }

        private static bool IsRunning() => IPGlobalProperties.GetIPGlobalProperties()
                .GetActiveTcpListeners()
                .Select(x => x.Port)
                .Contains(Port);
    }
}
