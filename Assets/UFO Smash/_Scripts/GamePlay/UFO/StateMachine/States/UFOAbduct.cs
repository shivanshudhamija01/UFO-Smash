using UnityEngine.UI;
using UnityEngine;

public class UFOAbduct : BaseState<UFOController>
{
    private Canvas healthBar;
    public UFOAbduct(UFOController controller) : base(controller)
    {
    }
    public override void OnEnterState()
    {
        healthBar = controller.GetHealthBar();
        healthBar.gameObject.SetActive(true);
    }
    public override void UpdateState()
    {
    }
    public override void OnExitState()
    {

    }
    public override void FixedUpdateState()
    {

    }
}