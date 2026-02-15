namespace Shared.Core.View.DialogQueue
{
    public static class DialogQueueExtensions
    {
        public static void InterruptDialogQueue(this IQueueableDialog dialog)
        {
            dialog.CurrentDialogQueue?.InterruptDialogQueue();
        }

        public static bool IsLastDialogInQueue(this IQueueableDialog dialog)
        {
            if (dialog.CurrentDialogQueue == null) return true;

            bool nextPopupInvalid = true;

            foreach (var operation in dialog.CurrentDialogQueue.PopUpsQueue)
            {
                if (operation.Dialog == dialog)
                {
                    continue;
                }

                if (operation.Validator.Invoke())
                {
                    nextPopupInvalid = false;
                    break;
                }
            }

            return nextPopupInvalid;
        }

        public static bool HasValidDialog(this DialogQueue queue)
        {
            foreach (var operation in queue.PopUpsQueue)
            {
                if (operation.Validator.Invoke())
                {
                    return false;
                }
            }

            return true;
        }
    }
}