using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using UnityEngine;
using System;
using System.Collections;
using System.Numerics;
using System.IO;
using Panik;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
using System.ComponentModel;
using I2.Loc;
using PowerupAPI.Powerups;

namespace PowerupAPI.Base{

[BepInPlugin("com.unconscious.powerupapi", "Custom Powerup API", "1.0.0")]
public class CustomPowerupAPI : BaseUnityPlugin
{

    public static int FirstModdedId => (int)PowerupScript.Identifier.count;
    public static int CustomCount = 0;

    public static Dictionary<PowerupScript.Identifier, GameObject> customIdDict = new Dictionary<PowerupScript.Identifier, GameObject>();
    public static List<CustomPowerup> CustomPowerups = new List<CustomPowerup>();

    public static CustomPowerupAPI instance;


    void Awake()
    {
        instance = this;
        //TryAddLocalizationSource(new LanguageSourceData());

        SceneManager.sceneLoaded += OnSceneLoaded;
        
        //LocalizationManager.LocalizeAll(Force: true);
        var harmony = new Harmony("com.unconscious.powerupapi");
        harmony.PatchAll();
        Logger.LogInfo("PowerUpGroupInjectMod patches applied!");
    }

    public static void AddCustomPowerup(CustomPowerup customPowerup){
        CustomPowerups.Add(customPowerup);
    }

    private void TryAddLocalizationSource(LanguageSourceData sourceData)
    {
        var method = typeof(LocalizationManager).GetMethod(
            "AddSource",
            BindingFlags.NonPublic | BindingFlags.Static
        );

        if (method != null)
        {
            method.Invoke(null, new object[] { sourceData });
        }
        else
        {
            Debug.LogError("Could not find LocalizationManager.AddSource via reflection.");
        }
    }
public static void AddI2Term(string termKey, string translation, string language)
{
    LocalizationManager.InitializeIfNeeded();

    var source = LocalizationManager.Sources[0];

    // Ensure language exists
    int langIndex = source.GetLanguageIndex(language);
    if (langIndex < 0)
    {
        source.AddLanguage(language);
        langIndex = source.GetLanguageIndex(language);
    }

    // Add or get existing term
    var termData = source.GetTermData(termKey) ?? source.AddTerm(termKey, eTermType.Text, false);
    termData.SetTranslation(langIndex, translation);

    // Refresh internal dictionary
    source.UpdateDictionary(true);

    // Optional debug
    Debug.Log($"Added term '{termKey}' with translation '{translation}' in '{language}'");
}

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {



        // 3. Only instantiate in the target scene
        if (scene.name == "03GameScene") // Replace with your actual scene name
        {
            CustomCount = 0;

            //Get a base charm (1tp) to clone and modify
            GameObject test = AssetMaster.GetPrefab("Powerup 1 Trick Pony");
            if (test == null){
                Debug.LogError("Cannot Find 1TP");
                return;
            }

            foreach (var powerup in CustomPowerups)
            {

                AddI2Term(powerup.prefab.name + "_NAME", powerup.displayName, Translation.LanguageI2NameGet(Translation.Language.English));
                AddI2Term(powerup.prefab.name + "_DESC", powerup.description, Translation.LanguageI2NameGet(Translation.Language.English));

                //Clone base charm
                GameObject basePowerup = Instantiate(test);
                basePowerup.name = powerup.prefab.name;

                //Destroy clone mesh
                Transform meshCharm = basePowerup.transform.Find("MeshHolder/Mesh_Charm_OneTrickPony");
                UnityEngine.Object.DestroyImmediate(meshCharm.gameObject);

                //Intantiate Mesh
                GameObject newMesh = Instantiate(powerup.prefab);
                newMesh.name = "Mesh_Charm";

                //Reparent Mesh
                Transform meshHolder = basePowerup.transform.Find("MeshHolder");
                newMesh.transform.SetParent(meshHolder, false); 

                //Set PS1 Shader
                Renderer renderer = newMesh.GetComponent<Renderer>();
                Shader psxShader = Shader.Find("PSXEffects/PS1Shader");
                renderer.material.shader = psxShader;

                //Destroy base DiegeticMenuElement to prevent duplicates on initialize.
                DiegeticMenuElement dme = basePowerup.GetComponent<PowerupScript>().DiegeticMenuElement_Get();
                DestroyImmediate(dme);

                basePowerup.SetActive(false);

                if(powerup.sound != null){
                    AssetMaster.AddSound(powerup.sound);
                    basePowerup.GetComponent<PowerupScript>().triggerSpecificSound = powerup.sound;
                }

                AssetMaster.AddPrefab(basePowerup);

                PowerupScript.Identifier id = AddCustomPowerupToGame(basePowerup);
                powerup.identifier = id;

                //Debug.Log("Clone DME " + dme.GetInstanceID());
                //Debug.Log("Outline DME " + dme.myOutline.GetInstanceID());
                //PrintHierarchyRecursive(clone.transform);
            }
        }
    }

    private void AddToAssetMaster(GameObject go){
        AssetMaster.AddPrefab(go);

        GameObject test = AssetMaster.GetPrefab(go.name);
        if (test == null){
            Debug.Log("FAILED TO ADD TO ASSET MASTER");
        }

    }

