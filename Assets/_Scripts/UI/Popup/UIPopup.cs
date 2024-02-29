using LOONACIA.Unity.Managers;

public class UIPopup : UIBase
{
    protected override void Init()
    {
        ManagerRoot.UI.SetCanvas(gameObject, true);
    }

    public virtual void Close()
    {
        ManagerRoot.UI.ClosePopupUI(this);
    }
}