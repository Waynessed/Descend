using UnityEngine;

public class SMB_IntroExit : StateMachineBehaviour
{
    // 可在 Inspector 上手动指定；留空则运行时自动查找
    public TutorialManager tutorial;

    // 仅当本次进入后第一次退出时触发，避免多次重复
    bool firedThisEntry = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        firedThisEntry = false;
        if (!tutorial)
        {
            // 尝试在场景中找一次（也可以用 tag/FindObjectOfType）
            tutorial = Object.FindFirstObjectByType<TutorialManager>();
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (firedThisEntry) return;
        firedThisEntry = true;

        if (tutorial)
        {
            tutorial.BeginTutorialOnce();   // 只会生效一次（见下一步）
        }
        else
        {
            Debug.LogWarning("[SMB_IntroExit] TutorialManager not found.");
        }
    }
}
