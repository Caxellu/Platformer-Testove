using Zenject;

public class PlayerFireUseCase
{
    private readonly SignalBus _signalBus;
    private readonly IBulletFactory _bulletFactory;
    private readonly IBullletSpawnPos _spawnPos;
    private readonly IUnitDirection _direction;
    private readonly ICoroutineManager _coroutines;

    private float _bulletSpeed;
    private int _damage;
    private float _cooldown;
    private bool _isCooldown;

    public PlayerFireUseCase(SignalBus signalBus, IBulletFactory bulletFactory,
        IBullletSpawnPos spawnPos, IUnitDirection direction, ICoroutineManager coroutines)
    {
        _signalBus = signalBus;
        _bulletFactory = bulletFactory;
        _spawnPos = spawnPos;
        _direction = direction;
        _coroutines = coroutines;
    }

    public void Initialize(float speed, int damage, float cooldown)
    {
        _bulletSpeed = speed;
        _damage = damage;
        _cooldown = cooldown;

        _signalBus.Subscribe<FireSignal>(OnFireSignal);
        _signalBus.Subscribe<BulletHitSignal>(OnBulletHit);

    }

    public void TryFire()
    {
        if (_isCooldown )
            return;

        _isCooldown = true;

        _coroutines.RunDelayedAction(_cooldown, () => _isCooldown = false);

        _signalBus.Fire(new FireSignal(_direction.IsRightDir));
    }

    private void OnFireSignal(FireSignal signal)
    {
        _bulletFactory.SpawnBullet(_spawnPos.Position, signal.IsRightDir, _damage, _bulletSpeed);
    }

    private void OnBulletHit(BulletHitSignal signal)
    {
        _bulletFactory.SpawnHitBullet(signal.Pos);
    }
}
