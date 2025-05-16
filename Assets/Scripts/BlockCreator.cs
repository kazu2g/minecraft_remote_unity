using UnityEditor;
using UnityEngine;
using System.IO;

public class BlockCreator : MonoBehaviour
{
    string resourcePath = "Assets/Resources/Prefabs"; // プレハブを保存するパス
    public GameObject targetCubePrefab; // プレハブ化されたターゲットキューブをインスペクターで割り当ててください

    public void ProcessBlock(string blockId)
    {
        if (string.IsNullOrEmpty(blockId))
        {
            Debug.LogError("blockIdが無効です。");
            return;
        }

        // 保存パスを定義
        string prefabPath = Path.Combine(resourcePath, $"{blockId}.prefab");

        // プレハブが既に存在するか確認
        if (File.Exists(prefabPath))
        {
            Debug.Log($"既存のプレハブ '{blockId}' が見つかりました。");
            return;
        }

        // マテリアルの保存パスを定義
        string materialPath = Path.Combine("Assets/Materials", $"{blockId}.mat");

        try
        {
            // マテリアルを取得または作成
            Material cubeMaterial = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            if (cubeMaterial == null)
            {
                cubeMaterial = new Material(Shader.Find("Standard")) { name = blockId };

                // テクスチャを検索して適用
                string[] guids = AssetDatabase.FindAssets($"t:Texture2D {blockId}");
                if (guids.Length > 0)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                    Texture2D loadedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                    if (loadedTexture != null)
                    {
                        cubeMaterial.SetTexture("_MainTex", loadedTexture);
                        Debug.Log($"新しいマテリアル '{cubeMaterial.name}' を作成し、テクスチャ '{blockId}' を適用しました。");
                    }
                    else
                    {
                        Debug.LogError($"パス '{assetPath}' でテクスチャの読み込みに失敗しました。");
                    }
                }
                else
                {
                    Debug.LogError($"プロジェクト内で名前 '{blockId}' のテクスチャが見つかりませんでした。");
                }

                // マテリアルを保存
                Directory.CreateDirectory("Assets/Materials");
                AssetDatabase.CreateAsset(cubeMaterial, materialPath);
                AssetDatabase.SaveAssets();
                Debug.Log($"新しいマテリアル '{cubeMaterial.name}' を '{materialPath}' に保存しました。");
            }
            else
            {
                Debug.Log($"既存のマテリアル '{cubeMaterial.name}' を使用します。");
            }

            // ターゲットキューブの新しいインスタンスを作成
            GameObject targetCubeInstance = new GameObject(blockId);
            Renderer cubeRenderer = targetCubeInstance.AddComponent<MeshRenderer>();
            targetCubeInstance.AddComponent<MeshFilter>().mesh = targetCubePrefab.GetComponent<MeshFilter>().sharedMesh;

            if (cubeRenderer != null)
            {
                cubeRenderer.material = cubeMaterial;
                Debug.Log($"マテリアル '{cubeMaterial.name}' をターゲットキューブに適用しました。");
            }
            else
            {
                Debug.LogError("ターゲットキューブにRendererコンポーネントが見つかりません。");
            }

            // プレハブとして保存
            Directory.CreateDirectory(resourcePath);
            PrefabUtility.SaveAsPrefabAsset(targetCubeInstance, prefabPath);
            Debug.Log($"ターゲットキューブ '{blockId}' をプレハブとして '{prefabPath}' に保存しました。");

            // メモリ上のインスタンスを削除
            DestroyImmediate(targetCubeInstance);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"エラーが発生しました: {ex.Message}");
        }
    }
 }
