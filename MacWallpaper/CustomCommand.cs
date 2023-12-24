using System;
using System.Windows.Input;

namespace MacWallpaper
{
    public class DownloadButtonCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            if (parameter is Ass ass)
                return ass.downloadState == DownloadState.none;
            return false;
        }

        public void Execute(object parameter)
        {
            if (parameter is Ass ass)
                ass.Download();
        }
    }

    public class CancelDownloadButtonCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            if (parameter is Ass ass)
                return ass.downloadState == DownloadState.downloading;
            return false;
        }

        public void Execute(object parameter)
        {
            if (parameter is Ass ass)
                ass.CancelDownload();
        }
    }

    public class OpenFolderButtonCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            if (parameter is Ass ass)
                return ass.downloadState == DownloadState.downloaded;
            return false;
        }

        public void Execute(object parameter)
        {
            if (parameter is Ass ass)
                ass.OpenFolder();
        }
    }

    public class PreviewButtonCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            if (parameter is Ass ass)
                return ass.downloadState == DownloadState.downloaded;
            return false;
        }

        public void Execute(object parameter)
        {
            if (parameter is Ass ass)
                ass.Preview();
        }
    }
}