    private PowerupScript.Identifier AddCustomPowerupToGame(GameObject customPowerup){
        Debug.Log("Adding " + customPowerup.name);


        CustomCount++;
        PowerupScript.Identifier CustomID = (PowerupScript.Identifier)(CustomCount + FirstModdedId);

        if (customIdDict.ContainsKey(CustomID)){
            customIdDict.Remove(CustomID);
        }

        customIdDict.Add(CustomID, customPowerup);

        var fieldInfo = AccessTools.Field(typeof(PowerupScript), "dict_IdentifierToPrefabName");
        if (fieldInfo != null)
        {
            var dict = (Dictionary<PowerupScript.Identifier, string>)fieldInfo.GetValue(null);
            if (!dict.ContainsKey(CustomID)){
                dict.Add(CustomID, customPowerup.name);
            }
            Debug.Log("Adding " + (int) CustomID + " to the dict to prefab list as: " + customPowerup.name);
        }

        return CustomID;

    }

        // Call this function to print the hierarchy of the current object
    void PrintHierarchyRecursive(Transform parent, string indent = "")
    {
        // Print the current object
        Debug.Log(indent + parent.name);

        // Recursively print all children
        foreach (Transform child in parent)
        {
            PrintHierarchyRecursive(child, indent + "  ");
        }
    }

    public static void InitializeCustomAll(bool placePowerups, bool isNewGame){
            foreach (CustomPowerup custPowerup in CustomPowerups)
            {
            var id = custPowerup.identifier;

            Debug.Log("Spawning " + custPowerup.prefab.name + " as id:" + (int) id);
            PowerupScript powerup = PowerupScript.Spawn(id);

            if(powerup == null){
                Debug.LogError("powerup.spawn returned NULL");
                return;
            }
            if(powerup.gameObject == null){
                Debug.LogError("powerup gameobject is NULL");
                return;
            }
            powerup.gameObject.SetActive(true);

            string unlockTextKey = "POWERUP_UNLOCK_MISSION_NONE";
            if(custPowerup.unlockText != null){
                unlockTextKey = custPowerup.prefab.name + "_UNLOCK";
            }

            Debug.Log("Initalizing " + (int) id);
            powerup.Initialize(
                isNewGame,
                custPowerup.category,
                id,
                custPowerup.archetype,
                custPowerup.isInstantPowerup,
                custPowerup.maxBuyTimes,
                custPowerup.storeRerollChance,
                custPowerup.startingPrice,
                custPowerup.unlockPrice,
                custPowerup.prefab.name + "_NAME",
                custPowerup.prefab.name + "_DESC",
                unlockTextKey,
                custPowerup.onEquip,
                custPowerup.onUnequip,
                custPowerup.onPutInDrawer,
                custPowerup.onThrowAway
            );            

           // UnityEngine.Debug.Log("INIT GO: " + powerup.gameObject.GetInstanceID());
            //UnityEngine.Debug.Log("INIT PUS: " + powerup.GetInstanceID());

            //DiegeticMenuElement dme = powerup.DiegeticMenuElement_Get();
            //Debug.Log("INIT DME " + dme.GetInstanceID());

           // Debug.Log("Outline " + dme.myOutline.GetInstanceID());


           // Debug.Log("Initialized " + (int) id);
           // Debug.Log("Name " + powerup.gameObject.name);
           // Debug.Log("Category " + powerup.category);
          //  Debug.Log("ID " + powerup.identifier);
          //  Debug.Log("Archetype " + powerup.archetype);
            //instance.PrintHierarchyRecursive(powerup.gameObject.transform);
        }
        
    }


}


/*class MyPatch
{
    static MethodBase TargetMethod()
    {
        return typeof(DiegeticMenuElement).GetMethod("IsMouseOver", BindingFlags.Instance | BindingFlags.NonPublic);
    }

    static void Postfix(DiegeticMenuElement __instance)
    {
        var field = typeof(DiegeticMenuElement).GetField("hits", BindingFlags.NonPublic | BindingFlags.Instance);
        RaycastHit[] hits = (RaycastHit[])field.GetValue(__instance);

        foreach (var hit in hits)
        {
            if (hit.collider != null)
                //UnityEngine.Debug.Log("Hit: " + hit.collider.name);
                if (hit.collider.name.Contains("Cirno") || hit.collider.name.Contains("Rorschach") ){


                    bool flag = VirtualCursors.IsCursorVisible(0);
                    if (!flag && !AimCrossScript.IsEnabled())
                    {
                        UnityEngine.Debug.Log("FLAG ERROR");
                    }

                    UnityEngine.Debug.Log("Hit: " + hit.collider.name);
                    DiegeticMenuElement dme = hit.collider.GetComponent<DiegeticMenuElement>();
                    
                    PowerupScript pus = hit.collider.GetComponent<PowerupScript>();
                    DiegeticMenuElement dme2 = pus.DiegeticMenuElement_Get();

                    UnityEngine.Debug.Log("Hit GO: " + hit.collider.gameObject.GetInstanceID());
                    UnityEngine.Debug.Log("Hit PUS: " + pus.GetInstanceID() + " enabled: " + pus.enabled);
                    UnityEngine.Debug.Log("Hit DME: " + dme.GetInstanceID() + " enabled: " + dme.enabled);
                    UnityEngine.Debug.Log("Hit DME2: " + dme2.GetInstanceID() + " enabled: " + dme2.enabled);

                    var field2 = typeof(DiegeticMenuElement).GetField("myController", BindingFlags.NonPublic | BindingFlags.Instance);
                    DiegeticMenuController myController = (DiegeticMenuController)field2.GetValue(__instance);
                    if (myController != DiegeticMenuController.ActiveMenu){
                        UnityEngine.Debug.Log("myController is not active ");
                    }else{
                        UnityEngine.Debug.Log("myController is active ");
                    }
                }
        }
    }
}
*/

[HarmonyPatch]
class EnsurePowerupDataArrayPatch
{
    static MethodBase TargetMethod()
    {
        return typeof(GameplayData).GetMethod(
            "_EnsurePowerupDataArray",
            BindingFlags.NonPublic | BindingFlags.Static
        );
    }

