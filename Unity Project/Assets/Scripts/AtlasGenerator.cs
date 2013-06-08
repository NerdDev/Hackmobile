/*
 * Texture atlas generator. Based of lightmap algorithm here: http://www.blackpawn.com/texts/lightmaps/.
 * Jacob Schieck - 2011 - http://www.jacobschieck.com
 * Usage 
 *      1. Open the window (Window/Atlas Creator).
 *      2. Set a width and height for atlas.
 *      3. Drag directory with textures for atlasing from Unity project.
 *      4. Click create, it will ask a save location for the atlas(s) if more than one is needed.
 *              Also creates a txt file with orignal file names, and new positions on the atlas.
 * */



//using UnityEngine;
//using System.Collections;
//using UnityEditor;
//using System.Collections.Generic;
//using System.IO;
//using System.Text.RegularExpressions;
//using System.Linq;
//
//public class AtlasGenerator : EditorWindow
//{
//    static int width = 512;
//    static int height = 512;
//    static bool searchSubDir = true;
//    UnityEngine.Object directory;
//
//    [MenuItem("Window/Atlas Creator")]
//    static void OpenWindow()
//    {
//        EditorWindow.GetWindow(typeof(AtlasGenerator));
//    }
//
//    void OnGUI()
//    {
//        EditorGUIUtility.LookLikeInspector();
//        width = EditorGUILayout.IntField("Width:", width);
//        height = EditorGUILayout.IntField("Height:", height);
//        directory = EditorGUILayout.ObjectField("Textures Directory:", directory, typeof(UnityEngine.Object));
//        searchSubDir = EditorGUILayout.Toggle("Search sub-directories:", searchSubDir);
//        if (GUILayout.Button("Create Atlas(s)"))
//        {
//            Execute(AssetDatabase.GetAssetPath(directory));
//        }
//    }
//
//    void Execute(string path)
//    {
//        DirectoryInfo di = new DirectoryInfo(path);
//        Run(di);
//    }
//
//    void Run(DirectoryInfo directory)
//    {
//        List<FileInfo> imageFiles = GetImageFiles(directory);
//        List<ImageName> textureList = new List<ImageName>();
//
//
//        foreach (FileInfo f in imageFiles)
//        {
//
//            string path = @"Assets\" + Regex.Split(f.FullName, @"Assets")[1].Remove(0, 1);
//            Texture2D image = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
//
//            if (image.width > width || image.height > height)
//            {
//                Debug.LogError("Error: '" + f.Name + "' (" + image.width + "x" + image.height + ") is larger than the atlas (" + width + "x" + height + ")");
//                return;
//            }
//
//            textureList.Add(new ImageName(image, f.Name));
//        }
//
//        List<AtlasedTexture> textures = new List<AtlasedTexture>();
//        textures.Add(new AtlasedTexture(width, height));
//
//        int count = 0;
//        foreach (ImageName imageName in textureList)
//        {
//            bool added = false;
//            foreach (AtlasedTexture texture in textures)
//            {
//                if (texture.AddImage(imageName.image, imageName.name))
//                {
//                    added = true;
//                    break;
//                }
//            }
//
//            if (!added)
//            {
//                AtlasedTexture texture = new AtlasedTexture(width, height);
//                texture.AddImage(imageName.image, imageName.name);
//                textures.Add(texture);
//                Debug.Log("Creating another atlas");
//            }
//        }
//
//        count = 0;
//
//        foreach (AtlasedTexture texture in textures)
//        {
//            string path = EditorUtility.SaveFilePanel("Save atlased texture...", Application.dataPath, "atlasedTexture" + count, "png");
//            Debug.Log("Writing atlas: " + path);
//            texture.Write(path);
//            count++;
//        }
//    }
//
//    private List<FileInfo> GetImageFiles(DirectoryInfo directory)
//    {
//        string[] files = Directory.GetFiles(directory.FullName, "*.*", (searchSubDir) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
//            .Where(s => s.ToLower().EndsWith(".png") || s.ToLower().EndsWith(".tga") || s.ToLower().EndsWith(".jpg") ||
//                   s.ToLower().EndsWith(".psd") || s.ToLower().EndsWith(".dds")).ToArray();
//        Debug.Log(files.Length + " textures found.");
//        List<FileInfo> fileList = new List<FileInfo>();
//        foreach (string p in files) fileList.Add(new FileInfo(p));
//        return new List<FileInfo>(fileList);
//    }
//
//    private class ImageName
//    {
//        public Texture2D image;
//        public string name;
//
//        public ImageName(Texture2D image, string name)
//        {
//            this.image = image;
//            this.name = name;
//        }
//    }
//
//    public class AtlasedTexture
//    {
//        private class Node
//        {
//            public Rect rect;
//            public Node[] child;
//            public Texture2D image;
//
//            public Node(int x, int y, int width, int height)
//            {
//                rect = new Rect(x, y, width, height);
//                child = new Node[2];
//                child[0] = null;
//                child[1] = null;
//                image = null;
//            }
//
//            public bool IsLeaf()
//            {
//                return child[0] == null && child[1] == null;
//            }
//
//            public Node Insert(Texture2D image)
//            {
//                if (!IsLeaf())
//                {
//                    Node newNode = child[0].Insert(image);
//
//                    if (newNode != null)
//                    {
//                        return newNode;
//                    }
//
//                    return child[1].Insert(image);
//                }
//                else
//                {
//                    if (this.image != null)
//                    {
//                        return null;
//                    }
//
//                    if (image.width > rect.width || image.height > rect.height)
//                    {
//                        return null;
//                    }
//
//                    if (image.width == rect.width && image.height == rect.height)
//                    {
//                        this.image = image;
//                        return this;
//                    }
//
//                    int dw = (int)rect.width - image.width;
//                    int dh = (int)rect.height - image.height;
//
//                    if (dw > dh)
//                    {
//                        child[0] = new Node((int)rect.x, (int)rect.y, (int)image.width, (int)rect.height);
//                        child[1] = new Node((int)rect.x + (int)image.width, (int)rect.y, (int)rect.width - (int)image.width, (int)rect.height);
//                    }
//                    else
//                    {
//                        child[0] = new Node((int)rect.x, (int)rect.y, (int)rect.width, (int)image.height);
//                        child[1] = new Node((int)rect.x, (int)rect.y + (int)image.height, (int)rect.width, (int)rect.height - image.height);
//                    }
//
//                    return child[0].Insert(image);
//                }
//            }
//        }
//        private Texture2D image;
//        private Node root;
//        private Dictionary<string, Rect> rectangleMap;
//
//        public AtlasedTexture(int width, int height)
//        {
//            image = new Texture2D(width, height);
//            root = new Node(0, 0, width, height);
//            rectangleMap = new Dictionary<string, Rect>();
//        }
//
//        public bool AddImage(Texture2D img, string name)
//        {
//            Node node = root.Insert(img);
//            if (node == null)
//            {
//                return false;
//            }
//            rectangleMap.Add(name, node.rect);
//            image.SetPixels((int)node.rect.x, (int)node.rect.y, (int)node.rect.width, (int)node.rect.height, img.GetPixels());
//            image.Apply();
//            return true;
//        }
//
//        public void Write(string name)
//        {
//            TextWriter tw = new StreamWriter(Path.ChangeExtension(name, ".txt"));
//            foreach (KeyValuePair<string, Rect> e in rectangleMap)
//            {
//                Rect r = e.Value;
//                tw.WriteLine(e.Key + "," + r.x + "," + r.y + "," + r.width + "," + r.height);
//            }
//            tw.Close();
//            byte[] bytes = image.EncodeToPNG();
//
//            FileStream f = new FileStream(name, FileMode.Create, FileAccess.Write);
//            BinaryWriter b = new BinaryWriter(f);
//            for (int i = 0; i < bytes.Length; i++) b.Write(bytes[i]);
//            b.Close();
//        }
//    }
//}
