// ============================================================
//  DECORATOR PATTERN (GoF, Structural)
//  Component          : IWeaponStats
//  Concrete component : BaseWeaponStats
//  Decorator (base)   : WeaponStatsDecorator
//  Concrete decorators: SilencerDecorator, RapidFireDecorator,
//                       ExtendedMagDecorator
//  Client             : Gun
// ============================================================

// Component interface: what a gun's stats look like
public interface IWeaponStats
{
    float FireRate { get; }     // seconds between shots (lower = faster)
    int MagSize { get; }        // bullets per magazine
    float SfxVolume { get; }    // shot sound volume
    string Description { get; } // for display / debugging
}

// Concrete component: the plain gun with no attachments
public class BaseWeaponStats : IWeaponStats
{
    public float FireRate { get; }
    public int MagSize { get; }
    public float SfxVolume { get; }
    public string Description => "Base Gun";

    public BaseWeaponStats(float fireRate, int magSize, float sfxVolume)
    {
        FireRate = fireRate;
        MagSize = magSize;
        SfxVolume = sfxVolume;
    }
}

// Base decorator: implements the SAME interface and HAS-A wrapped IWeaponStats.
// By default it just passes everything through to the wrapped object.
public abstract class WeaponStatsDecorator : IWeaponStats
{
    protected readonly IWeaponStats inner;

    protected WeaponStatsDecorator(IWeaponStats inner)
    {
        this.inner = inner;
    }

    public virtual float FireRate => inner.FireRate;
    public virtual int MagSize => inner.MagSize;
    public virtual float SfxVolume => inner.SfxVolume;
    public virtual string Description => inner.Description;
}

// Concrete decorator 1: makes the shot much quieter
public class SilencerDecorator : WeaponStatsDecorator
{
    public SilencerDecorator(IWeaponStats inner) : base(inner) { }

    public override float SfxVolume => inner.SfxVolume * 0.2f;
    public override string Description => inner.Description + " + Silencer";
}

// Concrete decorator 2: doubles the fire rate (half the delay between shots)
public class RapidFireDecorator : WeaponStatsDecorator
{
    public RapidFireDecorator(IWeaponStats inner) : base(inner) { }

    public override float FireRate => inner.FireRate * 0.5f;
    public override string Description => inner.Description + " + Rapid Fire";
}

// Concrete decorator 3: adds 10 bullets to the magazine
public class ExtendedMagDecorator : WeaponStatsDecorator
{
    public ExtendedMagDecorator(IWeaponStats inner) : base(inner) { }

    public override int MagSize => inner.MagSize + 10;
    public override string Description => inner.Description + " + Extended Mag";
}