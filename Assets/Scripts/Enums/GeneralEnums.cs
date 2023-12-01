
public enum EGameState : short
{
    StartingGame,
    Playing,
    EndingGame,
    Paused
}

public enum EObjectPooling : short
{
    PlayerShip,
    Asteroid,
    FlyingSaucer,
    Bullet,
    InputHandler,
    ExplosionParticle,
    None
}

public enum ESpawnPreset : short
{
    AssetPackOne,
    AssetPackTwo
}

public enum EEnemyType : short
{
    Asteroid,
    FlyingSaucer
}

public enum EPlayerType : short
{
    PlayerShip
}