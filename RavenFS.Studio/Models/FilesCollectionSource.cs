﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using RavenFS.Client;
using RavenFS.Studio.Extensions;
using RavenFS.Studio.Infrastructure;
using RavenFS.Extensions;

namespace RavenFS.Studio.Models
{
    public class FilesCollectionSource : VirtualCollectionSource<FileSystemModel>
    {
        private int? _fileCount;
        
        private const int MaximumNumberOfFolders = 1024;
        private List<DirectoryModel> _subFolders = new List<DirectoryModel>();
        private TaskScheduler _scheduler;

        public FilesCollectionSource()
        {
            _scheduler = TaskScheduler.FromCurrentSynchronizationContext();
        }

        public override int? Count
        {
            get { return _fileCount + _subFolders.Count; }
        }


        public override Task<IList<FileSystemModel>> GetPageAsync(int start, int pageSize)
        {
            if (start < _subFolders.Count - pageSize)
            {
                var tcs = new TaskCompletionSource<IList<FileSystemModel>>();
                tcs.SetResult(_subFolders.GetRange(start, pageSize).Cast<FileSystemModel>().ToList());
                return tcs.Task;
            }
            else
            {
                var filesStart = Math.Max(start - _subFolders.Count, 0);
                var subFoldersToInclude = Math.Min(Math.Max(pageSize - start, 0), _subFolders.Count);
                var filesPageSize = pageSize - subFoldersToInclude;

                return ApplicationModel.Current.Client.GetFilesAsync("/", FilesSortOptions.Name, filesStart, filesPageSize)
                        .ContinueOnSuccess(t => (IList<FileSystemModel>)GetLastSubFolders(subFoldersToInclude).Concat(ToFileSystemModels(t.Files)).ToList());

            }
        }

        private IEnumerable<DirectoryModel> GetLastSubFolders(int subFoldersToInclude)
        {
            return _subFolders.Skip(_subFolders.Count - subFoldersToInclude).Take(subFoldersToInclude);
        }

        private static IEnumerable<FileSystemModel> ToFileSystemModels(IEnumerable<FileInfo> t)
        {
            return t.Select(fi => new FileModel
                                      {
                                          FormattedTotalSize = fi.HumaneTotalSize, 
                                          FullPath = fi.Name,
                                          Metadata = fi.Metadata
                                      });
        }

        public void Refresh()
        {
            BeginUpdateItemCount();
        }

        private void BeginUpdateItemCount()
        {
            var getFoldersTask = ApplicationModel.Current.Client.GetFoldersAsync(pageSize: MaximumNumberOfFolders);
            var getFilesCountTask = ApplicationModel.Current.Client.GetFilesAsync("/", pageSize: 1);

            TaskEx.WhenAll(getFoldersTask, getFilesCountTask)
                .ContinueWith(_ =>
                                  {
                                      _subFolders.Clear();
                                      _subFolders.AddRange(getFoldersTask.Result.Select(n => new DirectoryModel() { FullPath = n}));
                                      _fileCount = getFilesCountTask.Result.FileCount;

                                      OnSourceChanged(EventArgs.Empty);
                                  }, _scheduler);
        }
    }
}
