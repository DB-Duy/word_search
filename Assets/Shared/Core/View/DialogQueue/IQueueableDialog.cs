using System;

namespace Shared.Core.View.DialogQueue
{
    public interface IQueueableDialog
    {
        DialogQueue CurrentDialogQueue { get; set; }
        Action OnDialogHide { get; set; }
    }
}