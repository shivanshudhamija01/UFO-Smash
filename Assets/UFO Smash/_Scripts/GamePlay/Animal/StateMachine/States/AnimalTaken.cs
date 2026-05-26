using UnityEngine;

public class AnimalTaken
    : BaseState<AnimalController>
{
    private Transform transform;
    private Transform visual;
    private float shrinkSpeed = 3f;
    private float minScale = 0.2f;
    private Vector3 targetScale = Vector3.zero;
    private Lane lane;
    private AnimalSpawner animalSpawner;
    private IEventBus eventBus;
    public AnimalTaken(AnimalController controller) : base(controller)
    {
    }

    public override void OnEnterState()
    {
        Init();
    }

    public override void UpdateState()
    {
        ShrinkAndDisappear();
    }

    public override void OnExitState()
    {
        // Reset scale for pooling
        transform.localScale = Vector3.one;
        eventBus.Publish(new Events.OnAnimalTaken());
        if (visual != null)
        {
            visual.localRotation = Quaternion.identity;
        }
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
            ReturnToPool();
        }

        // can use the method instead of lerp 
        // transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, shrinkSpeed * Time.deltaTime);
    }
    private void Init()
    {
        transform = controller.GetTransform();
        lane = controller.AssignedLane;
        animalSpawner = controller.GetAnimalSpawner();
        visual = controller.GetVisualTransform();
        if (eventBus == null)
        {
            eventBus = ServiceLocator.Get<IEventBus>();
        }
        // Debug.Log("Animal is taken");
    }
    private void ReturnToPool()
    {
        // lane.currentAnimals = Mathf.Max(0, lane.currentAnimals - 1);
        animalSpawner.AnimalRemoved(this.controller);
        transform.localScale = Vector3.one;
        controller.gameObject.SetActive(false);
    }
}


// Here in exit state ,i have to fire an event to notify that, okay the game is over now