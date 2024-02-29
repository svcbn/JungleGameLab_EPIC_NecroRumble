using LOONACIA.Unity.Managers;

public class UIScene : UIBase
{
    protected override void Init()
    {
        Init(false);
    }

    protected void Init(bool overlayCanvas_ = false, bool screenSpaceCameraCanvas_ = false) // todo: 개선필요
    {
        if(overlayCanvas_){
            ManagerRoot.UI.SetOverlayCanvas(gameObject);
        }else if(screenSpaceCameraCanvas_){
            ManagerRoot.UI.SetScreenSpaceCameraCanvas(gameObject);
        }else{
            ManagerRoot.UI.SetCanvas(gameObject, true);
        }
    }
}