    static void Postfix(GameplayData _inst)
    {
        var fieldValue = AccessTools.Field(typeof(GameplayData), "powerupsData").GetValue(_inst);
        GameplayData.PowerupData[] powerupsData = (GameplayData.PowerupData[])fieldValue;

        int newSize = CustomPowerupAPI.CustomCount + CustomPowerupAPI.FirstModdedId + 1;

        if (powerupsData == null || powerupsData.Length < newSize)
        {
            Array.Resize(ref powerupsData, newSize);
        }

        foreach (CustomPowerup powerup in CustomPowerupAPI.CustomPowerups)
        {
            int newPowerupIndex = (int)powerup.identifier;
            if (powerupsData[newPowerupIndex] == null)
            {
                powerupsData[newPowerupIndex] = new GameplayData.PowerupData();
                var identifier = (PowerupScript.Identifier)newPowerupIndex;
                var idString = PlatformDataMaster.EnumEntryToString(identifier);
                powerupsData[newPowerupIndex].Initialize(identifier, idString);
                Debug.Log("[Mod] Added new powerup: " + idString);
            }
        }

        AccessTools.Field(typeof(GameplayData), "powerupsData").SetValue(_inst, powerupsData);
    }

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        Debug.Log("[Mod] Transpiler for _EnsurePowerupDataArray is running");
        var codes = new List<CodeInstruction>(instructions);

        for (int i = 0; i < codes.Count - 5; i++)
        {
            
            
            // Match the exact instruction sequence
            if (codes[i].opcode == OpCodes.Ldarg_0 &&
                codes[i + 1].opcode == OpCodes.Ldfld &&
                codes[i + 2].opcode == OpCodes.Ldlen &&
                codes[i + 3].opcode == OpCodes.Conv_I4 &&
                codes[i + 4].opcode == OpCodes.Ldloc_0 &&
                codes[i + 5].opcode == OpCodes.Beq)
            {

                var targetLabel = codes[i + 5].operand;

                // NOP out the conditional logic
                for (int j = 0; j < 5; j++){
                    var labels = codes[i + j].labels;
                    
                    var nopInstruction = new CodeInstruction(OpCodes.Nop);
                    nopInstruction.labels.AddRange(labels);
                    codes[i + j] = nopInstruction;

                    
                }

                codes[i + 5] = new CodeInstruction(OpCodes.Br_S, targetLabel);

                break;
            }
        }
        Debug.LogWarning("returning..");
        return codes;
    }
}

[HarmonyPatch(typeof(PowerupScript))]
[HarmonyPatch(nameof(PowerupScript.InitializeAll))]
class Transpiler_PowerupScript_InitializeAll
{
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var code = new List<CodeInstruction>(instructions);

        int lastInitializeCallIndex = -1;

        // Find ALL Initialize() calls
        for (int i = 0; i < code.Count - 1; i++)
        {
            if (code[i].opcode == OpCodes.Callvirt && code[i].operand is MethodInfo method
                && method.Name == "Initialize")
            {
                lastInitializeCallIndex = i;
            }
        }

        if (lastInitializeCallIndex != -1)
        {
            Debug.Log($"[Transpiler] Last Initialize() call at index {lastInitializeCallIndex}, injecting after.");

            var injectedInstructions = new List<CodeInstruction>
            {
                new CodeInstruction(OpCodes.Ldarg_0), // <-- Load "placePowerups" (first parameter)
                new CodeInstruction(OpCodes.Ldarg_1), // <-- Load "isNewGame" (second parameter)
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Transpiler_PowerupScript_InitializeAll), nameof(InjectedCode)))
            };

            code.InsertRange(lastInitializeCallIndex + 1, injectedInstructions);
        }
        else
        {
            Debug.LogError("[Transpiler] ERROR: No Initialize() call found!");
        }

        return code;
    }

    public static void InjectedCode(bool placePowerups, bool isNewGame)
    {
        CustomPowerupAPI.InitializeCustomAll(placePowerups, isNewGame);
    }
}
}