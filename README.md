<br />
<div align="center">

  <h3 align="center">Clover Pit Custom Charm API</h3>

  <p align="center">
    An API which allows for easy addition of extra charms to the game.
  </p>
</div>


## Installation

1. Download and install [BepInEx](https://github.com/BepInEx/BepInEx) for your game.
2. [Download the latest `.dll` file from the Releases section](https://github.com/emilyallerdings/CustomCharmAPI/releases).
3. Place the `.dll` into the `BepInEx/plugins` folder in your game directory.

CustomCharmAPI

CustomCharmAPI is a modding API that makes it easy to add custom charms (powerups) to your Unity game using BepInEx. This API handles prefab cloning, event registration, I2 localization integration, and asset injection, all with minimal boilerplate.

✨ Features

Easy charm registration

Custom equip/unequip events

Automatic localization key generation

AssetBundle support for prefabs and sounds

Compatibility with other mods using BepInEx + Harmony

🧩 Installation

Install BepInEx for your game.

Download the latest .dll file from the Releases section.

Place the .dll in your BepInEx/plugins folder.

🧪 Basic Tutorial: Creating a Custom Charm

Here’s a step-by-step guide to creating a custom charm using CustomCharmAPI.

🔧 1. Create Your Plugin

[BepInPlugin("com.yourname.yourmod", "My Custom Charm", "1.0.0")]
[BepInDependency("com.unconscious.powerupapi", BepInDependency.DependencyFlags.HardDependency)]
public class MyCustomCharm : BaseUnityPlugin
{
    void Awake()
    {
        string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "mycharmbundle");
        AssetBundle bundle = AssetBundle.LoadFromFile(path);

        GameObject charmPrefab = bundle.LoadAsset<GameObject>("mycharm");
        AudioClip charmSound = bundle.LoadAsset<AudioClip>("mysound");

        charmPrefab.name = "Powerup My Charm";
        charmPrefab.transform.localScale = Vector3.one;
        charmPrefab.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);

        CustomPowerup charm = new CustomPowerup
        {
            prefab = charmPrefab,
            sound = charmSound,
            displayName = "My Custom Charm",
            description = "Grants a mysterious buff!",
            startingPrice = 2,
            onEquip = MyOnEquip,
            onUnequip = MyOnUnequip
        };

        CustomPowerupAPI.AddCustomPowerup(charm);
    }

    private static void MyOnEquip(PowerupScript powerup)
    {
        Debug.Log("Equipped My Custom Charm!");
    }

    private static void MyOnUnequip(PowerupScript powerup)
    {
        Debug.Log("Unequipped My Custom Charm.");
    }
}

📦 2. Prepare Asset Bundle

Create a prefab (e.g. mycharm) and optionally a sound (mysound) in Unity.

Export them as an asset bundle.

Place the bundle in the same folder as your .dll.

🧊 Example Mod: CirnoFumoCharm

A complete example plugin is available: CirnoFumoCharm.cs

Loads a Cirno prefab and sound

Implements onEquip/onUnequip logic

Modifies symbol values if equipped

📜 License

MIT License

🙌 Credits

Developed by emilyallerdings

Feel free to contribute, suggest improvements, or build your own charms!



<p align="right">(<a href="#readme-top">back to top</a>)</p>

