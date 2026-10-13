using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class ArenaPresentation : MonoBehaviour
{
    private Transform displayLighter;
    private float displayBaseY;
    private readonly Color amber = new Color(1f, .55f, .18f);
    public static Material Material(Color color, bool unlit = false)
    {
        var catalog = Resources.Load<ReleaseCatalog>("ReleaseCatalog");
        var mat = new Material(unlit ? catalog.UnlitShader : catalog.LitShader);
        mat.color = color;
        return mat;
    }
    public static GameObject Box(string name, Vector3 position, Vector3 scale, Material material)
    {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name;
        obj.transform.position = position;
        obj.transform.localScale = scale;
        Destroy(obj.GetComponent<Collider>());
        obj.GetComponent<Renderer>().sharedMaterial = material;
        return obj;
    }
    public static GameObject Marker(Vector3 at, Color color)
    {
        var obj = new GameObject("Spawn warning");
        var line = obj.AddComponent<LineRenderer>();
        line.sharedMaterial = Material(color, true);
        line.widthMultiplier = .045f;
        line.loop = true;
        line.positionCount = 40;
        line.useWorldSpace = false;
        for (int i = 0; i < 40; i++) { float a = i * Mathf.PI * 2 / 40; line.SetPosition(i, new Vector3(Mathf.Cos(a) * .48f, .035f, Mathf.Sin(a) * .48f)); }
        obj.transform.position = at;
        return obj;
    }
    public void Setup(bool gameplay)
    {
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(.52f, .58f, .7f);
        RenderSettings.fog = false;
        var sun = FindFirstObjectByType<Light>();
        if (sun != null) { sun.color = new Color(1, .88f, .72f); sun.intensity = 1.1f; sun.transform.rotation = Quaternion.Euler(48, -35, 0); sun.shadows = LightShadows.Soft; }
        var edge = Material(amber, true);
        if (gameplay)
        {
            var plane = GameObject.Find("Plane");
            if (plane != null) plane.GetComponent<Renderer>().sharedMaterial = Material(new Color(.075f, .105f, .145f));
            Box("Arena foundation", new Vector3(0, -.24f, 0), new Vector3(10, .45f, 10), Material(new Color(.045f, .06f, .085f)));
            Box("North edge", new Vector3(0, .012f, 4.98f), new Vector3(10, .025f, .035f), edge);
            Box("South edge", new Vector3(0, .012f, -4.98f), new Vector3(10, .025f, .035f), edge);
            Box("East edge", new Vector3(4.98f, .012f, 0), new Vector3(.035f, .025f, 10), edge);
            Box("West edge", new Vector3(-4.98f, .012f, 0), new Vector3(.035f, .025f, 10), edge);
            var grid = Material(new Color(.115f, .155f, .20f), true);
            for (int i = -4; i <= 4; i++)
            {
                Box("Floor grid", new Vector3(i, .005f, 0), new Vector3(.012f, .009f, 9.8f), grid);
                Box("Floor grid", new Vector3(0, .005f, i), new Vector3(9.8f, .009f, .012f), grid);
            }
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var old = player.GetComponent<SimpleSampleCharacterControl>();
                if (old != null) old.enabled = false;
                player.transform.position = new Vector3(0, .05f, -2.5f);
                player.AddComponent<LighterPlayerController>();
            }
        }
        else
        {
            var camera = Camera.main;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(.025f, .035f, .06f);
            camera.orthographic = true;
            camera.orthographicSize = 3.7f;
            camera.transform.position = new Vector3(0, 3, -9);
            camera.transform.LookAt(new Vector3(0, 1, 0));
            var catalog = Resources.Load<ReleaseCatalog>("ReleaseCatalog");
            if (catalog != null && catalog.DisplayPrefab != null)
            {
                var lighter = Instantiate(catalog.DisplayPrefab, new Vector3(2.4f, .1f, 0), Quaternion.Euler(0, -30, -12));
                lighter.transform.localScale *= 4.5f;
                displayLighter = lighter.transform;
                displayBaseY = lighter.transform.position.y;
                Marker(new Vector3(2.4f, -.15f, 0), amber).transform.localScale = Vector3.one * 3.5f;
            }
            var fill = new GameObject("Cool rim light").AddComponent<Light>();
            fill.type = LightType.Point;
            fill.color = new Color(.3f, .65f, 1);
            fill.intensity = 3;
            fill.range = 12;
            fill.transform.position = new Vector3(4, 3, 3);
        }
    }
    private void Update()
    {
        if (displayLighter == null) return;
        displayLighter.Rotate(0, Time.deltaTime * 12, 0, Space.World);
        var p = displayLighter.position;
        p.y = displayBaseY + Mathf.Sin(Time.unscaledTime * 1.2f) * .12f;
        displayLighter.position = p;
    }
}
