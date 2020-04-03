﻿
// (c) Copyright Cory Plotts.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows;

namespace Yamed.Control
{
    public static class WindowUtils
    {
        public static Window FindOwnerWindow()
        {
            Window ownerWindow = null;

            if (Application.Current != null)
            {
                if (Application.Current.MainWindow != null && Application.Current.MainWindow.Visibility == Visibility.Visible)
                {
                    // first: set the owner window as the current application's main window, if visible.
                    ownerWindow = Application.Current.MainWindow;
                }
                else
                {
                    // second: try and find a visible window in the list of the current application's windows
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window.Visibility == Visibility.Visible)
                        {
                            ownerWindow = window;
                            break;
                        }
                    }
                }
            }

            if (ownerWindow == null)
            {
                // third: try and find a visible window in the list of current presentation sources
                foreach (PresentationSource presentationSource in PresentationSource.CurrentSources)
                {
                    if
                    (
                      presentationSource.RootVisual is Window &&
                      ((Window)presentationSource.RootVisual).Visibility == Visibility.Visible
                    )
                    {
                        ownerWindow = (Window)presentationSource.RootVisual;
                        break;
                    }
                }
            }

            return ownerWindow;
        }
    }
}