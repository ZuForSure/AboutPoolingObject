using Lean.Pool;
using UnityEngine;

public class AutoMove : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float duration;
    [SerializeField] private float time;
    [SerializeField] private LessonTwo pool;

    private void Awake()
    {
        pool = FindAnyObjectByType<LessonTwo>();
    }

    private void Update()
    {
        Movement();
    }
    private void Movement()
    {
        Vector3 move = speed * Time.deltaTime * Vector3.up;
        transform.Translate(move);
        time += Time.deltaTime;
        if (time >= duration)
        {
            time = 0;

            switch (pool.State)
            {
                case State.Pool:
                    pool.ReturnPool(gameObject);
                    break;
                case State.LeanPool:
                    LeanPool.Despawn(gameObject);
                    break;
            }

        }

    }
}
