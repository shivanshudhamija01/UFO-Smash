using UnityEngine;

public class AnimalTaken
    : BaseState<AnimalController>
{
    private Transform transform;
    private float shrinkSpeed = 3f;
    private float minScale = 0.2f;
    private Vector3 targetScale = Vector3.zero;

    public AnimalTaken(AnimalController controller) : base(controller)
    {
    }

    public override void OnEnterState()
    {
        transform = controller.GetTransform();

        Debug.Log("Animal is taken");
    }

    public override void UpdateState()
    {
        ShrinkAndDisappear();
    }

    public override void OnExitState()
    {
        // Reset scale for pooling
        transform.localScale = Vector3.one;
    }

    public override void FixedUpdateState()
    {
    }

    private void ShrinkAndDisappear()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, shrinkSpeed * Time.deltaTime);

        // Close enough to disappear
        if (Mathf.Abs(transform.localScale.x) <= minScale)
        {
            transform.localScale = Vector3.one;

            controller.gameObject.SetActive(false);

            // Optional pooling callback
            // controller
            // .GetAnimalSpawner()
            // .ReturnAnimal(controller);
        }
    }
}