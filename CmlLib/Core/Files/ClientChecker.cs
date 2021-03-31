﻿using CmlLib.Core.Installer;
using CmlLib.Core.Files;
using CmlLib.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CmlLib.Core.Files
{
    public sealed class ClientChecker : IFileChecker
    {
        public event DownloadFileChangedHandler ChangeFile;

        public bool CheckHash { get; set; } = true;

        public DownloadFile[] CheckFiles(MinecraftPath path, MVersion version)
        {
            ChangeFile?.Invoke(new DownloadFileChangedEventArgs(MFile.Minecraft, version.Jar, 1, 0));
            var result = CheckClientFile(path, version);
            ChangeFile?.Invoke(new DownloadFileChangedEventArgs(MFile.Minecraft, version.Jar, 1, 1));
            return new DownloadFile[] { result };
        }

        public async Task<DownloadFile[]> CheckFilesTaskAsync(MinecraftPath path, MVersion version)
        {
            ChangeFile?.Invoke(new DownloadFileChangedEventArgs(MFile.Minecraft, version.Jar, 1, 0));
            var result = await Task.Run(() => CheckClientFile(path, version));
            ChangeFile?.Invoke(new DownloadFileChangedEventArgs(MFile.Minecraft, version.Jar, 1, 1));
            return new DownloadFile[] { result };
        }

        private DownloadFile CheckClientFile(MinecraftPath path, MVersion version)
        {
            if (string.IsNullOrEmpty(version.ClientDownloadUrl)) return null;

            string id = version.Jar;
            string clientPath = path.GetVersionJarPath(id);

            if (!IOUtil.CheckFileValidation(clientPath, version.ClientHash))
            {
                return new DownloadFile
                {
                    Type = MFile.Minecraft,
                    Name = id,
                    Path = clientPath,
                    Url = version.ClientDownloadUrl
                };
            }
            else
                return null;
        }
    }
}