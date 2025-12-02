using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DollInputController : MonoBehaviour
{
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // 1. waking input (WASD)
        bool walking = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
                       Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
        anim.SetBool("isWalking", walking);

        // 2. crouch input (Ctrl)
        bool crouch = Input.GetKey(KeyCode.C);

        // switching crouch state
        if (crouch && anim.GetBool("isCrouching") == false)
        {
            anim.SetBool("isCrouching", true);
            anim.ResetTrigger("standUpTrigger");
            anim.SetTrigger("crouchTrigger");     // play crouch once
        }

        // exit crouch
        if (!crouch && anim.GetBool("isCrouching") == true)
        {
            anim.SetBool("isCrouching", false);
            anim.ResetTrigger("crouchTrigger");
            anim.SetTrigger("standUpTrigger");    // play stand up once
        }

        // 3. crawling state
        bool crawling = anim.GetBool("isCrouching") && walking;
        anim.SetBool("isCrawling", crawling);
    }
}
