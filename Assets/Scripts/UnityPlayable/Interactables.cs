using UnityEngine;

public class Interactables : MonoBehaviour
{
    [SerializeField] AnimationClip animationClip;
    public AnimationClip GetAnimationClip() => animationClip;
    public GameObject optimalInteractionPoint; // the point from where the interaction animation plays best

    DrunkCharacterControllerCopied player; //reference to player controller script
    private void OnTriggerEnter(Collider other)
    {
        other.TryGetComponent(out player);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out player)) player = null;//need to figure out what this does

    }
    private void Update()
    {
        if(player && Input.GetKeyDown(KeyCode.Space))
        {
            player.Accept(this);
        }
    }
}
