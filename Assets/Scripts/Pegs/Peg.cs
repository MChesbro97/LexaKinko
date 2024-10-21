using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peg : MonoBehaviour
{
    private GameManager gameManager;
    private ReactivationPeg reactivationPeg;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        if (gameManager != null)
        {
            gameManager.RegisterPeg(this.gameObject);
        }

        reactivationPeg = FindObjectOfType<ReactivationPeg>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Ball ball = collision.gameObject.GetComponent<Ball>();

            if (ball != null)
            {
                // Accumulate 1 point in the ball for hitting the peg
                //ball.AccumulatePointFromPeg();

                // Deactivate the peg after being hit
                gameObject.SetActive(false);

                if (reactivationPeg != null)
                {
                    reactivationPeg.AddPegToReactivation(gameObject);
                }

                Debug.Log("Peg deactivated and added to reactivation list.");
            }
        }
    }

    public void ReactivatePeg()
    {
        gameObject.SetActive(true);
        Debug.Log("Peg reactivated");
    }
}
