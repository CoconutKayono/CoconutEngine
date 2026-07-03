#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameLogic.Editor
{
    /// <summary>
    /// 生成精确几何的扇形贴图（45° 楔形，Pivot 在圆心），并写入 RadialMenu 资源目录。
    /// </summary>
    public static class RadialMenuTextureGenerator
    {
        private const string OutputDir = "Assets/AssetRaw/UI/RadialMenu";
        private const int Size = 512;
        private const float Center = Size * 0.5f;
        private const float OuterRadius = Size * 0.48f;
        private const float SliceDegrees = 45f;

        [MenuItem("TEngine/UI/Generate Radial Menu Wedge Textures")]
        public static void GenerateWedgeTextures()
        {
            Directory.CreateDirectory(OutputDir);

            SaveTexture("radial_sector_normal_proc.png", CreateWedge(new Color(0.1f, 0.1f, 0.12f, 0.72f), false));
            SaveTexture("radial_sector_highlight_proc.png", CreateWedge(new Color(0.35f, 0.35f, 0.38f, 0.88f), true));

            AssetDatabase.Refresh();
            ConfigureSpriteImport($"{OutputDir}/radial_sector_normal_proc.png");
            ConfigureSpriteImport($"{OutputDir}/radial_sector_highlight_proc.png");

            Debug.Log("[RadialMenu] 程序化扇形贴图已生成，Pivot 位于圆心 (0.5, 0.5)。");
        }

        private static Texture2D CreateWedge(Color fill, bool highlight)
        {
            var tex = new Texture2D(Size, Size, TextureFormat.RGBA32, false);
            var pixels = new Color[Size * Size];
            var halfSlice = SliceDegrees * 0.5f;

            for (var y = 0; y < Size; y++)
            {
                for (var x = 0; x < Size; x++)
                {
                    var dx = x - Center + 0.5f;
                    var dy = y - Center + 0.5f;
                    var dist = Mathf.Sqrt(dx * dx + dy * dy);
                    var angle = Mathf.Atan2(dx, dy) * Mathf.Rad2Deg;
                    if (angle < 0f)
                    {
                        angle += 360f;
                    }

                    var inSlice = angle <= halfSlice || angle >= 360f - halfSlice;
                    var alpha = 0f;
                    if (inSlice && dist <= OuterRadius)
                    {
                        alpha = fill.a;
                        if (highlight && dist > OuterRadius - 3f)
                        {
                            alpha = Mathf.Min(1f, alpha + 0.25f);
                        }

                        var dot = ((x + y) % 7 == 0) ? 0.06f : 0f;
                        var c = new Color(fill.r + dot, fill.g + dot, fill.b + dot, alpha);
                        pixels[y * Size + x] = c;
                    }
                    else if (inSlice && dist <= OuterRadius + 1.5f)
                    {
                        pixels[y * Size + x] = new Color(0.05f, 0.05f, 0.05f, 0.9f);
                    }
                    else
                    {
                        pixels[y * Size + x] = Color.clear;
                    }
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }

        private static void SaveTexture(string fileName, Texture2D tex)
        {
            var path = Path.Combine(OutputDir, fileName);
            File.WriteAllBytes(path, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
        }

        private static void ConfigureSpriteImport(string assetPath)
        {
            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null)
            {
                return;
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.filterMode = FilterMode.Bilinear;
            importer.spritePivot = new Vector2(0.5f, 0.5f);
            importer.SaveAndReimport();
        }
    }
}
#endif
