using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UFOHover : BaseState<UFOController>
{
    private Transform transform;
    private IAbductable lockedAnimal;
    private Transform lockedAnimalTransform;
    private Vector2 offset;
    private float manualMoveSpeed;

    private Coroutine approachRoutine;
    private IAnimalService animalService;
    public UFOHover(UFOController controller)
        : base(controller)
    {
    }

    public override void OnEnterState()
    {
        if (animalService == null)
        {
            animalService = ServiceLocator.Get<IAnimalService>();
        }
        transform = controller.GetTransform();

        lockedAnimal = LockAnimal();
        AnimalController animalController = lockedAnimal as AnimalController;
        if(animalController != null)
        {
            animalController.SetLocked(true);
        }
        controller.SetLockedAnimal(lockedAnimal);
        if (lockedAnimal == null)
        {
            Debug.LogWarning("Locked animal is null");
            return;
        }
        lockedAnimalTransform = lockedAnimal.GetTransform();

        offset = controller.GetOffset();

        manualMoveSpeed = controller.GetManualSpeed();


        approachRoutine = controller.StartCoroutine(UFOIntroMovement());
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

            if (t > 0.25f)
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

        controller.GetTorchLight().gameObject.SetActive(true);
        lockedAnimal.BeginAbduction(controller.transform, controller);
        controller.GetStateMachine().ChangeState(UFOStates.abduct);
        // Here i need to change the UFO state to the abducting state, most important state, 
    }
    private IAbductable LockAnimal()
    {
        List<AnimalController> list = animalService.GetAnimalInScene();
        int index = Random.Range(0, list.Count);
        AnimalController animal = list[index];
        if (animal != null)
        {
            animalService.RemoveAnimal(animal);
        }
        return animal;
    }
}
