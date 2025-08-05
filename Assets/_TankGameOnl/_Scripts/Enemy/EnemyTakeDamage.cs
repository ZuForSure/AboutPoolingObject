using Lean.Pool;
using Mirror;

public class EnemyTakeDamage : NetworkBehaviour
{
    public int maxHp = 10;
    public int currentHP = 10;

    private void OnEnable()
    {
        this.currentHP = this.maxHp;
    }

    public virtual void DeductHP(int amount)
    {
        if (this.currentHP == 0) this.DeSpawnEnemey();

        this.currentHP -= amount;
    }

    [Server]
    protected virtual void DeSpawnEnemey()
    {
        NetworkServer.UnSpawn(transform.parent.gameObject); 
        LeanPool.Despawn(transform.parent);
    }
}
