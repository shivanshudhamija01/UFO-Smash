using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UFOHover : BaseState<UFOController>
{
    private Transform transform;
    private IAbductable lockedAnimal;
    private Transform lockedAnimalTransform;

    private List<AnimalController> lockedAnimals = new List<AnimalController>();
    private Vector2 offset;
    private float manualMoveSpeed;

    private Coroutine approachRoutine;
    private IAnimalService animalService;
    private SpriteRenderer ufoSpriteRenderer;
    public UFOHover(UFOController controller)
        : base(controller)
    {
    }

    // public override void OnEnterState()
    // {
    //     if (animalService == null)
    //     {
    //         animalService = ServiceLocator.Get<IAnimalService>();
    //     }
    //     transform = controller.GetTransform();

    //     ufoSpriteRenderer = controller.GetSpriteRenderer();
    //     lockedAnimal = LockAnimal();
    //     AnimalController animalController = lockedAnimal as AnimalController;

    //     if (animalController != null)
    //     {
    //         animalController.SetLocked(true);
    //         ufoSpriteRenderer.sortingOrder = animalController.GetSortingOrder() + 1;
    //     }
    //     controller.SetLockedAnimal(lockedAnimal);
    //     if (lockedAnimal == null)
    //     {
    //         return;
    //     }
    //     lockedAnimalTransform = lockedAnimal.GetTransform();

    //     offset = controller.GetOffset();

    //     manualMoveSpeed = controller.GetManualSpeed();


    //     approachRoutine = controller.StartCoroutine(UFOIntroMovement());
    // }
    public override void OnEnterState()
    {
        if (animalService == null)
        {
            animalService = ServiceLocator.GetService<IAnimalService>();
        }

        transform = controller.GetTransform();

        ufoSpriteRenderer = controller.GetSpriteRenderer();

        if (controller.GetUFOType() == UFOType.Boss)
        {
            LockNearestAnimals(3);
            controller.SetLockedAnimals(lockedAnimals);
            if (lockedAnimals.Count == 0)
                return;

            AnimalController primaryTarget =
                lockedAnimals[0];

            lockedAnimal = primaryTarget;

            lockedAnimalTransform =
                primaryTarget.transform;

            ufoSpriteRenderer.sortingOrder =
                primaryTarget.GetSortingOrder() + 1;
        }
        else
        {
            lockedAnimal = LockAnimal();

            AnimalController animalController =
                lockedAnimal as AnimalController;

            if (animalController != null)
            {
                animalController.SetLocked(true);

                ufoSpriteRenderer.sortingOrder =
                    animalController.GetSortingOrder() + 1;
            }

            controller.SetLockedAnimal(lockedAnimal);

            if (lockedAnimal == null)
                return;

            lockedAnimalTransform =
                lockedAnimal.GetTransform();
        }

        offset = controller.GetOffset();

        manualMoveSpeed = controller.GetManualSpeed();

        approachRoutine =
            controller.StartCoroutine(UFOIntroMovement());
    }

    public override void UpdateState()
    {
        // Coroutine-driven state
    }

    public override void FixedUpdateState()
    {
    }

    public override void OnExitState()
    {
        if (approachRoutine != null)
        {
            controller.StopCoroutine(approachRoutine);
        }
    }

    private IEnumerator UFOIntroMovement()
    {
        Vector2 direction = (lockedAnimalTransform.position - transform.position).normalized;

        int dir = direction.x < 0 ? -1 : 1;

        float angle = dir * 30f;

        Vector2 shiftValue = new Vector2(dir * offset.x, offset.y);

        // Big overshoot with jerk
        yield return controller.StartCoroutine(OverShootAndTiltWithJerk(shiftValue.x, shiftValue.y, angle));

        yield return new WaitForSeconds(0.15f);

        // Reverse overshoot
        yield return controller.StartCoroutine(OverShootAndTiltWithJerk(-shiftValue.x, shiftValue.y, -angle));

        // Slower corrections
        manualMoveSpeed = 0.7f;

        yield return controller.StartCoroutine(OverShootAndTilt(shiftValue.x / 2f, shiftValue.y, angle / 2f));

        yield return controller.StartCoroutine(OverShootAndTilt(-shiftValue.x / 2f, shiftValue.y, -angle / 2f));

        // Final move
        yield return controller.StartCoroutine(MoveToAnimal());

        // Switch to next state
        controller.GetStateMachine().ChangeState(UFOStates.abduct);
    }

    private IEnumerator OverShootAndTiltWithJerk(float x, float y, float targetAngle)
    {
        Vector2 startPos = transform.position;

        Quaternion startRot = transform.rotation;

        Vector2 targetPos = new Vector2(lockedAnimalTransform.position.x + x, lockedAnimalTransform.position.y + y);

        Quaternion targetRot = Quaternion.Euler(0, 0, targetAngle);

        float distance = Vector2.Distance(startPos, targetPos);

        // Distance-based timing
        float duration = distance / manualMoveSpeed;

        // Clamp for consistency
        duration = Mathf.Clamp(duration, 0.25f, 0.85f);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / duration;

            transform.position = Vector2.Lerp(startPos, targetPos, t);

            // 0.25f
            if (t > 0.1f)
            {
                float smoothT = Mathf.SmoothStep(0, 1, t);

                transform.rotation = Quaternion.Lerp(startRot, targetRot, smoothT);
            }

            yield return null;
        }

        transform.position = targetPos;

        transform.rotation = targetRot;
    }

    private IEnumerator OverShootAndTilt(float x, float y, float targetAngle)
    {
        Vector2 startPos = transform.position;

        Quaternion startRot = transform.rotation;

        Vector2 targetPos = new Vector2(lockedAnimalTransform.position.x + x, lockedAnimalTransform.position.y + y);

        Quaternion targetRot = Quaternion.Euler(0, 0, targetAngle);

        float duration = manualMoveSpeed;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / duration;

            t = Mathf.SmoothStep(0, 1, t);

            transform.position = Vector2.Lerp(startPos, targetPos, t);

            transform.rotation = Quaternion.Lerp(startRot, targetRot, t);

            yield return null;
        }

        transform.position = targetPos;

        transform.rotation = targetRot;
    }

    private IEnumerator MoveToAnimal()
    {
        Vector2 startPos = transform.position;

        Vector2 targetPos = new Vector2(lockedAnimalTransform.position.x, lockedAnimalTransform.position.y + offset.y);

        Quaternion targetRot = Quaternion.Euler(0, 0, 0);

        float duration = 0.4f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / duration;

            t = Mathf.SmoothStep(0, 1, t);

            transform.position = Vector2.Lerp(startPos, targetPos, t);

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, t);

            yield return null;
        }

        transform.position = targetPos;

        // controller.GetTorchLight().gameObject.SetActive(true);
        // lockedAnimal.BeginAbduction(controller.transform, controller);
        // controller.GetStateMachine().ChangeState(UFOStates.abduct);
        controller.GetTorchLight().gameObject.SetActive(true);

        if (controller.GetUFOType() == UFOType.Boss)
        {
            foreach (AnimalController animal in lockedAnimals)
            {
                if (animal == null)
                    continue;

                animal.BeginAbduction(
                    controller.transform,
                    controller);
            }
        }
        else
        {
            lockedAnimal.BeginAbduction(
                controller.transform,
                controller);
        }

        controller.GetStateMachine()
                  .ChangeState(UFOStates.abduct);
    }

    private IAbductable LockAnimal()
    {
        List<AnimalController> list = animalService.GetAnimalInScene();

        if (list == null || list.Count == 0)
            return null;

        int index = Random.Range(0, list.Count);

        AnimalController animal = list[index];

        animalService.RemoveAnimal(animal);

        return animal;
    }
    private void LockNearestAnimals(int count)
    {
        List<AnimalController> animals =
            animalService.GetAnimalInScene();

        if (animals == null || animals.Count == 0)
            return;

        animals.Sort((a, b) =>
        {
            float distA =
                Vector2.Distance(
                    transform.position,
                    a.transform.position);

            float distB =
                Vector2.Distance(
                    transform.position,
                    b.transform.position);

            return distA.CompareTo(distB);
        });

        int maxCount =
            Mathf.Min(count, animals.Count);

        for (int i = 0; i < maxCount; i++)
        {
            AnimalController animal =
                animals[i];

            animal.SetLocked(true);

            lockedAnimals.Add(animal);
        }
        foreach (AnimalController animal in lockedAnimals)
        {
            animalService.RemoveAnimal(animal);
        }
    }

}
