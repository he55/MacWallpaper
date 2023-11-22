﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MacWallpaper
{

    public class DownloadButtonCommand : ICommand
    {
        //public event EventHandler CanExecuteChanged;

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
                ass.Download4kWallpaper();
        }
    }

    public class CancelDownloadButtonCommand : ICommand
    {
        //public event EventHandler CanExecuteChanged;

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

}