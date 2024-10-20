using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactivationPeg : Peg
{
    // List to store references to deactivated pegs
    private List<GameObject> deactivatedPegs = new List<GameObject>();

    // Function to add a peg to the list of pegs to be reactivated
    public void AddPegToReactivation(GameObject peg)
    {
        if (!deactivatedPegs.Contains(peg))
        {
            deactivatedPegs.Add(peg);
        }
    }

    // Function to reactivate all stored pegs
    public void ReactivatePegs()
    {
        foreach (var peg in deactivatedPegs)
        {
            peg.SetActive(true); // Reactivate the peg
        }

        // Clear the list after reactivating
        deactivatedPegs.Clear();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            // When the ball hits the Reactivation Peg, reactivate all stored pegs
            ReactivatePegs();
            Debug.Log("Reactivated all pegs.");
        }
    }
}
