<br />
<div align="center">

  <h3 align="center">Clover Pit Custom Charm API</h3>

  <p align="center">
    An API which allows for easy addition of extra charms to the game.
  </p>
</div>


##  Features

- Register custom powerups (charms) with custom models and sounds
- Hook into game events like equip/unequip
- Supports localization through I2
- Asset bundle support for models and sounds
- Automatically integrates with the game's charm system
- Compatible with other BepInEx mods

##  Installation

1. Download and install [BepInEx](https://github.com/BepInEx/BepInEx) for your game.
2. [Download the latest release `.dll` from the Releases section](https://github.com/emilyallerdings/CustomCharmAPI/releases).
3. Place the `.dll` in your `BepInEx/plugins/` folder.

##  Basic Tutorial: Creating a Custom Charm

Here’s a step-by-step guide using the API:

```csharp
[BepInPlugin("com.yourname.mycharm", "My Custom Charm", "1.0.0")]
[BepInDependency("com.unconscious.powerupapi", BepInDependency.DependencyFlags.HardDependency)]
public class MyCharmPlugin : BaseUnityPlugin
{
    void Awake()
    {
        string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "mybundle");
        AssetBundle bundle = AssetBundle.LoadFromFile(path);

        GameObject prefab = bundle.LoadAsset<GameObject>("mycharm");
        AudioClip sound = bundle.LoadAsset<AudioClip>("mysound");

        prefab.name = "Powerup My Charm";
        prefab.transform.localScale = Vector3.one;
        prefab.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);

        CustomPowerup myCharm = new CustomPowerup
        {
            prefab = prefab,
            sound = sound,
            displayName = "My Custom Charm",
            description = "Grants a mysterious buff.",
            startingPrice = 2,
            onEquip = OnEquip,
            onUnequip = OnUnequip
        };

        CustomPowerupAPI.AddCustomPowerup(myCharm);
    }

    private static void OnEquip(PowerupScript powerup) => Debug.Log("Equipped charm!");
    private static void OnUnequip(PowerupScript powerup) => Debug.Log("Unequipped charm.");
}

<p align="right">(<a href="#readme-top">back to top</a>)</p>

