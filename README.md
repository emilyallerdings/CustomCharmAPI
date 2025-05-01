<br />
<div align="center">

  <h3 align="center">Clover Pit Custom Charm API</h3>

  <p align="center">
    An API which allows for easy addition of extra charms to the CloverPit game!
  </p>
</div>


##  Features

- Register custom powerups (charms) with custom models and sounds
- Hook into game events like equip/unequip
- Automatically integrates with the game's charm system

## 🚀 Installation

1. Download and install [BepInEx](https://github.com/BepInEx/BepInEx) for your game.
2. [Download the latest release `.dll` from the Releases section](https://github.com/emilyallerdings/CustomCharmAPI/releases).
3. Place the `.dll` in your `BepInEx/plugins/` folder.

## 🧪 Basic Tutorial: Creating a Custom Charm

### 🔧 1. Create Your Plugin

Use BepInEx to define a new mod and reference `CustomCharmAPI`. Here's a minimal example:

```csharp
[BepInPlugin("com.yourname.mycharm", "My Custom Charm", "1.0.0")]
[BepInDependency("com.unconscious.powerupapi", BepInDependency.DependencyFlags.HardDependency)]
public class MyCharmPlugin : BaseUnityPlugin
{
    void Awake()
    {
        //Load your asset bundle next to your plugin dll.
        string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "mybundle");
        AssetBundle bundle = AssetBundle.LoadFromFile(path);

        //get the prefab and sound from the bundle if you have them.
        GameObject prefab = bundle.LoadAsset<GameObject>("mycharm");
        AudioClip sound = bundle.LoadAsset<AudioClip>("mysound");

        //Name your prefab.
        prefab.name = "Powerup My Charm";

        //OPTIONALLY: set rotation/scale of your model if needed.
        prefab.transform.localScale = Vector3.one;
        prefab.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);

        //Create new CustomPowerup Object with params, here's and example below. There are more params you can look through in the method declaration.
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

        //Add your custom powerup to the powerup API.
        CustomPowerupAPI.AddCustomPowerup(myCharm);
    }

    //Define logic for your charm to run onEquip/onUnequip/onPutInDrawer/onThrowaway.
    private static void OnEquip(PowerupScript powerup) => Debug.Log("Equipped charm!");
    private static void OnUnequip(PowerupScript powerup) => Debug.Log("Unequipped charm.");
}
```
---

### 📦 2. Bundle Your Assets

Create your charm model and optional audio clip in Unity, then:

- Name your GameObject (e.g. `mycharm`, `mysound`)  
- Export as an AssetBundle (`mybundle`)  
- Place it next to your plugin `.dll`  

---

### 🛠️ 3. Build Your Plugin

Build your plugin with dotnet. You can copy my `.csproj` from this repository for basic dependencies.
Make sure you have CustomCharmAPI installed in the plugins folder and that you properly set the game directory in your `.csproj`.
```<GameDir>D:\SteamLibrary\steamapps\common\CloverPit Demo</GameDir>```

---

---

### ▶️ 3. Run the Game

Launch the game with BepInEx. If your mod is correctly configured, the charm will be registered when the main game scene loads (e.g. `03GameScene`).  
You can check `BepInEx/LogOutput.log` for confirmation and debug messages or the BepInEx console (if it's enabled).

---

## 🧊 Example: CirnoFumoCharm

This repo includes a working example in the [`CirnoFumoCharm`](https://github.com/emilyallerdings/CirnoPlugin) folder.

It demonstrates:

- Loading an asset bundle (`cirno`)  
- Assigning a sound (`cirnosound`) and prefab (`cirno`)
- Adding a buff that increases slot machine sevens base value from 7 to 9  
- Implementing `onEquip` and `onUnequip` behavior  
- Applying Harmony patches to modify game logic (For making sevens base value 9)

---

## 🚧 TODO / Planned Features

- **Multiple Language Support**  
  Extend localization to fully support multiple languages using I2 integration
  
- **Terminal**  
  Add charms to terminal in game for viewing.

- **Locked Charms**  
  Allow for charms to be locked and have unlock requirements.

---

## 🛠️ Contributing

Pull requests and improvements are welcome. Feel free to fork and suggest features!

<p align="right">(<a href="#readme-top">back to top</a>)</p>

