using System;
using System.Threading.Tasks;
using UnityEngine.Events;

/// <summary>
/// 對話欄，開啟後會等待下一個有效操作並回傳
/// </summary>
public class Dialog : Page
{
    public UnityEvent OnConfirm;
    public UnityEvent OnCancel;

    private TaskCompletionSource<bool> userActionTcs;

    public override void Open()
    {
        base.Open();
        // 開啟後等待進一步操作
        userActionTcs = new TaskCompletionSource<bool>();
    }

    public void Confirm()
    {
        userActionTcs.TrySetResult(true);
        OnConfirm?.Invoke();
    }

    public void Cancel()
    {
        userActionTcs.TrySetResult(false);
        OnCancel?.Invoke();
        Close();
    }

    /// <summary>
    /// 等待下一個操作，confirm回傳true，cancel回傳false
    /// </summary>
    /// <returns></returns>
    public Task<bool> WaitForAction()
    {
        return userActionTcs.Task;
    }
}
