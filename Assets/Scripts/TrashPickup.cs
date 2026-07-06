using UnityEngine;
using System.Collections;
using TMPro;

public class TrashPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public float pickupRange = 2.5f;
    public KeyCode pickupKey = KeyCode.E;
    public Transform handPosition;

    [Header("UI")]
    public GameObject pickupPromptUI;
    public TMP_Text promptText;

    [Header("References")]
    public GameManager gameManager;

    private GameObject nearestTrash = null;
    private GameObject heldTrash = null;
    private bool isHolding = false;

    void Update()
    {
        if (!isHolding)
        {
            FindNearestTrash();

            if (nearestTrash != null && Input.GetKeyDown(pickupKey))
                PickUp(nearestTrash);
        }
        else
        {
            if (pickupPromptUI != null)
            {
                pickupPromptUI.SetActive(true);
                if (promptText != null)
                    promptText.text = "Press E to Drop in Dustbin";
            }

            if (Input.GetKeyDown(pickupKey))
                DropIntoDustbin();
        }
    }

    void FindNearestTrash()
    {
        GameObject[] trashes = GameObject.FindGameObjectsWithTag("Trash");
        float closestDist = pickupRange;
        nearestTrash = null;

        foreach (GameObject trash in trashes)
        {
            float dist = Vector3.Distance(
                transform.position,
                trash.transform.position
            );
            if (dist < closestDist)
            {
                closestDist = dist;
                nearestTrash = trash;
            }
        }

        if (pickupPromptUI != null)
            pickupPromptUI.SetActive(nearestTrash != null);

        if (nearestTrash != null && promptText != null)
            promptText.text = "Press E to Pick Up";
    }

    void PickUp(GameObject trash)
    {
        isHolding = true;
        heldTrash = trash;

        // attach to hand
        trash.transform.SetParent(handPosition);
        trash.transform.localPosition = Vector3.zero;
        trash.transform.localRotation = Quaternion.identity;
        trash.transform.localScale = Vector3.one * 0.3f;

        // disable physics while held
        Rigidbody rb = trash.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        Collider col = trash.GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }

    void DropIntoDustbin()
    {
        // find the NEAREST dustbin to player
        GameObject[] dustbins = GameObject.FindGameObjectsWithTag("Dustbin");
        GameObject nearestBin = null;
        float closestDist = Mathf.Infinity;

        foreach (GameObject bin in dustbins)
        {
            float dist = Vector3.Distance(
                transform.position,
                bin.transform.position
            );
            if (dist < closestDist)
            {
                closestDist = dist;
                nearestBin = bin;
            }
        }

        if (nearestBin != null)
        {
            StartCoroutine(FlyToDustbin(heldTrash, nearestBin.transform.position));
        }
        else
        {
            // no dustbin found — drop it
            heldTrash.transform.SetParent(null);
            Rigidbody rb = heldTrash.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = false;

            Collider col = heldTrash.GetComponent<Collider>();
            if (col != null) col.enabled = true;
        }

        isHolding = false;
        heldTrash = null;

        if (pickupPromptUI != null)
            pickupPromptUI.SetActive(false);
    }

    IEnumerator FlyToDustbin(GameObject trash, Vector3 targetPos)
    {
        trash.transform.SetParent(null);
        float duration = 0.6f;
        float elapsed = 0f;
        Vector3 startPos = trash.transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // nice arc effect
            Vector3 pos = Vector3.Lerp(startPos, targetPos, t);
            pos.y += Mathf.Sin(t * Mathf.PI) * 2f;
            trash.transform.position = pos;

            yield return null;
        }

        // notify game manager
        if (gameManager != null)
            gameManager.OnTrashCollected();

        Destroy(trash);
    }
}