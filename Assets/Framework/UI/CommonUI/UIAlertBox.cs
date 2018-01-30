using System;
using UnityEngine;
using UnityEngine.UI;

public enum AlertMode
{
    Ok,
    OkCancel,
}

/// <summary>
/// 选择结果
/// </summary>
public enum AlertResult
{
    OK = 1,
    CANCEL = 2,
    NO = 3,
}

public class UIAlertBoxContext
{
    public string title;
    public string content;
    public AlertMode mode;
    public string okBtnContent;
    public string cancelBtnContent;

    public UIAlertBoxContext(string title, string content, AlertMode mode, string okContent = "", string cancelContent = "")
    {
        this.title = title;
        this.content = content;
        this.mode = mode;
        okBtnContent = okContent;
        cancelBtnContent = cancelContent;
    }
}


public class UIAlertBox : UIWin
{
    [SerializeField] private Text titleTxt;
    [SerializeField] private Text contentTxt;
    [SerializeField] private Button okBtn;
    [SerializeField] private Text okBtnTxt;
    [SerializeField] private Button cancelBtn;
    [SerializeField] private Text cancelBtnTxt;

    public Action<AlertResult> OnCommit;

    private string _content;
    private AlertMode _mode;
    private AlertResult _alertResult = AlertResult.CANCEL;


    protected override void OnAddUIEvent()
    {
        okBtn.onClick.AddListener(OnBtnOkClick);
        cancelBtn.onClick.AddListener(OnBtnCancelClick);
    }

    protected override void OnRemoveUIEvent()
    {
        okBtn.onClick.RemoveAllListeners();
        cancelBtn.onClick.RemoveAllListeners();
    }

    protected override void OnOpened()
    {
        base.OnOpened();
        UIAlertBoxContext tContext = _context as UIAlertBoxContext;
        if (tContext == null)
        {
            _mode = AlertMode.OkCancel;
            CloseSelf();
            return;
        }
        _mode = tContext.mode;
        _content = tContext.content;
        titleTxt.text = tContext.title;
        contentTxt.text = tContext.content.Replace(" ", "\u00A0"); ;
        if (tContext.mode == AlertMode.Ok)
        {
            cancelBtn.gameObject.SetActive(false);
        }
        else
        {
            cancelBtn.gameObject.SetActive(true);
        }
        if(!string.IsNullOrEmpty(tContext.okBtnContent))
            okBtnTxt.text = tContext.okBtnContent;
        if(!string.IsNullOrEmpty(tContext.cancelBtnContent))
            cancelBtnTxt.text = tContext.cancelBtnContent;
    }

    protected override void OnClosed()
    {
        base.OnClosed();
        if (OnCommit != null)
        {
            OnCommit(_mode == AlertMode.Ok ? AlertResult.OK : _alertResult);
        }
    }

    private void OnBtnOkClick()
    {
        _alertResult = AlertResult.OK;
        CloseSelf();
    }

    private void OnBtnCancelClick()
    {
        _alertResult = AlertResult.CANCEL;
        CloseSelf();
    }

    public bool IsMatchContent(string content)
    {
        return _content.Equals(content);
    }
}
