using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class UpgradeGraphEditor : EditorWindow
{
    private List<UpgradeSO> allUpgrades;
    private UpgradeSO selectedUpgrade;
    private UpgradeSO linkingUpgrade;
    private Connection selectedConnection;
    private Vector2 scrollPosition;
    private bool isAddingConnection;
    private double lastClickTime = 0;
    private const double doubleClickTime = 0.3;

    private const float nodeWidth = 200f;
    private const float nodeHeight = 165f;
    private const float arrowSize = 20f;
    private const float connectionClickWidth = 8f;

    [MenuItem("Window/Upgrade Graph Editor")]
    public static void ShowWindow()
    {
        GetWindow<UpgradeGraphEditor>("Upgrade Graph Editor");
    }

    private void OnEnable()
    {
        LoadAllUpgrades();
    }

    private void LoadAllUpgrades()
    {
        allUpgrades = new List<UpgradeSO>();
        string[] guids = AssetDatabase.FindAssets("t:UpgradeSO");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            UpgradeSO upgrade = AssetDatabase.LoadAssetAtPath<UpgradeSO>(path);
            if (upgrade != null) allUpgrades.Add(upgrade);
        }
    }

    private void OnGUI()
    {
        HandleDoubleClick();
        DrawToolbar();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        {
            DrawConnections();
            DrawNodes();
            HandleEvents();
        }
        EditorGUILayout.EndScrollView();

        if (isAddingConnection && linkingUpgrade != null)
        {
            DrawTempConnection(linkingUpgrade, Event.current.mousePosition);
            Repaint();
        }

        DrawSelectionPanel();
    }

    private void HandleDoubleClick()
    {
        Event e = Event.current;
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            if (GetUpgradeAtPosition(e.mousePosition) == null && !IsClickOnConnection(e.mousePosition))
            {
                double currentTime = EditorApplication.timeSinceStartup;
                if (currentTime - lastClickTime < doubleClickTime)
                {
                    if (isAddingConnection)
                    {
                        isAddingConnection = false;
                    }
                    else
                    {
                        selectedConnection = null;
                    }

                    Repaint();
                }

                lastClickTime = currentTime;
            }
        }
    }

    private void DrawToolbar()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        {
            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton))
            {
                LoadAllUpgrades();
                Repaint();
            }
            if (GUILayout.Button("Save All", EditorStyles.toolbarButton))
            {
                SaveAllChanges();
            }
        }
        GUILayout.EndHorizontal();
    }

    private void SaveAllChanges()
    {
        foreach (var upgrade in allUpgrades)
        {
            EditorUtility.SetDirty(upgrade);
        }
        AssetDatabase.SaveAssets();
        Debug.Log("All upgrades saved");
    }

    private void DrawNodes()
    {
        if (allUpgrades == null) return;

        BeginWindows();
        for (int i = 0; i < allUpgrades.Count; i++)
        {
            UpgradeSO upgrade = allUpgrades[i];
            Rect rect = new Rect(upgrade.editorPosition.x, upgrade.editorPosition.y, nodeWidth, nodeHeight);
            upgrade.editorPosition = GUI.Window(i, rect, DrawNodeWindow, upgrade.uniqueName).position;
        }
        EndWindows();
    }

    private void DrawNodeWindow(int id)
    {
        UpgradeSO upgrade = allUpgrades[id];
        GUILayout.BeginVertical();

        if (upgrade.icon != null)
        {
            GUILayout.Label(upgrade.icon.texture, GUILayout.Width(48), GUILayout.Height(48));
        }

        GUILayout.Label(upgrade.label);
        GUILayout.Label($"Cost: {upgrade.cost}");

        GUILayout.Space(5);

        if (GUILayout.Button("Select", GUILayout.Height(20)))
        {
            SelectNode(upgrade);
        }

        if (selectedUpgrade == upgrade && !isAddingConnection)
        {
            if (GUILayout.Button("Add Connection", GUILayout.Height(20)))
            {
                StartConnection(upgrade);
            }
        }

        GUILayout.EndVertical();
        GUI.DragWindow();
    }

    private void SelectNode(UpgradeSO upgrade)
    {
        selectedUpgrade = upgrade;
        if (isAddingConnection) CompleteConnection(upgrade);
        selectedConnection = null;
        Repaint();
    }

    private void StartConnection(UpgradeSO fromUpgrade)
    {
        isAddingConnection = true;
        linkingUpgrade = fromUpgrade;
        selectedUpgrade = null;
        selectedConnection = null;
        Repaint();
    }

    private void CompleteConnection(UpgradeSO toUpgrade)
    {
        if (!isAddingConnection || linkingUpgrade == null) return;

        if (toUpgrade != linkingUpgrade && !linkingUpgrade.nextUpgradesList.Contains(toUpgrade))
        {
            linkingUpgrade.nextUpgradesList.Add(toUpgrade);
            EditorUtility.SetDirty(linkingUpgrade);
            AssetDatabase.SaveAssets();
        }

        isAddingConnection = false;
        linkingUpgrade = null;
        Repaint();
    }

    private void DrawConnections()
    {
        if (allUpgrades == null) return;

        foreach (UpgradeSO upgrade in allUpgrades)
        {
            foreach (UpgradeSO nextUpgrade in upgrade.nextUpgradesList.ToList())
            {
                if (nextUpgrade != null)
                {
                    DrawConnection(upgrade, nextUpgrade);
                }
            }
        }
    }

    private void DrawConnection(UpgradeSO from, UpgradeSO to)
    {
        Vector2 startPos = new Vector2(from.editorPosition.x + nodeWidth, from.editorPosition.y + nodeHeight / 2);
        Vector2 endPos = new Vector2(to.editorPosition.x, to.editorPosition.y + nodeHeight / 2);
        Vector2 direction = (endPos - startPos).normalized;

        bool isSelected = selectedConnection != null && selectedConnection.from == from && selectedConnection.to == to;
        Handles.color = isSelected ? Color.yellow : Color.white;
        Handles.DrawAAPolyLine(3f, startPos, endPos);
        DrawArrow(endPos - direction * 10f, direction);

        if (HandleUtility.DistancePointToLineSegment(Event.current.mousePosition, startPos, endPos) < connectionClickWidth)
        {
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                selectedConnection = new Connection(from, to);
                selectedUpgrade = null;
                Event.current.Use();
                Repaint();
            }
        }
    }

    private void DrawTempConnection(UpgradeSO from, Vector2 toPos)
    {
        Vector2 startPos = new Vector2(from.editorPosition.x + nodeWidth, from.editorPosition.y + nodeHeight / 2);
        Handles.color = Color.green;
        Handles.DrawAAPolyLine(3f, startPos, toPos);
        DrawArrow(toPos, (toPos - startPos).normalized);
    }

    private void DrawArrow(Vector2 pos, Vector2 direction)
    {
        Vector2 right = Quaternion.Euler(0, 0, 30) * direction * arrowSize;
        Vector2 left = Quaternion.Euler(0, 0, -30) * direction * arrowSize;
        Handles.DrawAAConvexPolygon(pos, pos - right, pos - left);
    }

    private void HandleEvents()
    {
        Event e = Event.current;
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Delete)
        {
            DeleteSelectedConnection();
        }
    }

    private bool IsClickOnConnection(Vector2 clickPos)
    {
        foreach (UpgradeSO upgrade in allUpgrades)
        {
            foreach (UpgradeSO nextUpgrade in upgrade.nextUpgradesList)
            {
                if (nextUpgrade != null)
                {
                    Vector2 startPos = new Vector2(upgrade.editorPosition.x + nodeWidth, upgrade.editorPosition.y + nodeHeight / 2);
                    Vector2 endPos = new Vector2(nextUpgrade.editorPosition.x, nextUpgrade.editorPosition.y + nodeHeight / 2);
                    if (HandleUtility.DistancePointToLineSegment(clickPos, startPos, endPos) < connectionClickWidth)
                        return true;
                }
            }
        }
        return false;
    }

    private UpgradeSO GetUpgradeAtPosition(Vector2 pos)
    {
        foreach (UpgradeSO upgrade in allUpgrades)
        {
            Rect rect = new Rect(upgrade.editorPosition.x, upgrade.editorPosition.y, nodeWidth, nodeHeight);
            if (rect.Contains(pos)) return upgrade;
        }
        return null;
    }

    private void DeleteSelectedConnection()
    {
        if (selectedConnection == null) return;

        Undo.RecordObject(selectedConnection.from, "Delete Upgrade Connection");
        bool wasRemoved = selectedConnection.from.nextUpgradesList.Remove(selectedConnection.to);

        if (wasRemoved)
        {
            EditorUtility.SetDirty(selectedConnection.from);
            AssetDatabase.SaveAssets();
        }

        selectedConnection = null;
        Repaint();
    }

    private void DrawSelectionPanel()
    {
        if (selectedUpgrade == null && selectedConnection == null) return;

        GUILayout.BeginArea(new Rect(position.width - 250, 20, 230, 150), GUI.skin.box);
        if (selectedUpgrade != null)
        {
            GUILayout.Label($"Selected: {selectedUpgrade.uniqueName}", EditorStyles.boldLabel);
            GUILayout.Label(selectedUpgrade.label);
            GUILayout.Label($"Cost: {selectedUpgrade.cost}");

            if (selectedUpgrade.icon != null)
            {
                GUILayout.Label(selectedUpgrade.icon.texture, GUILayout.Width(48), GUILayout.Height(48));
            }

            if (GUILayout.Button("Ping"))
            {
                EditorGUIUtility.PingObject(selectedUpgrade);
            }
        }
        else if (selectedConnection != null)
        {
            GUILayout.Label("Selected Connection", EditorStyles.boldLabel);
            GUILayout.Label($"{selectedConnection.from.uniqueName} -> {selectedConnection.to.uniqueName}");

            if (GUILayout.Button("Delete Connection", GUILayout.Height(30)))
            {
                DeleteSelectedConnection();
            }
        }
        GUILayout.EndArea();
    }

    private class Connection
    {
        public UpgradeSO from;
        public UpgradeSO to;
        public Connection(UpgradeSO from, UpgradeSO to)
        {
            this.from = from;
            this.to = to;
        }
    }
}
