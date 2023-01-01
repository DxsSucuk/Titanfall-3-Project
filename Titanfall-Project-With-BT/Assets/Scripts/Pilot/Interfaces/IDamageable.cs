using Fusion;

public interface IDamageable 
{
    
    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All)]
    public void DamageRPC(float damage, float armorPiercing);
}
