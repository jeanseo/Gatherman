using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Gatherman.Data
{
    class InputAlertDialogBase<T> : PopupPage
    {
        // the awaitable task
        public Task<T> PageClosedTask { get { return PageClosedTaskCompletionSource.Task; } }

        // the task completion source
        public TaskCompletionSource<T> PageClosedTaskCompletionSource { get; set; }

        public InputAlertDialogBase(View contentBody)
        {
            // init the task completion source
            PageClosedTaskCompletionSource = new System.Threading.Tasks.TaskCompletionSource<T>();
        }

    }
}
