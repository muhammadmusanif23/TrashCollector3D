using UnityEngine;
using System.Collections;

public class TrashSpawner : MonoBehaviour
{
    [Header("Trash Objects")]
    public GameObject[] trashItems;

    [Header("Relocate Settings")]
    public float relocateInterval = 5f;

    [Header("Spawn Boundaries")]
    public float minX = -15f;
    public float maxX = 15f;
    public float minZ = -15f;
    public float maxZ = 15f;
    public float groundY = 0.5f;

    private bool gameRunning = true;
    private Vector3[] originalPositions;

    void Start()
    {
        // save original positions of all trash
        originalPositions = new Vector3[trashItems.Length];
        for (int i = 0; i < trashItems.Length; i++)
        {
            if (trashItems[i] != null)
                originalPositions[i] = trashItems[i].transform.position;
        }

        // start relocating after first interval
        StartCoroutine(RelocateTrash());
    }

    public void StopRelocating()
    {
        gameRunning = false;
        StopAllCoroutines();
    }

    IEnumerator RelocateTrash()
    {
        // wait full interval before FIRST relocation
        // so trash stays in original position at game start
        yield return new WaitForSeconds(relocateInterval);

        while (gameRunning)
        {
            foreach (GameObject trash in trashItems)
            {
                // only move trash that still exists
                // and is not being held by player
                if (trash != null && trash.activeInHierarchy
                    && trash.transform.parent == null)
                {
                    Vector3 newPos = new Vector3(
                        Random.Range(minX, maxX),
                        groundY,
                        Random.Range(minZ, maxZ)
                    );
                    trash.transform.position = newPos;
                }
            }

            // wait before next relocation
            yield return new WaitForSeconds(relocateInterval);
        }
    }
}