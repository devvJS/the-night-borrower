using UnityEngine;
using System;
using System.Collections.Generic;

public class DiscoveryLog : MonoBehaviour
{
    public static DiscoveryLog Instance { get; private set; }

    private List<DiscoveryEntry> entries = new List<DiscoveryEntry>();
    private HashSet<string> loggedClueIds = new HashSet<string>();

    public IReadOnlyList<DiscoveryEntry> Entries => entries;
    public bool HasDiscovery(string clueId) => loggedClueIds.Contains(clueId);
    public int Count => entries.Count;

    public event Action<DiscoveryEntry> OnDiscoveryLogged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        GameEvents.OnClueDiscovered += HandleClueDiscovered;
    }

    private void OnDisable()
    {
        GameEvents.OnClueDiscovered -= HandleClueDiscovered;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void HandleClueDiscovered(string clueId, string objectId)
    {
        if (loggedClueIds.Contains(clueId)) return;

        InteractableObject source = FindSourceObject(objectId);

        var entry = new DiscoveryEntry
        {
            entryId = $"discovery_{clueId}",
            clueId = clueId,
            objectId = objectId,
            title = source != null ? source.DisplayName : objectId,
            description = source != null ? source.ClueText : "",
            category = EntryCategory.Clue
        };

        entries.Add(entry);
        loggedClueIds.Add(clueId);

        OnDiscoveryLogged?.Invoke(entry);
        GameEvents.EntryCreated(entry.entryId);
    }

    private static InteractableObject FindSourceObject(string objectId)
    {
        var objects = FindObjectsOfType<InteractableObject>();
        foreach (var obj in objects)
        {
            if (obj.ObjectId == objectId)
                return obj;
        }
        return null;
    }
}
