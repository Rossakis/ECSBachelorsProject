using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Given a 3D model and an animation for it, this editor window will bake a sequence of animated meshes of that model, which is used by our DOTS-oriented animation system 
/// </summary>
public class MeshBakerWindow : EditorWindow
{
    private GameObject fbxModel;
    private AnimationClip animationClip;
    [Tooltip("If not specified, will use the model's name.")]
    private string modelName = string.Empty;
    [Tooltip("If not specified, will use the animation clip's name.")]
    private string animationName = string.Empty;
    private int sampleRate = 30;

    [MenuItem("Tools/Mesh Baker")]
    static void Init()
    {
        MeshBakerWindow window = (MeshBakerWindow)GetWindow(typeof(MeshBakerWindow));
        window.titleContent = new GUIContent("Mesh Baker");
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("FBX Mesh Baker", EditorStyles.boldLabel);
        fbxModel = (GameObject)EditorGUILayout.ObjectField("FBX Model", fbxModel, typeof(GameObject), false);
        animationClip = (AnimationClip)EditorGUILayout.ObjectField("Animation Clip", animationClip, typeof(AnimationClip), false);
        modelName = EditorGUILayout.TextField("Custom Model Name", modelName);
        animationName = EditorGUILayout.TextField("Custom Animation Name", animationName);
        sampleRate = EditorGUILayout.IntField("Sample Rate (FPS)", sampleRate);

        if (GUILayout.Button("Bake") && fbxModel != null && animationClip != null)
        {
            BakeAnimationToMeshes();
        }
    }

    void BakeAnimationToMeshes()
    {
    string model = string.IsNullOrEmpty(modelName) ? fbxModel.name : modelName;
    string anim = string.IsNullOrEmpty(animationName) ? animationClip.name : animationName;
    string folderPath = $"Assets/BakedMeshes/{model}/{anim}";

    if (!AssetDatabase.IsValidFolder(folderPath))
        Directory.CreateDirectory(folderPath);

    GameObject tempInstance = Instantiate(fbxModel);
    tempInstance.hideFlags = HideFlags.HideAndDontSave;

    Animator animator = tempInstance.GetComponent<Animator>();
    if (animator == null)
        animator = tempInstance.AddComponent<Animator>();

    SkinnedMeshRenderer[] skinnedMeshes = tempInstance.GetComponentsInChildren<SkinnedMeshRenderer>();
    if (skinnedMeshes.Length == 0)
    {
        Debug.LogError("No SkinnedMeshRenderers found in model.");
        DestroyImmediate(tempInstance);
        return;
    }

    AnimationMode.StartAnimationMode();

    float duration = animationClip.length;
    int frameCount = Mathf.CeilToInt(duration * sampleRate);

    for (int i = 0; i <= frameCount; i++)
    {
        float time = (float)i / sampleRate;

        // Sample animation at the current time
        AnimationMode.BeginSampling();
        AnimationMode.SampleAnimationClip(tempInstance, animationClip, time);
        AnimationMode.EndSampling();

        List<CombineInstance> combineList = new List<CombineInstance>();

        foreach (var smr in skinnedMeshes)
        {
            Mesh bakedPart = new Mesh();
            smr.BakeMesh(bakedPart);

            CombineInstance ci = new CombineInstance
            {
                mesh = bakedPart,
                transform = smr.transform.localToWorldMatrix // Position mesh correctly
            };

            combineList.Add(ci);
        }

        // Combine all parts into a single mesh
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combineList.ToArray(), true, true); // true: merge submeshes and apply transforms

        string meshName = $"{model}_{anim}_frame{i:D4}.asset";
        AssetDatabase.CreateAsset(combinedMesh, $"{folderPath}/{meshName}");
    }

    DestroyImmediate(tempInstance);
    AnimationMode.StopAnimationMode();

    AssetDatabase.SaveAssets();
    AssetDatabase.Refresh();

    Debug.Log("Baking and combination complete!");
}

}
